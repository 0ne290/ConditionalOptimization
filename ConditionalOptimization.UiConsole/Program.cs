using ConditionalOptimization.Logic;

namespace ConditionalOptimization.UiConsole;

class Program
{
    static void Main()
    {
		//var adjacencyMatrix = new[,]
		//{
		//	{ true, true, true, true },
		//	{ false, true, true, true },
		//	{ false, false, true, true },
		//	{ true, true, false, false }
		//};
		
		var adjacencyMatrix = new[,]
		{
			{ false, true, false, false, false, false, false },
			{ true, true, true, true, true, false, false },
			{ false, true, false, true, false, true, false },
			{ false, false, true, false, true, false, true },
			{ false, false, true, false, true, false, true },
			{ false, false, false, true, false, true, false },
			{ false, true, false, false, false, true, false }
		};

        var bipartiteGraph = new BipartiteGraph(adjacencyMatrix);
        
        var greatestMatching = bipartiteGraph.FordFulkersonAlgorithm();
        //var greatestMatching = bipartiteGraph.DepthFirstSearch(bipartiteGraph.Source, bipartiteGraph.Drain);
        //var greatestMatching = bipartiteGraph.SearchMinimumVertexCoverOfAGraph();
        Console.WriteLine();
        Console.WriteLine();
        foreach (var vertex in greatestMatching)
            Console.Write($"{vertex.Id} --> ");

        Console.WriteLine();
		Console.Write("Для завершения программы нажмите любую клавишу...");
		Console.ReadKey();
    }
}