using System;
using Godot;

// namespace Soundscape.Libraries;

[GlobalClass]
public partial class AudioLib : Godot.GodotObject
{
	public static float seconds_to_hz(float p_seconds)
	{
		if (p_seconds <= 0.00001f) return 0.0f;
		return 1.0f / p_seconds;
	}

	public static float hz_to_seconds(float p_hz)
	{
		if (p_hz <= 0.00001f) return 0.0f;
		return 1.0f / p_hz;
	}
	
	public static float midi_to_hz(int p_midi_note)
	{
		return 440.0f * (float)Math.Pow(2.0, (p_midi_note - 69) / 12.0);
	}
	
	public static int hz_to_midi(float p_hz)
	{
		// Hasn't been tested yet
		return (int)(Math.Log(p_hz / 440.0f) * 12.0) + 69;
	}
}
