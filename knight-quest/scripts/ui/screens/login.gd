extends CanvasLayer

var scene: PackedScene = preload("res://scenes/ui/screens/main_menu.tscn")


@onready var login_button: Button = %LoginButton


func _ready() -> void:
	login_button.pressed.connect(_on_login_button_pressed)


func _on_login_button_pressed() -> void:
	get_tree().change_scene_to_packed(scene)
