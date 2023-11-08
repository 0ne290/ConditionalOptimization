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
	public void DeleteEdge(Vertex adjacentVertex)
    {
        if (_adjacentVertices.Remove(adjacentVertex)) return;
        Console.WriteLine("Suuupir");
    }
	
	public int Id { get; }
	public List<Vertex> AdjacentVertices => new(_adjacentVertices);
    public bool Visited { get; set; }

	private readonly List<Vertex> _adjacentVertices;
	
	private static int _counter = 1;
}