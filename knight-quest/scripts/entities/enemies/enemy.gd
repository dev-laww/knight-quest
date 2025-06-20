extends Node2D
class_name Enemy

signal died
signal action_completed

@onready var stats_manager: StatsManager = $StatsManager


func _ready() -> void:
    stats_manager.stat_depleted.connect(_on_stat_depleted)


func take_turn(player_stats_manager: StatsManager) -> void:
    player_stats_manager.take_damage(stats_manager.damage, StatsManager.ModifyMode.VALUE)
    print("Enemy took its turn, dealt damage: ", stats_manager.damage)
    action_completed.emit()


func _on_stat_depleted(type: StatsManager.StatType) -> void:
    if type == StatsManager.StatType.Health:
        print("Enemy has been defeated")
        died.emit()
        queue_free()
    else:
        print("Enemy stat depleted: ", type)