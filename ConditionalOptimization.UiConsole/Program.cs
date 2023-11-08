using ConditionalOptimization.Logic;

namespace ConditionalOptimization.UiConsole;

class Program
{
    static void Main()
    {
		var adjacencyMatrix = new bool[4, 4]
		{
			{ true, true, true, true },
			{ false, true, true, true },
			{ false, false, true, true },
			{ true, true, false, false }
		};
		
        var bipartiteGraph = new BipartiteGraph(adjacencyMatrix);
		var greatestMatching = bipartiteGraph.FordFulkersonAlgorithm();
		//var greatestMatching = bipartiteGraph.DepthFirstSearch(bipartiteGraph.Source, bipartiteGraph.Drain);
		foreach (var vertex in greatestMatching)
			Console.Write($"{vertex.Id} --> ");
		
		Console.WriteLine();
		Console.Write("Для завершения программы нажмите любую клавишу...");
		Console.ReadKey();
    }
}