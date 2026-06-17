extends MeshInstance3D

@onready var speaker: SynthSpeaker3D = $SynthSpeaker3D

var tone: OscWavetable
var lfo: OscSine
var rnd: OscSampleAndHold
var env: Envelope
var vca: Amp
var bridge: SignalBridge

func _ready() -> void:
	tone = OscWavetable.new()
	lfo = OscSine.new()
	rnd = OscSampleAndHold.new()
	env = Envelope.new()
	vca = Amp.new()
	bridge = SignalBridge.new()
	
	tone.set_base_value("frequency", 440)
	tone.patch_out(vca, "input", 0, 1, 0, 1)
	
	lfo.set_base_value("frequency", 5)
	lfo.patch_out(tone, "frequency", -1, 1, -5, 5)
	
	rnd.set_base_value("frequency", 12)
	rnd.patch_out(tone, "frequency", -1, 1, -440, 440)
	# Ranges are unnecessary for every patch_out
	# It would be cleaner to define this per-module (if at all)
	
	# Option for additive vs multiplicative mods? Can this be abstracted
	# By the mod class itself?
	
	env.set_base_value("gate", 0.0)
	env.set_base_value("attack", 0.05)
	env.set_base_value("hold", 0.0)
	env.set_base_value("decay", 0.1)
	env.set_base_value("sustain", 0.2)
	env.set_base_value("release", 0.1)
	# Takes longer than expected to fade out
	env.patch_out(tone, "frequency", 0, 1, 50, 0)
	env.patch_out(vca, "level", 0, 1, 0, 1)
	env.set_frame_callback(Callable(self, "_on_env"))
	
	speaker.connect_source(vca)

func _on_env(p_value: float) -> void:
	print("Env Value: ", p_value)
	set_instance_shader_parameter("u_alpha", p_value)

func _process(_delta: float) -> void:
	env.process_frame()
	
	if Input.is_action_pressed("interact"):
		env.set_base_value("gate", 1.0)
		# Gate values should actually be multiplied rather than added?
	else:
		env.set_base_value("gate", 0.0)
