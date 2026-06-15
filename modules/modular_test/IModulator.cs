using System;
using Godot;

namespace Soundscape.Modules;

public interface IModulator
{
	void process_frame(float p_delta);
	float get_current_value();
	
	// The Universal Patch Point: Receives control signals (gates, trigger spikes)
	// from other modules or scripts without breaking polymorphism.
	void set_control_signal(float p_signal);
}
