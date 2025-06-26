extends Node
class_name RunManager

signal player_turn_started
signal player_answered(correct: bool)
signal player_timeout
signal execute_phase_started
signal enemy_turn_started
signal turn_completed

@export var question_manager: QuestionManager
@export var heads_up_display: HeadsUpDisplay
@export var run_config: RunConfig

@onready var player_stats_manager: StatsManager = $PlayerStatsManager
@onready var turn_timer: Timer = $TurnTimer
@onready var current_question: Question = question_manager.current_question

var state_machine: CallableStateMachine = CallableStateMachine.new()
var player_answer_index: int = -1

var current_enemies: Array[Enemy] = []
var current_encounter_index: int = -1
var current_encounter: Encounter

func _ready() -> void:
    state_machine.add_state(idle)
    state_machine.add_state(player_turn, enter_player_turn, leave_player_turn)
    state_machine.add_state(enemy_turn, enter_enemy_turn)
    state_machine.add_state(execute, enter_execute)
    state_machine.add_state(encounter_transition)
    state_machine.add_state(victory)
    state_machine.add_state(defeat)

    state_machine.set_initial_state(idle)

    heads_up_display.answer_selected.connect(submit_player_answer)
    turn_timer.timeout.connect(_on_turn_timeout)

    get_tree().create_timer(3).timeout.connect(start)

func start() -> void:
    start_next_encounter()

    state_machine.change_state(player_turn)
    state_machine.update()

func idle() -> void:
    print("Idle state")

func player_turn() -> void:
    pass

func enter_player_turn() -> void:
    print("=== PLAYER TURN ===")
    question_manager.get_question()
    player_answer_index = -1
    turn_timer.start()
    player_turn_started.emit()

    state_machine.update()

func leave_player_turn():
    turn_timer.stop()

func enter_execute() -> void:
    print("=== EXECUTE PHASE ===")

    execute_phase_started.emit()
    state_machine.update()

func execute() -> void:
    print("Executing turn...")

    player_stats_manager.tick_status_effects()

    for enemy in current_enemies:
        if not enemy.is_alive():
            continue

        enemy.stats_manager.tick_status_effects()

    if player_stats_manager.health <= 0:
        state_machine.change_state(defeat)
        state_machine.update()
        return

    turn_completed.emit()

    if _has_alive_enemies():
        state_machine.change_state(player_turn)
    else:
        state_machine.change_state(encounter_transition)
        state_machine.update()

func enter_enemy_turn() -> void:
    print("=== ENEMY TURN ===")
    enemy_turn_started.emit()

    state_machine.update()

func enemy_turn() -> void:
    await get_tree().create_timer(3.0).timeout

    for enemy in _alive_enemies():
        await enemy.take_turn(player_stats_manager)

    state_machine.change_state(execute)

func submit_player_answer(answer_index: int) -> void:
    if state_machine.current_state != "player_turn": return

    player_answer_index = answer_index

    var is_correct = question_manager.check_answer(answer_index)

    print("Player answered: ", question_manager.current_question.answers[answer_index])
    print("Answer is: ", "Correct" if is_correct else "Incorrect")

    if is_correct:
        var enemy = _alive_enemies().pick_random()

        enemy.take_damage(question_manager.current_question.damage, StatsManager.ModifyMode.VALUE)
        print("Enemy took damage: ", question_manager.current_question.damage)

    # play attack animations

    player_answered.emit(is_correct)
    state_machine.change_state(enemy_turn)

func encounter_transition() -> void:
    if current_encounter_index >= run_config.encounters.size() - 1:
        state_machine.change_state(victory)
        state_machine.update()
        return

    start_next_encounter()

func victory() -> void:
    print("Player has won!")

func defeat() -> void:
    print("Player has been defeated!")


func start_next_encounter() -> void:
    current_encounter_index += 1
    current_enemies.clear()

    if current_encounter_index >= run_config.encounters.size() - 1:
        state_machine.change_state(victory)
        state_machine.update()
        return

    current_encounter = run_config.encounters[current_encounter_index]

    for enemy_scene in current_encounter.enemies:
        var instance = enemy_scene.instantiate() as Enemy
        current_enemies.append(instance)

        # TODO: add enemy to scene tree

func _has_alive_enemies() -> bool:
    for enemy in current_enemies:
        if enemy.is_alive():
            return true

    return false

func _on_turn_timeout() -> void:
    if state_machine.current_state != "player_turn": return

    print("Player turn timed out!")

    player_timeout.emit()
    state_machine.change_state(enemy_turn)

func _alive_enemies() -> Array[Enemy]:
    var alive_enemies: Array[Enemy] = []

    for enemy in current_enemies:
        if enemy.is_alive():
            alive_enemies.append(enemy)

    return alive_enemies