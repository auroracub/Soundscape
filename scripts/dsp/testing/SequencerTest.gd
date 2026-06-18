extends MeshInstance3D

@onready var speaker: SynthSpeaker3D = $SynthSpeaker3D

var seq = AsciiSequencer.new()
var sel = Selector.new()
var voice = Voice.new()
var tone = OscWavetable.new()
var lfo1 = OscSine.new()
var lfo2 = OscTri.new()
var lfo3 = OscGlide.new()
var env = Envelope.new()
var vca = Amp.new()
var rnd = OscSampleAndHold.new()

func _ready() -> void:
	seq.set_sequence("7-2-5-2-7-2-5-2-7-2-5-2-1-3-5-6-")
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
	
	lfo1.set_value("frequency", 1.0)
	lfo1.map_add(lfo3, "morph", 0.0, 1.0)
	
	lfo3.set_value("frequency", 8.0)
	# lfo3.set_value("morph", 0.01)
	lfo3.map_add(tone, "frequency", -200, 200)
	
	#voice.set_value("glide", 0.01)
	#voice.set_patch(tone, "frequency")
	
	tone.set_value("frequency", 440.0)
	#tone.set_patch(vca, "input")
	
	#lfo1.set_value("frequency", 0.4)
	# lfo1.map_add(tone, "frequency", -0.1, 0.1)
	#lfo1.map_add(lfo2, "morph", 0.0, 1.0)
	
	#lfo2.set_value("frequency", 1.25)
	# lfo2.set_value("morph", 0.5)
	#lfo2.map_add(tone, "position", 0.3, 0.1)
	
	#env.set_value("gate", 0.0)
	#env.set_value("attack", 0.05)
	#env.set_value("hold", 0.05)
	#env.set_value("decay", 0.4)
	#env.set_value("sustain", 0.5)
	#env.set_value("release", 0.1)
	#env.set_patch(vca, "level")
	#env.set_callback(Callable(self, "debug"))
	
	#rnd.set_value("frequency", 12.0)
	#rnd.patch(env, "gate", -0.8, 0.2)
	#rnd.patch(tone, "frequency", -330.0, 330.0)
	
	speaker.connect_source(tone)

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

func debug(p_value: float) -> void:
	print("Debug Value: ", p_value)
	set_instance_shader_parameter("u_alpha", p_value)
