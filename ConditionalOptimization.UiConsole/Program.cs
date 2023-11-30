using ConditionalOptimization.Logic;

namespace ConditionalOptimization.UiConsole;

internal static class Program
{
	private static void Main()
	{
		Console.WriteLine("Корректность вводимой матрицы в файле \"CostTable.txt\" зависит только от Вас - программа не проверяет правильность вводимой матрицы. Если Вы нарушите первое правило, то программа вернет какое-нибудь стандартное исключение (скорее всего что-то типа \"Index out of range\"). Если второе - \"Capacity cannot be less than zero\".");
		Console.WriteLine();
		Console.WriteLine("Правило №1: кол-во эл-ов в строках и кол-во самих строк не должно быть меньше введенной размерности;");
        Console.WriteLine("Правило №2: кол-во эл-ов в строках и кол-во самих строк не должно быть больше введенной размерности;");
        Console.WriteLine("Правило №3: разделителем между эл-ами строки должен быть один пробел.");
        Console.WriteLine();
		
		Console.Write("Введите размерность матрицы, прописанной в файле \"CostTable.txt\" (если введете число меньше 2, то будет взята стандартная таблица из варианта 32 контрольной работы): ");
		var dimensionOriginal = Convert.ToInt32(Console.ReadLine());
		Console.WriteLine();
		double[,] originalCostTable;
		if (dimensionOriginal < 2)
		{
			originalCostTable = new double[,]
			{
				{ 1, 4, 6, 3 },
				{ 9, 7, 10, 9 },
				{ 4, 5, 11, 7 },
				{ 8, 7, 8, 5 }
			};
		}
		else
			originalCostTable = MatrixReaderFromFle.LoadMatrix("CostTable.txt", dimensionOriginal).Result;
	    
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