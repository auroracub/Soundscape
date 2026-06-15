using System;
using Godot;

namespace Soundscape.Modules;

[GlobalClass]
public partial class Envelope : Modular
{
	public enum EnvelopeMode { Ahd, Ahdsr }
	public enum EnvelopeStage { Idle, Attack, Hold, Decay, Sustain, Release }

	[Export] public EnvelopeMode mode { get; set; } = EnvelopeMode.Ahdsr;

	[ExportGroup("Envelope Time (Seconds)")]
	[Export] public float attack { get; set; } = 0.1f;
	[Export] public float hold { get; set; } = 0.05f;
	[Export] public float decay { get; set; } = 0.2f;
	[Export] public float release { get; set; } = 0.3f;
	[Export(PropertyHint.Range, "0,1,0.01")] public float sustain { get; set; } = 0.6f;

	private EnvelopeStage _current_stage = EnvelopeStage.Idle;
	private float _stage_timer = 0.0f;
	private float _start_level_of_stage = 0.0f;
	private float _current_output_value = 0.0f;
	private bool _last_gate_state = false;

	public override void process_frame(float p_delta)
	{
		if (_current_stage == EnvelopeStage.Idle) return;

		_stage_timer += p_delta;
		update_envelope_state();
	}

	public override void set_control_signal(float p_signal)
	{
		bool current_gate = p_signal > 0.5f;

		// Detect gate
		if (current_gate && !_last_gate_state)
		{
			// Note on (trigger attack)
			transition_to(EnvelopeStage.Attack);
		}
		else if (!current_gate && _last_gate_state)
		{
			// Note off (trigger release)
			if (_current_stage != EnvelopeStage.Idle)
			{
				transition_to(EnvelopeStage.Release);
			}
		}

		_last_gate_state = current_gate;
	}

	private void update_envelope_state()
	{
		switch (_current_stage)
		{
			case EnvelopeStage.Attack:
				if (attack <= 0.0f)
				{
					_current_output_value = 1.0f;
					transition_to(EnvelopeStage.Hold);
				}
				else
				{
					float pct = Mathf.Clamp(_stage_timer / attack, 0.0f, 1.0f);
					_current_output_value = Mathf.Lerp(_start_level_of_stage, 1.0f, pct);
					if (pct >= 1.0f) transition_to(EnvelopeStage.Hold);
				}
				break;

			case EnvelopeStage.Hold:
				_current_output_value = 1.0f;
				if (_stage_timer >= hold) transition_to(EnvelopeStage.Decay);
				break;

			case EnvelopeStage.Decay:
				float target_level = (mode == EnvelopeMode.Ahdsr) ? sustain : 0.0f;
				if (decay <= 0.0f)
				{
					_current_output_value = target_level;
					transition_to(mode == EnvelopeMode.Ahdsr ? EnvelopeStage.Sustain : EnvelopeStage.Idle);
				}
				else
				{
					float pct = Mathf.Clamp(_stage_timer / decay, 0.0f, 1.0f);
					_current_output_value = Mathf.Lerp(1.0f, target_level, pct);
					if (pct >= 1.0f)
					{
						transition_to(mode == EnvelopeMode.Ahdsr ? EnvelopeStage.Sustain : EnvelopeStage.Idle);
					}
				}
				break;

			case EnvelopeStage.Sustain:
				_current_output_value = sustain;
				break;

			case EnvelopeStage.Release:
				if (release <= 0.0f)
				{
					_current_output_value = 0.0f;
					transition_to(EnvelopeStage.Idle);
				}
				else
				{
					float pct = Mathf.Clamp(_stage_timer / release, 0.0f, 1.0f);
					_current_output_value = Mathf.Lerp(_start_level_of_stage, 0.0f, pct);
					if (pct >= 1.0f) transition_to(EnvelopeStage.Idle);
				}
				break;
		}
	}

	private void transition_to(EnvelopeStage p_next_stage)
	{
		_current_stage = p_next_stage;
		_stage_timer = 0.0f;
		_start_level_of_stage = _current_output_value;
	}

	protected override float compute_sample()
	{
		return _current_output_value;
	}
}
