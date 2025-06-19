extends Node
class_name TurnManager

signal player_turn_started
signal player_answered(correct: bool)
signal player_timeout
signal execute_phase_started
signal enemy_turn_started
signal turn_completed

@export var question_manager: QuestionManager
@export var heads_up_display: HeadsUpDisplay
@onready var turn_timer: Timer = $TurnTimer
@onready var current_question: Question = question_manager.current_question

var state_machine: CallableStateMachine = CallableStateMachine.new()
var player_answer_index: int = -1
var came_from_player_turn: bool = false

func _ready() -> void:
    state_machine.add_state(idle_state)
    state_machine.add_state(player_turn_state, enter_player_turn, leave_player_turn)
    state_machine.add_state(execute_state, enter_execute)
    state_machine.add_state(enemy_turn_state, enter_enemy_turn, leave_enemy_turn)
    state_machine.set_initial_state(idle_state)
    heads_up_display.answer_selected.connect(submit_player_answer)
    turn_timer.timeout.connect(_on_turn_timeout)

func start() -> void:
    print("TurnManager started")
    state_machine.change_state(player_turn_state)
    state_machine.update()

func idle_state() -> void:
    print("Idle state")
    pass

func player_turn_state() -> void:
    pass

func enter_player_turn() -> void:
    print("=== PLAYER TURN ===")
    question_manager.get_question()
    player_answer_index = -1
    turn_timer.start()
    player_turn_started.emit()

    state_machine.update()

func leave_player_turn():
    came_from_player_turn = true
    turn_timer.stop()

func enter_execute() -> void:
    print("=== EXECUTE PHASE ===")
    execute_phase_started.emit()

    state_machine.update()

func execute_state() -> void:
    await execute()

    if came_from_player_turn:
        state_machine.change_state(enemy_turn_state)
    else:
        turn_completed.emit()
        state_machine.change_state(player_turn_state)

func enter_enemy_turn() -> void:
    print("=== ENEMY TURN ===")
    enemy_turn_started.emit()

    state_machine.update()

func enemy_turn_state() -> void:
    await get_tree().create_timer(3.0).timeout
    state_machine.change_state(execute_state)

func leave_enemy_turn() -> void:
    came_from_player_turn = false

func submit_player_answer(answer_index: int) -> void:
    if state_machine.current_state != "player_turn_state": return
    player_answer_index = answer_index
    var is_correct = question_manager.check_answer(answer_index)
    print("Player answered: ", question_manager.current_question.answers[answer_index])
    print("Answer is: ", "Correct" if is_correct else "Incorrect")
    player_answered.emit(is_correct)
    state_machine.change_state(execute_state)

func execute() -> void:
    print("Executing status effects and game logic...")
    await get_tree().create_timer(4.0).timeout

func _on_turn_timeout() -> void:
    if state_machine.current_state != "player_turn_state": return
    print("Player turn timed out!")
    player_timeout.emit()
    state_machine.change_state(execute_state)
