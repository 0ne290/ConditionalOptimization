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
            var graphExceptMinimumVertexCover = GraphExceptMinimumVertexCover(minimumVertexCover);
            AlphaConversion(costTable, graphExceptMinimumVertexCover);
            _bipartiteGraph = new BipartiteGraph(costTable);
            greatestMatching = _bipartiteGraph.FordFulkersonAlgorithm();
        }
        
        foreach (var node in greatestMatching)
            Console.Write($"{node} --> ");

        //return result;
    }
    
    private static void SubTheMinimumCellFromARow(Table<double> table)
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
    
    private static void SubTheMinimumCellFromAColumn(Table<double> table)
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

    private IDictionary<string, ISet<int>> GraphExceptMinimumVertexCover(IDictionary<string, ISet<int>> minimumVertexCover)
    {
        var graphExceptMinimumVertexCover = new Dictionary<string, ISet<int>>
        {
            { "leftNodes", _bipartiteGraph.LeftNodes },
            { "rightNodes", _bipartiteGraph.RightNodes }
        };

        graphExceptMinimumVertexCover["leftNodes"].ExceptWith(minimumVertexCover["leftNodes"]);
        graphExceptMinimumVertexCover["rightNodes"].ExceptWith(minimumVertexCover["rightNodes"]);

        return graphExceptMinimumVertexCover;
    }

    private void AlphaConversion(Table<double> costTable, IDictionary<string, ISet<int>> graphExceptMinimumVertexCover)
    {
        var selectedRows = graphExceptMinimumVertexCover["leftNodes"].ToArray();
        var selectedColumns = graphExceptMinimumVertexCover["rightNodes"].ToArray();

        for (var i = 0; i < selectedRows.Length; i++)
            selectedRows[i] -= BipartiteGraph.LeftPartIndex;
        for (var i = 0; i < selectedColumns.Length; i++)
            selectedColumns[i] -= _bipartiteGraph.RightPartIndex;
        
        var cells = costTable.GetCellsAtTheIntersectionOfRowsAndColumns(selectedRows, selectedColumns);
        
        var alphaCell = cells.Min();
        if (alphaCell == null)
            throw new ArgumentNullException(nameof(alphaCell));
        var alpha = alphaCell.Value;
        
        foreach (var row in selectedRows)
            foreach (var cell in costTable.Rows[row])
                cell.Value -= alpha;
        
        var unselectedColumns = _bipartiteGraph.RightNodes;
        unselectedColumns.ExceptWith(graphExceptMinimumVertexCover["rightNodes"]);
        foreach (var column in unselectedColumns)
            foreach (var cell in costTable.Columns[column - _bipartiteGraph.RightPartIndex])
                cell.Value += alpha;
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