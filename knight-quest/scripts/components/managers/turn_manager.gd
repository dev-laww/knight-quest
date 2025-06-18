extends Node
class_name TurnManager

signal player_turn_started
signal player_answered(correct: bool)
signal player_timeout
signal execute_phase_started
signal enemy_turn_started
signal turn_completed

@export var question_manager: QuestionManager
@onready var turn_timer: Timer = $TurnTimer

var state_machine: CallableStateMachine = CallableStateMachine.new()
var current_question: Question
var player_answer_index: int = -1
var came_from_player_turn: bool = false

func _ready() -> void:
    state_machine.add_state(player_turn_state, enter_player_turn, leave_player_turn)
    state_machine.add_state(execute_state, enter_execute)
    state_machine.add_state(enemy_turn_state, enter_enemy_turn)
    
    state_machine.set_initial_state(player_turn_state)
    turn_timer.timeout.connect(_on_turn_timeout)

func _process(_delta: float) -> void:
    state_machine.update()


func player_turn_state() -> void:
    pass

func enter_player_turn() -> void:
    print("=== PLAYER TURN ===")
    current_question = question_manager.get_question()
    player_answer_index = -1
    turn_timer.start()
    player_turn_started.emit()

func leave_player_turn():
    turn_timer.stop()


func enter_execute() -> void:
    print("=== EXECUTE PHASE ===")
    execute_phase_started.emit()

func execute_state() -> void:
    execute()

    await get_tree().create_timer(1.0).timeout
    
    if came_from_player_turn:
        came_from_player_turn = false
        state_machine.change_state(enemy_turn_state)
    else:
        turn_completed.emit()
        state_machine.change_state(player_turn_state)

func enter_enemy_turn() -> void:
    print("=== ENEMY TURN ===")
    enemy_turn_started.emit()

func enemy_turn_state() -> void:
    enemy_turn()
    state_machine.change_state(execute_state)


func submit_player_answer(answer_index: int) -> void:
    if state_machine.current_state != "player_turn_state":
        return
    
    player_answer_index = answer_index
    var is_correct = question_manager.check_answer(current_question, answer_index)
    print("Player answered: ", current_question.answers[answer_index])
    print("Answer is: ", "Correct" if is_correct else "Incorrect")
    
    player_answered.emit(is_correct)
    came_from_player_turn = true
    state_machine.change_state(execute_state)

func get_current_question() -> Question:
    return current_question

func execute() -> void:
    print("Executing status effects and game logic...")
    await get_tree().create_timer(1.0).timeout

func enemy_turn() -> void:
    print("Enemy is taking their turn...")
    await get_tree().create_timer(2.0).timeout

func _on_turn_timeout() -> void:
    if state_machine.current_state == "player_turn_state":
        print("Player turn timed out!")
        player_timeout.emit()
        came_from_player_turn = true
        state_machine.change_state(execute_state)