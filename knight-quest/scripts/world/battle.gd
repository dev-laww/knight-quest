extends Node2D

@export var player_scene: PackedScene
@export var enemy_scene: PackedScene


@onready var turn_manager: TurnManager = $TurnManager
@onready var start_button: Button = $StartButton


func _ready() -> void:
    start_button.pressed.connect(turn_manager.start)
