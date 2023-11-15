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
        _edges = new List<Edge>(_adjacencyMatrix.GetLength(0) * _adjacencyMatrix.GetLength(0));
        Source = new Vertex();
        Drain = new Vertex();
        
        CreateGraph();
	}
	private void CreateGraph()
	{
		CreateVertices();
		CreateEdges();
	}
	private void CreateVertices()
	{
		for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
		{
			_leftVertices[i] = new Vertex();
			var edge = new Edge(Source, _leftVertices[i]);
			edge.ConnectVertices();
			
			_rightVertices[i] = new Vertex();
			edge = new Edge(_rightVertices[i], Drain);
			edge.ConnectVertices();
		}
	}
	private void CreateEdges()
	{
		for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
		{
			for (int j = 0; j < _adjacencyMatrix.GetLength(0); j++)
			{
				if (_adjacencyMatrix[i, j])
				{
					var edge = new Edge(_leftVertices[i], _rightVertices[j]);
					_edges.Add(edge);
					edge.ConnectVertices();
				}
			}
		}
	}

	/*public List<Vertex> SearchMinimumVertexCoverOfAGraph(List<Vertex> greatestMatching)
	{
		if (!ValidateVertices(greatestMatching))
			throw new Exception("Vertices must be part of the graph and must not be equal");
		List<Vertex> minimumVertexCover = new List<Vertex>(20);
		
		for (int i = 0; i < greatestMatching.Count - 1; i++)
			greatestMatching[i].InvertEdge(greatestMatching[i + 1]);

		var leftVerticesExceptGreatestMatching = _leftVertices.Except(greatestMatching);
		foreach (var vertex in leftVerticesExceptGreatestMatching)
			minimumVertexCover.AddRange(DepthFirstSearch(vertex));

		minimumVertexCover = minimumVertexCover.Distinct().ToList();
		var unvisitedLeftVertices = _leftVertices.Except(minimumVertexCover.Intersect(_leftVertices));
		minimumVertexCover.RemoveAll(vertex => _leftVertices.Contains(vertex));
		minimumVertexCover.AddRange(unvisitedLeftVertices);
		
		RestoreEdges();

		return minimumVertexCover;
	}
	public List<Vertex> SearchMinimumVertexCoverOfAGraph()
	{
		var greatestMatching = FordFulkersonAlgorithm();
		List<Vertex> minimumVertexCover = new List<Vertex>(20);
		
		for (int i = 0; i < greatestMatching.Count - 1; i++)
			greatestMatching[i].InvertEdge(greatestMatching[i + 1]);

		var leftVerticesExceptGreatestMatching = _leftVertices.Except(greatestMatching);
		foreach (var vertex in leftVerticesExceptGreatestMatching)
			minimumVertexCover.AddRange(DepthFirstSearch(vertex));

		minimumVertexCover = minimumVertexCover.Distinct().ToList();
		var unvisitedLeftVertices = _leftVertices.Except(minimumVertexCover.Intersect(_leftVertices));
		minimumVertexCover.RemoveAll(vertex => _leftVertices.Contains(vertex));
		minimumVertexCover.AddRange(unvisitedLeftVertices);

		RestoreEdges();

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
	}*/
	
	public void FordFulkersonAlgorithm()
	{
		var forks = new Stack<Vertex>();
		forks.Push(Source);
		var straightEdges = new Stack<Edge>();
		var backEdges = new Stack<Edge>();
		Vertex fork;
		List<Vertex> visitedVertices = new List<Vertex>(20);

		while (forks.Count > 0)
		{
			if (straightEdges.TryPop(out var straightEdge))
				straightEdge.Flow += 1;
			else if (backEdges.TryPop(out var backEdge))
				backEdge.Flow -= 1;
			fork = forks.Pop();
			
			if (fork == Drain)
			{
				visitedVertices.ForEach(vertex => vertex.Visited = false);
				visitedVertices.Clear();
				forks.Clear();
				forks.Push(Source);
				straightEdges.Clear();
				backEdges.Clear();
			}
			
			if (!fork.Visited)
			{
				fork.Visited = true;
				visitedVertices.Add(fork);
				
				fork.OutgoingEdges.ForEach(edge =>
				{
					if (edge.Flow < edge.Capacity)
					{
						var vertex = edge.End;
						if (!vertex.Visited)
						{
							forks.Push(vertex);
							straightEdges.Push(edge);
						}
					}
				});
				fork.IncomingEdges.ForEach(edge =>
				{
					if (edge.Flow == edge.Capacity)
					{
						var vertex = edge.Start;
						if (!vertex.Visited)
						{
							forks.Push(vertex);
							backEdges.Push(edge);
						}
					}
				});
			}
		}
	}
	/*private void RestoreEdges()
	{
		Source.RemoveEdges();
		Array.ForEach(_leftVertices, vertex => vertex.RemoveEdges());
		Array.ForEach(_rightVertices, vertex => vertex.RemoveEdges());
		Drain.RemoveEdges();

		ConnectVertices();
	}*/
	
	public List<Vertex> DepthFirstSearch(Vertex startingVertex)
	{
		if (!ValidateVertex(startingVertex))
			throw new Exception("Vertex must be part of the graph");
		
		var forks = new Stack<Vertex>();
		forks.Push(startingVertex);
		Vertex fork;
		List<Vertex> path = new List<Vertex>(20);

		while (forks.Count > 0)
		{
			fork = forks.Pop();
			if (!fork.Visited)
			{
				fork.Visited = true;
				path.Add(fork);
				
				fork.OutgoingEdges.ForEach(edge =>
				{
					var vertex = edge.End;
					if (!vertex.Visited)
						forks.Push(vertex);
				});
			}
		}

		path.ForEach(vertex => vertex.Visited = false);
			
		path.Remove(startingVertex);

		return path;
	}
	public List<Vertex> DepthFirstSearch(Vertex startingVertex, Vertex endVertex)
	{
		if (startingVertex == endVertex || !ValidateVertex(startingVertex) || !ValidateVertex(endVertex))
			throw new Exception("Vertices must be part of the graph and must not be equal");
		
		var forks = new Stack<Vertex>();
		forks.Push(startingVertex);
		Vertex fork;
		List<Vertex> path = new List<Vertex>(20);

		while (forks.Count > 0)
		{
			fork = forks.Pop();
			if (!fork.Visited)
			{
				fork.Visited = true;
				path.Add(fork);

				if (fork == endVertex)
					break;
				
				fork.OutgoingEdges.ForEach(edge =>
				{
					var vertex = edge.End;
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

	public IEnumerable<Edge> GetLoadedEdges() => _edges.Where(e => e.Flow == e.Capacity);
	
	public bool[,] AdjacencyMatrix => (bool[,])_adjacencyMatrix.Clone();
	public List<Edge> Edges => new(_edges);
	public Vertex[] LeftVertices => (Vertex[])_leftVertices.Clone();
	public Vertex[] RightVertices => (Vertex[])_rightVertices.Clone();
	public Vertex Source { get; }
	public Vertex Drain { get; }
	
	private bool[,] _adjacencyMatrix;
	private List<Edge> _edges;
	private Vertex[] _leftVertices, _rightVertices;
}
