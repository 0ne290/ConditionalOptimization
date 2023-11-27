namespace ConditionalOptimization.Logic;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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

        _bipartiteGraph = new BipartiteGraph(costTable);
        var greatestMatching = _bipartiteGraph.FordFulkersonAlgorithm();

        while (greatestMatching.Count / 2 < costTable.Dimension)
        {
            var minimumVertexCover = _bipartiteGraph.SearchMinimumVertexCover(greatestMatching);
            AlphaConversion(costTable, minimumVertexCover);
            //Console.WriteLine();
            //foreach (var row in costTable.Rows)
            //{
            //    foreach (var cell in row)
            //        Console.Write($"{cell.Value} --> ");
            //    Console.WriteLine();
            //}
            //Console.WriteLine();
            _bipartiteGraph = new BipartiteGraph(costTable);
            greatestMatching = _bipartiteGraph.FordFulkersonAlgorithm();
        }
        
        foreach (var node in greatestMatching)
            Console.Write($"{node} --> ");

        //return result;
    }
    
    private void SubTheMinimumCellFromARow(Table<double> table)
    {
        foreach (var row in table.Rows)
        {
            var rowMinimumCell = row.Min();
            if (rowMinimumCell == null)
                throw new ArgumentNullException(nameof(rowMinimumCell));
            var rowMinimum = rowMinimumCell.Value;
            foreach (var cell in row)
                cell.Value -= rowMinimum;
        }
    }
    
    private void SubTheMinimumCellFromAColumn(Table<double> table)
    {
        foreach (var column in table.Columns)
        {
            var columnMinimumCell = column.Min();
            if (columnMinimumCell == null)
                throw new ArgumentNullException(nameof(columnMinimumCell));
            var columnMinimum = columnMinimumCell.Value;
            foreach (var cell in column)
                cell.Value -= columnMinimum;
        }
    }

    private void AlphaConversion(Table<double> costTable, IDictionary<string, ICollection<int>> minimumVertexCover)
    {
        var selectedRows = minimumVertexCover["leftNodes"].ToArray();
        var rightNodes = (ISet<int>)_bipartiteGraph.RightNodes;
        rightNodes.ExceptWith(minimumVertexCover["rightNodes"]);
        var selectedColumns = rightNodes.ToArray();

        for (var i = 0; i < selectedRows.Length; i++)
            selectedRows[i] -= BipartiteGraph.LeftPartIndex;
        for (var i = 0; i < selectedColumns.Length; i++)
            selectedColumns[i] -= _bipartiteGraph.RightPartIndex;
        
        var cells = costTable.GetCellsAtTheIntersectionOfRowsAndColumns(selectedRows, selectedColumns);
        
        var minimumCell = cells.Min();
        if (minimumCell == null)
            throw new ArgumentNullException(nameof(minimumCell));
        var minimum = minimumCell.Value;
        Console.WriteLine(minimum);
        
        foreach (var row in selectedRows)
        {
            foreach (var cell in costTable.Rows[row])
            {
                //Console.WriteLine(cell.Value);
                cell.Value -= minimum;
                //Console.WriteLine(cell.Value);
            }
        }
        
        //Console.WriteLine();
        //foreach (var row in costTable.Rows)
        //{
        //    foreach (var cell in row)
        //        Console.Write($"{cell.Value} --> ");
        //    Console.WriteLine();
        //}
        //Console.WriteLine();

        var unselectedColumns = minimumVertexCover["rightNodes"];
        foreach (var column in unselectedColumns)
            foreach (var cell in costTable.Columns[column - _bipartiteGraph.RightPartIndex])
                cell.Value += minimum;
        
        //Console.WriteLine();
        //foreach (var row in costTable.Rows)
        //{
        //    foreach (var cell in row)
        //        Console.Write($"{cell.Value} --> ");
        //    Console.WriteLine();
        //}
        //Console.WriteLine();
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
    private BipartiteGraph _bipartiteGraph;
}