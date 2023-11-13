namespace ConditionalOptimization.Logic;

public class TheAssignmentProblem
{
    public TheAssignmentProblem(double[,] costTable) => CostTable = costTable;// Еще кста тут хуй пойми че за предупреждение
    
    public TheAssignmentProblemDto HungarianAlgorithm()
    {
        var result = new TheAssignmentProblemDto();
        result.CostTable = CostTable;
        var costTable = new Table<double>(_costTable);
        
        foreach (var row in costTable.Rows)
        {
            var rowMinimumCell = row.Min();
            //if (rowMinimumCell is null) раскоментишь эту тупизну - предупреждение исчезнет
            //    throw new Exception("Row cannot be null or consist only of null cells");
            foreach (var cell in row)
                cell.Value -= rowMinimumCell.Value;
        }

        return result;
    }

    public double[,] CostTable
    {
        get => (double[,])_costTable.Clone();
        set
        {
            if (value.GetLength(0) != value.GetLength(1))
                throw new Exception("The cost array must be square");
            _costTable = (double[,])value.Clone();
        }
    }

    private double[,] _costTable;
}