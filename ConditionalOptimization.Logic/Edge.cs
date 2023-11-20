namespace ConditionalOptimization.Logic;

public class Edge
{
    public Edge(int capacity = 1, int flow = 0)
    {
        Capacity = capacity;
        Flow = flow;
    }

    public int Capacity { get; }
    public int Flow { get; set; }
}