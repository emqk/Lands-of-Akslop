using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    State<T> currentState;
    T owner;

    public StateMachine(T _owner)
    {
        owner = _owner;
    }

    public void Update()
    {
        if (currentState != null)
           currentState.UpdateState(owner);
    }

    public void ChangeState(State<T> newState)
    {
        if (currentState != null)
            currentState.ExitState(owner);

        currentState = newState;
        currentState.EnterState(owner);
    }
}
