namespace ConditionalOptimization.Logic;

public class Vertex
{
	public Vertex()
	{
		Id = _counter;
		_counter++;
		_adjacentVertices = new List<Vertex>(10);
		_invertedEdges = new List<Vertex>(10);
		Visited = false;
	}

	public void AddAnUndirectedEdge(Vertex vertex)
	{
		AddStraightEdge(vertex);
		AddBackEdge(vertex);
	}
	public void AddBackEdge(Vertex vertex)
	{
		vertex.AddStraightEdge(this);
	}
	public void AddInvertedEdge(Vertex vertex)
	{
		//if (!vertex.AdjacentVertices.Contains(this))
		//	throw new Exception("Adding an inverted edge is only possible in place of an existing edge");
		vertex.DeleteEdge(this);
		AddBackEdge(vertex);
		_invertedEdges.Add(vertex);
	}
	public void AddStraightEdge(Vertex vertex)
	{
		if (_adjacentVertices.Contains(vertex))
			throw new Exception("The specified vertex is already adjacent to this vertex");
		_adjacentVertices.Add(vertex);
	}
	
	public void InvertEdge(Vertex adjacentVertex)
	{
		adjacentVertex.AddInvertedEdge(this);
	}
	public bool DeleteEdge(Vertex adjacentVertex) => _adjacentVertices.Remove(adjacentVertex);

	public void RemoveEdges()
	{
		_invertedEdges.Clear();
		_adjacentVertices.Clear();
	}

	public bool EdgeIsInverted(Vertex vertex) => _invertedEdges.Contains(vertex);
	
	public int Id { get; }
	public List<Vertex> AdjacentVertices => new(_adjacentVertices);
	public bool Visited { get; set; }

	private List<Vertex> _invertedEdges;
	private readonly List<Vertex> _adjacentVertices;
	private static int _counter = 1;
}