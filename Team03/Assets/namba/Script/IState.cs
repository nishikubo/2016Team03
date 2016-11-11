using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class IState<T>
{
    protected T owner;

    public IState(T owner)
    {
        this.owner = owner;
    } 

    public virtual void Initialize() { }

    public virtual void Execute() { }

    public virtual void End() { }
}
