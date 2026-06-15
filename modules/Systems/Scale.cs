using System;
using Godot;

namespace Soundscape.Modules;

[GlobalClass]
public partial class Scale : Resource
{
	[Export] public string ScaleName { get; set; } = "12-EDO";
	[Export] public string Description { get; set; } = "Standard 12 equal divisions of the octave.";
	
	// Contiguous unmanaged array containing base ratios or cents for 1 period/octave
	[Export] public float[] TuningData { get; set; } = Array.Empty<float>();
	
	[Export] public int NotesPerOctave { get; set; } = 12;
	[Export] public float OctaveRatio { get; set; } = 2.0f; // Usually 2.0 (pure octave), can be stretched
	[Export] public float BaseFrequency { get; set; } = 440.0f;
	[Export] public int BaseMidiNote { get; set; } = 69; // MIDI 69 is A4 (440Hz)
	
	public float[] FrequencyCache 
	{
		get 
		{
			if (!_isCached) CacheFrequencies();
			return _frequencyCache;
		}
	}
	
	// Pre-calculated absolute pitch array cache for ultra-fast, branchless index mappings
	private float[] _frequencyCache = Array.Empty<float>();
	private bool _isCached = false;

	public Scale() { } // Mandated parameterless constructor for Godot Interop

	// Factory method to quickly instantiate an Equal Division scale (e.g., 12-EDO, 19-EDO, 22-EDO)
	public static Scale CreateEDO(int divisions, float baseFreq = 440.0f, int baseMidi = 69)
	{
		var scale = new Scale
		{
			ScaleName = $"{divisions}-EDO",
			Description = $"{divisions} Equal Divisions of the Octave.",
			NotesPerOctave = divisions,
			BaseFrequency = baseFreq,
			BaseMidiNote = baseMidi,
			TuningData = new float[divisions]
		};

		// Populate relative octave steps mathematically
		for (int i = 0; i < divisions; i++)
		{
			scale.TuningData[i] = Mathf.Pow(2.0f, (float)(i + 1) / divisions);
		}
		
		scale.CacheFrequencies();
		return scale;
	}

	// Pre-bakes a pool of absolute frequencies to keep the audio thread branchless
	public void CacheFrequencies(int lowMidiBound = 0, int highMidiBound = 127)
	{
		int totalNotes = highMidiBound - lowMidiBound + 1;
		_frequencyCache = new float[totalNotes];

		for (int m = lowMidiBound; m <= highMidiBound; m++)
		{
			int relativeNote = m - BaseMidiNote;
			
			// Calculate which octave bank and note step the index lands on
			int octaveOffset = Mathf.FloorToInt((float)relativeNote / NotesPerOctave);
			int scaleIndex = relativeNote - (octaveOffset * NotesPerOctave);

			float octaveFactor = Mathf.Pow(OctaveRatio, octaveOffset);
			
			// Note 0 is the base octave root frequency (ratio 1.0)
			float stepRatio = 1.0f;
			if (scaleIndex > 0 && scaleIndex <= TuningData.Length)
			{
				stepRatio = TuningData[scaleIndex - 1];
			}
			else if (scaleIndex < 0)
			{
				// Handle negative array offsets wrap safely
				stepRatio = TuningData[TuningData.Length + scaleIndex];
				octaveFactor /= OctaveRatio;
			}

			_frequencyCache[m - lowMidiBound] = BaseFrequency * stepRatio * octaveFactor;
		}
		_isCached = true;
	}

	// Fast O(1) buffer lookup wrapper
	public float GetFrequencyByMidi(int midiNote)
	{
		if (!_isCached) CacheFrequencies();
		int clampedMidi = Mathf.Clamp(midiNote, 0, _frequencyCache.Length - 1);
		return _frequencyCache[clampedMidi];
	}
}
