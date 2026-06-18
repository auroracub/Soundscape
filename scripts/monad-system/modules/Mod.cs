using System;

public class Mod
{
	public float base_value { get; set; }
	private Func<float> _mod_chain;

	public Mod(float p_default_value)
	{
		base_value = p_default_value;
		_mod_chain = () => 0.0f;
	}

	public void add_mod(Func<float> p_mod_function)
	{
		var old_chain = _mod_chain;
		_mod_chain = () => old_chain() + p_mod_function();
	}
	
	public float evaluate()
	{
		return base_value + _mod_chain();
	}
}
