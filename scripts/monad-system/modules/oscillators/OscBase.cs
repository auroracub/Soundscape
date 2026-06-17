using Godot;
using System;

[GlobalClass]
public abstract partial class OscBase : AudioModule
{
	public Mod frequency_param = new Mod(440.0f);
	public Mod amplitude_param = new Mod(1.0f);
	
	protected float phase = 0.0f;
	protected long last_ticked_frame = -1;
	
	public override Mod get_mod_from_name(string p_mod_name)
	{
		switch (p_mod_name.ToLower())
		{
			case "freq":
			case "frequency":
				return frequency_param;
			case "amp":
			case "amplitude":
				return amplitude_param;
			default:
				GD.PrintErr($"[AudioModule Error] No mod named '{p_mod_name}' found in '{this.GetType().Name}'");
				return null;
		}
	}
	
	public override void patch_in(string p_parameter_name, Func<float> p_mod_provider)
		=> base.patch_in(p_parameter_name, p_mod_provider);
	
	public override AudioModule patch_out(AudioModule p_target_module, string p_target_param, float p_source_min = 0.0f, float p_source_max = 1.0f, float p_target_min = -1.0f, float p_target_max = 1.0f)
		=> base.patch_out(p_target_module, p_target_param, p_source_min, p_source_max, p_target_min, p_target_max);//, p_source_channel);
	
	protected void advance_phase(float p_freq)
	{
		phase += (MathF.Tau * p_freq) / sample_rate;
		if (phase >= MathF.Tau) phase -= MathF.Tau;
	}
}
