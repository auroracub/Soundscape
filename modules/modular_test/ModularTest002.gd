class_name ModularTest002
extends Node

var modular_voice: ModularVoice

var master_vibrato: BaseModulator
var wave_morpher: BaseModulator
var noise_gen: BaseModulator

func _ready() -> void:
	modular_voice = ModularVoice.new()
	
	# Instantiate your different specialized shapes
	master_vibrato = SineModulator.new()
	master_vibrato.frequency_hz = 6.0 # 6Hz speed
	
	wave_morpher = WavetableModulator.new()
	wave_morpher.frequency_hz = 110.0 # Audio-rate synth speed!
	wave_morpher.morph_position = 0.5 # Blended halfway between Tri and Saw
	
	noise_gen = SampleAndHoldModulator.new()
	noise_gen.frequency_hz = 12.0 # Fast random stepping cascade
	
	# ⚡ UNIVERSAL POLYMORPHISM PROVED:
	# Any of these three objects can be dragged or assigned into the exact same patch cable
	var vibrato_patch = ModularPatch.new()
	vibrato_patch.modulator = master_vibrato # Could seamlessly swap to 'noise_gen' here!
	vibrato_patch.destination_parameter = "pitch"
	vibrato_patch.modulation_amount = 0.05
	
	modular_voice.patches.append(vibrato_patch)
