using Godot;
using System;

[GlobalClass]
public partial class RaytracedAudioPlayer3D : AudioStreamPlayer3D
{
	[Export] public bool EnableMuffle { get; set; } = true;
	[Export] public uint CollisionMask { get; set; } = 1;
	[Export] public float MuffleDb { get; set; } = -12.0f;

	private float _baseVolumeDb;

	public override void _Ready()
	{
		_baseVolumeDb = VolumeDb;
		
		// Force assignment to the plugin's native "Reverb" bus if not explicitly configured
		if (Bus == "Master" || string.IsNullOrEmpty(Bus))
		{
			Bus = "Reverb";
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!EnableMuffle || !Playing || Stream == null) return;

		var camera = GetViewport()?.GetCamera3D();
		if (camera == null) return;

		Vector3 listenerPos = camera.GlobalPosition;
		Vector3 sourcePos = GlobalPosition;

		// Skip calculations if out of physical hearing bounds
		if (sourcePos.DistanceSquaredTo(listenerPos) > MaxDistance * MaxDistance) return;

		var spaceState = GetWorld3D().DirectSpaceState;
		if (spaceState == null) return;

		// Step 1: Cast forward ray from source to listener
		var forwardQuery = PhysicsRayQueryParameters3D.Create(sourcePos, listenerPos, CollisionMask);
		var forwardHit = spaceState.IntersectRay(forwardQuery);

		if (forwardHit.Count > 0)
		{
			Vector3 entryPoint = (Vector3)forwardHit["position"];

			// Step 2: Cast backward ray from listener to entry point to deduce wall thickness
			var backwardQuery = PhysicsRayQueryParameters3D.Create(listenerPos, entryPoint, CollisionMask);
			var backwardHit = spaceState.IntersectRay(backwardQuery);

			float thickness = 0.0f;
			if (backwardHit.Count > 0)
			{
				Vector3 exitPoint = (Vector3)backwardHit["position"];
				thickness = entryPoint.DistanceTo(exitPoint);
			}

			// Smoothly interpolate down to the muffled decibel setting based on wall thickness
			float targetVolume = _baseVolumeDb + MuffleDb * Mathf.Max(1.0f, thickness);
			VolumeDb = Mathf.Lerp(VolumeDb, targetVolume, 0.15f);
		}
		else
		{
			// Clear path to ear: return to default clear volume levels
			VolumeDb = Mathf.Lerp(VolumeDb, _baseVolumeDb, 0.15f);
		}
	}
}
