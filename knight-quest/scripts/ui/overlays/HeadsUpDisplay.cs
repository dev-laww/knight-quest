using System.Collections.Generic;
using Game.Autoloads;
using Game.Data;
using Godot;
using GodotUtilities;
using Game.Utils;

namespace Game.UI;

[Scene]
public partial class HeadsUpDisplay : MarginContainer
{
    [Node] private Marker2D playerPosition;
    [Node] private Marker2D enemyPosition;
    [Node] private RichTextLabel questionLabel;
    [Node] private Button firstAnswerButton;

    [Signal] public delegate void AnswerSelectedEventHandler(int index);

    public Vector2 PlayerGlobalPosition => playerPosition.GetGlobalPosition();
    public Vector2 EnemyGlobalPosition => enemyPosition.GetGlobalPosition();

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

    public void Reset()
    {
        questionLabel.Text = string.Empty;

        var answerButtons = firstAnswerButton.ButtonGroup.GetButtons();

        foreach (var button in answerButtons)
        {
            if (button is not Button answerButton) continue;

            answerButton.Text = string.Empty;
            answerButton.Visible = false;
            answerButton.ButtonPressed = false;
            answerButton.Disabled = false;
        }
    }

    public void ToggleAnswerButtons(bool disabled = false)
    {
        var answerButtons = firstAnswerButton.ButtonGroup.GetButtons();

        foreach (var button in answerButtons)
        {
            if (button is not Button answerButton) continue;

            if (answerButton.ButtonPressed)
            {
                answerButton.ButtonPressed = false;
                continue;
            }

            answerButton.Disabled = disabled;
        }
    }

    private void OnAnswerButtonPressed(BaseButton button)
    {
        var answerIndex = button.GetMeta("answer_index").As<int>();

        Logger.Debug($"Answer selected: {answerIndex}");

        EmitSignalAnswerSelected(answerIndex);
        ToggleAnswerButtons(true);
    }

    private void OnQuestionRequested(Question question)
    {
        Reset();

        questionLabel.Text = question.QuestionText;

        var answerButtons = firstAnswerButton.ButtonGroup.GetButtons();

        for (var i = 0; i < answerButtons.Count; i++)
        {
            if (answerButtons[i] is not Button answerButton) continue;

            if (i < question.Answers.Length)
            {
                answerButton.Text = question.Answers[i];
                answerButton.Visible = true;
            }
            else
            {
                answerButton.Visible = false;
            }
        }
    }
}