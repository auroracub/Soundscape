using Godot;
using System;

[GlobalClass]
public partial class OscSampleAndHold : OscBase
{
	protected override void update_state()
	{
		float frequency = frequency_param.evaluate();
		float amplitude = amplitude_param.evaluate();
		float current_phase = phase;
		
		advance_phase(frequency);

		// If the phase wraps around, it's time to generate a new sample
		if (phase < current_phase)
		{
			cached_mono_sample = (float)GD.RandRange(-1.0, 1.0) * amplitude;
		}
		
		cached_stereo_sample.X = cached_mono_sample;
		cached_stereo_sample.Y = cached_mono_sample;
	}
}
