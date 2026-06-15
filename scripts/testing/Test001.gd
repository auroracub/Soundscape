extends Node

var synth_voice: SynthVoice
@onready var synth_speaker: SynthSpeaker3D = $Speaker

@export var frequency: float = 440.0;

var lfo = OscSine.new()
var envelope = Envelope.new()
var random = OscSampleAndHold.new()
var relay = Relay.new()

func _ready() -> void:
	synth_voice = SynthVoice.new()
	synth_voice.glide_time = 2.5
	synth_voice.set_current_frequency(frequency)
	
	lfo.frequency_hz = 0.5
	
	envelope.attack = 0.01
	envelope.hold = 0.0
	envelope.decay = 0.15
	envelope.sustain = 0.2
	envelope.release = 0.15
	# envelope.debug_enabled = true
	envelope.subscribers.append(relay)
	
	random.frequency_hz = 4.0
	random.matrix_target = "random"
	random.debug_enabled = true
	
	relay.matrix_source = "random"
	relay.in_lo = -1.0
	relay.in_hi = 1.0
	relay.out_lo = 0.0
	relay.out_hi = 1.0
	relay.target = envelope
	
	var vibrato_patch = Patch.new()
	vibrato_patch.modulator = lfo
	vibrato_patch.destination = "pitch"
	vibrato_patch.map_lo = -0.01
	vibrato_patch.map_hi = 0.01
	synth_voice.patches.append(vibrato_patch)
	
	var random_patch = Patch.new()
	random_patch.modulator = random
	random_patch.destination = "pitch"
	random_patch.map_lo = -110.0
	random_patch.map_hi = 110.0
	synth_voice.patches.append(random_patch)
	
	var amp_patch = Patch.new()
	amp_patch.modulator = envelope
	amp_patch.destination = "amp"
	amp_patch.map_lo = 0.0
	amp_patch.map_hi = 1.0
	synth_voice.patches.append(amp_patch)
	
	var tone_patch = Patch.new()
	tone_patch.modulator = envelope
	tone_patch.destination = "tone"
	tone_patch.map_lo = 0.0
	tone_patch.map_hi = 0.2
	synth_voice.patches.append(tone_patch)
	
	synth_speaker.audio_input_source = synth_voice
	
	
	# 2. Instantiate and attach the spatial 3D emitter node
	# speaker = ModularSpeaker3D.new()
	# add_child(speaker)

	# 3. Patch the voice into the speaker's abstract audio input channel
	#speaker.audio_input_source = synth_voice

func _unhandled_input(event: InputEvent) -> void:
	# Test triggering note events
	if event is InputEventKey and event.pressed and not event.is_echo():
		if event.keycode == KEY_F:
			envelope.set_control_signal(1.0)
			
	if event is InputEventKey and not event.pressed:
		if event.keycode == KEY_F:
			envelope.set_control_signal(0.0)
