using System;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    protected StateMachine stateMachine;

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Tick() { }

    private void Awake()
    {
        stateMachine = GetComponentInParent<StateMachine>();

        if (stateMachine is not null) return;

        Debug.LogError(
            $"State {name} is not a child of StateMachine. " +
            "Please ensure that this state is a child of a GameObject with a StateMachine component."
        );
    }
}