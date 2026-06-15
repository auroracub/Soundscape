class_name FirstPersonController
extends CharacterBody3D

# --- Exported Parameters ---
@export_category("Movement Constants")
@export var walk_speed: float = 5.0
@export var jump_velocity: float = 4.5
@export var acceleration: float = 8.0
@export var deacceleration: float = 12.0

@export_category("Mouse Look")
@export var mouse_sensitivity: float = 0.002
@export_range(-90.0, 90.0) var vertical_look_limit_min: float = -89.0
@export_range(-90.0, 90.0) var vertical_look_limit_max: float = 89.0

# --- Node References ---
@onready var head: Node3D = $Head
@onready var camera: Camera3D = $Head/Camera3D

# --- Internal State Tracking ---
var move_direction: Vector3 = Vector3.ZERO
var target_speed: float = 0.0

# Capture and lock mouse on launch
func _ready() -> void:
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED

# Main input handling for looking around
func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
		# 1. Rotate the whole body horizontally (Y axis)
		rotate_y(-event.relative.x * mouse_sensitivity)
		
		# 2. Rotate only the head vertically (X axis)
		head.rotate_x(-event.relative.y * mouse_sensitivity)
		
		# 3. Clamp vertical looking to prevent flipping upside down
		head.rotation.x = clamp(
			head.rotation.x, 
			deg_to_rad(vertical_look_limit_min), 
			deg_to_rad(vertical_look_limit_max)
		)
		
	# UI/Menu escape hatch to unlock mouse
	if event.is_action_pressed("ui_cancel"):
		_toggle_mouse_capture()

# Primary physics loops
func _physics_process(delta: float) -> void:
	_apply_gravity(delta)
	_handle_jump()
	_gather_movement_input()
	_apply_movement_physics(delta)
	
	move_and_slide()

# --- Modulable Architecture Sub-Functions ---

func _apply_gravity(delta: float) -> void:
	if not is_on_floor():
		# Uses project-defined default gravity settings
		velocity += get_gravity() * delta

func _handle_jump() -> void:
	if Input.is_action_just_pressed("jump") and is_on_floor():
		velocity.y = jump_velocity

func _gather_movement_input() -> void:
	# Grab 2D vector relative to keyboard inputs
	var input_dir = Input.get_vector("move_left", "move_right", "move_forward", "move_backward")
	
	# Transform the flat 2D input relative to the direction the player is looking
	move_direction = (transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized()
	
	# Base default state speed config
	target_speed = walk_speed

func _apply_movement_physics(delta: float) -> void:
	# Calculate target velocities across horizontal axes (ignoring falling/jumping Y velocity)
	var horizontal_velocity = velocity
	horizontal_velocity.y = 0
	
	var target_velocity = move_direction * target_speed
	
	# Use different interpolation speeds depending on whether the player is moving or stopping
	var current_accel = acceleration if move_direction.dot(horizontal_velocity) > 0 else deacceleration
	
	# Smoothly blend current velocity to target velocity
	horizontal_velocity = horizontal_velocity.lerp(target_velocity, current_accel * delta)
	
	# Re-apply horizontal values back onto the main velocity engine
	velocity.x = horizontal_velocity.x
	velocity.z = horizontal_velocity.z

func _toggle_mouse_capture() -> void:
	if Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
		Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
	else:
		Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
