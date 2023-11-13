namespace ConditionalOptimization.Logic;

public class BipartiteGraph
{
	public BipartiteGraph(bool[,] adjacencyMatrix)
	{
		if (adjacencyMatrix.GetLength(0) != adjacencyMatrix.GetLength(1))
			throw new Exception("The incidence matrix must be square");
		_adjacencyMatrix = (bool[,])adjacencyMatrix.Clone();

		_leftVertices = new Vertex[_adjacencyMatrix.GetLength(0)];
        _rightVertices = new Vertex[_adjacencyMatrix.GetLength(0)];
        Source = new Vertex();
        Drain = new Vertex();
        
        CreateGraph();
	}
	private void CreateGraph()
	{
		CreateVertices();
		ConnectVertices();
	}
	private void CreateVertices()
	{
		for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
		{
			_leftVertices[i] = new Vertex();
			Source.AddStraightEdge(_leftVertices[i]);
			_rightVertices[i] = new Vertex();
			_rightVertices[i].AddStraightEdge(Drain);
		}
	}
	private void ConnectVertices()
	{
		for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
			for (int j = 0; j < _adjacencyMatrix.GetLength(0); j++)
				if (_adjacencyMatrix[i, j])
					_leftVertices[i].AddStraightEdge(_rightVertices[j]);
	}

	public List<Vertex> SearchMinimumVertexCoverOfAGraph(List<Vertex> maximumMatching)
	{
		if (ValidateVertices(maximumMatching))
			throw new Exception("Vertices must be part of the graph and must not be equal");
		List<Vertex> minimumVertexCover = new List<Vertex>();

		return minimumVertexCover;
	}
	public List<Vertex> SearchMinimumVertexCoverOfAGraph()
	{
		var maximumMatching = FordFulkersonAlgorithm();
		List<Vertex> minimumVertexCover = new List<Vertex>();

		return minimumVertexCover;
	}
	private bool ValidateVertices(List<Vertex> vertices)
	{
		var duplicates = vertices.GroupBy(v => v)
			.Where(g => g.Count() > 1)
			.Select(x => x.Key);
		
		if (duplicates.Any())
			return false;
		
		foreach (var vertex in vertices)
			if (vertex != Source && vertex != Drain && !_leftVertices.Contains(vertex) && !_rightVertices.Contains(vertex))
				return false;
		
		return true;
	}
	
	public List<Vertex> FordFulkersonAlgorithm()
	{
		var copyOfGraph = new BipartiteGraph(AdjacencyMatrix);
		var path = DepthFirstSearch(Source, Drain);
		var maximumMatching = path;

		while (path.Count > 0)
		{
			maximumMatching = path;
			
			path.Intersect(copyOfGraph.LeftVertices).ToList().ForEach(leftVertex => copyOfGraph.Source.DeleteEdge(leftVertex));
			path.Intersect(copyOfGraph.RightVertices).ToList().ForEach(rightVertex => rightVertex.DeleteEdge(copyOfGraph.Drain));
			
			for (int i = 0; i < path.Count - 1; i++)
				path[i].InvertEdge(path[i + 1]);

			path = DepthFirstSearch(Source, Drain);
		}

		return maximumMatching;
	}
	public List<Vertex> DepthFirstSearch(Vertex startingVertex, Vertex endVertex)
	{
		if (startingVertex == endVertex || !ValidateVertex(startingVertex) || !ValidateVertex(endVertex))
			throw new Exception("Vertices must be part of the graph and must not be equal");
		
		var forks = new Stack<Vertex>();
		forks.Push(startingVertex);
		Vertex fork;
		List<Vertex> path = new List<Vertex>();

		while (forks.Count > 0)
		{
			fork = forks.Pop();
			if (!fork.Visited)
			{
				fork.Visited = true;
				path.Add(fork);

				if (fork == endVertex)
					break;
				
				fork.AdjacentVertices.ForEach(vertex =>
				{
					if (!vertex.Visited)
						forks.Push(vertex);
				});
			}
		}

		path.ForEach(vertex => vertex.Visited = false);
			
		path.Remove(startingVertex);
		path.Remove(endVertex);

		return path;
	}
	private bool ValidateVertex(Vertex vertex) =>
		vertex == Source || vertex == Drain || _leftVertices.Contains(vertex) || _rightVertices.Contains(vertex);
	
	public bool[,] AdjacencyMatrix => (bool[,])_adjacencyMatrix.Clone();
	public Vertex[] LeftVertices => (Vertex[])_leftVertices.Clone();
	public Vertex[] RightVertices => (Vertex[])_rightVertices.Clone();
	public Vertex Source { get; }
	public Vertex Drain { get; }
	
	private bool[,] _adjacencyMatrix;
	private Vertex[] _leftVertices, _rightVertices;
}
