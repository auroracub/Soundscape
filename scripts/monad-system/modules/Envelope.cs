using Godot;
using System;

[GlobalClass]
public partial class Envelope : AudioModule
{
	public Mod gate_param = new Mod(0.0f);
	public Mod attack_param = new Mod(0.1f);
	public Mod hold_param = new Mod(0.0f);
	public Mod decay_param = new Mod(0.2f);
	public Mod sustain_param = new Mod(0.7f);
	public Mod release_param = new Mod(0.5f);

	private float _current_level = 0.0f;
	private bool _is_note_on = false;
	private float _prev_trigger = 0.0f;
	private float _hold_timer = 0.0f;
	private long _last_ticked_frame = -1;

	public Envelope() { rebuild_signal_chain(); }
	
	public override Mod get_mod_from_name(string p_mod_name)
	{
		switch (p_mod_name.ToLower())
		{
			case "gate":
				return gate_param;
			case "attack":
				return attack_param;
			case "hold":
				return hold_param;
			case "decay":
				return decay_param;
			case "sustain":
				return sustain_param;
			case "release":
				return release_param;
			default:
				GD.PrintErr($"[AudioModule Error] No mod named '{p_mod_name}' found in '{this.GetType().Name}'");
				return null;
		}
	}
	
	/*
	// Doesn't solve latency
	public void set_gate(float p_value)
	{
		// Set the gate value immediately
		// skipping audio signal latency
		gate_param.base_value = p_value;
	}
	*/
	
	protected override void update_state()
	{
		if (_last_ticked_frame == AudioClock.current_sample_index) return;
		_last_ticked_frame = AudioClock.current_sample_index;

		//float trigger = gate_param.evaluate();
		//float attack_value = Mathf.Max(0.001f, attack_param.evaluate());
		//float hold_value = Mathf.Max(0.0f, hold_param.evaluate());
		//float decay_value = Mathf.Max(0.001f, decay_param.evaluate());
		//float sustain_value = Mathf.Clamp(sustain_param.evaluate(), 0.0f, 1.0f);
		//float release_value = Mathf.Max(0.001f, release_param.evaluate());
		
		float trigger = gate_param.evaluate();
		float a = Mathf.Max(0.001f, attack_param.evaluate()) * sample_rate;
		float h = Mathf.Max(0.0f, hold_param.evaluate()) * sample_rate;
		float d = Mathf.Max(0.001f, decay_param.evaluate()) * sample_rate;
		float s = Mathf.Clamp(sustain_param.evaluate(), 0.0f, 1.0f);
		float r = Mathf.Max(0.001f, release_param.evaluate()) * sample_rate;
		
		//if (trigger > 0.0f && _prev_trigger <= 0.0f) { _is_note_on = true; _hold_timer = 0.0f; }
		//if (trigger <= 0.0f && _prev_trigger > 0.0f) _is_note_on = false;
		//_prev_trigger = trigger;
		
		// Gating
		if (trigger > 0.0f && _prev_trigger <= 0.0f) 
		{ 
			_is_note_on = true; 
			_hold_timer = 0.0f; // Reset timer on new trigger
		}
		
		if (trigger <= 0.0f && _prev_trigger > 0.0f) _is_note_on = false;
		_prev_trigger = trigger;
		
		//if (_is_note_on)
		//{
			//if (_current_level < 1.0f && _hold_timer == 0.0f)
			//{
				//_current_level += 1.0f / (attack_value * sample_rate);
				//if (_current_level >= 1.0f) _current_level = 1.0f;
			//}
			//else if (_hold_timer < hold_value) _hold_timer += 1.0f / sample_rate;
			//else if (_current_level > sustain_value)
			//{
				//_current_level -= (1.0f - sustain_value) / (decay_value * sample_rate);
				//if (_current_level < sustain_value) _current_level = sustain_value;
			//}
			//else if (_current_level < sustain_value) _current_level = sustain_value;
		//}
		//else if (_current_level > 0.0f)
		//{
			//_current_level -= sustain_value / (release_value * sample_rate);
			//if (_current_level < 0.0f) _current_level = 0.0f;
		//}
		
		// AHDSR processing
		if (_is_note_on)
		{
			// Attack
			if (_current_level < 1.0f && _hold_timer <= 0.0f)
			{
				_current_level += 1.0f / a;
				if (_current_level >= 1.0f) _current_level = 1.0f;
			}
			// Hold
			else if (_hold_timer < h)
			{
				_hold_timer += 1.0f;
				_current_level = 1.0f;
			}
			// Decay to Sustain
			else if (_current_level > s)
			{
				_current_level -= (1.0f - s) / d;
				if (_current_level < s) _current_level = s;
			}
		}
		else
		{
			// Release
			_current_level -= _current_level / r;
			if (_current_level < 0.0f) _current_level = 0.0f;
		}
		
		_cached_mono_sample = _current_level;
		_cached_stereo_sample.X = _cached_mono_sample;
		_cached_stereo_sample.Y = _cached_mono_sample;
	}
}
