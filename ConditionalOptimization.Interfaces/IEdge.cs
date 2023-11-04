namespace ConditionalOptimization.Interfaces;

public interface IEdge
{
	IVertex DestinationVertex { get; set; }
	int? Capacity { get; set; }
	int? Flow { get; set; }
}