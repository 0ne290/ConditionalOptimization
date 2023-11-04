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
	
	public List<IEdge> FordFulkersonAlgorithm()
	{
		List<IEdge> maximumMatching;
		
		maximumMatching = DeepFirstSearch(ref maximumMatching)
	}
	private List<IEdge> DeepFirstSearch(IVertex vertex, IVertex desiredIVertex, Predicate<IVertex> skipCondition, ref List<IVertex> path)
	{
		foreach (var edge in vertex.Edges)
			if (edge.DestinationVertex.Flow != edge.DestinationVertex.Capacity)
				{
					Console.WriteLine($"{edge.DestinationVertex.Flow} {edge.DestinationVertex.Capacity} {edge.DestinationVertex.Id}");
					DeepFirstSearch(edge.DestinationVertex, desiredIVertex, skipCondition, ref path);
				}
		
		path.Add(vertex);
		if (vertex != desiredIVertex)
			foreach (var edge in vertex.Edges)
				if (!skipCondition(edge.DestinationVertex))
				{
					Console.WriteLine($"{edge.DestinationVertex.Flow} {edge.DestinationVertex.Capacity} {edge.DestinationVertex.Id}");
					DeepFirstSearch(edge.DestinationVertex, desiredIVertex, skipCondition, ref path);
				}
	}
	
	public bool[,] AdjacencyMatrix
	{
		get => _adjacencyMatrix;
		set
		{
			if (value.GetLength(0) != value.GetLength(1))
				throw new Exception("The incidence matrix must be square");
			else
				_adjacencyMatrix = value;
		}
	}
	
	private bool[,] _adjacencyMatrix;
	private IVertex[] _leftVertices, _rightVertices;
	private IVertex _source, _drain;
}