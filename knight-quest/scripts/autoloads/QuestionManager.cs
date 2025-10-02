using Godot;
using System;
using System.Collections.Generic;
using Game.Data;

using Logger = Game.Utils.Logger;

namespace Game.Autoloads;

public partial class QuestionManager : Autoload<QuestionManager>
{
    [Signal] 
    public delegate void QuestionRequestedEventHandler(Question question);

    public Question CurrentQuestion { get; private set; }
    private List<Question> questions = new();
    private int currentIndex = -1;
    private Random rng = new();

    public void LoadQuestions(Question[] question)
    {
        this.questions = new List<Question>(question);
        for (int i = this.questions.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (this.questions[i], this.questions[j]) = (this.questions[j], this.questions[i]);
        }
        currentIndex = -1;
    }

    public Question GetNextQuestion()
    {
        if (questions.Count == 0)
        {
            Logger.Error("No questions loaded!");
            return null;
        }

        currentIndex++;
        if (currentIndex >= questions.Count)
        {
            Logger.Error("No more questions available.");
            return null;
        }

        CurrentQuestion = questions[currentIndex];
        EmitSignal(SignalName.QuestionRequested, CurrentQuestion);
        return CurrentQuestion;
    }

    public bool IsAnswerCorrect(int answerIndex)
        => CurrentQuestion != null && answerIndex == CurrentQuestion.CorrectAnswerIndex;
}