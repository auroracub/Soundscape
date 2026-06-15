using System;
using Godot;

namespace Soundscape.Modules;

[GlobalClass]
public partial class SynthSpeaker3D : RaytracedAudioPlayer3D
{
	[Export] public float sample_rate { get; set; } = 22050.0f;

	private ISoundSource _active_audio_source;

	[Export]
	public RefCounted audio_input_source
	{
		get => _active_audio_source as RefCounted;
		set => _active_audio_source = value is ISoundSource source ? source : null;
	}
	
	private AudioStreamGeneratorPlayback _playback;

	public override void _Ready()
	{
		var generator = new AudioStreamGenerator();
		generator.MixRate = sample_rate;
		Stream = generator;

		// Recommended settings:
		// VolumeDb = -18.0f;
		// UnitSize = 10.0f;
		// MaxDb = -18.0f;
		// MaxDistance = 200.0f;

		Play();
		_playback = GetStreamPlayback() as AudioStreamGeneratorPlayback;
	}

	public override void _Process(double p_delta)
	{
		if (_playback == null || _active_audio_source == null) return;

		int frames_available = _playback.GetFramesAvailable();
		if (frames_available <= 0) return;

		float delta = 1.0f / sample_rate;
		Vector2[] sample_buffer = new Vector2[frames_available];

		for (int i = 0; i < frames_available; i++)
		{
			sample_buffer[i] = _active_audio_source.generate_sample(delta, sample_rate);
		}

		_playback.PushBuffer(sample_buffer);
	}
}
