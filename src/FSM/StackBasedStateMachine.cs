using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Emo
public class StackBasedStateMachine<T>
{
    public virtual BaseState<T> CurrState => CurrentState.Peek();
    private Stack<BaseState<T>> CurrentState = null;
    public T Controller;
    private Dictionary<string, BaseState<T>> StateList;
    public StackBasedStateMachine(T controller)
    {
        Controller = controller;
        CurrentState = new Stack<BaseState<T>>();
        StateList = new Dictionary<string, BaseState<T>>();
    }

    public void Update()
    {
        if (CurrentState.Count > 0 && CurrentState.Peek() != null)
        {
            CurrentState.Peek().OnUpdate(Controller);
        }
    }

    public void FixedUpdate()
    {
        if (CurrentState.Count > 0 && CurrentState.Peek() != null)
        {
            CurrentState.Peek().OnFixedUpdate(Controller);
        }
    }

    public void AddState(string stateName, BaseState<T> newStateToAdd)
    {
        if (!StateList.ContainsKey(stateName))
        {
            newStateToAdd.Parent = this;
            StateList.Add(stateName, newStateToAdd);
        }
    }

    public BaseState<T> GetStateByName(string stateName)
    {
        return StateList[stateName];
    }

    public void PushState(string stateName)
    {
        BaseState<T> state = GetStateByName(stateName);
        //call on exit for current state

        if (CurrentState.Count > 0)
        {
            CurrentState.Peek().OnExit(Controller);
        }


        //push it to the stack
        CurrentState.Push(state);
        //call on enter for next state
        state.OnEnter(Controller);
    }

    public void PushDynamicState(BaseState<T> baseState)
    {
        if (CurrentState.Count > 0)
        {
            CurrentState.Peek().OnExit(Controller);
        }
        baseState.Parent = this;
        CurrentState.Push(baseState);
        baseState.OnEnter(Controller);
    }

    public void PopState()
    {
        if (CurrentState.Count > 1)
        {
            CurrentState.Peek().OnExit(Controller);
            CurrentState.Pop();
            CurrentState.Peek().OnEnter(Controller);
        }
        else
        {
            Debug.Log(CurrentState.Count);
        }
    }

    public void CollapseToBaseState()
    {
        for (int i = 0; i < CurrentState.Count; i++)
        {
            CurrentState.Peek().OnExit(Controller);
        }

        PushState("Base");
    }


    public T GetCurrentStateAndCast<T>() where T : BaseState<T>
    {
        if (CurrState is T)
        {
            return CurrState as T;
        }

        return null;
    }

    public string GetCurrentStateName()
    {
        return CurrentState.Count > 0 ? CurrentState.Peek().ToString() : "No Current State";
    }
}
