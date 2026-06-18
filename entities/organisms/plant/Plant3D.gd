class_name Plant3D
extends Node3D

@onready var speaker: SynthSpeaker3D = $Speaker
@onready var mesh: MeshInstance3D = $Mesh
@onready var label: Label3D = $Label

var seq = AsciiSequencer.new()
var sel = Selector.new()
var voice = Voice.new()
var tone = OscWavetable.new()
var lfo1 = OscSine.new()
# var env = Envelope.new()
var vca = Amp.new()
# var rnd = OscSampleAndHold.new()

func _ready() -> void:
	# Generate DNA
	
	var dna = " ".repeat(32)
	var alphabet = "0123456789-?!."
	
	for i in range(dna.length()):
		dna[i] = alphabet[randi_range(0, alphabet.length() - 1)]
	
	label.text = dna
	
	# Create audio graph
	
	seq.set_sequence(dna)
	seq.set_value("frequency", 5.0)
	seq.set_patch(sel, "position")
	
	sel.list = [
		783.99 * 0.25,		# 1 - G5
		880.0 * 0.25,		# 2 - A5
		932.33 * 0.25,		# 3 - Bb5
		1108.73 * 0.25,		# 4 - C#6
		1174.66 * 0.25,		# 5 - D6
		1318.51 * 0.25,		# 6 - E6
		1479.98 * 0.25		# 7 - F#6
	];
	sel.set_patch(voice, "frequency")
	
	voice.set_value("glide", 0.05)
	voice.set_patch(tone, "frequency")
	
	tone.set_patch(vca, "input")
	tone.set_output_callback(Callable(self, "set_shader_param_b"))
	
	lfo1.set_value("frequency", 1.0)
	lfo1.map_multiply(vca, "level", 0.75, 1.0)
	lfo1.set_output_callback(Callable(self, "set_shader_param_a"))
	
	#rnd.set_value("frequency", 12.0)
	#rnd.patch(env, "gate", -0.8, 0.2)
	#rnd.patch(tone, "frequency", -330.0, 330.0)
	
	speaker.connect_source(vca)

#func _process(delta: float) -> void:
	#env.process_frame()
	#
	#if Game.player_ref:
		#var player_height = Game.player_ref.global_position.y
		#var target_freq = remap(player_height, -5.0, 5.0, 200.0, 800.0)
		#target_freq = clamp(target_freq, 200.0, 800.0)
		#tone.set_value("frequency", target_freq)
	#
	#if Input.is_action_just_pressed("interact"):
		#env.set_value("gate", 1.0)
	#elif Input.is_action_just_released("interact"):
		#env.set_value("gate", 0.0)

func set_shader_param_a(p_value: float) -> void:
	mesh.set_instance_shader_parameter("u_param_a", p_value)

func set_shader_param_b(p_value: float) -> void:
	mesh.set_instance_shader_parameter("u_param_b", remap(p_value, -1.0, 1.0, 0.0, 2.0))
