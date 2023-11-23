using System.Xml;
using ConditionalOptimization.Logic.Contracts;

namespace ConditionalOptimization.Logic;

public class BipartiteGraph
{
	public BipartiteGraph(bool[,] adjacencyList)
	{
		if (adjacencyList.GetLength(0) != adjacencyList.GetLength(1))
			throw new Exception("The adjacency matrix must be square");
		
		_graphAdjacencyMatrix = new bool[adjacencyList.GetLength(0) * 2 + 2, adjacencyList.GetLength(0) * 2 + 2];
		_transportNetworkAdjacencyMatrix = new bool[NumberOfNodes, NumberOfNodes];
		
		var rightPartIndex = adjacencyList.GetLength(0) + 1;
		var leftPartIndex = 1;
		var drainIndex = NumberOfNodes - 1;
		var sourceIndex = 0;
		
		for (var i = rightPartIndex; i < drainIndex; i++)
		{
			_graphAdjacencyMatrix[i, drainIndex] = true;
			_transportNetworkAdjacencyMatrix[i, drainIndex] = true;
			_transportNetworkAdjacencyMatrix[drainIndex, i] = true;
		}

		var k = 0;
		for (var i = leftPartIndex; i < rightPartIndex; i++)
		{
			_graphAdjacencyMatrix[sourceIndex, i] = true;
			_transportNetworkAdjacencyMatrix[sourceIndex, i] = true;
			_transportNetworkAdjacencyMatrix[i, sourceIndex] = true;
			for (var j = rightPartIndex; j < drainIndex; j++)
			{
				_graphAdjacencyMatrix[i, j] = adjacencyList[k, j - rightPartIndex];
				_transportNetworkAdjacencyMatrix[i, j] = adjacencyList[k, j - rightPartIndex];
				_transportNetworkAdjacencyMatrix[j, i] = adjacencyList[k, j - rightPartIndex];
			}
			k++;
		}
		
		_edges = new Edge[NumberOfNodes, NumberOfNodes];
		for (var i = 0; i < NumberOfNodes; i++)
		{
			for (var j = 0; j < NumberOfNodes; j++)
			{
				if (_graphAdjacencyMatrix[i, j])
				{
					_edges[i, j] = new Edge(1, false);
					_edges[j, i] = new Edge(0, true);
				}
				else if (!_transportNetworkAdjacencyMatrix[i, j])
					_edges[i, j] = new Edge();
			}
		}
		
		_graphAdjacencyLists = new List<List<int>>(NumberOfNodes);
		_transportNetworkAdjacencyLists = new List<List<int>>(NumberOfNodes);
		for (var i = 0; i < NumberOfNodes; i++)
		{
			_graphAdjacencyLists.Add(new List<int>(NumberOfNodes));
			_transportNetworkAdjacencyLists.Add(new List<int>(NumberOfNodes));
			for (var j = 0; j < NumberOfNodes; j++)
			{
				if (_graphAdjacencyMatrix[i, j])
					_graphAdjacencyLists[i].Add(j);
				if (_transportNetworkAdjacencyMatrix[i, j])
					_transportNetworkAdjacencyLists[i].Add(j);
			}
			_graphAdjacencyLists[i].TrimExcess();
			_transportNetworkAdjacencyLists[i].TrimExcess();
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
			if (!_transportNetworkAdjacencyMatrix[searchRoute[i], node])
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
	
	private IEnumerable<int> GetAdjacentNodes(int node) => _transportNetworkAdjacencyLists[node];
	private Edge GetEdge(int startingNode, int endNode) => _edges[startingNode, endNode];
	
	public int NumberOfNodes => _graphAdjacencyMatrix.GetLength(0);
	
	private readonly bool[,] _graphAdjacencyMatrix;
	private readonly bool[,] _transportNetworkAdjacencyMatrix;
	private readonly List<List<int>> _graphAdjacencyLists;
	private readonly List<List<int>> _transportNetworkAdjacencyLists;
	private readonly Edge[,] _edges;
}