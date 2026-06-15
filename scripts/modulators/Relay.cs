using Godot;
using System;

namespace Soundscape.Modules;

[GlobalClass]
public partial class Relay : Modular
{
	[Export] public string matrix_source { get; set; } = "";
	[Export] public Modular target { get; set; }
	[Export] public float in_lo = 0.0f;
	[Export] public float in_hi = 1.0f;
	[Export] public float out_lo = 0.0f;
	[Export] public float out_hi = 1.0f;
	
	public override void process_frame_concrete(float p_delta)
	{
		if (target == null || string.IsNullOrEmpty(matrix_source)) return;
		
		_last_value = Libraries.MathLib.remap(in_lo, in_hi, out_lo, out_hi, Matrix.read(matrix_source));
		
		target.set_control_signal(_last_value);
	}
	
	protected override float compute_sample()
	{
		// return _last_value;
		return 0.0f;
	}
}
