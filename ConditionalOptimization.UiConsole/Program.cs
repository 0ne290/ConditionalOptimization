using ConditionalOptimization.Logic;

namespace ConditionalOptimization.UiConsole;

internal static class Program
{
	private static void Main()
	{
		Console.WriteLine("Начало выполнения подпрограммы решения задачи о назачениях венгерским алгоритмом.");
		Console.WriteLine();
		SolveTheAssignmentProblemUsingTheHungarianAlgorithm();
		
		Console.WriteLine("Начало выполнения подпрограммы решения задачи нелинейного программирования методом множителей Лагранжа.");
		Console.WriteLine();
		SolveANonlinearProgrammingProblemUsingTheLagrangeMultiplierMethod();
		
        Console.WriteLine();
		Console.Write("Для завершения программы нажмите любую клавишу...");
		Console.ReadKey();
    }

	private static void SolveTheAssignmentProblemUsingTheHungarianAlgorithm()
	{
		Console.Write("Введите любую непустую последовательность, если хотите прочитать матрицу стоимостей из файла \"CostTable.txt\". В противном случае будет взята стандартная таблица из варианта 32 контрольной работы): ");
		double[,] originalCostTable;
		if (string.IsNullOrWhiteSpace(Console.ReadLine()))
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
			originalCostTable = MatrixReaderFromFile.LoadMatrix("CostTable.txt");
		
		Console.WriteLine();
	    
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
	}
	
	private static void SolveANonlinearProgrammingProblemUsingTheLagrangeMultiplierMethod()
	{
		Console.Write("Введите точность: ");
		var accuracy = Convert.ToDouble(Console.ReadLine());
		
		var systemOfNonlinearEquations = SelectAProblem(accuracy);
		
		var result = systemOfNonlinearEquations.SolveByNewtonsMethod(out var lambda);

		Console.Write("x* = { ");
		var resultString = result.Aggregate("", (current, root) => current + $"{root}, ");
		resultString = resultString.Remove(resultString.Length - 2);
		Console.Write($"{resultString} }}");
		
		Console.WriteLine();
		Console.WriteLine(systemOfNonlinearEquations.TargetFunctionString);
		Console.WriteLine($"f(x*) = {systemOfNonlinearEquations.TargetFunction(result)}");
		Console.WriteLine(systemOfNonlinearEquations.LimitFunctionString);
		Console.WriteLine($"h(x*) = {systemOfNonlinearEquations.LimitFunction(result)}");
		
		Console.WriteLine();
		Console.WriteLine($"Lambda coefficient = {lambda}");
	}

	private static SystemOfNonlinearEquations SelectAProblem(double accuracy)
	{
		Console.WriteLine("Ниже программа попросит Вас ввести координаты точки начального приближения. Настоятельно не рекомендую вводить нули или числа около нуля - есть шанс получить матрицу Якоби с нулевым детерминантом и тогда ее инвертация станет невозможной и программа вернет не числа");
		Console.Write("Введите номер решаемой ЗНП (от 1 до 3, в противном случае будет выбрана 1 ЗНП, сами ЗНП смотрите в файле \"Problems.png\"): ");
		var problemNumber = Convert.ToInt32(Console.ReadLine());

		Func<double[], double>[] vectorFunction;
		Func<double[], double>[,] jacobianMatrix;
		Func<double[], double> targetFunction, limitFunction;
		string limitFunctionString, targetFunctionString;
		double[] initialApproximation;
		switch (problemNumber)
		{
			case 1:
				vectorFunction = new Func<double[], double>[]
                {
                	args => 2 * (args[0] - 6) - 2 * args[2] * args[0],
                	args => 2 * (args[1] - 8) - 2 * args[2] * args[1],
                	args => 4 - Math.Pow(args[0], 2) - Math.Pow(args[1], 2)
                };
				jacobianMatrix = new Func<double[], double>[,]
                {
                	{ args => 2 - 2 * args[2], _ => 0, args => -2 * args[0] },
                	{ _ => 0, args => 2 - 2 * args[2], args => -2 * args[1] },
                	{ args => -2 * args[0], args => -2 * args[1], _ => 0 }
                };
				limitFunction = (args) => 4d - Math.Pow(args[0], 2d) - Math.Pow(args[1], 2d);
				targetFunction = (args) => 1d + Math.Pow(args[0] - 6d, 2d) + Math.Pow(args[1] - 8d, 2d);
				limitFunctionString = "h(x) = 4 - x1^2 - x2^2 = 0";
				targetFunctionString = "f(x) = 1 + (x1 - 6)^2 + (x2 - 8)^2";
				initialApproximation = new double[3];
				GetInitialApproximation(initialApproximation);
				break;
			case 2:
				vectorFunction = new Func<double[], double>[]
				{
					args => 2 * (args[0] + args[2]),
					args => 2 * args[1] + args[2],
					args => 2 * args[0] + args[1] - 2
				};
				jacobianMatrix = new Func<double[], double>[,]
				{
					{ _ => 2, _ => 0, _ => 2 },
					{ _ => 0, _ => 2, _ => 1 },
					{ _ => 2, _ => 1, _ => 0 }
				};
				limitFunction = (args) => 2d * args[0] + args[1] - 2;
				targetFunction = (args) => Math.Pow(args[0], 2d) + Math.Pow(args[1], 2d);
				limitFunctionString = "h(x) = 2x1 + x2 - 2 = 0";
				targetFunctionString = "f(x) = x1^2 + x2^2";
				initialApproximation = new double[3];
				GetInitialApproximation(initialApproximation);
				break;
			case 3:
				vectorFunction = new Func<double[], double>[]
				{
					args => 2 * args[0] + args[2] - 8,
					args => 2 * args[1] + args[2] - 8,
					args => args[0] + args[1] - 5
				};
				jacobianMatrix = new Func<double[], double>[,]
				{
					{ _ => 2, _ => 0, _ => 1 },
					{ _ => 0, _ => 2, _ => 1 },
					{ _ => 1, _ => 1, _ => 0 }
				};
				limitFunction = (args) => args[0] + args[1] - 5;
				targetFunction = (args) => Math.Pow(args[0] - 4d, 2d) + Math.Pow(args[1] - 4d, 2d);
				limitFunctionString = "h(x) = x1 + x2 - 5 = 0";
				targetFunctionString = "f(x) = (x1 - 4)^2 + (x2 - 4)^2";
				initialApproximation = new double[3];
				GetInitialApproximation(initialApproximation);
				break;
			default:
				goto case 1;
		}
		
		return new SystemOfNonlinearEquations(vectorFunction, jacobianMatrix, initialApproximation, accuracy, limitFunction, limitFunctionString, targetFunction, targetFunctionString);
	}
	
	private static void GetInitialApproximation(IList<double> initialApproximation)
	{
		for (var i = 0; i < initialApproximation.Count; i++)
		{
			Console.Write($"Введите {i + 1} координату точки начального приближения: ");
			initialApproximation[i] = Convert.ToInt32(Console.ReadLine());
		}
	}
}