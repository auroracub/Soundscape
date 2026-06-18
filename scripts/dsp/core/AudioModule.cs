using Godot;
using System;

public abstract partial class AudioModule : RefCounted
{
	// Currently not sure how to remove the necessity for a sample rate
	// variable in AudioModule, but this seems like a good compromise.
	// The value is given a default but updated to whatever value gets
	// passed in during process. I'm sure this could lead to some tricky
	// edge cases like using two speakers with different sample rates
	// (it either causes a glitch or halves the performance), but it
	// works for now.
	public float last_sample_rate = 44100.0f;
	protected long last_sample_index = -1;
	
	protected float cached_mono_sample = 0.0f;
	protected Vector2 cached_stereo_sample = Vector2.Zero;
	
	private Callable _output_callback;
	private bool _has_output_callback = false;
	
	// Audio modules are bipolar (-1 to 1) by default
	public virtual float default_source_min => -1.0f;
	public virtual float default_source_max => 1.0f;
	
	public abstract Mod get_mod_from_name(string p_mod_name);
	protected abstract void update_state(float dt);

	public float process_mono_sample(float p_sample_rate)
	{
		if (last_sample_rate == p_sample_rate && last_sample_index == AudioClock.current_sample_index) return cached_mono_sample;
		last_sample_rate = p_sample_rate;
		last_sample_index = AudioClock.current_sample_index;
		update_state(p_sample_rate);
		return cached_mono_sample;
	}

	public Vector2 process_stereo_sample(float p_sample_rate)
	{
		if (last_sample_rate == p_sample_rate && last_sample_index == AudioClock.current_sample_index) return cached_stereo_sample;
		last_sample_rate = p_sample_rate;
		last_sample_index = AudioClock.current_sample_index;
		update_state(p_sample_rate);
		return cached_stereo_sample;
	}

	public virtual void set_value(string p_mod_name, float p_value)
	{
		Mod mod = get_mod_from_name(p_mod_name.ToLower());
		if (mod != null) mod.base_value = p_value;

		return;
	}
	
	public virtual void set_patch(AudioModule p_target_module, string p_target_mod_name)
	{
		if (p_target_module == null)
		{
			GD.PrintErr($"[AudioModule Error] '{p_target_module}' is null.");
			return;
		}

		Mod target_mod = p_target_module.get_mod_from_name(p_target_mod_name.ToLower());
		if (target_mod != null)
		{
			target_mod.set_patch(new PatchConnection(this));
		}
		else
		{
			GD.PrintErr($"[AudioModule Error] Parameter '{p_target_mod_name}' not found.");
		}
	}
	
	public virtual void remove_patch(AudioModule p_target_module, string p_target_mod_name)
	{
		if (p_target_module == null)
		{
			GD.PrintErr($"[AudioModule Error] '{p_target_module}' is null.");
			return;
		}

		Mod target_mod = p_target_module.get_mod_from_name(p_target_mod_name.ToLower());
		if (target_mod != null)
		{
			target_mod.remove_patch();
		}
		else
		{
			GD.PrintErr($"[AudioModule Error] Parameter '{p_target_mod_name}' not found.");
		}
	}
	
	// patch_in is not needed
	// public virtual void patch_in(AudioModule p_target_module, string p_mod_name)
	// {
	// 	Mod mod = get_mod_from_name(p_mod_name.ToLower());
		
	// 	if (mod != null) {
	// 		mod.add_connection(p_connection);
	// 		rebuild_signal_chain();
	// 	}
	// }

	public virtual void map_add(AudioModule p_target_module, string p_target_mod_name, 
		float p_target_min = -1.0f, float p_target_max = 1.0f)
	{
		if (p_target_module == null)
		{
			GD.PrintErr($"[AudioModule Error] '{p_target_module}' is null.");
			return;
		}

		Mod target_mod = p_target_module.get_mod_from_name(p_target_mod_name.ToLower());
		if (target_mod != null)
		{
			target_mod.map_add(new MapConnection(this, p_target_min, p_target_max, default_source_min, default_source_max));
		}
		else
		{
			GD.PrintErr($"[AudioModule Error] Parameter '{p_target_mod_name}' not found.");
		}
	}

	public virtual void map_add_default(AudioModule p_target_module, string p_target_mod_name)
	{
		map_add(p_target_module, p_target_mod_name);
	}
	
	public virtual void map_multiply(AudioModule p_target_module, string p_target_mod_name, 
		float p_target_min = -1.0f, float p_target_max = 1.0f)
	{
		if (p_target_module == null)
		{
			GD.PrintErr($"[AudioModule Error] '{p_target_module}' is null.");
			return;
		}

		Mod target_mod = p_target_module.get_mod_from_name(p_target_mod_name.ToLower());
		if (target_mod != null)
		{
			target_mod.map_multiply(new MapConnection(this, p_target_min, p_target_max, default_source_min, default_source_max));
		}
		else
		{
			GD.PrintErr($"[AudioModule Error] Parameter '{p_target_mod_name}' not found.");
		}
	}

	public virtual void map_multiply_default(AudioModule p_target_module, string p_target_mod_name)
	{
		map_multiply(p_target_module, p_target_mod_name);
	}
	
	public void set_output_callback(Callable p_callback)
	{
		_output_callback = p_callback;
		// ISSUE:
		// Not sure how to determine whether callback is valid
		// No IsValid or IsDefault for Godot callbacks, and this
		// checking for null value doesn't seem to work
		_has_output_callback = _output_callback.Target != null;
		//GD.Print(_frame_callback.Target);
		//
		//if (!_has_callback)
		//{
			//GD.PrintErr($"[AudioModule Error] Callback `{_frame_callback.Target}` is null.");
		//}
	}
	
	public void remove_output_callback(Callable p_callback)
	{
		// _frame_callback = null;
		_has_output_callback = false;
	}
	
	// Frame delta can be included, but it's not currently necessary
	public void process_frame()
	{
		if (!_has_output_callback) return;
		_output_callback.Call(cached_mono_sample);
	}
}
