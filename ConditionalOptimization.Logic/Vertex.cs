﻿namespace ConditionalOptimization.Logic;

public class Vertex
{
	public Vertex()
	{
		Id = _counter;
		Visited = false;
		_outgoingEdges = new List<Edge>(10);
		_incomingEdges = new List<Edge>(10);
		_counter++;
	}

	public void AddOutgoingEdge(Edge edge)
	{
		if (_outgoingEdges.Contains(edge))
			throw new Exception("The specified edge already originates from the vertex");
		_outgoingEdges.Add(edge);
	}
	public void AddIncomingEdge(Edge edge)
	{
		if (_incomingEdges.Contains(edge))
			throw new Exception("The specified edge is already included in the vertex");
		_incomingEdges.Add(edge);
	}
	
	public void DeleteOutgoingEdge(Edge edge)
	{
		_outgoingEdges.Remove(edge);
	}
	public void DeleteIncomingEdge(Edge edge)
	{
		_incomingEdges.Remove(edge);
	}

	public List<Edge> OutgoingEdges => new(_outgoingEdges);
	public List<Edge> IncomingEdges => new(_incomingEdges);
	public int Id { get; }
	public bool Visited { get; set; }
	
	private List<Edge> _outgoingEdges;
	private List<Edge> _incomingEdges;
	private static int _counter = 1;
}