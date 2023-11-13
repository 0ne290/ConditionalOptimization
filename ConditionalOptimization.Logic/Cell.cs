namespace ConditionalOptimization.Logic;

public class Cell<T> : IComparable<Cell<T>> where T : IComparable<T>
{
    public Cell(T value) => Value = value;
    
    public int CompareTo(Cell<T> cell)// уберешь знак вопроса - предупреждения появятся
    {
        return Value.CompareTo(cell.Value);
    }
    
    public T Value { get; set; }
}