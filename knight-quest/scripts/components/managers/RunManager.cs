using System.Collections.Generic;
using System.Linq;
using Game.Autoloads;
using Game.Data;
using Game.Entities;
using Game.UI;
using Game.Utils;
using Godot;
using Godot.Collections;
using GodotUtilities;
using GodotUtilities.Logic;

namespace Game;

[Scene]
public partial class RunManager : Node
{
    [Export] private HeadsUpDisplay hud;
    [Export] private RunConfiguration configuration;

    [Node] private Timer turnTimer;

    [Signal] public delegate void TurnEndedEventHandler();
    [Signal] public delegate void PlayerTurnTimeoutEventHandler();
    [Signal] public delegate void PlayerAnsweredEventHandler(bool correct);
    [Signal] public delegate void EncounterStartedEventHandler(Entity[] enemies);

    private ImmediateDelegateStateMachine stateMachine = new();
    private Player player => this.GetPlayer();

    private Encounter currentEncounter;
    private readonly List<Entity> currentEnemies = [];
    private int currentEncounterIndex = -1;
    private Array<Entity> aliveEnemies => new(currentEnemies.Where(enemy => enemy.IsAlive).ToArray());
    private int currentTurn;

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;

        WireNodes();
    }

    public override async void _Ready()
    {
        this.AddToGroup();

        stateMachine.AddState(Idle);
        stateMachine.AddState(PlayerTurn);
        stateMachine.AddState(EnemyTurn);
        stateMachine.AddState(EncounterTransition);
        stateMachine.AddState(Execute);
        stateMachine.AddState(Victory);
        stateMachine.AddState(Defeat);

        await ToSignal(this, SignalName.Ready);

        turnTimer.Timeout += OnTurnTimerTimeout;
        hud.AnswerSelected += OnAnswerSelected;

        GetTree().CreateTimer(1f).Timeout += Start;
    }

    public void Start()
    {
        StartNextEncounter();
        stateMachine.ChangeState(PlayerTurn);
    }

    private void Idle() { }

    private void PlayerTurn()
    {
        QuestionManager.GetQuestion();
        turnTimer.Start(configuration.TurnDuration);
        Logger.Debug("Player's turn started, waiting for answer.");
    }

    private async void EnemyTurn()
    {
        Logger.Debug("Enemy's turn started, selecting an enemy to act.");
        var enemy = aliveEnemies.PickRandom();

        await enemy.TakeTurn(player);

        stateMachine.ChangeState(Execute);
    }

    private void Execute()
    {
        player.StatsManager.TickStatusEffects();

        foreach (var enemy in aliveEnemies)
        {
            enemy.StatsManager.TickStatusEffects();
        }

        EmitSignalTurnEnded();

        if (!player.IsAlive)
        {
            stateMachine.ChangeState(Defeat);
            return;
        }

        if (aliveEnemies.Count > 0)
        {
            stateMachine.ChangeState(PlayerTurn);
        }
        else
        {
            Logger.Debug("All enemies defeated, starting next encounter.");
            stateMachine.ChangeState(EncounterTransition);
        }
    }

    private void EncounterTransition()
    {
        if (currentEncounterIndex >= configuration.Encounters.Length - 1)
        {
            Logger.Debug("No more encounters left, switching to victory state.");
            stateMachine.ChangeState(Victory);
            return;
        }

        StartNextEncounter();
        stateMachine.ChangeState(PlayerTurn);
    }

    private void Victory()
    {
        Logger.Debug("Victory state reached, handling victory logic.");
    }

    private void Defeat()
    {
        Logger.Debug("Defeat state reached, handling defeat logic.");
    }

    private void OnTurnTimerTimeout()
    {
        if (stateMachine.GetCurrentState() != PlayerTurn) return;

        Logger.Debug("Turn timer timed out, switching to enemy turn.");

        EmitSignalPlayerTurnTimeout();
        stateMachine.ChangeState(EnemyTurn);
    }

    private async void OnAnswerSelected(int answerIndex)
    {
        if (stateMachine.GetCurrentState() != PlayerTurn) return;

        var correct = QuestionManager.IsAnswerCorrect(answerIndex);

        Logger.Debug($"Player answered  {(correct ? "correctly" : "incorrectly")}");

        EmitSignalPlayerAnswered(correct);

        if (correct)
        {
            var enemy = aliveEnemies.PickRandom();
            await player.TakeTurn(enemy);

            // TODO: make player take less damage or something else based on the answer
            stateMachine.ChangeState(Execute);
            return;
        }

        turnTimer.Stop();
        stateMachine.ChangeState(EnemyTurn);
    }

    private void StartNextEncounter()
    {
        if (currentEncounterIndex >= configuration.Encounters.Length - 1)
        {
            Logger.Debug("All encounters completed, switching to victory state.");
            stateMachine.ChangeState(Victory);
            return;
        }

        currentEncounterIndex++;
        currentEncounter = configuration.Encounters[currentEncounterIndex];
        currentEnemies.Clear();

        foreach (var enemy in currentEncounter.Enemies)
        {
            var enemyEntity = enemy.InstanceOrFree<Entity>();

            currentEnemies.Add(enemyEntity);
        }

        EmitSignalEncounterStarted(currentEnemies.ToArray());
    }
}