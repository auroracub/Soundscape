using System;
using Godot;

namespace Soundscape.Modules;

[GlobalClass]
public abstract partial class BaseModulator : Resource, IModulator
{
	[Export] public float frequency_hz { get; set; } = 440.0f;
	
	protected float _phase = 0.0f;
	protected float _current_output_sample = 0.0f;

	public BaseModulator() { }

	public virtual void process_frame(float p_delta)
	{
		_phase += (frequency_hz * p_delta);// + PhaseModulation;
		
		// Keep the floating point phase bounded cleanly within 0.0 and 1.0
		if (_phase >= 1.0f) _phase -= (float)Math.Floor(_phase);
		if (_phase < 0.0f) _phase += (float)Math.Ceiling(Math.Abs(_phase));
	
		//float next_phase = _phase + (frequency_hz * p_delta);
		//_phase = next_phase - (float)System.Math.Floor(next_phase);
		//
		//_current_output_sample = compute_sample(p_delta);
	}

	public float get_current_value() => compute_sample(); // _current_output_sample;

	public virtual void set_control_signal(float p_signal)
	{
		if (p_signal > 0.5f) _phase = 0.0f;
	}

	protected abstract float compute_sample();
}
