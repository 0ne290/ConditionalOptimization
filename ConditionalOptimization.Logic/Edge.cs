using ConditionalOptimization.Interfaces;

namespace ConditionalOptimization.Logic;

public class Edge : IEdge
{
	public Edge(IVertex beginningVertex, IVertex destinationVertex)
	{
		BeginningVertex = beginningVertex;
		DestinationVertex = destinationVertex;
		Capacity = 1;
	}
	public Edge(IVertex beginningVertex, IVertex destinationVertex, int? capacity)
	{
		BeginningVertex = beginningVertex;
		DestinationVertex = destinationVertex;
		Capacity = capacity;
	}
	
	public IVertex BeginningVertex { get; set; }
	public IVertex DestinationVertex { get; set; }
	public int? Capacity
	{
		get => _capacity;
		set
		{
			if (value < 0)
				throw new Exception("Capacity cannot be less than zero");
			else if (value is null)
			{
				_capacity = value;
				_flow = null;
			}
			else
			{
				_capacity = value;
				_flow = 0;
			}
		}
	}
	public int? Flow
	{
		get => _flow;
		set
		{
			if (Capacity is null)
				_flow = null;
			else if (value > Capacity || value < 0)
				throw new Exception("Flow cannot be greater than capacity and less than zero");
			else
				_flow = value;
		}
	}
	
	private int? _capacity;
	private int _flow;
}