using Godot;
using System;

[GlobalClass]
public partial class SynthSpeaker3D : AudioStreamPlayer3D
{
	[Export] public float sample_rate { get; set; } = 22050.0f;
	
	private AudioStreamGeneratorPlayback _playback;
	private StereoSignal _input_signal;
	protected float _master_gain = 0.5f; // Lower the volume to remove clipping/artifacts
	
	public void connect_source(AudioModule p_module)
	{
		_input_signal = p_module.get_audio_signal();
	}

	public override void _Ready()
	{
		var generator = new AudioStreamGenerator();
		generator.MixRate = sample_rate;
		Stream = generator;

		// Recommended settings:
		// VolumeDb = -18.0f;
		// UnitSize = 10.0f;
		MaxDb = -18.0f;
		// MaxDistance = 200.0f;

		Play();
		_playback = GetStreamPlayback() as AudioStreamGeneratorPlayback;
	}

	public override void _Process(double p_delta)
	{
		if (_playback == null || _input_signal == null) return;

		int frames_available = _playback.GetFramesAvailable();
		if (frames_available <= 0) return;

		Vector2[] sample_buffer = new Vector2[frames_available];

		for (int i = 0; i < frames_available; i++)
		{
			AudioClock.current_sample_index++;
			
			Vector2 sample = _input_signal.evaluate() * _master_gain;
			sample.X = Math.Clamp(sample.X, -1.0f, 1.0f);
			sample.Y = Math.Clamp(sample.Y, -1.0f, 1.0f);
			
			sample_buffer[i] = sample;
		}

		_playback.PushBuffer(sample_buffer);
	}
}
