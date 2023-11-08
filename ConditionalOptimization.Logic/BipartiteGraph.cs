namespace ConditionalOptimization.Logic;

public class BipartiteGraph
{
	public BipartiteGraph(bool[,] adjacencyMatrix)
	{
		AdjacencyMatrix = adjacencyMatrix;
		ReconstructGraph();
	}
	
	public void ReconstructGraph()
	{
		_leftVertices = new Vertex[AdjacencyMatrix.GetLength(0)];
		_rightVertices = new Vertex[AdjacencyMatrix.GetLength(0)];
		Source = new Vertex();
		Drain = new Vertex();
		
		for (int i = 0; i < AdjacencyMatrix.GetLength(0); i++)
		{
			_leftVertices[i] = new Vertex();
			Source.AddStraightEdge(_leftVertices[i]);
			_rightVertices[i] = new Vertex();
			_rightVertices[i].AddStraightEdge(Drain);
		}
		
		for (int i = 0; i < AdjacencyMatrix.GetLength(0); i++)
			for (int j = 0; j < AdjacencyMatrix.GetLength(0); j++)
				if (AdjacencyMatrix[i, j])
					_leftVertices[i].AddStraightEdge(_rightVertices[j]);
	}
	
	public List<Vertex> FordFulkersonAlgorithm()
	{
		var copyOfGraph = new BipartiteGraph(AdjacencyMatrix);
		var path = DepthFirstSearch(copyOfGraph.Source, copyOfGraph.Drain);
		var maximumMatching = path;

		while (path.Count > 0)
		{
			maximumMatching = path;
			
			foreach (var leftVertex in path.Intersect(copyOfGraph.LeftVertices))
			{
				Console.WriteLine(leftVertex.Id);
				copyOfGraph.Source.DeleteEdge(leftVertex);
			}
			foreach (var rightVertex in path.Intersect(copyOfGraph.RightVertices))
				rightVertex.DeleteEdge(copyOfGraph.Drain);
			for (int i = 0; i < path.Count - 1; i++)
				path[i].InvertEdge(path[i + 1]);

			path = DepthFirstSearch(copyOfGraph.Source, copyOfGraph.Drain);
		}

		return maximumMatching;
	}
	public List<Vertex> DepthFirstSearch(Vertex source, Vertex targetVertex)
	{
		var forks = new Stack<Vertex>();
		forks.Push(source);
		Vertex fork;
		List<Vertex> path = new List<Vertex>();

		while (forks.Count > 0)
		{
			fork = forks.Pop();
			if (!fork.Visited)
			{
				fork.Visited = true;
				path.Add(fork);

				if (fork == targetVertex)
					break;

				foreach (var vertex in fork.AdjacentVertices)
					if (!vertex.Visited)
						forks.Push(vertex);
			}
		}

		foreach (var vertex in path)
			vertex.Visited = false;
			
		path.Remove(source);
		path.Remove(targetVertex);

		return path;
	}
	
	public bool[,] AdjacencyMatrix
	{
		get => (bool[,])_adjacencyMatrix.Clone();
		set
		{
			if (value.GetLength(0) != value.GetLength(1))
				throw new Exception("The incidence matrix must be square");
			_adjacencyMatrix = value;
		}
	}
	public Vertex[] LeftVertices
	{
		get => (Vertex[])_leftVertices.Clone();
		private set => _leftVertices = value;
	}
	public Vertex[] RightVertices
	{
		get => (Vertex[])_rightVertices.Clone();
		private set => _rightVertices = value;
	}
	public Vertex Source { get; private set; }
	public Vertex Drain { get; private set; }
	
	private bool[,] _adjacencyMatrix;
	private Vertex[] _leftVertices, _rightVertices;
}
