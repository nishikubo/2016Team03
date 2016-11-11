using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemyBase<T, TEnum> : MonoBehaviour
    where T : class where TEnum : System.IConvertible
{
    protected List<IState<T>> statelist = new List<IState<T>>();
    protected StateManager<T> stateManager;
        
    public virtual void ChangeState(TEnum state)
    {
        if(stateManager == null)
        {
            return;
        }

        stateManager.ChangStatee(statelist[state.ToInt32(null)]);
    }

    public virtual bool IsCurrentState(TEnum state)
    {
        if(stateManager == null)
        {
            return false;
        }

        return stateManager.CurrentState == statelist[state.ToInt32(null)];
    }

    protected virtual void Update()
    {
        if(stateManager != null)
        {
            stateManager.Update();
        }
    }
}

