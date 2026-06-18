using Godot;
using System;

public abstract partial class AudioModule : RefCounted
{
	protected float sample_rate = 44100.0f;
	
	protected MonoSignal _cached_mono_chain;
	protected StereoSignal _cached_stereo_chain;
	
	protected volatile float _cached_mono_sample = 0.0f;
	// protected volatile float _cached_stereo_sample.X = 0.0f;
	// protected volatile float _cached_stereo_sample.Y = 0.0f;
	protected Vector2 _cached_stereo_sample = Vector2.Zero;
	
	private Callable _frame_callback;
	private bool _has_callback = false;

	public virtual StereoSignal get_audio_signal() => _cached_stereo_chain;
	public virtual MonoSignal get_mod_signal() => _cached_mono_chain;
	public abstract Mod get_mod_from_name(string p_mod_name);
	
	public virtual void set_base_value(string p_mod_name, float p_value)
	{
		Mod mod = get_mod_from_name(p_mod_name.ToLower());
		
		if (mod != null) {
			mod.base_value = p_value;
		}
	}
	
	public virtual void patch_in(string p_mod_name, Func<float> p_mod_function)
	{
		Mod mod = get_mod_from_name(p_mod_name.ToLower());
		
		if (mod != null) {
			mod.add_mod(p_mod_function);
			rebuild_signal_chain();
		}
	}
	
	public virtual AudioModule patch_out(AudioModule p_target_module, string p_target_param, float p_source_min = 0.0f, float p_source_max = 1.0f, float p_target_min = -1.0f, float p_target_max = 1.0f)
	{
		var source_mod = this.get_mod_signal();
		
		if (p_target_module == null)
		{
			GD.PrintErr($"[AudioModule Error] 'p_target_module' is null, has it been initialized before using 'map_out'?");
			return null;
		}
		
		Func<float> remapped = () =>
		{
			float source_val = source_mod.evaluate();
			return MathLib.remap(p_source_min, p_source_max, p_target_min, p_target_max, source_val);
		};

		p_target_module.patch_in(p_target_param, remapped);
		return p_target_module;
	}
	
	protected abstract void update_state();
	
	protected virtual void rebuild_signal_chain()
	{
		_cached_stereo_chain = new StereoSignal(() => { 
			update_state();
			return _cached_stereo_sample; 
		});
		
		_cached_mono_chain = new MonoSignal(() => { 
			update_state(); 
			return _cached_mono_sample; 
		});
	}
	
	public void set_frame_callback(Callable p_callback)
	{
		_frame_callback = p_callback;
		_has_callback = !_frame_callback.Equals(default(Callable));
	}
	
	public void process_frame()
	{
		if (!_has_callback) return;

		try
		{
			_frame_callback.Call(_cached_mono_sample);
		}
		catch (Exception e)
		{
			GD.PushError($"[AudioModule Error] Unknown exception {e.ToString()} while processing signal callback");
			return;
		}
	}
}
