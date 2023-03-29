using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISimpleBase
{
    void Act();
}

public class SimpleManager<SimpleT> : IManagerBase where SimpleT : ISimpleBase
{
    public static SimpleManager<SimpleT> Instance { get { return Create(); } private set { Instance = value; } }

    public int ActPriority { get; private set; } = 0;

    readonly List<SimpleT> simpleTs = new List<SimpleT>();

    private SimpleManager(int actPriority)
    {
        ActPriority = actPriority;
    }

    public static SimpleManager<SimpleT> Create(int actPriority = 0)
    {
        if (Instance == null)
        {
            Instance = new SimpleManager<SimpleT>(actPriority);
            ManagerParent.Instance.AddManager(Instance);
        }

        return Instance;
    }

    public static void Destroy()
    {
        if (Instance == null)
        {
            return;
        }

        ManagerParent.Instance.RemoveManager(Instance);
        Instance = null;
    }

    public void AwakeInitialize()
    {

    }

    public void LateAwakeInitialize()
    {
        
    }

    public void Act()
    {
        foreach (SimpleT simpleT in simpleTs)
        {
            simpleT.Act();
        }
    }

    public void Add(SimpleT simpleT)
    {
        simpleTs.Add(simpleT);
    }

    public bool Remove(SimpleT simpleT)
    {
        return simpleTs.Remove(simpleT);
    }

    public void SetPriority(int priority)
    {
        ActPriority = priority;
    }
}
