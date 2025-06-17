extends MarginContainer
class_name HeadsUpDisplay


@onready var player_position_marker: Marker2D = %PlayerPosition
@onready var enemy_position_marker: Marker2D = %EnemyPosition
@onready var question_label: RichTextLabel = %QuestionLabel
@onready var answer_buttons: Array[Node] = [%FirstAnswerButton, %SecondAnswerButton, %ThirdAnswerButton, %FourthAnswerButton]
@onready var answer_button_group: ButtonGroup = %FirstAnswerButton.button_group


signal answer_selected(selected_index: int)


func set_question(question: Question) -> void:
    question_label.text = question.question_text

    for i in range(answer_buttons.size()):
        if i < question.answers.size():
            answer_buttons[i].text = question.answers[i]
            answer_buttons[i].visible = true
        else:
            answer_buttons[i].visible = false

func _on_answer_selected(button: BaseButton) -> void:
    var index = button.get_meta("answer_index", -1)

    if index == -1:
        push_error("Button does not have an answer index set.")
        return

    answer_selected.emit(index)


func deploy_player(player: Node2D) -> void:
    player_position_marker.add_child.call_deferred(player)

func deploy_enemy(enemy: Node2D) -> void:
    enemy_position_marker.add_child.call_deferred(enemy)
