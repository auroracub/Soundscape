using Godot;

namespace Soundscape.Modules;

[GlobalClass]
public partial class WavetableModulator : BaseOscillator
{
	[Export] public float morph_position { get; set; } = 0.0f;

	protected override float compute_sample()
	{
		float sine_wave   = Mathf.Sin(_phase * Mathf.Tau);
		float tri_wave    = (Mathf.Abs(_phase - 0.5f) * 4.0f) - 1.0f;
		float saw_wave    = (_phase * 2.0f) - 1.0f;
		float square_wave = (_phase < 0.5f) ? 1.0f : -1.0f;

		float scaled_alpha = Mathf.Clamp(morph_position, 0.0f, 0.9999f) * 3.0f;
		int idx = (int)scaled_alpha;
		float weight = scaled_alpha - idx;

		float smooth_weight = weight * weight * (3.0f - 2.0f * weight);

		float current_left = (sine_wave * (idx == 0 ? 1f : 0f)) + 
							(tri_wave  * (idx == 1 ? 1f : 0f)) + 
							(saw_wave  * (idx == 2 ? 1f : 0f));
							
		float current_right = (tri_wave  * (idx == 0 ? 1f : 0f)) + 
							 (saw_wave  * (idx == 1 ? 1f : 0f)) + 
							 (square_wave * (idx == 2 ? 1f : 0f));

		return Mathf.Lerp(current_left, current_right, smooth_weight);
	}
}
