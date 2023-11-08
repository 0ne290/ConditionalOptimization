using ConditionalOptimization.Logic;

namespace ConditionalOptimization.UiConsole;

class Program
{
    static void Main()
    {
		var adjacencyMatrix = new[,]
		{
			{ true, true, true, true },
			{ false, true, true, true },
			{ false, false, true, true },
			{ true, true, false, false }
		};
		
        var bipartiteGraph = new BipartiteGraph();
		bipartiteGraph.ReconstructGraph(adjacencyMatrix);
		var greatestMatching = bipartiteGraph.FordFulkersonAlgorithm();
		bipartiteGraph.PrintGraph(greatestMatching);
		
		Console.WriteLine();
		Console.Write("Для завершения программы нажмите любую клавишу...");
		Console.ReadKey();
    }
}