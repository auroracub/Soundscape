using Godot;

namespace Soundscape.Modules;

public interface ISoundSource
{
	// High-speed method to generate a single mono sample step
	Vector2 generate_sample(float p_delta, float p_sample_rate);
}
