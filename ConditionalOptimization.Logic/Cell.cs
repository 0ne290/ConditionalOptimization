namespace ConditionalOptimization.Logic;

public class Cell<T> : IComparable<Cell<T>> where T : IComparable<T>
{
    public Cell(T value) => Value = value;
    
    public int CompareTo(Cell<T>? cell)
    {
        if (cell == null)
            throw new ArgumentNullException(nameof(cell));
        return Value.CompareTo(cell.Value);
    }
    
    public T Value { get; set; }
}