using System;
using Godot.Collections;
using Godot;

namespace Soundscape.Modules;

//public interface IModulator
//{
	//void process_frame(float p_delta);
	//float get_current_value();
	//void set_control_signal(float p_signal);
//}

[GlobalClass]
public abstract partial class Modular : Resource//, IModulator
{
	public Modular() { }
	
	[Export] public bool debug_enabled = false;
	[Export] public string matrix_target { get; set; } = "";
	[Export] public Array<Modular> subscribers { get; set; } = new();
	
	protected float _last_value = 0.0f;
	protected bool _is_dirty = true;
	
	// public void mark_dirty() => _is_dirty = true;
	
	public void process_frame(float p_delta)
	{
		if (!_is_dirty) return;
		
		process_frame_concrete(p_delta);
		
		for (int i = 0; i < subscribers.Count; ++i)
		{
			subscribers[i]?.process_frame(p_delta);
		}
		
		// Automatically broadcast value if a matrix slot is assigned
		if (!string.IsNullOrEmpty(matrix_target))
		{
			Matrix.write(matrix_target, _last_value);
		}
		
		_is_dirty = true;
	}
	
	public abstract void process_frame_concrete(float p_delta);

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
