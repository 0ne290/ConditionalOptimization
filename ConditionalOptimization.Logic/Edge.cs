namespace ConditionalOptimization.Logic;

public class Edge
{
    public Edge(Vertex start, Vertex end)
    {
        Start = start;
        End = end;
        _capacity = 1;
        _flow = 0;
    }
    
    public void ConnectVertices() => Start.AddEdge(this);
    public void Delete() => Start.DeleteEdge(this);
    public void Invert()
    {
        Start.DeleteEdge(this);
        End.AddEdge(this);
        var temp = Start;
        Start = End;
        End = temp;
    }
    
    public Vertex Start { get; private set; }
    public Vertex End { get; private set; }
    public int Capacity
    {
        get => _capacity;
        set
        {
            if (value < 1)
                throw new Exception("The capacity of the edge cannot be less than one");
            _capacity = value;
        }
    }
    public int Flow
    {
        get => _flow;
        set
        {
            if (value < 0 || value > _capacity)
                throw new Exception("Edge flow cannot be negative or greater than capacity");
            _flow = value;
        }
    }

    private int _capacity, _flow;
}
