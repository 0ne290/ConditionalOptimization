namespace ConditionalOptimization.Logic;

public class Edge
{
	public Edge() { }
	public Edge(int capacity, bool isReverse)
	{
		Capacity = capacity;
		_weight = capacity;
		IsReverse = isReverse;
	}
	
	public void SendFlow(int flow) => Capacity -= flow;
	public void ReceiveFlow(int flow) => Capacity += flow;
	
	public void ResetСapacity() => Capacity = _weight;
	
	public bool IsBusy() => _capacity < 1;
	
	public bool IsReverse { get; }
	public int Capacity
	{
		get => _capacity;
		private set
		{
			if (value < 0)
				throw new Exception("Capacity cannot be less than zero");
			_capacity = value;
		}
	}
	
	private int _capacity;
	private int _weight;
}