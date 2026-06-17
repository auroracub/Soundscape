using System;
using Godot;

// namespace Soundscape.Libraries;

[GlobalClass]
public partial class AudioLib : Godot.GodotObject
{
	public static float SecondsToHz(float seconds)
	{
		if (seconds <= 0.00001f) return 0.0f;
		return 1.0f / seconds;
	}

	public static float HzToSeconds(float hz)
	{
		if (hz <= 0.00001f) return 0.0f;
		return 1.0f / hz;
	}

	// Converts a standard MIDI note integer calculation down into true Hertz frequency
	public static float MidiToHz(int midiNote)
	{
		return 440.0f * (float)Math.Pow(2.0, (midiNote - 69) / 12.0);
	}
	
	// Converts a standard MIDI note integer calculation down into true Hertz frequency
	public static int HzToMidi(float hz)
	{
		// Hasn't been tested yet
		return (int)(Math.Log(hz / 440.0f) * 12.0) + 69;
	}
}
