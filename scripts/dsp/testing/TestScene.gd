extends Node

@export var player_node: Node

func _ready() -> void:
	Game.register_player(player_node)
