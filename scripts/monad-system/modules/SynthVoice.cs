using Godot;
using System;

[GlobalClass]
public partial class SynthVoice : AudioModule
{
	public Mod target_frequency_param = new Mod(440.0f);
	public Mod glide_time_param = new Mod(0.0f);
	public OscWavetable _oscillator = new OscWavetable();
	public Envelope _envelope = new Envelope();
	
	private float _current_frequency = 440.0f;
	private long _last_ticked_frame = -1;

	public SynthVoice()
	{
		rebuild_signal_chain();
	}
	
	public override Mod get_mod_from_name(string p_mod_name)
	{
		switch (p_mod_name.ToLower())
		{
			case "freq":
			case "frequency":
				return target_frequency_param;
			case "glide":
				return glide_time_param;
			case "gate":
				return _envelope.gate_param;
			case "morph":
			case "position":
			case "pos":
				return _oscillator.morph_param;
			case "attack":
				return _envelope.attack_param;
			case "hold":
				return _envelope.hold_param;
			case "decay":
				return _envelope.decay_param;
			case "sustain":
				return _envelope.sustain_param;
			case "release":
				return _envelope.release_param;
			default:
				GD.PrintErr($"[AudioModule Error] No mod named '{p_mod_name}' found in '{this.GetType().Name}'");
				return null;
		}
	}
	
	public override void patch_in(string p_parameter_name, Func<float> p_mod_provider)
		=> base.patch_in(p_parameter_name, p_mod_provider);
	
	public override AudioModule patch_out(AudioModule p_target_module, string p_target_param, float p_source_min = 0.0f, float p_source_max = 1.0f, float p_target_min = -1.0f, float p_target_max = 1.0f)
		=> base.patch_out(p_target_module, p_target_param, p_source_min, p_source_max, p_target_min, p_target_max);
	
	public override StereoSignal get_audio_signal() => _cached_audio_chain;
	public override MonoSignal get_mod_signal() => _envelope.get_mod_signal();
	
	protected override void update_state() { }
	
	protected override void rebuild_signal_chain() {
		Func<float> glide = () =>
		{
			if (_last_ticked_frame == AudioClock.current_sample_index) return _current_frequency;
			_last_ticked_frame = AudioClock.current_sample_index;

			float target_freq = target_frequency_param.evaluate();
			float glide_time = Mathf.Max(0.0f, glide_time_param.evaluate());

			if (glide_time <= 0.0f) _current_frequency = target_freq;
			else
			{
				float difference = target_freq - _current_frequency;
				float step = difference / (glide_time * sample_rate);
				
				if (Mathf.Abs(step) >= Mathf.Abs(difference)) _current_frequency = target_freq;
				else _current_frequency += step;
			}
			return _current_frequency;
		};

		_oscillator.patch_in("frequency", glide);
		
		_cached_audio_chain = new StereoSignal(() =>
		{
			float osc_frame = _oscillator.get_mod_signal().evaluate();
			float env_frame = _envelope.get_mod_signal().evaluate();
			
			_cached_mod_sample = osc_frame * env_frame;
			_cached_audio_left_sample = _cached_mod_sample;
			_cached_audio_right_sample = _cached_mod_sample;
			return new Vector2(_cached_audio_left_sample, _cached_audio_right_sample);
		});
		
		_cached_mod_chain = new MonoSignal(() =>
		{
			float osc_frame = _oscillator.get_mod_signal().evaluate();
			float env_frame = _envelope.get_mod_signal().evaluate();
			
			_cached_mod_sample = osc_frame * env_frame;
			return _cached_mod_sample;
		});
	}
}
