namespace ConditionalOptimization.Logic;

public class Edge 
{ 
 public Edge(int capacity) => Capacity = capacity; 
 
 public void SendFlow(int flow) => Capacity -= flow; 
 public void ReceiveFlow(int flow) => Capacity += flow; 
 
 public bool IsBusy() => _capacity < 1; 
 public bool IsFree() => _capacity > 0; 
 
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
}
