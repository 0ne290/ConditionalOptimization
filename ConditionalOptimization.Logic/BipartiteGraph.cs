using ConditionalOptimization.Logic.Contracts;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace ConditionalOptimization.Logic;

public class BipartiteGraph
{
	public BipartiteGraph(bool[,] bipartiteGraphAdjacencyMatrix)
	{
		if (bipartiteGraphAdjacencyMatrix.GetLength(0) != bipartiteGraphAdjacencyMatrix.GetLength(1))
			throw new Exception("The adjacency matrix must be square");
		
		_adjacencyMatrix = new bool[bipartiteGraphAdjacencyMatrix.GetLength(0) * 2 + 2, bipartiteGraphAdjacencyMatrix.GetLength(0) * 2 + 2];
		_edges = new Edge[NumberOfNodes, NumberOfNodes];
		_transportNetworkAdjacencyLists = new List<List<int>>(NumberOfNodes);
		_graphAdjacencyLists = new List<List<int>>(NumberOfNodes);
		
		_rightPartIndex = bipartiteGraphAdjacencyMatrix.GetLength(0) + 1;
		_drainIndex = NumberOfNodes - 1;
		
		var directedGraphAdjacencyMatrix = CreateAdjacencyMatrix(bipartiteGraphAdjacencyMatrix);
		CreateEdges(directedGraphAdjacencyMatrix);
		CreateAdjacencyLists();
	}

	private bool[,] CreateAdjacencyMatrix(bool[,] bipartiteGraphAdjacencyMatrix)
	{
		var directedGraphAdjacencyMatrix = new bool[NumberOfNodes, NumberOfNodes];
		
		for (var i = _rightPartIndex; i < _drainIndex; i++)
		{
			directedGraphAdjacencyMatrix[i, _drainIndex] = true;
			_adjacencyMatrix[i, _drainIndex] = true;
			_adjacencyMatrix[_drainIndex, i] = true;
		}

		var k = 0;
		for (var i = LeftPartIndex; i < _rightPartIndex; i++)
		{
			directedGraphAdjacencyMatrix[SourceIndex, i] = true;
			_adjacencyMatrix[SourceIndex, i] = true;
			_adjacencyMatrix[i, SourceIndex] = true;
			for (var j = _rightPartIndex; j < _drainIndex; j++)
			{
				directedGraphAdjacencyMatrix[i, j] = bipartiteGraphAdjacencyMatrix[k, j - _rightPartIndex];
				_adjacencyMatrix[i, j] = bipartiteGraphAdjacencyMatrix[k, j - _rightPartIndex];
				_adjacencyMatrix[j, i] = bipartiteGraphAdjacencyMatrix[k, j - _rightPartIndex];
			}
			k++;
		}

		return directedGraphAdjacencyMatrix;
	}

	private void CreateEdges(bool[,] directedGraphAdjacencyMatrix)
	{
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
		for (var i = 0; i < NumberOfNodes; i++)
		{
			_transportNetworkAdjacencyLists.Add(new List<int>(NumberOfNodes));
			_graphAdjacencyLists.Add(new List<int>(NumberOfNodes));
			for (var j = 0; j < NumberOfNodes; j++)
			{
				if (_adjacencyMatrix[i, j])
				{
					_transportNetworkAdjacencyLists[i].Add(j);
					if (j != 0 && j != NumberOfNodes - 1)
						_graphAdjacencyLists[i].Add(j);
				}
			}
			_transportNetworkAdjacencyLists[i].TrimExcess();
		}
		
		_graphAdjacencyLists[0] = new List<int>(0);
		_graphAdjacencyLists[NumberOfNodes - 1] = new List<int>(0);
	}
	
	public IList<int> FordFulkersonAlgorithm()
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
		
		var searchRoute = NodeSearch(startNode, targetNode, _transportNetworkAdjacencyLists, nodesStorage);
		if (searchRoute[^1] != targetNode)
			return new List<int>(0);
		
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
	
	private IList<int> NodeSearch(int startNode, int targetNode, IReadOnlyList<IReadOnlyList<int>> adjacencyLists, INodeStorage nodesStorage)
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
			
			var neighbours = adjacencyLists[currentNode];
			foreach (var nodeToGo in neighbours)
			{
				if (visited[nodeToGo] || GetEdge(currentNode, nodeToGo).IsBusy())
					continue;
				
				nodesStorage.Insert(nodeToGo);
			}
		}
		
		return visitedNodes;
	}
	
	private IList<int> GetReverseFreeEdges()
	{
		var edges = new List<int>(NumberOfNodes);
		
		for (var i = LeftPartIndex; i < _rightPartIndex; i++)
		{
			for (var j = _rightPartIndex; j < _drainIndex; j++)
			{
				if (!GetEdge(j, i).IsBusy() && GetEdge(j, i).IsReverse)
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

	public ICollection<int> SearchMinimumVertexCover(IList<int> greatestMatching)
	{
		for (var i = 0; i < greatestMatching.Count; i += 2)
		{
			GetEdge(greatestMatching[i], greatestMatching[i + 1]).SendFlow(1);
			GetEdge(greatestMatching[i + 1], greatestMatching[i]).ReceiveFlow(1);
		}

		var leftUnvisitedNodes = new HashSet<int>(_rightPartIndex - LeftPartIndex);
		var minimumVertexCover = leftUnvisitedNodes;
		for (var i = LeftPartIndex; i < _rightPartIndex; i++)
			leftUnvisitedNodes.Add(i);
		
		var rightVisitedNodes = new HashSet<int>();
		
		for (var i = LeftPartIndex; i < _rightPartIndex; i++)
		{
			if (greatestMatching.Contains(i))
				continue;
			var searchRoute = NodeSearch(i, -1, _graphAdjacencyLists, new NodeStack(NumberOfNodes));
			foreach (var node in searchRoute)
			{
				leftUnvisitedNodes.Remove(node);
				if (node >= _rightPartIndex)
					rightVisitedNodes.Add(node);
			}
		}
		
		RestoreEdges();
		
		leftUnvisitedNodes.UnionWith(rightVisitedNodes);
		return minimumVertexCover;
	}
	
	private Edge GetEdge(int startingNode, int endNode) => _edges[startingNode, endNode];

	private int NumberOfNodes => _adjacencyMatrix.GetLength(0);

	private readonly bool[,] _adjacencyMatrix;
	private readonly List<List<int>> _transportNetworkAdjacencyLists;
	private readonly List<List<int>> _graphAdjacencyLists;
	private readonly Edge[,] _edges;

	private readonly int _rightPartIndex;
	private readonly int _drainIndex;
	private const int LeftPartIndex = 1;
	private const int SourceIndex = 0;
}