using Godot;
using System;

[GlobalClass]
public partial class Voice : AudioModule
{
	public Mod target_frequency_param = new Mod(440.0f);
	public Mod glide_time_param = new Mod(0.0f);
	
	private float _current_frequency = 440.0f;
	
	public override Mod get_mod_from_name(string p_mod_name)
	{
		switch (p_mod_name.ToLower())
		{
			case "glide":
				return glide_time_param;
			case "freq":
			case "frequency":
				return target_frequency_param;
			default:
				GD.PrintErr($"[AudioModule Error] No mod named '{p_mod_name}' found in '{this.GetType().Name}'");
				return null;
		}
	}
	
	protected override void update_state(float p_sample_rate)
	{
		float target_freq = target_frequency_param.evaluate();
		float glide_time = Mathf.Max(0.0f, glide_time_param.evaluate()) * p_sample_rate;

		if (glide_time <= 0.0f) _current_frequency = target_freq;
		else
		{
			float difference = target_freq - _current_frequency;
			float step = difference / glide_time;
			
			if (Mathf.Abs(step) >= Mathf.Abs(difference)) _current_frequency = target_freq;
			else _current_frequency += step;
		}
		
		cached_mono_sample = _current_frequency;
		cached_stereo_sample.X = cached_mono_sample;
		cached_stereo_sample.Y = cached_mono_sample;
	}
}
