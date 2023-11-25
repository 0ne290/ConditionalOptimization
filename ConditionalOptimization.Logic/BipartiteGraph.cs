using ConditionalOptimization.Logic.Contracts;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace ConditionalOptimization.Logic;

public class BipartiteGraph
{
	public BipartiteGraph(bool[,] bipartiteGraphAdjacencyMatrix)
	{
		if (bipartiteGraphAdjacencyMatrix.GetLength(0) != bipartiteGraphAdjacencyMatrix.GetLength(1))
			throw new Exception("The adjacency matrix must be square");
		
		var directedGraphAdjacencyMatrix = CreateAdjacencyMatrix(bipartiteGraphAdjacencyMatrix);
		CreateEdges(directedGraphAdjacencyMatrix);
		CreateAdjacencyLists();
	}

	private bool[,] CreateAdjacencyMatrix(bool[,] bipartiteGraphAdjacencyMatrix)
	{
		_adjacencyMatrix = new bool[bipartiteGraphAdjacencyMatrix.GetLength(0) * 2 + 2, bipartiteGraphAdjacencyMatrix.GetLength(0) * 2 + 2];
		var directedGraphAdjacencyMatrix = new bool[NumberOfNodes, NumberOfNodes];
		
		var rightPartIndex = bipartiteGraphAdjacencyMatrix.GetLength(0) + 1;
		const int leftPartIndex = 1;
		var drainIndex = NumberOfNodes - 1;
		const int sourceIndex = 0;
		
		for (var i = rightPartIndex; i < drainIndex; i++)
		{
			directedGraphAdjacencyMatrix[i, drainIndex] = true;
			_adjacencyMatrix[i, drainIndex] = true;
			_adjacencyMatrix[drainIndex, i] = true;
		}

		var k = 0;
		for (var i = leftPartIndex; i < rightPartIndex; i++)
		{
			directedGraphAdjacencyMatrix[sourceIndex, i] = true;
			_adjacencyMatrix[sourceIndex, i] = true;
			_adjacencyMatrix[i, sourceIndex] = true;
			for (var j = rightPartIndex; j < drainIndex; j++)
			{
				directedGraphAdjacencyMatrix[i, j] = bipartiteGraphAdjacencyMatrix[k, j - rightPartIndex];
				_adjacencyMatrix[i, j] = bipartiteGraphAdjacencyMatrix[k, j - rightPartIndex];
				_adjacencyMatrix[j, i] = bipartiteGraphAdjacencyMatrix[k, j - rightPartIndex];
			}
			k++;
		}

		return directedGraphAdjacencyMatrix;
	}

	private void CreateEdges(bool[,] directedGraphAdjacencyMatrix)
	{
		_edges = new Edge[NumberOfNodes, NumberOfNodes];
		for (var i = 0; i < NumberOfNodes; i++)
		{
			for (var j = 0; j < NumberOfNodes; j++)
			{
				if (directedGraphAdjacencyMatrix[i, j])
				{
					_edges[i, j] = new Edge(1, false);
					_edges[j, i] = new Edge(0, true);
				}
				else if (!_adjacencyMatrix[i, j])
					_edges[i, j] = new Edge();
			}
		}
	}

	private void CreateAdjacencyLists()
	{
		_adjacencyLists = new List<List<int>>(NumberOfNodes);
		for (var i = 0; i < NumberOfNodes; i++)
		{
			_adjacencyLists.Add(new List<int>(NumberOfNodes));
			for (var j = 0; j < NumberOfNodes; j++)
				if (_adjacencyMatrix[i, j])
					_adjacencyLists[i].Add(j);
			_adjacencyLists[i].TrimExcess();
		}
	}
	
	public IEnumerable<int> FordFulkersonAlgorithm()
	{
		var path = FindPathToNode(0, NumberOfNodes - 1, new NodeStack(NumberOfNodes));
		while (path.Count > 1)
		{
			for (var i = 0; i < path.Count - 1; i++)
			{
				GetEdge(path[i], path[i + 1]).SendFlow(1);
				GetEdge(path[i + 1], path[i]).ReceiveFlow(1);
			}
			path = FindPathToNode(0, NumberOfNodes - 1, new NodeStack(NumberOfNodes));
		}
		
		var greatestMatching = GetReverseFreeEdges();
		RestoreEdges();
	
		return greatestMatching;
	}
	
	private IList<int> FindPathToNode(int startNode, int targetNode, INodeStorage nodesStorage)
	{
		var path = new List<int>(NumberOfNodes);
		
		var searchRoute = NodeSearch(startNode, targetNode, nodesStorage);
		if (searchRoute.Count < 1)
		{
			path.TrimExcess();
			
			return path;
		}
		
		var node = searchRoute[^1];
		path.Add(node);
		
		for (var i = searchRoute.Count - 2; i > -1; i--)
		{
			if (!_adjacencyMatrix[searchRoute[i], node])
				continue;
			node = searchRoute[i];
			path.Add(node);
		}
		
		path.TrimExcess();
		path.Reverse();
		
		return path;
	}
	
	private IList<int> NodeSearch(int startNode, int targetNode, INodeStorage nodesStorage)
	{
		var visited = new bool[NumberOfNodes];
		
		nodesStorage.Insert(startNode);
		
		var visitedNodes = new List<int>(NumberOfNodes);
		
		while (!nodesStorage.IsEmpty())
		{
			var currentNode = nodesStorage.GetFirst();
			
			if (visited[currentNode])
				continue;
			
			visited[currentNode] = true;
			visitedNodes.Add(currentNode);
			
			if (currentNode == targetNode)
				return visitedNodes;
			
			var neighbours = GetAdjacentNodes(currentNode);
			foreach (var nodeToGo in neighbours)
			{
				if (visited[nodeToGo] || GetEdge(currentNode, nodeToGo).IsBusy())
					continue;
				
				nodesStorage.Insert(nodeToGo);
			}
		}
		
		return new List<int>(0);
	}
	
	private IList<int> GetReverseFreeEdges()
	{
		var edges = new List<int>(NumberOfNodes);
		
		for (var i = 0; i < NumberOfNodes; i++)
		{
			for (var j = 0; j < NumberOfNodes; j++)
			{
				if (!GetEdge(i, j).IsBusy() && GetEdge(i, j).IsReverse)
				{
					edges.Add(i);
					edges.Add(j);
				}
			}
		}
		
		return edges;
	}
	
	private void RestoreEdges()
	{
		for (var i = 0; i < NumberOfNodes; i++)
			for (var j = 0; j < NumberOfNodes; j++)
				GetEdge(i, j).ResetСapacity();
	}
	
	private IEnumerable<int> GetAdjacentNodes(int node) => _adjacencyLists[node];
	private Edge GetEdge(int startingNode, int endNode) => _edges[startingNode, endNode];

	private int NumberOfNodes => _adjacencyMatrix.GetLength(0);

	private bool[,] _adjacencyMatrix;
	private List<List<int>> _adjacencyLists;
	private Edge[,] _edges;
}