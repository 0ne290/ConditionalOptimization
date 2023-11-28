namespace ConditionalOptimization.Logic;

public class TheAssignmentProblemDto
{
    public TheAssignmentProblemDto(bool[,] assignmentTable, double[,] costTable, double minimumCost)
    {
        AssignmentTable = assignmentTable;
        CostTable = costTable;
        MinimumCost = minimumCost;
    }
    
    public bool[,] AssignmentTable { get; set; }
    public double[,] CostTable { get; set; }
    public double MinimumCost { get; set; }
}