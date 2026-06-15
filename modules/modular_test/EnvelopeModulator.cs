using System;
using Godot;

namespace Soundscape.Modules;

[GlobalClass]
public partial class EnvelopeModulator : BaseModulator
{
	public enum EnvelopeMode { Ahd, Ahdsr }
	public enum EnvelopeStage { Idle, Attack, Hold, Decay, Sustain, Release }

	[Export] public EnvelopeMode Mode { get; set; } = EnvelopeMode.Ahdsr;

	[ExportGroup("Times (Seconds)")]
	[Export] public float AttackTime { get; set; } = 0.1f;
	[Export] public float HoldTime { get; set; } = 0.05f;
	[Export] public float DecayTime { get; set; } = 0.2f;
	[Export] public float ReleaseTime { get; set; } = 0.3f;

	[ExportGroup("Sustain Configuration")]
	[Export(PropertyHint.Range, "0,1,0.01")] public float SustainLevel { get; set; } = 0.6f;

	private EnvelopeStage _currentStage = EnvelopeStage.Idle;
	private float _stageTimer = 0.0f;
	private float _startLevelOfStage = 0.0f;
	private float _currentOutputValue = 0.0f;
	private bool _lastGateState = false;

	public override void process_frame(float p_delta)
	{
		if (_currentStage == EnvelopeStage.Idle) return;

		_stageTimer += p_delta;
		UpdateEnvelopeState();
	}

	public override void set_control_signal(float p_signal)
	{
		bool currentGate = p_signal > 0.5f;

		// Detect Edge Triggers (Gate High vs Gate Low)
		if (currentGate && !_lastGateState)
		{
			// Note On (Trigger Attack)
			TransitionTo(EnvelopeStage.Attack);
		}
		else if (!currentGate && _lastGateState)
		{
			// Note Off (Trigger Release)
			if (_currentStage != EnvelopeStage.Idle)
			{
				TransitionTo(EnvelopeStage.Release);
			}
		}

		_lastGateState = currentGate;
	}

	private void UpdateEnvelopeState()
	{
		switch (_currentStage)
		{
			case EnvelopeStage.Attack:
				if (AttackTime <= 0.0f)
				{
					_currentOutputValue = 1.0f;
					TransitionTo(EnvelopeStage.Hold);
				}
				else
				{
					float pct = Mathf.Clamp(_stageTimer / AttackTime, 0.0f, 1.0f);
					_currentOutputValue = Mathf.Lerp(_startLevelOfStage, 1.0f, pct);
					if (pct >= 1.0f) TransitionTo(EnvelopeStage.Hold);
				}
				break;

			case EnvelopeStage.Hold:
				_currentOutputValue = 1.0f;
				if (_stageTimer >= HoldTime) TransitionTo(EnvelopeStage.Decay);
				break;

			case EnvelopeStage.Decay:
				float targetLevel = (Mode == EnvelopeMode.Ahdsr) ? SustainLevel : 0.0f;
				if (DecayTime <= 0.0f)
				{
					_currentOutputValue = targetLevel;
					TransitionTo(Mode == EnvelopeMode.Ahdsr ? EnvelopeStage.Sustain : EnvelopeStage.Idle);
				}
				else
				{
					float pct = Mathf.Clamp(_stageTimer / DecayTime, 0.0f, 1.0f);
					_currentOutputValue = Mathf.Lerp(1.0f, targetLevel, pct);
					if (pct >= 1.0f)
					{
						TransitionTo(Mode == EnvelopeMode.Ahdsr ? EnvelopeStage.Sustain : EnvelopeStage.Idle);
					}
				}
				break;

			case EnvelopeStage.Sustain:
				_currentOutputValue = SustainLevel;
				break;

			case EnvelopeStage.Release:
				if (ReleaseTime <= 0.0f)
				{
					_currentOutputValue = 0.0f;
					TransitionTo(EnvelopeStage.Idle);
				}
				else
				{
					float pct = Mathf.Clamp(_stageTimer / ReleaseTime, 0.0f, 1.0f);
					_currentOutputValue = Mathf.Lerp(_startLevelOfStage, 0.0f, pct);
					if (pct >= 1.0f) TransitionTo(EnvelopeStage.Idle);
				}
				break;
		}
	}

	private void TransitionTo(EnvelopeStage nextStage)
	{
		_currentStage = nextStage;
		_stageTimer = 0.0f;
		_startLevelOfStage = _currentOutputValue;
	}

	protected override float compute_sample()
	{
		return _currentOutputValue;
	}
}
