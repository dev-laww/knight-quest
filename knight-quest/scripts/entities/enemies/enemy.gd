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

    await get_tree().create_timer(.5).timeout

func take_damage(amount: int, modify_mode: StatsManager.ModifyMode = StatsManager.ModifyMode.VALUE) -> void:
    stats_manager.take_damage(amount, modify_mode)

func is_alive() -> bool:
    return stats_manager.current_health > 0

func _on_stat_depleted(type: StatsManager.StatType) -> void:
    if type == StatsManager.StatType.Health:
        print("Enemy has been defeated")
        died.emit()
        queue_free()
    else:
        print("Enemy stat depleted: ", type)