namespace ConditionalOptimization.Logic;

public class BipartiteGraph
{
	public void ReconstructGraph(bool[,] adjacencyMatrix)
    {
        AdjacencyMatrix = adjacencyMatrix;
        var length = AdjacencyMatrix.GetLength(0);
        
        _leftVertices = new Vertex[length];
		_rightVertices = new Vertex[length];
		
        Source = new Vertex();
		Drain = new Vertex();
		
		for (var i = 0; i < length; i++)
		{
			_leftVertices[i] = new Vertex();
			Source.AddStraightEdge(_leftVertices[i]);
			_rightVertices[i] = new Vertex();
			_rightVertices[i].AddStraightEdge(Drain);
		}
		
		for (var i = 0; i < length; i++)
			for (var j = 0; j < length; j++)
				if (AdjacencyMatrix[i, j])
					_leftVertices[i].AddStraightEdge(_rightVertices[j]);
	}
	
	public List<Vertex> FordFulkersonAlgorithm()
	{
		var copyOfGraph = new BipartiteGraph();
		copyOfGraph.ReconstructGraph(AdjacencyMatrix);
        var path = DepthFirstSearch(copyOfGraph.Source!, copyOfGraph.Drain!);
		var maximumMatching = path;

		while (path.Count > 0)
		{
			maximumMatching = path;

            path.Intersect(copyOfGraph.LeftVertices).ToList().ForEach(leftVertex =>
            {
                Console.WriteLine(leftVertex.Id);
                copyOfGraph.Source!.DeleteEdge(leftVertex);
            });

            path.Intersect(copyOfGraph.RightVertices).ToList().ForEach(rightVertex => rightVertex.DeleteEdge(copyOfGraph.Drain!));
				
			for (var i = 0; i < path.Count - 1; i++)
				path[i].InvertEdge(path[i + 1]);

			path = DepthFirstSearch(copyOfGraph.Source!, copyOfGraph.Drain!);
		}

		return maximumMatching;
	}
	public List<Vertex> DepthFirstSearch(Vertex source, Vertex targetVertex)
	{
		var forks = new Stack<Vertex>();
        var path = new List<Vertex>();

        forks.Push(source);
		while (forks.Count > 0)
		{
			var fork = forks.Pop();
            if (fork.Visited) continue;
            fork.Visited = true;
            path.Add(fork);

            if (fork == targetVertex) break;

            fork.AdjacentVertices.ForEach(vertex =>
            {
                if (vertex.Visited) return;
                forks.Push(vertex);
            });
        }

        path.ForEach(vertex => vertex.Visited = false);
	
		path.Remove(source);
		path.Remove(targetVertex);

		return path;
	}

    public void PrintGraph(List<Vertex> graph) => graph.ForEach(vertex => Console.Write($"{vertex.Id} --> "));

    public bool[,] AdjacencyMatrix
	{
		get => (bool[,])_adjacencyMatrix!.Clone();
		set
		{
			if (value.GetLength(0) != value.GetLength(1))
				throw new Exception("The incidence matrix must be square");
			_adjacencyMatrix = value;
		}
	}

    public Vertex[] LeftVertices => (Vertex[])_leftVertices!.Clone();
	public Vertex[] RightVertices => (Vertex[])_rightVertices!.Clone();
    public Vertex? Source { get; private set; }
	public Vertex? Drain { get; private set; }
	
	private bool[,]? _adjacencyMatrix;
	private Vertex[]? _leftVertices, _rightVertices;
}
