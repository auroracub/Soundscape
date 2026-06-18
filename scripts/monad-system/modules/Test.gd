extends MeshInstance3D

@onready var speaker: SynthSpeaker3D = $SynthSpeaker3D

var tone: OscWavetable
var lfo: OscSine
var env: Envelope
var vca: Amp

func _ready() -> void:
	tone = OscWavetable.new()
	lfo = OscSine.new()
	env = Envelope.new()
	vca = Amp.new()
	
	tone.set_base_value("frequency", 440)
	tone.patch_out(vca, "input", -1.0, 1.0)
	
	lfo.set_base_value("frequency", 5)
	lfo.patch_out(tone, "frequency", -5, 5) 
	
	env.set_base_value("gate", 0.0)
	env.set_base_value("attack", 0.01)
	env.set_base_value("hold", 0.0)
	env.set_base_value("decay", 0.2)
	env.set_base_value("sustain", 0.3)
	env.set_base_value("release", 0.2)
	env.patch_out(vca, "level", 0.0, 1.0)
	#env.set_frame_callback(Callable(self, "_on_env"))
	
	speaker.connect_source(vca)

func _process(delta: float) -> void:
	env.process_frame()
	
	# Push above 0.0 to trigger the gate
	if Input.is_action_just_pressed("interact"):
		env.set_base_value("gate", 1.0)
	elif Input.is_action_just_released("interact"):
		env.set_base_value("gate", 0.0)

#func _on_env(p_value: float) -> void:
	#print("Env Value: ", p_value)
	#set_instance_shader_parameter("u_alpha", p_value)
