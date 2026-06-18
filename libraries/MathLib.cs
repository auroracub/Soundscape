using System;
using Godot;

// namespace Soundscape.Libraries;

[GlobalClass]
public partial class MathLib : Godot.GodotObject
{
	//public static float Remapf(float fromMin, float fromMax, float toMin, float toMax, float value)
	//{
		//return Mathf.Remap(value, fromMin, fromMax, toMin, toMax);
	//}
	
	public static float remap(float p_input_min, float p_input_max, float p_output_min, float p_output_max, float value)
	{
		return p_output_min + (value - p_input_min) * (p_output_max - p_output_min) / (p_input_max - p_input_min);
	}
	
	public static float wrap(float p_value, float p_min, float p_max)
	{
		return (p_value - p_min) % p_max + p_min;
	}
}
