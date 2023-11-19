namespace ConditionalOptimization.Logic.Contracts;

public interface INodeStorage
{
    int GetFirst();
    void Insert(int node);
    bool IsEmpty();
}