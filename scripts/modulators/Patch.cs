using System;
using Godot;

namespace Soundscape.Modules;

[GlobalClass]
public partial class Patch : Resource
{
	[Export] public Modular modulator { get; set; }

	[Export] public string destination { get; set; } = "pitch";
	[Export] public float amount { get; set; } = 1.0f;
	
	// public Modular cached_source => modulator;
}
