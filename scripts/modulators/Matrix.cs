using System.Collections.Generic;
// using Godot

namespace Soundscape.Modules;

public static class Matrix// : RefCounted
{
	private static readonly Dictionary<string, float> _matrix = new();

	// Writes or updates a control signal value on a specific bus channel
	public static void write(string name, float value)
	{
		_matrix[name] = value;
	}

	// Reads the current control value from a bus channel (defaults to 0 if empty)
	public static float read(string name)
	{
		if (string.IsNullOrEmpty(name)) return 0.0f;
		return _matrix.TryGetValue(name, out float value) ? value : 0.0f;
	}
	
	// Clears out old signal values if resetting your audio patch environment
	public static void clear_all()
	{
		_matrix.Clear();
	}
}
