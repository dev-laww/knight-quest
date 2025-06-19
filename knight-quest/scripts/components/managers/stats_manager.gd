extends Node
class_name StatsManager

signal stat_changed(type: StatType, value: int)
signal stat_increased(type: StatType, value: int)
signal stat_decreased(type: StatType, value: int)
signal stat_depleted(type: StatType)

@export var max_health: int = 10
@export var invulnerable: bool = false

enum StatType {
    Health
}

enum ModifyMode {
    PERCENTAGE,
    VALUE
}

var _stats := {
    StatType.Health: 0,
}

var _max_stats := {}

var health: int:
    get: return _stats[StatType.Health]
    set(value): _set_stat(StatType.Health, value)

func _ready():
    _max_stats[StatType.Health] = max_health
    _stats[StatType.Health] = max_health

func heal(amount: int, mode: ModifyMode = ModifyMode.VALUE):
    match mode:
        ModifyMode.PERCENTAGE:
            health += int(_max_stats[StatType.Health] * (amount / 100.0))
        ModifyMode.VALUE:
            health += amount

func take_damage(amount: int, mode: ModifyMode = ModifyMode.VALUE):
    if invulnerable:
        return

    var damage_amount: int
    match mode:
        ModifyMode.PERCENTAGE:
            damage_amount = int(_max_stats[StatType.Health] * (amount / 100.0))
        ModifyMode.VALUE:
            damage_amount = amount

    health -= damage_amount

func set_invulnerable(value: bool):
    invulnerable = value

func _set_stat(type: StatType, value: int):
    if not _stats.has(type):
        return

    var previous = _stats[type]
    var max_value = _max_stats.get(type, previous)
    var clamped = clamp(value, 0, max_value)

    if previous == clamped:
        return

    _stats[type] = clamped
    stat_changed.emit(type, clamped)

    if clamped > previous:
        stat_increased.emit(type, clamped - previous)
    elif clamped < previous:
        stat_decreased.emit(type, previous - clamped)

    if clamped == 0:
        stat_depleted.emit(type)
