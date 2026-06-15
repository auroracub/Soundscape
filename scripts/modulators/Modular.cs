using System;
using Godot;

namespace Soundscape.Modules;

//public interface IModulator
//{
	//void process_frame(float p_delta);
	//float get_current_value();
	//void set_control_signal(float p_signal);
//}

[GlobalClass]
public abstract partial class Modular : Resource//, IModulator
{
	public Modular() { }

	public virtual void process_frame(float p_delta)
	{
		// General modulators execute their math variations per audio frame tick
	}

	public virtual float get_current_value()
	{
		return compute_sample();
	}

	public virtual void set_control_signal(float p_signal)
	{
		// Custom modulators override this to handle gates, clocks, triggers, etc
	}

	protected abstract float compute_sample();
}
