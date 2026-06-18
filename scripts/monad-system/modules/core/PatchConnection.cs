using Godot;

public class PatchConnection
{
	public AudioModule source { get; private set; }
	
	private float _scale;
	private float _bias;

	public PatchConnection(AudioModule p_source, float p_t_min, float p_t_max, float p_s_min, float p_s_max)
	{
		source = p_source;
		
		float src_range = p_s_max - p_s_min;
		if (src_range == 0.0f) src_range = 1.0f;

		// Precalculate linear coefficients from y = mx + c
		_scale = (p_t_max - p_t_min) / src_range;
		_bias = p_t_min - p_s_min * _scale;
	}

	public float evaluate()
	{
		return source.process_mono_sample(source.last_sample_rate) * _scale + _bias;
	}
}
