using ConditionalOptimization.Logic.Contracts;

namespace ConditionalOptimization.Logic;

public class NodeStack : INodeStorage
{
    public NodeStack(int maximumNumberOfNodes) => _nodes = new Stack<int>(maximumNumberOfNodes);

    public int GetFirst() => _nodes.Pop();
    public void Insert(int node) => _nodes.Push(node);
    public bool IsEmpty() => _nodes.Count == 0;
	
    private readonly Stack<int> _nodes;
}