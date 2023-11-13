namespace ConditionalOptimization.Logic;

public class Cell<T> : IComparable<Cell<T>> where T : IComparable<T>
{
    public Cell(T value) => Value = value;
    
    public int CompareTo(Cell<T>? cell)// уберешь знак вопроса - предупреждения появятся
    {
        //if (cell is null) раскоментишь - предупреждение пропадет
        //   throw new Exception("The collection cannot contain null cells");
        return Value.CompareTo(cell.Value);// это все из-за этого ебучего метода - эта падла принимает Nullable
    }
    
    public T Value { get; set; }
}