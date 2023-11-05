namespace ConditionalOptimization.Interfaces;

public interface IEdge
{
	IVertex BeginningVertex { get; set; }
	IVertex DestinationVertex { get; set; }
	int? Capacity { get; set; }
	int? Flow { get; set; }
}