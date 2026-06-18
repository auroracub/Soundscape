using Godot;

[GlobalClass]
public partial class Envelope : AudioModule
{
	public Mod gate_param = new Mod(0.0f); // Input threshold check uses > 0.0
	public Mod attack_param = new Mod(0.1f);
	public Mod hold_param = new Mod(0.0f);
	public Mod decay_param = new Mod(0.2f);
	public Mod sustain_param = new Mod(0.7f);
	public Mod release_param = new Mod(0.5f);

	private float _current_level = 0.0f; 
	private bool _is_note_on = false;
	private float _prev_trigger = -1.0f;
	private float _hold_timer = 0.0f;
	
	// Overrides to unipolar range (0 to 1)
	public override float default_source_min => 0.0f;
	public override float default_source_max => 1.0f;

	public override Mod get_mod_from_name(string p_mod_name)
	{
		switch (p_mod_name.ToLower())
		{
			case "gate":
			case "trigger":
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
	
	protected override void update_state(float p_sample_rate)
	{
		float trigger = gate_param.evaluate();
		
		float attack_rate = Mathf.Max(0.001f, attack_param.evaluate()) * p_sample_rate;
		float hold_frames = Mathf.Max(0.0f, hold_param.evaluate()) * p_sample_rate;
		float decay_rate = Mathf.Max(0.001f, decay_param.evaluate()) * p_sample_rate;
		float sustain_level = Mathf.Clamp(sustain_param.evaluate(), 0.0f, 1.0f);
		float release_rate = Mathf.Max(0.001f, release_param.evaluate()) * p_sample_rate;
		
		if (trigger > 0.0f && _prev_trigger <= 0.0f) 
		{ 
			_is_note_on = true; 
			_hold_timer = 0.0f; 
		}
		else if (trigger <= 0.0f && _prev_trigger > 0.0f) 
		{
			_is_note_on = false;
		}
		
		_prev_trigger = trigger;
		
		if (_is_note_on)
		{
			if (_current_level < 1.0f && _hold_timer <= 0.0f)
			{
				_current_level += 1.0f / attack_rate;
				if (_current_level >= 1.0f) _current_level = 1.0f;
			}
			else if (_hold_timer < hold_frames)
			{
				_hold_timer += 1.0f;
				_current_level = 1.0f;
			}
			else if (_current_level > sustain_level)
			{
				_current_level -= (1.0f - sustain_level) / decay_rate;
				if (_current_level < sustain_level) _current_level = sustain_level;
			}
		}
		else
		{
			_current_level -= _current_level / release_rate;
			if (_current_level < 0.0f) _current_level = 0.0f;
		}
		
		cached_mono_sample = _current_level;
		cached_stereo_sample.X = cached_mono_sample;
		cached_stereo_sample.Y = cached_mono_sample;
	}
}
