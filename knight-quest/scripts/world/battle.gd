extends Node2D


@onready var question_manager: QuestionManager = $QuestionManager
@onready var heads_up_display: HeadsUpDisplay = %HeadsUpDisplay

var question: Question


func _ready() -> void:
    question = question_manager.get_question()

    heads_up_display.set_question(question)
    heads_up_display.answer_selected.connect(_on_answer_selected)


func _on_answer_selected(selected_index: int) -> void:
    if question_manager.check_answer(question, selected_index):
        print("Correct answer selected!")
    else:
        print("Incorrect answer selected.")
