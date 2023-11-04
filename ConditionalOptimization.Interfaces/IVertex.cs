namespace ConditionalOptimization.Interfaces;

public interface IVertex
{
	void EdgeAdd(IVertex vertex);
	
	int Id { get; }
	List<IEdge> Edges { get; set; }
}