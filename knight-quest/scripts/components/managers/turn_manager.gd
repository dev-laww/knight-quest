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

func _ready():
	state_machine.add_state(
		player_turn_state,
		enter_player_turn,
		leave_player_turn
	)
	
	state_machine.add_state(
		execute_after_player_state,
		enter_execute_after_player
	)
	
	state_machine.add_state(
		enemy_turn_state,
		enter_enemy_turn,
		leave_enemy_turn
	)
	
	state_machine.add_state(
		execute_after_enemy_state,
		enter_execute_after_enemy,
		leave_execute_after_enemy
	)

	state_machine.set_initial_state(player_turn_state)

func _process(_delta):
	state_machine.update()

func enter_player_turn():
	print("=== PLAYER TURN ===")
	current_question = question_manager.get_question()
	player_answer_index = -1
	turn_timer.start()
	player_turn_started.emit()

func player_turn_state():
	pass

func leave_player_turn():
	turn_timer.stop()

func enter_execute_after_player():
	print("=== EXECUTE AFTER PLAYER ===")
	execute_phase_started.emit()

func execute_after_player_state():
	execute()
	state_machine.change_state(enemy_turn_state)

func enter_enemy_turn():
	print("=== ENEMY TURN ===")
	enemy_turn_started.emit()

func enemy_turn_state():
	enemy_turn()
    
	state_machine.change_state(execute_after_enemy_state)

func leave_enemy_turn():
	pass


func enter_execute_after_enemy():
	print("=== EXECUTE AFTER ENEMY ===")
	execute_phase_started.emit()

func execute_after_enemy_state():
	execute()

	turn_completed.emit()
	state_machine.change_state(player_turn_state)

func leave_execute_after_enemy():
	pass

func submit_player_answer(answer_index: int):
	if state_machine.current_state == "player_turn_state":
		player_answer_index = answer_index
		var is_correct = question_manager.check_answer(current_question, answer_index)
		print("Player answered: ", current_question.answers[answer_index])
		print("Answer is: ", "Correct" if is_correct else "Incorrect")

		player_answered.emit(is_correct)
		state_machine.change_state(execute_after_player_state)

func get_current_question() -> Question:
	return current_question


func execute():
	print("Executing status effects and game logic...")
	
	await get_tree().create_timer(1.0).timeout

func enemy_turn():
	print("Enemy is taking their turn...")

	await get_tree().create_timer(2.0).timeout

func _on_turn_timeout():
	if state_machine.current_state == "player_turn_state":
		print("Player turn timed out!")
		player_timeout.emit()
		state_machine.change_state(execute_after_player_state)