extends Node

var player_ref: Node

func register_player(p_ref: Node) -> void:
	player_ref = p_ref

func _unhandled_input(event: InputEvent) -> void:
	# Detect left-click, right-click, or middle-click
	if event is InputEventMouseButton and event.pressed:
		# Recapture the mouse only if it is currently visible or confined
		if Input.mouse_mode != Input.MOUSE_MODE_CAPTURED:
			Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
			# Optional: Mark the input as handled so UI elements don't accidentally click
			get_viewport().set_input_as_handled()
