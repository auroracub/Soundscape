using Godot;
using System;

[GlobalClass]
public partial class OscSine : OscBase
{
	// Work in progress...
	
	// Morph of -1 should be skewed left
	// Morph of 0 should be centered
	// Morph of 1 should be skewed right
	
	protected override void update_state(float p_sample_rate)
	{
		float frequency = frequency_param.evaluate();
		float amplitude = amplitude_param.evaluate();
		float morph = morph_param.evaluate();
		
		advance_phase(p_sample_rate, frequency);
		
		cached_mono_sample = (MathF.Sin(phase) + (morph * MathF.Sin(2.0f * phase))) * amplitude;
		cached_stereo_sample.X = cached_mono_sample;
		cached_stereo_sample.Y = cached_mono_sample;
	}
	
	// Blehh
	public override void set_value(string p_mod_name, float p_value)
		=> base.set_value(p_mod_name, p_value);
	public override void set_patch(AudioModule p_target_module, string p_target_mod_name)
		=> base.set_patch(p_target_module, p_target_mod_name);
	public override void remove_patch(AudioModule p_target_module, string p_target_mod_name)
		=> base.remove_patch(p_target_module, p_target_mod_name);
	public override void map_add(AudioModule p_target_module, string p_target_mod_name, 
		float p_target_min = -1.0f, float p_target_max = 1.0f)
		=> base.map_add(p_target_module, p_target_mod_name, p_target_min, p_target_max);
	public override void map_add_default(AudioModule p_target_module, string p_target_mod_name)
		=> base.map_add_default(p_target_module, p_target_mod_name);
	public override void map_multiply(AudioModule p_target_module, string p_target_mod_name, 
		float p_target_min = -1.0f, float p_target_max = 1.0f)
		=> base.map_multiply(p_target_module, p_target_mod_name, p_target_min, p_target_max);
	public override void map_multiply_default(AudioModule p_target_module, string p_target_mod_name)
		=> base.map_multiply_default(p_target_module, p_target_mod_name);
}
