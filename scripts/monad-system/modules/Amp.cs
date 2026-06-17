using Godot;
using System;

[GlobalClass]
public partial class Amp : AudioModule
{
	public Mod input_param = new Mod(0.0f);
	public Mod level_param = new Mod(0.0f);
	
	public Amp()
	{
		rebuild_signal_chain();
	}
	
	public override Mod get_mod_from_name(string p_mod_name)
	{
		switch (p_mod_name.ToLower())
		{
			case "input":
				return input_param;
			case "level":
			case "gain":
				return level_param;
			default:
				GD.PrintErr($"[AudioModule Error] No mod named '{p_mod_name}' found in '{this.GetType().Name}'");
				return null;
		}
	}
	
	//public override void patch_in(string p_parameter_name, Func<float> p_mod_provider)
		//=> base.patch_in(p_parameter_name, p_mod_provider);
	//
	//public override AudioModule patch_out(AudioModule p_target_module, string p_target_param, float p_source_min = 0.0f, float p_source_max = 1.0f, float p_target_min = -1.0f, float p_target_max = 1.0f)
		//=> base.patch_out(p_target_module, p_target_param, p_source_min, p_source_max, p_target_min, p_target_max);
	
	protected override void update_state() {}
	
	protected override void rebuild_signal_chain()
	{
		_cached_audio_chain = new StereoSignal(() =>
		{
			_cached_mod_sample = input_param.evaluate() * level_param.evaluate();
			_cached_audio_left_sample = _cached_mod_sample;
			_cached_audio_right_sample = _cached_mod_sample;
			return new Vector2(_cached_audio_left_sample, _cached_audio_right_sample);
		});
		
		_cached_mod_chain = new MonoSignal(() =>
		{
			_cached_mod_sample = input_param.evaluate() * level_param.evaluate();
			return _cached_mod_sample;
		});
	}
}
