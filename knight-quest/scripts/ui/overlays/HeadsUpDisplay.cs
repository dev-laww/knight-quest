using System.Collections.Generic;
using Game.Autoloads;
using Game.Data;
using Godot;
using GodotUtilities;
using GodotUtilities.Util;

namespace Game.UI;

[Scene]
public partial class HeadsUpDisplay : MarginContainer
{
    [Node] private Marker2D playerPosition;
    [Node] private Marker2D enemyPosition;
    [Node] private RichTextLabel questionLabel;
    [Node] private Button firstAnswerButton;
    [Node] private Button secondAnswerButton;
    [Node] private Button thirdAnswerButton;
    [Node] private Button fourthAnswerButton;

    [Signal] public delegate void AnswerSelectedEventHandler(int index);

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override void _Ready()
    {
        var answerButtonGroup = firstAnswerButton.ButtonGroup;

        answerButtonGroup.Pressed += OnAnswerButtonPressed;
        QuestionManager.Instance.QuestionRequested += OnQuestionRequested;
    }

    public override void _ExitTree()
    {
        QuestionManager.Instance.QuestionRequested -= OnQuestionRequested;
    }

    private void OnAnswerButtonPressed(BaseButton button)
    {
        var answerIndex = button.GetMeta("answer_index").As<int>();

        Logger.Debug($"Answer selected: {answerIndex}");

        EmitSignalAnswerSelected(answerIndex);
    }

    private void OnQuestionRequested(Question question)
    {
        questionLabel.Text = question.QuestionText;

        var answerButtons = new List<Button>
        {
            firstAnswerButton,
            secondAnswerButton,
            thirdAnswerButton,
            fourthAnswerButton
        };

        for (var i = 0; i < answerButtons.Count; i++)
        {
            if (i < question.Answers.Length)
            {
                answerButtons[i].Text = question.Answers[i];
                answerButtons[i].Visible = true;
            }
            else
            {
                answerButtons[i].Visible = false;
            }
        }
    }
}