namespace ConditionalOptimization.Interfaces;

public interface IFindingTheMaximumMatching
{
	List<IEdge> HopcroftKarpAlgorithm();
	List<IEdge> FordFulkersonAlgorithm();
}