using ConditionalOptimization.Logic;

namespace ConditionalOptimization.UiConsole;

internal static class Program
{
	private static void Main()
    {
	    //var adjacencyMatrix = new[,]
	    //{
	    //	{ true, false, false },
	    //	{ true, true, true },
	    //	{ false, true, false }
	    //};
	    
		//var adjacencyMatrix = new[]
		//{
		//	new[] { true, true, true, true },
		//	new[] { false, true, true, true },
		//	new[] { false, false, true, true },
		//	new[] { true, true, false, false }
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
        Console.WriteLine();
        Console.WriteLine();
        foreach (var node in greatestMatching)
	        Console.Write($"{node} --> ");

        Console.WriteLine();
		Console.Write("Для завершения программы нажмите любую клавишу...");
		Console.ReadKey();
    }
}