namespace ConditionalOptimization.Logic;

public class TheAssignmentProblem
{
    public TheAssignmentProblem(double[,] costTable) => CostTable = costTable;
    
    public void HungarianAlgorithm()//TheAssignmentProblemDto
    {
        //var result = new TheAssignmentProblemDto
        //{
        //    CostTable = CostTable
        //};
        var costTable = new Table<double>(_costTable);
        
        SubTheMinimumCellFromARow(costTable);
        SubTheMinimumCellFromAColumn(costTable);
        
        foreach (var row in costTable.Rows)
        {
            foreach (var cell in row)
                Console.Write($"{cell.Value} --> ");
            Console.WriteLine();
        }
        
        Console.WriteLine();
        
        foreach (var column in costTable.Columns)
        {
            foreach (var cell in column)
                Console.Write($"{cell.Value} --> ");
            Console.WriteLine();
        }
        
        //return result;
    }
    private void SubTheMinimumCellFromARow(Table<double> table)
    {
        foreach (var row in table.Rows)
        {
            var rowMinimumCell = row.Min();
            if (rowMinimumCell == null)
                throw new ArgumentNullException(nameof(rowMinimumCell));
            foreach (var cell in row)
                cell.Value -= rowMinimumCell.Value;
        }
    }
    private void SubTheMinimumCellFromAColumn(Table<double> table)
    {
        foreach (var column in table.Columns)
        {
            var columnMinimumCell = column.Min();
            if (columnMinimumCell == null)
                throw new ArgumentNullException(nameof(columnMinimumCell));
            foreach (var cell in column)
                cell.Value -= columnMinimumCell.Value;
        }
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

    private double[,] _costTable = new double[0, 0];
}