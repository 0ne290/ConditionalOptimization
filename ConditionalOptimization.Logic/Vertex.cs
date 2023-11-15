namespace ConditionalOptimization.Logic;

public class Vertex
{
	public Vertex()
	{
		Id = _counter;
		Visited = false;
		_edges = new List<Edge>(10);
		_counter++;
	}

	public void AddEdge(Edge edge)
	{
		if (_edges.Contains(edge))
			throw new Exception("The specified edge already originates from the vertex");
		_outgoingEdges.Add(edge);
	}
	
	public void DeleteOutgoingEdge(Edge edge)
	{
		if (!_edges.Remove(edge))
			throw new Exception("The specified edge does not leave a vertex");
	}

	public List<Edge> Edges => new(_edges);
	public int Id { get; }
	public bool Visited { get; set; }
	
	private List<Edge> edges;
	private static int _counter = 1;
}
