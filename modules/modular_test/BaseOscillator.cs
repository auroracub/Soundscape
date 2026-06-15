using System;
using Godot;

namespace Soundscape.Modules;

[GlobalClass]
public abstract partial class BaseOscillator : BaseModulator
{
	[Export] public float frequency_hz { get; set; } = 440.0f;
	[Export] public float phase_modulation { get; set; } = 0.0f;
	
	protected float _phase = 0.0f;

	public BaseOscillator() { }

	public override void process_frame(float p_delta)
	{
		_phase += frequency_hz * p_delta + phase_modulation;

		// Keep the floating point phase bounded cleanly within 0.0 and 1.0
		if (_phase >= 1.0f) _phase -= (float)Math.Floor(_phase);
		if (_phase < 0.0f) _phase += (float)Math.Ceiling(Math.Abs(_phase));
	}

	public override void set_control_signal(float p_signal)
	{
		// Retain standard oscillator behavior: reset wave phase on a hard sync trigger
		if (p_signal > 0.5f) _phase = 0.0f;
	}
}
