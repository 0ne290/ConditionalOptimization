namespace ConditionalOptimization.Logic;

public class Table<T> where T : IComparable<T>
{
    public Table(T[,] sourceArray)
    {
        if (sourceArray.GetLength(0) != sourceArray.GetLength(1))
            throw new Exception("The source array must be square");

        Dimension = sourceArray.GetLength(0);
        Rows = new Cell<T>[sourceArray.GetLength(0)][];
        Columns = new Cell<T>[sourceArray.GetLength(0)][];
        
        CreateRows(sourceArray);
        CreateColumns();
    }
    
    private void CreateRows(T[,] sourceArray)
    {
        for (var i = 0; i < Rows.Length; i++)
        {
            Rows[i] = new Cell<T>[Rows.Length];
            for (var j = 0; j < Columns.Length; j++)
                Rows[i][j] = new Cell<T>(sourceArray[i, j]);
        }
    }
    
    private void CreateColumns()
    {
        for (var i = 0; i < Columns.Length; i++)
        {
            Columns[i] = new Cell<T>[Columns.Length];
            for (var j = 0; j < Rows.Length; j++)
                Columns[i][j] = Rows[j][i];
        }
    }

    public IList<Cell<T>> GetCellsAtTheIntersectionOfRowsAndColumns(IEnumerable<int> rows, IEnumerable<int> columns)
    {
        Console.WriteLine();
        Console.WriteLine();
        foreach (var row in rows)
        {
            Console.Write($"{row} --> ");
        }
        Console.WriteLine();
        foreach (var row in columns)
        {
            Console.Write($"{row} --> ");
        }
        Console.WriteLine();
        Console.WriteLine();
        
        var cells = new List<Cell<T>>(Dimension * Dimension);
        
        cells.AddRange(from row in rows from column in columns select Rows[row][column]);

        return cells;
    }
    
    public int Dimension { get; }
    public Cell<T>[][] Rows { get; }
    public Cell<T>[][] Columns { get; }
}