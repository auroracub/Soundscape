using System;
using Godot;

namespace Soundscape.Modules;

[GlobalClass]
public partial class Patch : Resource
{
	[Export] public Modular modulator { get; set; }

	[Export] public string destination { get; set; } = "";
	[Export] public float map_lo { get; set; } = 0.0f;
	[Export] public float map_hi { get; set; } = 1.0f;
	//[Export] public float scale { get; set; } = 1.0f;
	//[Export] public float offset { get; set; } = 0.0f;
}
