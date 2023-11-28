using ConditionalOptimization.Logic.Contracts;

namespace ConditionalOptimization.Logic;

public class NodeStack : INodeStorage
{
    public int GetFirst() => _nodes.Pop();
    
    public void Insert(int node) => _nodes.Push(node);
    
    public bool IsEmpty() => _nodes.Count == 0;
	
    private readonly Stack<int> _nodes = new();
}