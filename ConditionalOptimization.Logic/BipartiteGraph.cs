using ConditionalOptimization.Logic.Contracts;

namespace ConditionalOptimization.Logic;

public class BipartiteGraph
{
    public BipartiteGraph(List<List<bool>> adjacencyMatrix)
    {
	    
	    _adjacencyMatrix = new List<List<bool>>(adjacencyMatrix);
	    _adjacencyMatrix.Insert(0, new List<bool>(NumberOfNodes));
	    _adjacencyMatrix.Add(new List<bool>(NumberOfNodes));
		foreach (var nodes in adjacencyMatrix)
		{
			_adjacencyMatrix[0].Add(true);
			
			if (nodes.Count != adjacencyMatrix.Count)
				throw new Exception("The incidence matrix must be square");
		}
		_adjacencyMatrix = new List<List<bool>>(adjacencyMatrix);
		
		_adjacencyLists = new List<List<int>>(NumberOfNodes * NumberOfNodes);
		for (var i = 0; i < NumberOfNodes; i++)
		{
			_adjacencyLists.Add(new List<int>(NumberOfNodes));
			for (var j = 0; j < NumberOfNodes; j++)
				if (adjacencyMatrix[i][j])
					_adjacencyLists[i].Add(j);
			_adjacencyLists[i].TrimExcess();
		}
		
		_edges = new Edge[NumberOfNodes][];
		for (var i = 0; i < NumberOfNodes; i++)
		{
			_edges[i] = new Edge[NumberOfNodes];
			for (var j = 0; j < NumberOfNodes; j++)
				_edges[i][j] = new Edge();
		}
	}

    public IEnumerable<int> FordFulkersonAlgorithm
    {
	    
    }
    public IEnumerable<int> FindPathToNode(int startNode, int targetNode, INodeStorage nodesStorage)
    {
	    var searchRoute = NodeSearch(startNode, targetNode, nodesStorage);
	    var path = new List<int>(NumberOfNodes);
	    var node = searchRoute[^1];
	    path.Add(node);
		
	    for (var i = searchRoute.Count - 2; i > -1; i--)
	    {
		    if (!_adjacencyMatrix[searchRoute[i]][node])
			    continue;
		    node = searchRoute[i];
		    path.Add(node);
	    }
		
	    path.TrimExcess();
		
	    return path;
    }
	private IList<int> NodeSearch(int startNode, int targetNode, INodeStorage nodesStorage)
	{
		var visited = new bool[NumberOfNodes];
		
		var dist = new int[NumberOfNodes];
		Array.Fill(dist, int.MaxValue);
		dist[startNode] = 0;
		
		nodesStorage.Insert(startNode);
		
		var visitedNodes = new List<int>(NumberOfNodes);
		
		while (!nodesStorage.IsEmpty())
		{
			var currentNode = nodesStorage.GetFirst();
			visitedNodes.Add(currentNode);
		
			if (currentNode == targetNode)
				return visitedNodes;
		
			var neighbours = GetAdjacentNodes(currentNode);
			foreach (var nodeToGo in neighbours)
			{
				if (!visited[nodeToGo])
				{
					visited[nodeToGo] = true;
		
					dist[nodeToGo] = dist[currentNode] + GetEdge(currentNode, nodeToGo).Weight;
					
					nodesStorage.Insert(nodeToGo);
				}
				else
				{
					var weightFromCurrentNode = GetEdge(nodeToGo, currentNode).Weight;
					if (dist[currentNode] + weightFromCurrentNode < dist[nodeToGo])
						dist[nodeToGo] = dist[currentNode] + weightFromCurrentNode;
				}
			}
		}
		
		return visitedNodes;
	}

	private IEnumerable<int> GetAdjacentNodes(int node) => _adjacencyLists[node];
	//private IEnumerable<Edge> GetIncidentEdges(int node) => _edges[node];
	private Edge GetEdge(int startingNode, int endNode) => _edges[startingNode][endNode];

	private int NumberOfNodes => _adjacencyMatrix.Count;

    private readonly List<List<bool>> _adjacencyMatrix;
	private readonly List<List<int>> _adjacencyLists;
	private readonly Edge[][] _edges;
}