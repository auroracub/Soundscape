using System;
using Godot;

namespace Soundscape.Modules;

[GlobalClass]
public partial class Scale : Resource
{
	[Export] public string scale_name { get; set; } = "12-EDO";
	[Export] public string description { get; set; } = "Standard 12 equal divisions of the octave.";
	
	// Contiguous unmanaged array containing base ratios or cents for 1 period/octave
	[Export] public float[] tuning_data { get; set; } = Array.Empty<float>();
	
	[Export] public int notes_per_octave { get; set; } = 12;
	[Export] public float octave_ratio { get; set; } = 2.0f; // Usually 2.0 (pure octave), can be stretched
	[Export] public float base_frequency { get; set; } = 440.0f;
	[Export] public int base_midi_note { get; set; } = 69; // MIDI 69 is A4 (440Hz)
	
	public float[] frequency_cache
	{
		get 
		{
			if (!_is_cached) cache_frequencies();
			return _frequency_cache;
		}
	}
	
	// Pre-calculated absolute pitch array cache for ultra-fast, branchless index mappings
	private float[] _frequency_cache = Array.Empty<float>();
	private bool _is_cached = false;

	public Scale() { }

	// Factory method to quickly instantiate an Equal Division scale
	public static Scale create_edo(int divisions, float base_freq = 440.0f, int base_midi = 69)
	{
		var scale = new Scale
		{
			scale_name = $"{divisions}-EDO",
			description = $"{divisions} Equal Divisions of the Octave.",
			notes_per_octave = divisions,
			base_frequency = base_freq,
			base_midi_note = base_midi,
			tuning_data = new float[divisions]
		};

		// Populate relative octave steps mathematically
		for (int i = 0; i < divisions; i++)
		{
			scale.tuning_data[i] = Mathf.Pow(2.0f, (float)(i + 1) / divisions);
		}
		
		scale.cache_frequencies();
		return scale;
	}

	// Caches a pool of absolute frequencies
	public void cache_frequencies(int low_midi_bound = 0, int high_midi_bound = 127)
	{
		int total_notes = high_midi_bound - low_midi_bound + 1;
		_frequency_cache = new float[total_notes];
		
		for (int m = low_midi_bound; m <= high_midi_bound; m++)
		{
			int relative_note = m - base_midi_note;
			
			// Calculate which octave bank and note step the index lands on
			int octave_offset = Mathf.FloorToInt((float)relative_note / notes_per_octave);
			int scale_index = relative_note - (octave_offset * notes_per_octave);

			float octave_factor = Mathf.Pow(octave_ratio, octave_offset);
			
			// Note 0 is the root frequency
			float step_ratio = 1.0f;
			if (scale_index > 0 && scale_index <= tuning_data.Length)
			{
				step_ratio = tuning_data[scale_index - 1];
			}
			else if (scale_index < 0)
			{
				// Handle negative array offsets wrap safely
				step_ratio = tuning_data[tuning_data.Length + scale_index];
				octave_factor /= octave_ratio;
			}

			_frequency_cache[m - low_midi_bound] = base_frequency * step_ratio * octave_factor;
		}
		_is_cached = true;
	}

	// Fast buffer lookup wrapper
	public float get_frequency_from_midi(int midiNote)
	{
		if (!_is_cached) cache_frequencies();
		int clamped_midi = Mathf.Clamp(midiNote, 0, _frequency_cache.Length - 1);
		return _frequency_cache[clamped_midi];
	}
}
