using System;
using Godot;

// namespace Soundscape.Libraries;

[GlobalClass]
public partial class MusicLib : RefCounted
{
	public static float get_degree_from_frequency(Soundscape.Modules.Scale scale, float target_frequency)
	{
		if (scale == null || target_frequency <= 0.0f) return 0.0f;

		float[] cache = scale.frequency_cache;
		int count = cache.Length;

		// Edge case guards
		if (target_frequency <= cache[0]) return 0.0f;
		if (target_frequency >= cache[count - 1]) return (float)(count - 1);

		// Binary Search to find the neighboring indices
		int low = 0;
		int high = count - 1;

		while (low <= high)
		{
			int mid = (low + high) >> 1; // Bitshift division by 2 for speed
			float mid_val = cache[mid];

			if (mid_val < target_frequency)
				low = mid + 1;
			else if (mid_val > target_frequency)
				high = mid - 1;
			else
				return (float)mid; // Exact frequency match
		}

		// Extract the two closest neighboring notes
		// After binary search finishes without an exact match, 'high' is the left index, 'low' is the right index.
		int idx_left = high;
		int idx_right = low;

		float freq_left = cache[idx_left];
		float freq_right = cache[idx_right];

		// Calculate the fractional position between the two notes
		float fractional_weight = (target_frequency - freq_left) / (freq_right - freq_left);

		// Convert fractional array index back into a scale degree relative to the scale's base midi note
		float index_relative = (float)idx_left + fractional_weight;
		float degree_from_base = index_relative - scale.base_midi_note;

		return degree_from_base;
	}
	
	//// High-performance, unrolled branchless-style decision tree for a 128-element monotonic array.
	//// Guarantees finding the bounding pair in exactly 7 direct pointer comparisons.
	//public static float GetDegreeFromFrequency(Scale scale, float targetFrequency)
	//{
		//if (scale == null || targetFrequency <= 0.0f) return 0.0f;
//
		//float[] cache = scale.FrequencyCache;
		//
		//// Safety check to ensure array size matches our unrolled 128-note decision tree
		//if (cache.Length != 128) 
		//{
			//return GetDegreeFallback(cache, targetFrequency, scale.BaseMidiNote);
		//}
//
		//// 1. Boundary Guards
		//if (targetFrequency <= cache[0]) return 0.0f - scale.BaseMidiNote;
		//if (targetFrequency >= cache[127]) return 127.0f - scale.BaseMidiNote;
//
		//// 2. Unrolled Binary Search Decision Tree (7 Steps for 128 Elements)
		//int idx = 64;
		//idx = (targetFrequency < cache[idx]) ? idx - 32 : idx + 32; // Step 1
		//idx = (targetFrequency < cache[idx]) ? idx - 16 : idx + 16; // Step 2
		//idx = (targetFrequency < cache[idx]) ? idx - 8  : idx + 8;  // Step 3
		//idx = (targetFrequency < cache[idx]) ? idx - 4  : idx + 4;  // Step 4
		//idx = (targetFrequency < cache[idx]) ? idx - 2  : idx + 2;  // Step 5
		//idx = (targetFrequency < cache[idx]) ? idx - 1  : idx + 1;  // Step 6
//
		//// Step 7: Final adjustment to lock onto the exact left bounding index
		//int idxLeft = (targetFrequency < cache[idx]) ? idx - 1 : idx;
		//int idxRight = idxLeft + 1;
//
		//// 3. Mathematical Interpolation between the strict monotonic neighbors
		//float freqLeft = cache[idxLeft];
		//float freqRight = cache[idxRight];
		//
		//float fractionalWeight = (targetFrequency - freqLeft) / (freqRight - freqLeft);
		//
		//return (float)idxLeft + fractionalWeight - scale.BaseMidiNote;
	//}
//
	//// Fallback standard binary search if your scale cache is ever dynamically re-sized outside of 128 notes
	//private static float GetDegreeFallback(float[] cache, float targetFrequency, int baseMidiNote)
	//{
		//int low = 0;
		//int high = cache.Length - 1;
		//if (targetFrequency <= cache[0]) return 0.0f - baseMidiNote;
		//if (targetFrequency >= cache[high]) return (float)high - baseMidiNote;
//
		//while (low <= high)
		//{
			//int mid = (low + high) >> 1;
			//if (cache[mid] < targetFrequency) low = mid + 1;
			//else if (cache[mid] > targetFrequency) high = mid - 1;
			//else return (float)mid - baseMidiNote;
		//}
		//
		//float fractionalWeight = (targetFrequency - cache[high]) / (cache[low] - cache[high]);
		//return (float)high + fractionalWeight - baseMidiNote;
	//}
	
	// Resolves the absolute target frequency by taking a starting MIDI note and moving N scale degrees up or down
	public static float get_nth_degree_frequency(Soundscape.Modules.Scale scale, int current_midi_note, int degree_offset)
	{
		if (scale == null) return 440.0f; // Fail-safe fallback

		// Scale steps translate directly to shifting our absolute MIDI note index bound
		int target_midi_note = current_midi_note + degree_offset;
		
		return scale.get_frequency_from_midi(target_midi_note);
	}

	// Convenience function to resolve the target frequency from an offset note index directly
	public static float get_frequency_from_root(Soundscape.Modules.Scale scale, int degree_from_root)
	{
		if (scale == null) return 440.0f;
		return scale.get_frequency_from_midi(scale.base_midi_note + degree_from_root);
	}
}
