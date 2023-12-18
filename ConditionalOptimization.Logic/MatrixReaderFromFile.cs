namespace ConditionalOptimization.Logic;

public static class MatrixReaderFromFile
{
    public static async Task<double[,]> LoadMatrix(string pathToFileMatrix)
    {
        var rowIndex = 0;
        using var streamReader = File.OpenText(pathToFileMatrix);

        var rows = await LoadRow(streamReader);
        var dimension = rows.Count;
        var matrix = new double[dimension, dimension];
        
        foreach (var row in rows)
        {
            if (row.Count != dimension)
                throw new Exception("The cost table must be square.");
            
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
