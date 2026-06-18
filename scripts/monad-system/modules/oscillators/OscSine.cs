using Godot;
using System;

[GlobalClass]
public partial class OscSine : OscBase
{
	public OscSine() { rebuild_signal_chain(); }

	public override void patch_in(string p_parameter_name, Func<float> p_mod_provider)
		=> base.patch_in(p_parameter_name, p_mod_provider);
	
	public override AudioModule patch_out(AudioModule p_target_module, string p_target_param, float p_source_min = 0.0f, float p_source_max = 1.0f, float p_target_min = -1.0f, float p_target_max = 1.0f)
		=> base.patch_out(p_target_module, p_target_param, p_source_min, p_source_max, p_target_min, p_target_max);//, p_source_channel);
	
	protected override void update_state()
	{
		if (last_ticked_frame == AudioClock.current_sample_index) return;
		last_ticked_frame = AudioClock.current_sample_index;

		float frequency = frequency_param.evaluate();
		float amplitude = amplitude_param.evaluate();
		advance_phase(frequency);
		
		_cached_mono_sample = MathF.Sin(phase) * amplitude;
		_cached_stereo_sample.X = _cached_mono_sample;
		_cached_stereo_sample.Y = _cached_mono_sample;
	}
}
