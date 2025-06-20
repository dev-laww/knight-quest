extends Node2D

@export var player_scene: PackedScene
@export var enemy_scene: PackedScene


@onready var run_manager: RunManager = $RunManager
@onready var heads_up_display: HeadsUpDisplay = $HeadsUpDisplay


func _ready() -> void:
    var player = player_scene.instantiate()
    var enemy = enemy_scene.instantiate()

    heads_up_display.deploy_player(player)
    heads_up_display.deploy_enemy(enemy)
