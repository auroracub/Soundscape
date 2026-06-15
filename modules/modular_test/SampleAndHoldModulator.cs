using Godot;

namespace Soundscape.Modules;

[GlobalClass]
public partial class SampleAndHoldModulator : BaseModulator
{
	private float _last_sampled_value = 0.0f;
	private float _previous_phase = 0.0f;

	protected override float compute_sample()
	{
		// Detect phase wrap boundary (indicates a new cycle start)
		// Since phase runs 0.0 -> 1.0, a wrap happens when current phase is less than previous phase
		float has_wrapped = (_phase < _previous_phase) ? 1.0f : 0.0f;
		_previous_phase = _phase;

		// Generate a pseudo-random hash value based on system time and frequency steps
		float seed = (float)GD.RandRange(0.0, 1000.0);
		float raw_random = (Mathf.Sin(seed * 12.9898f) * 43758.5453f);
		float new_random_sample = ((raw_random - Mathf.Floor(raw_random)) * 2.0f) - 1.0f;

		// Blending mask: updates value only if hasWrapped == 1.0f
		_last_sampled_value = Mathf.Lerp(_last_sampled_value, new_random_sample, has_wrapped);

		return _last_sampled_value;
	}
}
