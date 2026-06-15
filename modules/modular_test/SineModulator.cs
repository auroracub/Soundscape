using System;
using Godot;

namespace Soundscape.Modules;

[GlobalClass]
public partial class SineModulator : BaseModulator
{
	protected override float compute_sample()
	{
		return (float)Math.Sin(_phase * 2.0 * Math.PI);
	}
}
