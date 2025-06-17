extends MarginContainer
class_name HeadsUpDisplay


@onready var player_position: Marker2D = %PlayerPosition.global_position
@onready var enemy_position: Marker2D = %EnemyPosition.global_position
@onready var question_label: RichTextLabel = %QuestionLabel
@onready var answer_buttons: Array[Node] = [%FirstAnswerButton, %SecondAnswerButton, %ThirdAnswerButton, %FourthAnswerButton]
@onready var answer_button_group: ButtonGroup = %FirstAnswerButton.button_group


func _ready() -> void:
    pass


func set_question(question: Question) -> void:
    question_label.text = question.question_text
   
    for i in range(answer_buttons.size()):
        if i < question.answers.size():
            answer_buttons[i].text = question.answers[i]
            answer_buttons[i].visible = true
        else:
            answer_buttons[i].visible = false