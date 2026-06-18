using Godot;
using System;

public enum TileMode
{
	Clamp,
	Loop,
	Scale,
}

[GlobalClass]
public partial class Selector : AudioModule
{
	[Export] public float[] list = [];
	[Export] public TileMode tile_mode = TileMode.Clamp;
	[Export] public bool interpolate = false;
	
	public Mod position_param = new Mod(0.0f); // Used to index the list
	
	protected float _current_value = 0.0f;
	
	public override Mod get_mod_from_name(string p_mod_name)
	{
		switch (p_mod_name.ToLower())
		{
			case "pos":
			case "position":
				return position_param;
			default:
				GD.PrintErr($"[AudioModule Error] No mod named '{p_mod_name}' found in '{this.GetType().Name}'");
				return null;
		}
	}
	
	protected override void update_state(float p_sample_rate)
	{
		float position = position_param.evaluate() - 1;
		int index_a = (int)position;
		int index_b = index_a + 1;
		float fraction = MathF.Truncate(position); // position - index
		int max_index = list.Length - 1;
		
		switch (tile_mode)
		{
			case TileMode.Clamp:
				index_a = Math.Clamp(index_a, 0, max_index);
				
				if (interpolate && (index_b < max_index))
				{
					_current_value = float.Lerp(list[index_a], list[index_b], fraction);
				}
				else
				{
					_current_value = list[index_a];
				}
				break;
			case TileMode.Loop:
				index_a = (int)MathLib.wrap(index_a, 0, max_index);
				index_b = (int)MathLib.wrap(index_b, 0, max_index);
				
				if (interpolate)
				{
					_current_value = float.Lerp(list[index_a], list[index_b], fraction);
				}
				else
				{
					_current_value = list[index_a];
				}
				break;
			case TileMode.Scale:
				int temp_a = (int)MathLib.wrap(index_a, 0, max_index);
				int temp_b = (int)MathLib.wrap(index_b, 0, max_index);
				float mult_a = (int)(Math.Abs(index_a - temp_a) / list.Length) + 1;
				float mult_b = (int)(Math.Abs(index_b - temp_b) / list.Length) + 1;
				
				if (interpolate)
				{
					_current_value = float.Lerp(list[index_a] * mult_a, list[index_b] * mult_b, fraction);
				}
				else
				{
					_current_value = list[index_a] * mult_a;
				}
				break;
			default:
				GD.PrintErr("[Select Error] Case not implemented");
				break;
		}
		
		cached_mono_sample = _current_value;
		cached_stereo_sample.X = cached_mono_sample;
		cached_stereo_sample.Y = cached_mono_sample;
	}
}
