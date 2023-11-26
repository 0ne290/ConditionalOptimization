namespace ConditionalOptimization.Logic;

public class Table<T> where T : IComparable<T>
{
    public Table(T[,] sourceArray)
    {
        if (sourceArray.GetLength(0) != sourceArray.GetLength(1))
            throw new Exception("The source array must be square");
        
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
    
    public Cell<T>[][] Rows { get; }
    public Cell<T>[][] Columns { get; }
}