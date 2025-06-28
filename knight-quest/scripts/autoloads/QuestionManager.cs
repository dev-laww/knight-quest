using Godot;
using System;
using Game.Data;

namespace Game.Autoloads;

public partial class QuestionManager : Autoload<QuestionManager>
{
    [Signal] public delegate void QuestionRequestedEventHandler(Question question);

    public static Question CurrentQuestion { get; private set; }

    public static Question GetQuestion()
    {
        CurrentQuestion = new()
        {
            QuestionText = "What is the capital of France?",
            Answers =
            [
                "Berlin",
                "Madrid",
                "Paris",
                "Rome"
            ],
            CorrectAnswerIndex = 2
        };

        Instance.EmitSignalQuestionRequested(CurrentQuestion);

        return CurrentQuestion;
    }

    public static bool IsAnswerCorrect(int answerIndex)
    {
        if (CurrentQuestion != null) return answerIndex == CurrentQuestion.CorrectAnswerIndex;

        return false;
    }
}