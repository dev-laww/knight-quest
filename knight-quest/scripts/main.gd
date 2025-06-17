extends Node

var scene: PackedScene = preload("res://scenes/ui/screens/login.tscn")

func _ready() -> void:
    (func(): get_tree().change_scene_to_packed(scene)).call_deferred()
