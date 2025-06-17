extends Resource
class_name Question


@export_multiline var question_text: String
@export var answers: Array[String] = []
@export var correct_answer_index: int = 0:
    get:
        return correct_answer_index

    set(value):
        if value < 0 or value >= answers.size():
            return
        correct_answer_index = value
@export var correct_answer: String:
    get:
        if not answers.is_empty():
            return answers[correct_answer_index]
        return ""