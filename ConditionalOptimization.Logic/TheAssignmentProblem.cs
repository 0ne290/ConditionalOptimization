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

        while (greatestMatching.Count < costTable.Dimension)
        {
            var minimumVertexCover = _bipartiteGraph.SearchMinimumVertexCover(greatestMatching);
            AlphaConversion(costTable, minimumVertexCover);
            _bipartiteGraph = new BipartiteGraph(costTable);
            greatestMatching = _bipartiteGraph.FordFulkersonAlgorithm();
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
        var rows = minimumVertexCover["leftNodes"];
        var columns = _bipartiteGraph.RightNodes;
        columns.ExceptWith(minimumVertexCover["rightNodes"]);
        
        var cells = costTable.GetCellsAtTheIntersectionOfRowsAndColumns(rows, columns);
        
        var minimumCell = cells.Min();
        if (minimumCell == null)
            throw new ArgumentNullException(nameof(minimumCell));
        var minimum = minimumCell.Value;
        
        foreach (var row in rows)
            foreach (var cell in costTable.Rows[row])
                cell.Value -= minimum;

        columns = (ICollection<int>)minimumVertexCover["rightNodes"];
        foreach (var column in columns)
            foreach (var cell in costTable.Columns[column])
                cell.Value -= minimum;
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