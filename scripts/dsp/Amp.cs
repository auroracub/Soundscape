using Godot;

[GlobalClass]
public partial class Amp : AudioModule
{
	public Mod input_param = new Mod(0.0f);
	public Mod level_param = new Mod(0.0f);
	
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
	
	protected override void update_state(float p_sample_rate)
	{
		cached_mono_sample = input_param.evaluate() * level_param.evaluate();
		cached_stereo_sample.X = cached_mono_sample;
		cached_stereo_sample.Y = cached_mono_sample;
	}
}
