extends Node

var synth_voice: SynthVoice
@onready var synth_speaker: SynthSpeaker3D = $Speaker

@export var frequency: float = 440.0;

func _ready() -> void:
	# Initialize modular voice
	synth_voice = SynthVoice.new()
	synth_voice.set_current_frequency(frequency)
	
	# Add a basic LFO to modulate the pitch
	var lfo = OscSine.new()
	lfo.frequency_hz = 0.5
	
	var vibrato_patch = Patch.new()
	vibrato_patch.modulator = lfo
	vibrato_patch.destination = "pitch"
	vibrato_patch.amount = 0.01
	synth_voice.patches.append(vibrato_patch)
	
	synth_speaker.audio_input_source = synth_voice
	
	
	# 2. Instantiate and attach the spatial 3D emitter node
	# speaker = ModularSpeaker3D.new()
	# add_child(speaker)

	# 3. Patch the voice into the speaker's abstract audio input channel
	#speaker.audio_input_source = synth_voice

#func _unhandled_input(event: InputEvent) -> void:
	## Test triggering note events
	#if event is InputEventKey and event.pressed and not event.is_echo():
		#if event.keycode == KEY_F:
			#synth_voice.send_trigger(1.0)
			#
	#if event is InputEventKey and not event.pressed:
		#if event.keycode == KEY_F:
			#synth_voice.send_trigger(0.0)
