namespace ConditionalOptimization.Logic;

public static class MatrixReaderFromFile
{
    public static double[,] LoadMatrix(string pathToFileMatrix)
    {
        var rowIndex = 0;
        var rows = File.ReadAllLines(pathToFileMatrix);

        var dimension = rows.Length;
        var matrix = new double[dimension, dimension];
        
        foreach (var row in rows)
        {
            var numbers = row.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(double.Parse).ToArray();
            
            if (numbers.Length != dimension)
                throw new Exception("The cost table must be square.");
            
            var columnIndex = 0;
            
            foreach (var number in numbers)
                matrix[rowIndex, columnIndex++] = number;
            
            rowIndex++;
        }
 	
        return matrix;
    }
}
