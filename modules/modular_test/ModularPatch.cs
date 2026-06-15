using System;
using Godot;

namespace Soundscape.Modules;

[GlobalClass]
public partial class ModularPatch : Resource
{
	// ⚡ Change the type from Resource to BaseModulator
	[Export] public BaseModulator modulator { get; set; }

	[Export] public string destination { get; set; } = "pitch";
	[Export] public float amount { get; set; } = 1.0f;
	
	// ⚡ Directly return the modulator reference since it's already a strongly-typed object
	public BaseModulator cached_source => modulator;
}
