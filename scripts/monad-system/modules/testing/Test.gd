extends MeshInstance3D

@onready var speaker: Speaker3D = $Speaker3D

var tone: OscWavetable
var lfo: OscSine
var env: Envelope
var vca: Amp
var rnd: OscSampleAndHold

func _ready() -> void:
	tone = OscWavetable.new()
	lfo = OscSine.new()
	env = Envelope.new()
	vca = Amp.new()
	rnd = OscSampleAndHold.new()
	
	tone.set_mod_value("frequency", 440.0)
	tone.patch_out(vca, "input", -1.0, 1.0)
	
	lfo.set_mod_value("frequency", 2.5)
	lfo.patch_out(tone, "frequency", -5.0, 5.0) 
	
	env.set_mod_value("gate", 0.0)
	env.set_mod_value("attack", 0.05)
	env.set_mod_value("hold", 0.05)
	env.set_mod_value("decay", 0.4)
	env.set_mod_value("sustain", 0.5)
	env.set_mod_value("release", 0.1)
	env.patch_out(vca, "level", 0.0, 1.0)
	env.set_frame_callback(Callable(self, "_on_env"))
	
	# rnd.set_mod_value("frequency", 12.0)
	# rnd.patch_out(env, "gate", -0.8, 0.2)
	# rnd.patch_out(tone, "frequency", -330.0, 330.0)
	
	speaker.connect_source(vca)

func _process(delta: float) -> void:
	env.process_frame()
	var player_height = $"../Player3d".global_position.y
	var target_freq = remap(player_height, -5.0, 5.0, 200.0, 800.0)
	target_freq = clamp(target_freq, 200.0, 800.0)
	tone.set_mod_value("frequency", target_freq)
	# var target_value = clamp(player_height - 0.5, 0.0, 1.0)
	# env.set_mod_value("gate", target_value)
	
	if Input.is_action_just_pressed("interact"):
		env.set_mod_value("gate", 1.0)
	elif Input.is_action_just_released("interact"):
		env.set_mod_value("gate", 0.0)

func _on_env(p_value: float) -> void:
	print("Env Value: ", p_value)
	set_instance_shader_parameter("u_alpha", p_value)
