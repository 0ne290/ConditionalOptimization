namespace ConditionalOptimization.Logic;

public class SystemOfNonlinearEquations
{
    public SystemOfNonlinearEquations(Func<double[], double>[] vectorFunction, Func<double[], double>[,] jacobianMatrix, double[] initialApproximation, double accuracy, Func<double[], double> limitFunction, string limitFunctionString, Func<double[], double> targetFunction, string targetFunctionString)
    {
	    _vectorFunction = vectorFunction;
	    _jacobianMatrix = jacobianMatrix;
	    _initialApproximation = initialApproximation;
	    _accuracy = accuracy;
	    LimitFunction = limitFunction;
	    LimitFunctionString = limitFunctionString;
	    TargetFunction = targetFunction;
	    TargetFunctionString = targetFunctionString;
    }

	public double[] SolveByNewtonsMethod(out double lambda)
	{
		if (_jacobianMatrix.GetLength(1) != _vectorFunction.Length)
			throw new Exception("The number of columns of the Jacobian matrix must be equal to the dimension of the vector function");

		if (ComputeDeterminantMatrix(CalculateJacobianMatrix(_jacobianMatrix, _initialApproximation)) == 0)
			throw new Exception("With the initial approximation introduced, the Jacobi matrix has a zero determinant. Enter a different initial guess");
		
		double[] approximation = _initialApproximation, lastApproximation;
		do
		{
			lastApproximation = approximation;
			approximation = VectorSub(lastApproximation,
				MatrixMultipliedByVector(InvertAMatrixUsingTheCholeskyMethod(CalculateJacobianMatrix(_jacobianMatrix, lastApproximation)),
					CalculateVectorFunction(lastApproximation)));
		} while (!AccuracyAchieved(approximation, lastApproximation, _accuracy));

		lambda = approximation[^1];
		Array.Resize(ref approximation, approximation.Length - 1);

		return approximation;
	}
	
	private static void SwapRows(int rowNumber1, int rowNumber2, double[,] matrix)
	{
		for (var i = 0; i < matrix.GetLength(0); i++)
			(matrix[rowNumber1, i], matrix[rowNumber2, i]) = (matrix[rowNumber2, i], matrix[rowNumber1, i]);
	}
	
	private static double ComputeDeterminantMatrix(double[,] matrix)
	{
		double det = 1;
		const double eps = 1E-9;
		
		for (var i = 0; i < matrix.GetLength(0); i++) 
		{
			var k = i;
			
			for (var j = i + 1; j < matrix.GetLength(0); j++)
				if (Math.Abs(matrix[j, i]) > Math.Abs(matrix[k, i]))
					k = j;
			
			if (Math.Abs(matrix[k, i]) < eps) 
			{
				det = 0;
				break;
			}

			SwapRows(i, k, matrix);
			
			if (i != k)
				det = -det;
			
			det *= matrix[i, i];
			
			for (var j = i + 1; j < matrix.GetLength(0); j++)
				matrix[i, j] /= matrix[i, i];
			
			for (var j = 0; j < matrix.GetLength(0); j++)
				if (j != i && Math.Abs(matrix[j, i]) > eps)
					for (k = i + 1; k < matrix.GetLength(0); k++)
						matrix[j, k] -= matrix[i, k] * matrix[j, i];
		}

		return det;
	}

	private static double[] MatrixMultipliedByVector(double[,] matrix, IReadOnlyList<double> vector)
	{
		var result = new double[matrix.GetLength(0)];

		for (var i = 0; i < result.Length; i++)
			for (var j = 0; j < vector.Count; j++)
				result[i] += matrix[i, j] * vector[j];

		return result;
	}
	
	private static double[,] InvertAMatrixUsingTheCholeskyMethod(double[,] matrix)
	{
		var dimensional = matrix.GetLength(0);

		var inverseMatrix = new double[dimensional, dimensional];
		var triangularMatrixB = new double[dimensional, dimensional];
		var triangularMatrixC = new double[dimensional, dimensional];
		
		var identityMatrixVector = new double[dimensional];
		var rootsVector = new double[dimensional];
		var auxiliaryVector = new double[dimensional];

		int i;
        
		for (i = 0; i < dimensional; i++)
		{
			triangularMatrixB[i, 0] = matrix[i, 0];
			triangularMatrixC[0, i] = matrix[0, i] / triangularMatrixB[0, 0];
        }
        
		i = 1;
		var j = 1;
        while (i < dimensional)
        {
            for (i = j; i < dimensional; i++)
            {
                for (var k = 0; k < j; k++)
	                triangularMatrixB[i, j] += triangularMatrixB[i, k] * triangularMatrixC[k, j];
                triangularMatrixB[i, j] = matrix[i, j] - triangularMatrixB[i, j];
            }
            for (i = j; j < dimensional; j++)
            {
                for (var k = 0; k < i; k++)
	                triangularMatrixC[i, j] += triangularMatrixB[i, k] * triangularMatrixC[k, j];
                triangularMatrixC[i, j] = matrix[i, j] - triangularMatrixC[i, j];
                triangularMatrixC[i, j] *= 1 / triangularMatrixB[i, i];
            }
            j = i + 1;
        }
    
        /*
            Суть вычисления обратной матрицы методом Холецкого в том,
            что мы n раз решаем СЛАУ с неизменной исходной матрицей
            (n x n) в левой части системы и векторами-столбцами
            единичной матрицы в правой. Таким образом мы получаем n
            векторов с корнями. Каждый такой вектор будет занесен в
            обратную матрицу в качестве столбца. Пример: находим
            1-ый столбец обратной матрицы - решаем СЛАУ с матрицей A
            в левой части и 1-ым столбцом единичной матрицы в правой,
            в результате получаем вектор корней - он-то и станет 1-ым
            столбцом обратной матрицы.
        */
        for (j = 0; j < dimensional; j++)
        {
	        identityMatrixVector[j] = 1;
		
            auxiliaryVector[0] = identityMatrixVector[0] / triangularMatrixB[0, 0];
            for (i = 1; i < dimensional; i++)
            {
                auxiliaryVector[i] = 0;
                for (var k = 0; k < i; k++)
                    auxiliaryVector[i] += triangularMatrixB[i, k] * auxiliaryVector[k];
                auxiliaryVector[i] = identityMatrixVector[i] - auxiliaryVector[i];
                auxiliaryVector[i] *= 1 / triangularMatrixB[i, i];
            }
    
            rootsVector[dimensional - 1] = auxiliaryVector[dimensional - 1];
            for (i = dimensional - 2; i > -1; i--)
            {
                rootsVector[i] = 0;
                for (var k = i + 1; k < dimensional; k++)
                    rootsVector[i] += triangularMatrixC[i, k] * rootsVector[k];
                rootsVector[i] = auxiliaryVector[i] - rootsVector[i];
            }
    
            for (i = 0; i < dimensional; i++)
	            inverseMatrix[i, j] = rootsVector[i];

            identityMatrixVector[j] = 0;
        }

        return inverseMatrix;
	}

	private static double[,] CalculateJacobianMatrix(Func<double[], double>[,] jacobianMatrix, double[] vector)
	{
		var result = new double[jacobianMatrix.GetLength(0), jacobianMatrix.GetLength(1)];

		for (var i = 0; i < result.GetLength(0); i++)
			for (var j = 0; j < result.GetLength(1); j++)
				result[i, j] = jacobianMatrix[i, j](vector);

		return result;
	}
	
	private double[] CalculateVectorFunction(double[] vector)
	{
		var result = new double[_vectorFunction.Length];

		for (var i = 0; i < result.GetLength(0); i++)
			result[i] = _vectorFunction[i](vector);

		return result;
	}

	private static bool AccuracyAchieved(IReadOnlyList<double> approximation, IReadOnlyList<double> lastApproximation, double accuracy) =>
		CalculateVectorLength(VectorSub(approximation, lastApproximation)) < accuracy;

	private static double CalculateVectorLength(IEnumerable<double> vector)
	{
		var result = vector.Sum(t => Math.Pow(t, 2));

		return Math.Sqrt(result);
	}

	private static double[] VectorSub(IReadOnlyList<double> vector2, IReadOnlyList<double> vector1)
	{
		var result = new double[vector2.Count];

		for (var i = 0; i < vector2.Count; i++)
			result[i] = vector2[i] - vector1[i];
        
		return result;
	}
	
	public Func<double[], double> LimitFunction { get;  }
	
	public Func<double[], double> TargetFunction { get;  }

	public string LimitFunctionString { get; }
	
	public string TargetFunctionString { get; }

	private readonly Func<double[], double>[] _vectorFunction;

	private readonly Func<double[], double>[,] _jacobianMatrix;

	private readonly double[] _initialApproximation;

	private readonly double _accuracy;
}