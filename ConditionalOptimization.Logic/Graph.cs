using ConditionalOptimization.Logic.Contracts;

namespace ConditionalOptimization.Logic;

public class Graph
{
	private Graph(bool[,] adjacencyMatrix)
	{
		if (adjacencyMatrix.GetLength(0) != adjacencyMatrix.GetLength(1))
			throw new Exception("The adjacency matrix must be square");
		_adjacencyMatrix = (bool[,])adjacencyMatrix.Clone();
		
		_adjacencyLists = new List<List<int>>(NumberOfNodes);
		for (var i = 0; i < NumberOfNodes; i++)
		{
			_adjacencyLists.Add(new List<int>(NumberOfNodes));
			for (var j = 0; j < NumberOfNodes; j++)
				if (adjacencyMatrix[i, j])
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

	public static Graph CreateGraph(bool[,] adjacencyMatrix) => new Graph(adjacencyMatrix);
	public static Graph CreateBipartiteGraph(bool[][] adjacencyList)
	{
		var dimensionList = adjacencyList.Length;
		var dimensionArray = dimensionList * 2 + 2;
		var adjacencyArray = new bool[dimensionArray, dimensionArray];
		
		for (var i = 1; i < dimensionList + 1; i++)
			adjacencyArray[0, i] = true;
		
		for (var i = dimensionList + 1; i < dimensionArray - 1; i++)
			adjacencyArray[dimensionArray - 1, i] = true;

		var k = 0;
		for (var i = 1; i < dimensionList + 1; i++)
		{
			var array = adjacencyList[k];
			k++;
			if (array.Length != dimensionList)
				throw new Exception("The adjacency matrix must be square");
			adjacencyArray[i, 0] = true;
			for (var j = dimensionList + 1; j < dimensionArray - 1; j++)
				adjacencyArray[i, j] = array[j - (dimensionList + 1)];
		}

		k = 0;
		for (var i = dimensionList + 1; i < dimensionArray - 1; i++)
		{
			adjacencyArray[i, dimensionArray - 1] = true;
			for (var j = 1; j < dimensionList + 1; j++)
				adjacencyArray[i, j] = adjacencyList[j - 1][k];
			k++;
		}
		
		return new Graph(adjacencyArray);
	}

    //public IEnumerable<int> FordFulkersonAlgorithm
    //{
	//    
    //}
    public IEnumerable<int> FindPathToNode(int startNode, int targetNode, INodeStorage nodesStorage)
    {
	    var searchRoute = NodeSearch(startNode, targetNode, nodesStorage);
	    var path = new List<int>(NumberOfNodes);
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
		
	    return path;
    }
	public IList<int> NodeSearch(int startNode, int targetNode, INodeStorage nodesStorage)
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
				if (visited[nodeToGo])
					continue;
				
				nodesStorage.Insert(nodeToGo);
			}
		}
		
		return visitedNodes;
	}

	private IEnumerable<int> GetAdjacentNodes(int node) => _adjacencyLists[node];
	//private IEnumerable<Edge> GetIncidentEdges(int node) => _edges[node];
	//private Edge GetEdge(int startingNode, int endNode) => _edges[startingNode][endNode];

	public int NumberOfNodes => _adjacencyMatrix.GetLength(0);

    private readonly bool[,] _adjacencyMatrix;
	private readonly List<List<int>> _adjacencyLists;
	private readonly Edge[][] _edges;
}