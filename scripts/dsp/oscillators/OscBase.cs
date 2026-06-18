using Godot;
using System;

[GlobalClass]
public abstract partial class OscBase : AudioModule
{
	public Mod frequency_param = new Mod(440.0f);
	public Mod amplitude_param = new Mod(1.0f);
	protected float phase = 0.0f;
	
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
	
	protected void advance_phase(float p_sample_rate, float p_freq)
	{
		phase += (MathF.Tau * p_freq) / p_sample_rate;
		while (phase >= MathF.Tau) phase -= MathF.Tau;
		while (phase < 0.0f) phase += MathF.Tau;
	}
}
