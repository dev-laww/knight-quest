using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [Tooltip("Initial state to start the state machine")]
    public State InitialState;

    public State CurrentState { get; private set; }

    private List<State> states = new();

    private void Awake()
    {
        states.AddRange(GetComponentsInChildren<State>());
    }

    private void Start()
    {
        if (InitialState is null)
        {
            Debug.LogError("Initial state is not set. Please assign an initial state in the inspector.");
            return;
        }

        ChangeState(InitialState);
    }

    private void Update()
    {
        CurrentState?.Tick();
    }

    public void ChangeState(State newState)
    {
        if (newState == CurrentState) return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void ChangeState<T>() where T : State
    {
        var newState = states.Find(s => s is T);

        if (newState is null)
        {
            Debug.LogError($"State of type {typeof(T).Name} not found in the state machine.");
            return;
        }

        ChangeState(newState);
    }
}