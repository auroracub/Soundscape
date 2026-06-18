using Godot;

public class PatchConnection
{
	public AudioModule source { get; private set; }
	
	public PatchConnection(AudioModule p_source)
	{
		source = p_source;
	}

	public float evaluate()
	{
		return source.process_mono_sample(source.last_sample_rate);
	}
}
