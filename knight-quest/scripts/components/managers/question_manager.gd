extends Node
class_name QuestionManager


@export var heads_up_display: HeadsUpDisplay

var current_question: Question = null

    
func get_question() -> Question:
    current_question = Question.new()
 
    current_question.question_text = "What is the capital of France?"
    current_question.answers = ["Berlin", "Madrid", "Paris", "Rome"]
    current_question.correct_answer_index = 2

    heads_up_display.set_question(current_question)

    return current_question


func check_answer(selected_index: int) -> bool:
    if selected_index < 0 or selected_index >= current_question.answers.size():
        return false
    return selected_index == current_question.correct_answer_index
