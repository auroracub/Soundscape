using System.Collections.Generic;

public class Mod
{
	public float base_value { get; set; }
	private PatchConnection _patch = null;
	private List<MapConnection> _adders;
	private List<MapConnection> _multipliers;

	public Mod(float p_default_value)
	{
		base_value = p_default_value;
		_adders = new List<MapConnection>();
		_multipliers = new List<MapConnection>();
	}

	public void map_add(MapConnection p_map)
	{
		_adders.Add(p_map);
	}
	
	public void map_multiply(MapConnection p_map)
	{
		_multipliers.Add(p_map);
	}
	
	public void set_patch(PatchConnection p_patch)
	{
		_patch = p_patch;
	}
	
	public void remove_patch()
	{
		_patch = null;
	}
	
	public float evaluate()
	{
		// Patching is absolute by design
		float val = _patch == null ? base_value : _patch.evaluate();
		
		// Aggregate additive mapping
		for (int i = 0; i < _adders.Count; i++)
		{
			val += _adders[i].evaluate();
		}
		
		// Aggregate multiplicative mapping
		for (int i = 0; i < _multipliers.Count; i++)
		{
			val *= _multipliers[i].evaluate();
		}
		
		return val;
	}
}
