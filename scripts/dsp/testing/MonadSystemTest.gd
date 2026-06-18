extends MeshInstance3D

@onready var speaker: SynthSpeaker3D = $SynthSpeaker3D

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
	
	tone.set_value("frequency", 440.0)
	tone.set_patch(vca, "input")
	
	lfo.set_value("frequency", 2.5)
	lfo.map_add(tone, "frequency", -5.0, 5.0) 
	
	env.set_value("gate", 0.0)
	env.set_value("attack", 0.05)
	env.set_value("hold", 0.05)
	env.set_value("decay", 0.4)
	env.set_value("sustain", 0.5)
	env.set_value("release", 0.1)
	env.set_patch(vca, "level")
	env.set_callback(Callable(self, "debug"))
	
	# rnd.set_value("frequency", 12.0)
	# rnd.patch(env, "gate", -0.8, 0.2)
	# rnd.patch(tone, "frequency", -330.0, 330.0)
	
	speaker.connect_source(vca)

func _process(delta: float) -> void:
	env.process_frame()
	
	if Game.player_ref:
		var player_height = Game.player_ref.global_position.y
		var target_freq = remap(player_height, -5.0, 5.0, 200.0, 800.0)
		target_freq = clamp(target_freq, 200.0, 800.0)
		tone.set_value("frequency", target_freq)
	
	if Input.is_action_just_pressed("interact"):
		env.set_value("gate", 1.0)
	elif Input.is_action_just_released("interact"):
		env.set_value("gate", 0.0)

func debug(p_value: float) -> void:
	print("Debug Value: ", p_value)
	set_instance_shader_parameter("u_alpha", p_value)
