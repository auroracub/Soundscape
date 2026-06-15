using Godot;
using System;

[GlobalClass]
public partial class RaytracedAudioListener3D : AudioListener3D
{
	[Export] public bool EnableAcoustics { get; set; } = true;
	[Export] public uint CollisionMask { get; set; } = 1;
	[Export] public float MaxRayDistance { get; set; } = 40.0f;
	[Export] public string TargetBusName { get; set; } = "Reverb";

	private static readonly Vector3[] Directions = new Vector3[]
	{
		Vector3.Up, Vector3.Down, Vector3.Left, Vector3.Right, Vector3.Forward, Vector3.Back
	};

	private int _busIndex = -1;
	private AudioEffectReverb _reverbEffect;
	private AudioEffectLowPassFilter _lowpassEffect;

	public override void _Ready()
	{
		MakeCurrent(); // Lock this node as the active engine sound consumer
		CacheAudioEffects();
	}

	private void CacheAudioEffects()
	{
		_busIndex = AudioServer.GetBusIndex(TargetBusName);
		if (_busIndex == -1) return;

		int effectCount = AudioServer.GetBusEffectCount(_busIndex);
		for (int i = 0; i < effectCount; i++)
		{
			var effect = AudioServer.GetBusEffect(_busIndex, i);
			if (effect is AudioEffectReverb reverb)
			{
				_reverbEffect = reverb;
			}
			else if (effect is AudioEffectLowPassFilter lpf)
			{
				_lowpassEffect = lpf;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!EnableAcoustics || _busIndex == -1) return;

		var spaceState = GetWorld3D().DirectSpaceState;
		if (spaceState == null) return;

		float totalDistance = 0.0f;
		Vector3 origin = GlobalPosition;

		for (int i = 0; i < Directions.Length; i++)
		{
			Vector3 target = origin + (Directions[i] * MaxRayDistance);
			var query = PhysicsRayQueryParameters3D.Create(origin, target, CollisionMask);
			var hit = spaceState.IntersectRay(query);

			if (hit.Count > 0)
			{
				totalDistance += origin.DistanceTo((Vector3)hit["position"]);
			}
			else
			{
				totalDistance += MaxRayDistance;
			}
		}

		float averageRoomSize = totalDistance / Directions.Length;
		UpdateAudioServer(averageRoomSize);
	}

	private void UpdateAudioServer(float roomSize)
	{
		float normalizedSize = Mathf.Clamp(roomSize / MaxRayDistance, 0.0f, 1.0f);

		// 1. Scale reverb room echoes 1:1 with environmental open space
		if (_reverbEffect != null)
		{
			_reverbEffect.RoomSize = normalizedSize;
			_reverbEffect.Wet = Mathf.Lerp(0.2f, 0.7f, normalizedSize);
		}

		// 2. Adjust global dampening: open fields allow high frequencies, closed spaces compress them
		if (_lowpassEffect != null)
		{
			_lowpassEffect.CutoffHz = Mathf.Lerp(800.0f, 20000.0f, normalizedSize);
		}
	}
}
