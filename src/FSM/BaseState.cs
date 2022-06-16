using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState<T>
{
    public StackBasedStateMachine<T> Parent;
    public abstract void OnEnter(T controller);
    public abstract void OnUpdate(T controller);

    public abstract void OnFixedUpdate(T controller);
    public abstract void OnExit(T controller);
}
