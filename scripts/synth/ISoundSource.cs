using Godot;

namespace Soundscape.Modules;

public interface ISoundSource
{
	Vector2 generate_sample(float p_delta, float p_sample_rate);
}
