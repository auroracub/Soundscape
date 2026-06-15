class_name TestBlob
extends Node3D

@onready var synth_ref: MonoSynth3D = $Synth
@onready var blob_ref: Node3D = $Blob

@export var _dbg_enable_node: bool = true
@export var frequency: float = 440.0
@export var tempo: float = 120.0
@export var probability: float = 0.75

# Time tracking
var quarter_note_duration: float = 0.0
var current_time: float = 0.0

var base_height: float
var base_scale: Vector3
var octave_shift = false
var height_oscillator: SineOscillator

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	if not _dbg_enable_node:
		queue_free()
	
	base_height = position.y
	base_scale = scale
	
	synth_ref.SynthVoice.SetCurrentFrequency(frequency)
	
	# Calculate quarter note length (60 seconds / 120 BPM = 0.5 seconds)
	quarter_note_duration = 60.0 / tempo
	height_oscillator = SineOscillator.new()
	height_oscillator.RateHz = AudioLib.SecondsToHz(2.0)
	height_oscillator.PhasePosition = AudioLib.SecondsToHz(randf())

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	current_time += 1.0 * delta
	height_oscillator.Process(delta, 0.0)
	
	if current_time >= quarter_note_duration:
			current_time = fmod(current_time, quarter_note_duration)
			
			#voice_ref.note_off()
			
			if randf() < probability:
				if octave_shift:
					synth_ref.SynthVoice.SetTargetFrequency(synth_ref.SynthVoice.GetTargetFrequency() * 0.5)
					octave_shift = false
				else:
					if randf() > 0.75:
						synth_ref.SynthVoice.SetTargetFrequency(synth_ref.SynthVoice.GetTargetFrequency() * 2.0)
						octave_shift = true
				synth_ref.NoteOn(synth_ref.SynthVoice.GetTargetFrequency(), randf_range(0.5, 1.0))
			else:
				if randf() > 0.5:
					synth_ref.NoteOff()
	
	# Update size
	var lfo_scale = lerpf(0.95, 1.0, synth_ref.SynthVoice.LFO.GetCurrentValue())
	var pitch_scale = (synth_ref.SynthVoice.GetTargetFrequency() / max(synth_ref.SynthVoice.GetTargetFrequency(), 0.0001))
	scale = base_scale * lfo_scale# + Vector3(1.25, 0.75, 1.25).lerp(Vector3(0.75, 1.25, 0.75), pitch_scale)
	# old hue: MathLib.Remapf(0.0, 1.0, 226.0, 332.0, synth_ref.SynthVoice.Amp.GetCurrentValue())
	blob_ref.set_instance_shader_parameter("u_amp", synth_ref.SynthVoice.Amp.GetCurrentValue())
	# blob_ref.set_instance_shader_parameter("u_color", ColorLib.HclToRgb(0.0, MathLib.Remapf(0.0, 1.0, 0.5, 1.2, synth_ref.SynthVoice.Amp.GetCurrentValue()), 0.35)) # 0.2 + pitch_scale * 0.6))
	# print(voice_ref.voice.amplitude_envelope.get_current_value()) why so smol?
	
	# height_oscillator.get_current_value()
	position.y = base_height + MathLib.Remapf(-1.0, 1.0, 0.0, 0.2, height_oscillator.GetCurrentValue())

# saturation = amplitude envelope
# hue = wave position
# value 1.0 -> 1.5 pitch
