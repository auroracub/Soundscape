using Godot;
using System;

[GlobalClass]
public partial class SynthSpeaker2D : AudioStreamPlayer2D
{
	[Export] public float sample_rate { get; set; } = 44100.0f;
	
	private AudioStreamGeneratorPlayback _playback;
	private AudioModule _source_module;
	private float _master_gain = 0.5f; 
	
	public void connect_source(AudioModule p_module)
	{
		_source_module = p_module;
	}

	public override void _Ready()
	{
		var generator = new AudioStreamGenerator();
		generator.MixRate = sample_rate;
		generator.BufferLength = 0.05f; 
		
		Stream = generator;
		// Let them blow their ears out
		// MaxDb = -18.0f;

		Play();
		_playback = GetStreamPlayback() as AudioStreamGeneratorPlayback;
	}

	public override void _Process(double p_delta)
	{
		if (_playback == null || _source_module == null) return;

		int frames_available = _playback.GetFramesAvailable();
		if (frames_available <= 0) return;

		Vector2[] sample_buffer = new Vector2[frames_available];

		for (int i = 0; i < frames_available; i++)
		{
			AudioClock.current_sample_index++;
			
			Vector2 sample = _source_module.tick_stereo() * _master_gain;
			
			sample.X = Math.Clamp(sample.X, -1.0f, 1.0f);
			sample.Y = Math.Clamp(sample.Y, -1.0f, 1.0f);
			
			sample_buffer[i] = sample;
		}

		_playback.PushBuffer(sample_buffer);
	}
}
