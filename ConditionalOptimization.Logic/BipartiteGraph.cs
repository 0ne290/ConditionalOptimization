namespace ConditionalOptimization.Logic;

public class BipartiteGraph
{
	public BipartiteGraph(bool[,] adjacencyMatrix)
	{
		if (adjacencyMatrix.GetLength(0) != adjacencyMatrix.GetLength(1))
			throw new Exception("The incidence matrix must be square");
		_adjacencyMatrix = adjacencyMatrix;
		NumberOfVertices = 2 * _adjacencyMatrix.GetLength(0) + 2;
		
		_leftVertices = new Vertex[_adjacencyMatrix.GetLength(0)];
		_rightVertices = new Vertex[_adjacencyMatrix.GetLength(0)];
		Source = new Vertex();
		Drain = new Vertex();
        
		for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
		{
			_leftVertices[i] = new Vertex();
			Source.AddStraightEdge(_leftVertices[i]);
			_rightVertices[i] = new Vertex();
			_rightVertices[i].AddStraightEdge(Drain);
		}
        
		for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
			for (int j = 0; j < _adjacencyMatrix.GetLength(0); j++)
				if (_adjacencyMatrix[i, j])
					_leftVertices[i].AddStraightEdge(_rightVertices[j]);
	}
	
	public List<Vertex> FordFulkersonAlgorithm()
	{
		var copyOfGraph = new BipartiteGraph(AdjacencyMatrix);
		var path = DepthFirstSearch(1, 2);
		var maximumMatching = path;

		while (path.Count > 0)
		{
			maximumMatching = path;
			
			foreach (var leftVertex in path.Intersect(copyOfGraph.LeftVertices))
				copyOfGraph.Source.DeleteEdge(leftVertex);
			foreach (var rightVertex in path.Intersect(copyOfGraph.RightVertices))
				rightVertex.DeleteEdge(copyOfGraph.Drain);
			
			for (int i = 0; i < path.Count - 1; i++)
				path[i].InvertEdge(path[i + 1]);

			path = DepthFirstSearch(1, 2);
		}

		return maximumMatching;
	}
	public List<Vertex> DepthFirstSearch(int startingVertexId, int endVertexId)
	{
		var startingVertex = FindAVertexById(startingVertexId);
		var endVertex = FindAVertexById(endVertexId);
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
	private Vertex FindAVertexById(int id)
	{
		if (id > NumberOfVertices || id < 1)
			throw new Exception("Vertex with the specified id is not contained in the graph");

		if (id == 1)
			return Source;
		if (id == 2)
			return Drain;
		if (id % 2 == 1)
			return _leftVertices[id % _leftVertices.Length];
		return _rightVertices[id % _rightVertices.Length];
	}
	
	public bool[,] AdjacencyMatrix => (bool[,])_adjacencyMatrix.Clone();
	public Vertex[] LeftVertices => (Vertex[])_leftVertices.Clone();
	public Vertex[] RightVertices => (Vertex[])_rightVertices.Clone();
	public Vertex Source { get; }
	public Vertex Drain { get; }
	public int NumberOfVertices { get; }
	
	private bool[,] _adjacencyMatrix;
	private Vertex[] _leftVertices, _rightVertices;
}
