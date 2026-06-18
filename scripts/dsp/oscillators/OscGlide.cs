using Godot;
using System;

[GlobalClass]
public partial class OscGlide : OscBase
{
	// Work in progress...
	
	protected override void update_state(float p_sample_rate)
	{
		float frequency = frequency_param.evaluate();
		float amplitude = amplitude_param.evaluate();
		float morph = morph_param.evaluate();
		float current_phase = phase;
		
		advance_phase(p_sample_rate, frequency);

		// If the phase wraps around, it's time to generate a new sample
		if (phase < current_phase)
		{
			cached_mono_sample = (MathF.Sin(2.0f * morph * phase) + MathF.Sin(MathF.PI * phase)) * 0.5f * amplitude;
		}
		
		cached_stereo_sample.X = cached_mono_sample;
		cached_stereo_sample.Y = cached_mono_sample;
	}
}
