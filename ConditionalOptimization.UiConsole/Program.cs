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
			originalCostTable = MatrixReaderFromFile.LoadMatrix("CostTable.txt");
	    
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
		SelectAProblem(out var vectorFunction, out var jacobianMatrix, out var initialApproximation);
		
		Console.Write("Введите точность: ");
		var accuracy = Convert.ToDouble(Console.ReadLine());
		
		var systemOfNonlinearEquations = new SystemOfNonlinearEquations(vectorFunction);
		var result = systemOfNonlinearEquations.SolveByNewtonsMethod(jacobianMatrix, initialApproximation, accuracy);

		foreach (var root in result)
			Console.Write($"{root}, ");
	}

	private static void SelectAProblem(out Func<double[], double>[] vectorFunction, out Func<double[], double>[,] jacobianMatrix, out double[] initialApproximation)
	{
		Console.WriteLine("Ниже программа попросит Вас ввести координаты точки начального приближения. Настоятельно не рекомендую вводить нули или числа около нуля - есть шанс получить матрицу Якоби с нулевым детерминантом и тогда ее инвертация станет невозможной и программа вернет не числа");
		Console.Write("Введите номер решаемой ЗНП (от 1 до 3, в противном случае будет выбрана 1 ЗНП, сами ЗНП смотрите в файле \"Problems.png\"): ");
		var problemNumber = Convert.ToInt32(Console.ReadLine());
		
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
				initialApproximation = new double[3];
				GetInitialApproximation(initialApproximation);
				break;
			default:
				goto case 1;
		}
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