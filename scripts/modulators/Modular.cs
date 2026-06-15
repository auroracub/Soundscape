using System;
using Godot;

namespace Soundscape.Modules;

//public interface IModulator
//{
	//void process_frame(float p_delta);
	//float get_current_value();
	//void set_control_signal(float p_signal);
//}

[GlobalClass]
public abstract partial class Modular : RefCounted//Resource//, IModulator
{
	public Modular() { }
	
	[Export] public bool debug_enabled = false;
	
	protected float _last_value = 0.0f;
	
	public virtual void process_frame(float p_delta)
	{
		// General modulators execute their math variations per audio frame tick
	}

	public virtual float get_current_value()
	{
		return compute_sample();
	}

	public virtual void set_control_signal(float p_signal)
	{
		// Custom modulators override this to handle gates, clocks, triggers, etc
	}

	protected abstract float compute_sample();
	
	public void debug_print()
	{
		#if DEBUG
		GD.Print($"Value: {_last_value}");
		#endif
	}
	
	public void debug_print_check()
	{
		#if DEBUG
		if (debug_enabled) debug_print();
		#endif
	}
}
