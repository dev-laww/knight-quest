extends Node
class_name RunManager


@export var question_manager: QuestionManager
@export var turn_manager: TurnManager

@onready var player_stats_manager: StatsManager = $PlayerStatsManager
@onready var enemy_stats_manager: StatsManager = $EnemyStatsManager


func _ready() -> void:
    pass
