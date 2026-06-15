using System;
using Godot;

namespace Soundscape.Libraries;

[GlobalClass]
public partial class MathLib : Godot.GodotObject
{
	public static float Remapf(float fromMin, float fromMax, float toMin, float toMax, float value)
	{
		return Mathf.Remap(value, fromMin, fromMax, toMin, toMax);
	}

	public static float SecondsToHz(float seconds)
	{
		if (seconds <= 0.00001f) return 0.0f;
		return 1.0f / seconds;
	}
}
