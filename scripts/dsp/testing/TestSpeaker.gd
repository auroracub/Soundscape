extends Node

@export var frequency: float = 440.0;

@onready var speaker: SynthSpeaker3D = $SynthSpeaker3D

var voice = Voice.new()
var tone = OscWavetable.new()
var lfo = OscSine.new()
var rnd = OscSampleAndHold.new()
var amp = Amp.new()
var env = Envelope.new()

func _ready() -> void:
	voice.set_value("frequency", frequency)
	voice.set_value("glide", 0.2)
	voice.set_patch(tone, "frequency")
	voice.set_callback(Callable(self, "debug"))
	
	tone.set_value("frequency", frequency)
	tone.set_patch(amp, "input")
	
	lfo.set_value("frequency", 0.25)
	lfo.map_add(voice, "frequency", -0.01, 0.01)
	
	rnd.set_value("frequency", 5.0)
	rnd.map_add(env, "gate", -1.0, 1.0)
	rnd.map_multiply(voice, "frequency", 0.5, 1.0)
	rnd.set_callback(Callable(self, "debug"))
	
	env.set_value("gate", 1.0)
	env.set_value("attack", 0.01)
	env.set_value("hold", 0.0)
	env.set_value("decay", 0.15)
	env.set_value("sustain", 0.2)
	env.set_value("release", 0.15)
	env.set_patch(amp, "level")
	# env.patch_range(tone, "position", 0.0, 0.2)
	
	#var vibrato_patch = Patch.new()
	#vibrato_patch.modulator = lfo
	#vibrato_patch.destination = "pitch"
	#vibrato_patch.map_lo = -0.01
	#vibrato_patch.map_hi = 0.01
	#voice.patches.append(vibrato_patch)
	
	#var random_patch = Patch.new()
	#random_patch.modulator = random
	#random_patch.destination = "pitch"
	#random_patch.map_lo = -110.0
	#random_patch.map_hi = 110.0
	#synth_voice.patches.append(random_patch)
	
	#var amp_patch = Patch.new()
	#amp_patch.modulator = envelope
	#amp_patch.destination = "amp"
	#amp_patch.map_lo = 0.0
	#amp_patch.map_hi = 1.0
	#synth_voice.patches.append(amp_patch)
	
	#var tone_patch = Patch.new()
	#tone_patch.modulator = envelope
	#tone_patch.destination = "tone"
	#tone_patch.map_lo = 0.0
	#tone_patch.map_hi = 0.2
	#synth_voice.patches.append(tone_patch)
	
	speaker.connect_source(amp)

#func _unhandled_input(event: InputEvent) -> void:
	#env.process_frame()
	#
	#if Game.player_ref:
		#var player_height = Game.player_ref.global_position.y
		#var target_freq = remap(player_height, 0.0, 2.0, 200.0, 800.0)
		#target_freq = clamp(target_freq, 200.0, 800.0)
		#voice.set_value("frequency", target_freq)
	#
	## Test triggering note events
	#if event is InputEventKey and event.pressed and not event.is_echo():
		#if event.keycode == KEY_F:
			#env.set_value("gate", 1.0)
			#
	#if event is InputEventKey and not event.pressed:
		#if event.keycode == KEY_F:
			#env.set_value("gate", 0.0)

func debug(p_value: float) -> void:
	print("DEBUG VALUE: ", p_value)
