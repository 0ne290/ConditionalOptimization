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
	public void DeleteEdge(Vertex adjacentVertex)
	{
		if (!_adjacentVertices.Remove(adjacentVertex))
			Console.WriteLine("Suuupir");
			//throw new Exception("The specified vertex is not adjacent to the this vertex");
	}
	
	public int Id { get; private set; }
	public List<Vertex> AdjacentVertices
	{
		get => new List<Vertex>(_adjacentVertices);
		private set => _adjacentVertices = value;
	}
	public bool Visited { get; set; }

	private List<Vertex> _adjacentVertices;
	
	private static int _counter = 1;
}