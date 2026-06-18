using Godot;
using System;

public abstract partial class AudioModule : RefCounted
{
	protected float sample_rate = 44100.0f;
	protected long last_ticked_frame = -1;
	
	protected float cached_mono_sample = 0.0f;
	protected Vector2 cached_stereo_sample = Vector2.Zero;
	
	private Callable _frame_callback;
	private bool _has_callback = false;
	
	// Audio modules are bipolar (-1 to 1) by default
	public virtual float default_source_min => -1.0f;
	public virtual float default_source_max => 1.0f;
	
	public abstract Mod get_mod_from_name(string p_mod_name);
	protected abstract void update_state();

	public float tick_mono()
	{
		if (last_ticked_frame == AudioClock.current_sample_index) return cached_mono_sample;
		last_ticked_frame = AudioClock.current_sample_index;
		update_state();
		return cached_mono_sample;
	}

	public Vector2 tick_stereo()
	{
		if (last_ticked_frame == AudioClock.current_sample_index) return cached_stereo_sample;
		last_ticked_frame = AudioClock.current_sample_index;
		update_state();
		return cached_stereo_sample;
	}

	public virtual void set_base_value(string p_mod_name, float p_value)
	{
		Mod mod = get_mod_from_name(p_mod_name.ToLower());
		if (mod != null) mod.base_value = p_value;
	}
	
	public virtual AudioModule patch_out(AudioModule p_target_module, string p_target_param, 
		float p_target_min = -1.0f, float p_target_max = 1.0f)
	{
		if (p_target_module == null)
		{
			GD.PrintErr("[AudioModule Error] 'p_target_module' is null.");
			return null;
		}

		Mod target_mod = p_target_module.get_mod_from_name(p_target_param.ToLower());
		if (target_mod != null)
		{
			target_mod.add_connection(new PatchConnection(this, p_target_min, p_target_max, default_source_min, default_source_max));
		}
		else
		{
			GD.PrintErr($"[AudioModule Error] Parameter '{p_target_param}' not found.");
		}
		
		return p_target_module;
	}

	public void set_frame_callback(Callable p_callback)
	{
		_frame_callback = p_callback;
		_has_callback = _frame_callback.Target != null;
		//GD.Print(_frame_callback.Target);
		//
		//if (!_has_callback)
		//{
			//GD.PrintErr($"[AudioModule Error] Callback `{_frame_callback.Target}` is null.");
		//}
	}
	
	public void process_frame()
	{
		if (!_has_callback) return;
		_frame_callback.Call(cached_mono_sample);
	}
}
