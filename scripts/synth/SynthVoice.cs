using Godot;
using Godot.Collections;
using System;

namespace Soundscape.Synth;

[GlobalClass]
public partial class SynthVoice : RefCounted, ISoundSource
{
	[Export] public Array<Modules.Patch> patches { get; set; } = new();
	public Modules.OscWavetable tone { get; private set; } = new Modules.OscWavetable();
	
	[Export] public float glide_time { get; set; } = 0.0f;
	
	float _current_hz = 0.0f;
	float _target_hz = 0.0f;
	
	public float get_current_frequency() => _current_hz;
	public float get_target_frequency() => _target_hz;

	public void set_current_frequency(float p_frequency)
	{
		_current_hz = p_frequency;
		_target_hz = p_frequency;
	}

	public void set_target_frequency(float p_frequency)
	{
		_target_hz = p_frequency;
	}
	
	public void process_frame(double p_delta)
	{
		#if DEBUG
		for (int i = 0; i < patches.Count; i++)
		{
			patches[i]?.modulator?.debug_print_check();
		}
		#endif
	}
	
	public Vector2 generate_sample(float p_delta, float _sample_rate)
	{
		// Frame-rate independent glide calculation
		if (glide_time > 0.001f)
		{
			float alpha = 1.0f - (float)Math.Exp(-p_delta / glide_time);
			_current_hz = Mathf.Lerp(_current_hz, _target_hz, alpha);
		}
		else
		{
			_current_hz = _target_hz;
		}
		
		float pitch_mod = 0.0f;
		float tone_mod = 0.0f;
		float amp_mod = 0.0f;
		float pan_mod = 0.0f;

		// Evaluate and calculate cv patch input matrix
		for (int i = 0; i < patches.Count; i++)
		{
			var patch = patches[i];
			if (patch == null) continue;

			// Modular mod = patch.cached_source;
			Modules.Modular mod = patch.modulator;
			if (mod == null) continue;

			mod.process_frame(p_delta);
			float value = Libraries.MathLib.remap(0.0f, 1.0f, patch.map_lo, patch.map_hi, mod.get_current_value());
			
			// Route values safely using string mappings
			if (patch.destination == "pitch") pitch_mod += value;
			else if (patch.destination == "tone") tone_mod += value;
			else if (patch.destination == "amp") amp_mod += value;
			else if (patch.destination == "pan") pan_mod += value;
		}
		
		float final_hz = _current_hz * (1.0f + pitch_mod);
		tone.frequency_hz = final_hz;
		tone.morph_position = tone_mod;
		
		tone.process_frame(p_delta);
		
		float raw_wave = tone.get_current_value();
		float final_amplitude = raw_wave * amp_mod;

		float pan_angle = (Mathf.Clamp(pan_mod, -1.0f, 1.0f) + 1.0f) * (Mathf.Pi / 4.0f);
		return new Vector2(
			final_amplitude * Mathf.Cos(pan_angle),
			final_amplitude * Mathf.Sin(pan_angle)
		);
	}

	public void send_pulse(float p_signal)
	{
		for (int i = 0; i < patches.Count; i++)
		{
			// patches[i].cached_source?.set_control_signal(p_signal);
			patches[i].modulator?.set_control_signal(p_signal);
		}
	}
}
