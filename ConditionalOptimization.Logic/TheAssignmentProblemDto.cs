namespace ConditionalOptimization.Logic;

public class TheAssignmentProblemDto
{
    public bool[,] AssignmentTable { get; set; } = new bool[0,0];
    public double[,] CostTable { get; set; } = new double[0,0];
    public double MinimumCost { get; set; }
}