using System;
using Godot;

namespace Soundscape.Modules;

[GlobalClass]
public partial class OscSine : OscBase
{
	protected override float compute_sample()
	{
		_last_value = (float)Math.Sin(_phase * 2.0 * Math.PI);
		return _last_value;
	}
}
