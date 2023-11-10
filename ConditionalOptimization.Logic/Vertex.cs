namespace ConditionalOptimization.Logic;

public class Vertex
{
	public Vertex()
	{
		Id = _counter;
		_counter++;
		_adjacentVertices = new List<Vertex>();
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
	public void AddStraightEdge(Vertex vertex)
	{
		if (_adjacentVertices.Contains(vertex))
			throw new Exception("The specified vertex is already adjacent to this vertex");
		_adjacentVertices.Add(vertex);
	}
	
	public void InvertEdge(Vertex adjacentVertex)
	{
		DeleteEdge(adjacentVertex);
		AddBackEdge(adjacentVertex);
	}
	public bool DeleteEdge(Vertex adjacentVertex) => _adjacentVertices.Remove(adjacentVertex);
	
	public int Id { get; }
	public List<Vertex> AdjacentVertices => new(_adjacentVertices);
	
	public bool Visited { get; set; }

	private readonly List<Vertex> _adjacentVertices;
	
	private static int _counter = 1;
}
