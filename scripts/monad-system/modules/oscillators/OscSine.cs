using Godot;
using System;

[GlobalClass]
public partial class OscSine : OscBase
{
	protected override void update_state(float p_sample_rate)
	{
		float frequency = frequency_param.evaluate();
		float amplitude = amplitude_param.evaluate();
		
		advance_phase(p_sample_rate, frequency);
		
		cached_mono_sample = MathF.Sin(phase) * amplitude;
		cached_stereo_sample.X = cached_mono_sample;
		cached_stereo_sample.Y = cached_mono_sample;
	}
}
