using ConditionalOptimization.Interfaces;

namespace ConditionalOptimization.Logic;

public class BipartiteGraph
{
	public void ConstructGraph()
	{
		_leftVertices = new IVertex[AdjacencyMatrix.GetLength(0)];
		_rightVertices = new IVertex[AdjacencyMatrix.GetLength(0)];
		_source = new Vertex();
		_drain = new Vertex();
		
		for (int i = 0; i < AdjacencyMatrix.GetLength(0); i++)
		{
			_leftVertices[i] = new Vertex();
			_source.EdgeAdd(_leftVertices[i]);
			_rightVertices[i] = new Vertex();
			_rightVertices[i].EdgeAdd(_drain);
		}
		
		for (int i = 0; i < AdjacencyMatrix.GetLength(0); i++)
			for (int j = 0; j < AdjacencyMatrix.GetLength(0); j++)
				if (AdjacencyMatrix[i, j])
					_leftVertices[i].EdgeAdd(_rightVertices[j]);
	}
	
	public IEdge[]? FordFulkersonAlgorithm()
	{
		IEdge[]? predPath = null;
		var nextPath = new Stack<IEdge?>();
		var forks = new Stack<IVertex?>();
		var fork = _source;
		List<IEdge> edges;
		
		forks.Push(null);
		nextPath.Push(null);
		
		while (fork != null)
		{
			edges = fork.Edges;
			foreach (var edge in edges)
			{
				if (edge.Flow != edge.Capacity)
				{
					nextPath.Push(edge);
					forks.Push(fork);
					fork = edge.DestinationVertex;
					if (fork == _drain)
						break;
					edges = fork.Edges;
				}
			}

			predPath = nextPath.ToArray();
			fork = forks.Pop();
			nextPath.Pop();
		}

		return predPath;
	}
		
	public bool[,] AdjacencyMatrix
	{
		get => _adjacencyMatrix;
		set
		{
			if (value.GetLength(0) != value.GetLength(1))
				throw new Exception("The incidence matrix must be squarmmmmmm
			else
				_adjacencyMatrix = value;
		}
	}
	
	private bool[,] _adjacencyMatrix;
	private IVertex[] _leftVertices, _rightVertices;
	private IVertex _source, _drain;
}
