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
	
	public static float remap(float in_min, float in_max, float out_min, float out_max, float value)
	{
		return out_min + (value - in_min) * (out_max - out_min) / (in_max - in_min);
	}
	
	public static float seconds_to_hz(float seconds)
	{
		if (seconds <= 0.00001f) return 0.0f;
		return 1.0f / seconds;
	}
}
