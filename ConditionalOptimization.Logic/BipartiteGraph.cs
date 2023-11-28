using ConditionalOptimization.Logic.Contracts;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace ConditionalOptimization.Logic;

public class BipartiteGraph
{
	public BipartiteGraph(Table<double> bipartiteGraphAdjacencyMatrix)
	{
		_adjacencyMatrix = new bool[bipartiteGraphAdjacencyMatrix.Dimension * 2 + 2, bipartiteGraphAdjacencyMatrix.Dimension * 2 + 2];
		_edges = new Edge[NumberOfNodes, NumberOfNodes];
		_transportNetworkAdjacencyLists = new List<IList<int>>();
		_graphAdjacencyLists = new List<IList<int>>();
		
		RightPartIndex = bipartiteGraphAdjacencyMatrix.Dimension + 1;
		_drainIndex = NumberOfNodes - 1;

		_leftNodes = new HashSet<int>();
		for (var i = LeftPartIndex; i < RightPartIndex; i++)
			_leftNodes.Add(i);
		_rightNodes = new HashSet<int>();
		for (var i = RightPartIndex; i < _drainIndex; i++)
			_rightNodes.Add(i);
		
		var directedGraphAdjacencyMatrix = CreateAdjacencyMatrix(bipartiteGraphAdjacencyMatrix);
		CreateEdges(directedGraphAdjacencyMatrix);
		CreateAdjacencyLists();
	}

	private bool[,] CreateAdjacencyMatrix(Table<double> bipartiteGraphAdjacencyMatrix)
	{
		var directedGraphAdjacencyMatrix = new bool[NumberOfNodes, NumberOfNodes];
		
		for (var i = RightPartIndex; i < _drainIndex; i++)
		{
			directedGraphAdjacencyMatrix[i, _drainIndex] = true;
			_adjacencyMatrix[i, _drainIndex] = true;
			_adjacencyMatrix[_drainIndex, i] = true;
		}

		var k = 0;
		for (var i = LeftPartIndex; i < RightPartIndex; i++)
		{
			directedGraphAdjacencyMatrix[SourceIndex, i] = true;
			_adjacencyMatrix[SourceIndex, i] = true;
			_adjacencyMatrix[i, SourceIndex] = true;
			for (var j = RightPartIndex; j < _drainIndex; j++)
			{
				directedGraphAdjacencyMatrix[i, j] = !Convert.ToBoolean(bipartiteGraphAdjacencyMatrix.Rows[k][j - RightPartIndex].Value);
				_adjacencyMatrix[i, j] = !Convert.ToBoolean(bipartiteGraphAdjacencyMatrix.Rows[k][j - RightPartIndex].Value);
				_adjacencyMatrix[j, i] = !Convert.ToBoolean(bipartiteGraphAdjacencyMatrix.Rows[k][j - RightPartIndex].Value);
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
			_transportNetworkAdjacencyLists.Add(new List<int>());
			_graphAdjacencyLists.Add(new List<int>());
			for (var j = 0; j < NumberOfNodes; j++)
			{
				if (!_adjacencyMatrix[i, j])
					continue;
				_transportNetworkAdjacencyLists[i].Add(j);
				if (j != 0 && j != NumberOfNodes - 1)
					_graphAdjacencyLists[i].Add(j);
			}
		}
		
		_graphAdjacencyLists[0] = new List<int>(0);
		_graphAdjacencyLists[NumberOfNodes - 1] = new List<int>(0);
	}
	
	public IList<int> FordFulkersonAlgorithm()
	{
		var path = FindPathToNode(0, NumberOfNodes - 1, new NodeStack());
		while (path.Count > 1)
		{
			for (var i = 0; i < path.Count - 1; i++)
			{
				GetEdge(path[i], path[i + 1]).SendFlow(1);
				GetEdge(path[i + 1], path[i]).ReceiveFlow(1);
			}
			path = FindPathToNode(0, NumberOfNodes - 1, new NodeStack());
		}
		
		var greatestMatching = GetReverseFreeEdges();
		RestoreEdges();
	
		return greatestMatching;
	}
	
	private IList<int> FindPathToNode(int startNode, int targetNode, INodeStorage nodesStorage)
	{
		var path = new List<int>();
		
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
	
	private IList<int> NodeSearch(int startNode, int targetNode, IList<IList<int>> adjacencyLists, INodeStorage nodesStorage)
	{
		var visited = new bool[NumberOfNodes];
		
		nodesStorage.Insert(startNode);
		
		var visitedNodes = new List<int>();
		
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
		var edges = new List<int>();
		
		for (var i = LeftPartIndex; i < RightPartIndex; i++)
		{
			for (var j = RightPartIndex; j < _drainIndex; j++)
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

	public IDictionary<string, ISet<int>> SearchMinimumVertexCover(IList<int> greatestMatching)
	{
		for (var i = 0; i < greatestMatching.Count; i += 2)
		{
			GetEdge(greatestMatching[i], greatestMatching[i + 1]).SendFlow(1);
			GetEdge(greatestMatching[i + 1], greatestMatching[i]).ReceiveFlow(1);
		}

		var leftUnvisitedNodes = LeftNodes;
		var minimumVertexCover = new Dictionary<string, ISet<int>>();
		
		var rightVisitedNodes = new HashSet<int>();
		
		for (var i = LeftPartIndex; i < RightPartIndex; i++)
		{
			if (greatestMatching.Contains(i))
				continue;
			var searchRoute = NodeSearch(i, -1, _graphAdjacencyLists, new NodeStack());
			foreach (var node in searchRoute)
			{
				leftUnvisitedNodes.Remove(node);
				if (node >= RightPartIndex)
					rightVisitedNodes.Add(node);
			}
		}
		
		RestoreEdges();
		
		minimumVertexCover.Add("leftNodes", leftUnvisitedNodes);
		minimumVertexCover.Add("rightNodes", rightVisitedNodes);
		
		return minimumVertexCover;
	}
	
	private Edge GetEdge(int startingNode, int endNode) => _edges[startingNode, endNode];
	
	public ISet<int> LeftNodes => new HashSet<int>(_leftNodes);
	public ISet<int> RightNodes => new HashSet<int>(_rightNodes);
	public int RightPartIndex { get; }
	public static int LeftPartIndex => 1;
	private int NumberOfNodes => _adjacencyMatrix.GetLength(0);

	private readonly bool[,] _adjacencyMatrix;
	private readonly IList<IList<int>> _transportNetworkAdjacencyLists;
	private readonly IList<IList<int>> _graphAdjacencyLists;
	private readonly Edge[,] _edges;
	private readonly ISet<int> _leftNodes;
	private readonly ISet<int> _rightNodes;
	private readonly int _drainIndex;
	private const int SourceIndex = 0;
}