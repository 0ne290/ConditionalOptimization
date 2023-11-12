namespace ConditionalOptimization.Logic;

public class TheAssignmentProblem
{
    public TheAssignmentProblemDto HungarianAlgorithm(double[] costTable)
    {
        
    }
    
    private static readonly Lazy<TheAssignmentProblem> Lazy = new(() => new TheAssignmentProblem());
    public static TheAssignmentProblem GetInstance() => Lazy.Value;
}