namespace ConditionalOptimization.Logic;

public class TheAssignmentProblemDto
{
    public bool[,] AssignmentTable { get; set; }
    public double[,] CostTable { get; set; }
    public double MinimumCost { get; set; }
}