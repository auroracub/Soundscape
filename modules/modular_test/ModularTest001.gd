class_name ModularTest001
extends Node

var modular_voice: ModularVoice
var envelope: ModularEnvelope
var lfo: ModularLFO

func _ready() -> void:
	modular_voice = ModularVoice.new()
	
	# 1. Instantiate the modules as resources
	envelope = ModularEnvelope.new()
	lfo = ModularLfo.new()
	
	# 2. Virtual Patching: Connect Envelope -> Amplitude
	var amp_patch = ModConnection.new()
	amp_patch.source_shape = envelope
	amp_patch.destination_parameter = "Amp"
	amp_patch.modulation_amount = 1.0
	modular_voice.patches.append(amp_patch)
	
	# 3. Virtual Patching: Connect LFO -> Pitch (Vibrato)
	var pitch_patch = ModConnection.new()
	pitch_patch.source_shape = lfo
	pitch_patch.destination_parameter = "Pitch"
	pitch_patch.modulation_amount = 0.02 # 2% pitch shift bend
	modular_voice.patches.append(pitch_patch)

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventKey and event.pressed and not event.is_echo():
		if event.keycode == KEY_SPACE:
			# Send a gate high signal (1.0) to trigger the note
			modular_voice.send_trigger(1.0)
			
	if event is InputEventKey and not event.pressed:
		if event.keycode == KEY_SPACE:
			# Send a gate low signal (0.0) to release the note
			modular_voice.send_trigger(0.0)
