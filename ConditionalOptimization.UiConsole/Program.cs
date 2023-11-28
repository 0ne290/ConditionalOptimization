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

        //var bipartiteGraph = new BipartiteGraph(adjacencyMatrix);
        //
        //var greatestMatching = bipartiteGraph.FordFulkersonAlgorithm();
        //var minimumVertexCover = bipartiteGraph.SearchMinimumVertexCover(greatestMatching);
        //Console.WriteLine();
        //Console.WriteLine();
        //foreach (var node in minimumVertexCover)
	    //    Console.Write($"{node} --> ");

	    var originalCostTable = new double[,]
	    {
		    { 1, 4, 6, 3 },
		    { 9, 7, 10, 9 },
		    { 4, 5, 11, 7 },
		    { 8, 7, 8, 5 }
	    };
	    
	    var theAssignmentProblem = new TheAssignmentProblem(originalCostTable);
	    var theAssignmentProblemDto = theAssignmentProblem.HungarianAlgorithm();

	    var dimension = theAssignmentProblemDto.CostTable.GetLength(0);
	    var resultCostTable = theAssignmentProblemDto.CostTable;
	    Console.WriteLine("Матрица стоимостей:");
	    for (var i = 0; i < dimension; i++)
	    {
		    for (var j = 0; j < dimension; j++)
			    Console.Write($"{resultCostTable[i, j]} ");
		    Console.WriteLine();
	    }
	    
	    Console.WriteLine();
	    Console.WriteLine($"Стоимость самой выгодной совокупности назначений равна {theAssignmentProblemDto.MinimumCost}");
	    Console.WriteLine();

	    dimension = theAssignmentProblemDto.AssignmentTable.GetLength(0);
	    var assignmentTable = theAssignmentProblemDto.AssignmentTable;
	    Console.WriteLine("Матрица назначений:");
	    for (var i = 0; i < dimension; i++)
	    {
		    for (var j = 0; j < dimension; j++)
			    Console.Write($"{assignmentTable[i, j]} ");
		    Console.WriteLine();
	    }

        Console.WriteLine();
		Console.Write("Для завершения программы нажмите любую клавишу...");
		Console.ReadKey();
    }
}