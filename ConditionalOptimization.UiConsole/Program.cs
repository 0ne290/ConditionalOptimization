using ConditionalOptimization.Logic;

namespace ConditionalOptimization.UiConsole;

class Program
{
    static void Main()
    {
	    var adjacencyMatrix = new[,]
	    {
	    	{ true, false, false },
	    	{ true, true, true },
	    	{ false, true, false }
	    };
	    
		//var adjacencyMatrix = new[,]
		//{
		//	{ true, true, true, true },
		//	{ false, true, true, true },
		//	{ false, false, true, true },
		//	{ true, true, false, false }
		//};
		
		//var adjacencyMatrix = new[,]
		//{
		//	{ false, true, false, false, false, false, false },
		//	{ true, true, true, true, true, false, false },
		//	{ false, true, false, true, false, true, false },
		//	{ false, false, true, false, true, false, true },
		//	{ false, false, true, false, true, false, true },
		//	{ false, false, false, true, false, true, false },
		//	{ false, true, false, false, false, true, false }
		//};

        var bipartiteGraph = new BipartiteGraph(adjacencyMatrix);
        
        bipartiteGraph.FordFulkersonAlgorithm();
        var greatestMatching = bipartiteGraph.GetLoadedEdges();
        //var greatestMatching = bipartiteGraph.DepthFirstSearch(bipartiteGraph.Source, bipartiteGraph.Drain);
        //var greatestMatching = bipartiteGraph.SearchMinimumVertexCoverOfAGraph();
        Console.WriteLine();
        Console.WriteLine();
        foreach (var edge in greatestMatching)
            Console.Write($"{edge.Start.Id} --> {edge.End.Id}");

        Console.WriteLine();
		Console.Write("Для завершения программы нажмите любую клавишу...");
		Console.ReadKey();
    }
}