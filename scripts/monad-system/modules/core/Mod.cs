using System.Collections.Generic;

public class Mod
{
	public float base_value { get; set; }
	private List<PatchConnection> _connections;

	public Mod(float p_default_value)
	{
		base_value = p_default_value;
		_connections = new List<PatchConnection>();
	}

	public void add_connection(PatchConnection p_connection)
	{
		_connections.Add(p_connection);
	}
	
	public float evaluate()
	{
		float val = base_value;
		for (int i = 0; i < _connections.Count; i++)
		{
			val += _connections[i].evaluate();
		}
		return val;
	}
}
