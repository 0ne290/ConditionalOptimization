using ConditionalOptimization.Interfaces;

namespace ConditionalOptimization.Logic;

public class Vertex : IVertex
{
	public Vertex()
	{
		Id = _counter;
		_counter++;
		Edges = new List<IEdge>();
	}
	public Vertex(List<IEdge> edges)
	{
		Id = _counter;
		_counter++;
		Edges = edges;
	}
	
	public void EdgeAdd(IVertex vertex)
	{
		var edge = new Edge(this, vertex);
		Edges.Add(edge);
	}
	
	public int Id { get; private set; }
	public List<IEdge> Edges { get; set; }
	
	private static int _counter = 1;
}