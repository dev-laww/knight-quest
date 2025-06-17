extends Node
class_name QuestionManager


func get_question() -> Question:
    var question: Question = Question.new()
    
    question.question_text = "What is the capital of France?"
    question.answers = ["Berlin", "Madrid", "Paris", "Rome"]
    question.correct_answer_index = 2

    return question


func check_answer(question: Question, selected_index: int) -> bool:
    if selected_index < 0 or selected_index >= question.answers.size():
        return false
    return selected_index == question.correct_answer_index
