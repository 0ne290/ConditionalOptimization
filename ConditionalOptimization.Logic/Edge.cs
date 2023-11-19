namespace ConditionalOptimization.Logic;

public class Edge
{
    public Edge(int capacity = 1, int flow = 0, int weight = 1)
    {
        Capacity = capacity;
        Flow = flow;
        Weight = weight;
    }

    public int Capacity { get; }
    public int Flow { get; set; }
    public int Weight { get; }
}