using Godot;

namespace Soundscape.Synth;

public interface ISoundSource
{
	Vector2 generate_sample(float p_delta, float p_sample_rate);
	
	// Currently used for per-frame debugging purposes
	void process_frame(double p_delta);
}
