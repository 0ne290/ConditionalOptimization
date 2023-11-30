namespace ConditionalOptimization.Logic;

public static class MatrixReaderFromFle
{
    public static async Task<double[,]> LoadMatrix(string pathToFileMatrix, int dimension)
    {
        var rowIndex = 0;
        var matrix = new double[dimension, dimension];
        using var streamReader = File.OpenText(pathToFileMatrix);
 	
        await foreach (var row in LoadRow(streamReader))
        {
            var columnIndex = 0;
            foreach (var element in row)
                matrix[rowIndex, columnIndex++] = element;
            rowIndex++;
        }
 	
        return matrix;
    }
    
    private static async IAsyncEnumerable<IEnumerable<double>> LoadRow(StreamReader reader)
    {
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (!string.IsNullOrEmpty(line))
                yield return line
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(double.Parse);
        }
    }
}