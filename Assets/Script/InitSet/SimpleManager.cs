using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISimpleBase
{
    void Act();
}

public class SimpleManager<SimpleT> : MonoBehaviour, IManagerBase where SimpleT : ISimpleBase
{
    public int ActPriority { get; private set; } = 0;

    readonly List<SimpleT> simpleTs = new List<SimpleT>();

    public virtual void AwakeInitialize()
    {

    }

    public virtual void LateAwakeInitialize()
    {
        
    }

    public virtual void Act()
    {
        foreach (SimpleT simpleT in simpleTs)
        {
            simpleT.Act();
        }
    }

    public virtual void Add(SimpleT simpleT)
    {
        simpleTs.Add(simpleT);
    }

    public virtual bool Remove(SimpleT simpleT)
    {
        return simpleTs.Remove(simpleT);
    }

    public void SetPriority(int priority)
    {
        ActPriority = priority;
    }
}
