using Godot;
using System;

[GlobalClass]
public partial class AsciiSequencer : AudioModule
{
	public string sequence = "";
	
	public Mod frequency_param = new Mod(1.0f);
	
	private float _current_value = 0.0f;
	private int _current_index = 0;
	private float _timer = 0.0f;
	
	// Overrides to unipolar range (0 to 1)
	public override float default_source_min => 0.0f;
	public override float default_source_max => 1.0f;
	
	private float _last_value = 0.0f;
	private float _last_random_value = (float)GD.RandRange(-1.0, 1.0);
	private bool _rest = false;
	private bool _mute = false;
	private bool _quit = false;
	
	private Callable _reset_callback;
	private bool _has_reset_callback = false;
	
	public String get_alphabet() => "0123456789-?!.mMuUqQ";
	
	public void set_sequence(string p_sequence)
	{
		sequence = p_sequence;
		
		_current_index = 0;
		char symbol = sequence[_current_index];
		
		process_sequence();
		
		//if (char.IsDigit(symbol))
		//{
			//_current_value = (int)char.GetNumericValue(symbol);
		//}
	}
	
	public override Mod get_mod_from_name(string p_mod_name)
	{
		switch (p_mod_name.ToLower())
		{
			case "freq":
			case "frequency":
				return frequency_param;
			default:
				GD.PrintErr($"[AudioModule Error] No mod named '{p_mod_name}' found in '{this.GetType().Name}'");
				return null;
		}
	}
	
	protected override void update_state(float p_sample_rate)
	{
		if (_quit) return;
		
		float frequency = frequency_param.evaluate();
		
		if (_timer > AudioLib.hz_to_seconds(frequency))
		{
			_timer = 0.0f;
			_current_index = Math.Max(0, _current_index + 1);
			
			if (_current_index >= sequence.Length)
			{
				_current_index = 0;
				if (_has_reset_callback) _reset_callback.Call();
			}
			
			process_sequence();
		}
		else
		{
			_timer += 1.0f / p_sample_rate;
		}
		
		cached_mono_sample = _rest || _mute ? 0.0f : _current_value;
		cached_stereo_sample.X = cached_mono_sample;
		cached_stereo_sample.Y = cached_mono_sample;
	}
	
	protected void process_sequence()
	{
		_last_value = _current_value;
		_rest = false;
		
		char symbol = sequence[_current_index];
		
		switch (symbol)
		{
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				_current_value = (int)char.GetNumericValue(symbol);
				break;
			case '-':
				_current_value = _last_value;
				break;
			case '?':
				_last_random_value = (float)GD.RandRange(-1.0, 1.0);
				_current_value = _last_random_value;
				break;
			case '!':
				_current_value = _last_random_value;
				break;
			case '.':
				_current_value = 0.0f;
				_rest = true;
				break;
			case 'm':
			case 'M':
				_mute = true;
				break;
			case 'u':
			case 'U':
				_mute = false;
				break;
			case 'q':
			case 'Q':
				_quit = true;
				break;
		}
	}
	
	public void set_reset_callback(Callable p_callback)
	{
		_reset_callback = p_callback;
		_has_reset_callback = _reset_callback.Target != null;
	}
	
	public void remove_reset_callback(Callable p_callback)
	{
		_has_reset_callback = false;
	}
}
