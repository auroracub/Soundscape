using Godot;
using System;

[GlobalClass]
public partial class SignalBridge : AudioModule
{
	private AudioModule _source_module;
	private string _source_channel;
	private Callable _gd_callback;
	
	private float _source_min = 0.0f;
	private float _source_max = 1.0f;
	private float _target_min = 0.0f;
	private float _target_max = 1.0f;
	
	public override Mod get_mod_from_name(string p_mod_name)
	{
		switch (p_mod_name)
		{
			default:
				GD.PrintErr($"[AudioModule Error] No mod named '{p_mod_name}' found in '{this.GetType().Name}'");
				return null;
		}
	}
	
	public override void patch_in(string p_parameter_name, Func<float> p_mod_provider)
		=> base.patch_in(p_parameter_name, p_mod_provider);
	
	public override AudioModule patch_out(AudioModule p_target_module, string p_target_param, float p_source_min = 0.0f, float p_source_max = 1.0f, float p_target_min = -1.0f, float p_target_max = 1.0f)
		=> base.patch_out(p_target_module, p_target_param, p_source_min, p_source_max, p_target_min, p_target_max);//, p_source_channel);
	
	// Overriding patch_in isn't enough if to avoid evaluating the lambda.
	// Instead, intercept the module tracking directly
	public void connect_source(AudioModule p_source, float p_source_min = 0.0f, float p_source_max = 1.0f, float p_target_min = 0.0f, float p_target_max = 1.0f)// string p_channel = "default", float p_min = 0.0f, float p_max = 1.0f)
	{
		_source_module = p_source;
		//_source_channel = p_channel;
		_source_min = p_source_min;
		_source_max = p_source_max;
		_target_min = p_target_min;
		_target_max = p_target_max;
	}

	// public override void patch_in(string p_mod_name, Func<float> p_mod_function) { }
	// public override void set_base_value(string p_mod_name, float p_value) { }

	public void set_callback(Callable p_callback)
	{
		_gd_callback = p_callback;
	}

	public void process()
	{
		if (_gd_callback.Equals(default(Callable)) || _source_module == null) return;

		try
		{
			_gd_callback.Call(MathLib.remap(_source_min, _source_max, _target_min, _target_max, _cached_mod_sample));
		}
		catch (Exception)
		{
			return;
		}
	}
	
	protected override void update_state() {}
	
	//public override StereoSignal get_output_signal() => null;
	//public override MonoSignal get_modulation_signal()//(string p_channel_name = "default")
		//=> null;
}
