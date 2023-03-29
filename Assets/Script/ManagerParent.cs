using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IManagerBase
{
    int ActPriority { get; }
    void AwakeInitialize();
    void LateAwakeInitialize();
    void Act();
}

class ManagerBaseComparer : IComparer<IManagerBase>
{
    public int Compare(IManagerBase mb1, IManagerBase mb2)
    {
        if (mb1.ActPriority < mb2.ActPriority)
        {
            return 1;
        }
        else if (mb1.ActPriority > mb2.ActPriority)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}

public class ManagerParent : MonoBehaviour
{
    public static ManagerParent Instance { get; private set; }

    readonly List<IManagerBase> managerList = new List<IManagerBase>();

    readonly ManagerBaseComparer comparer = new ManagerBaseComparer();

    public bool IsStarted { get; private set; } = false;

    void Awake()
    {
        Initialize();
    }

    IEnumerator AwakeInitialize()
    {
        gameObject.GetComponents<IManagerBase>(managerList);
        managerList.Sort(comparer);

        foreach (IManagerBase managerBase in managerList)
        {
            managerBase.AwakeInitialize();
            yield return null;
        }

        foreach (IManagerBase managerBase in managerList)
        {
            managerBase.LateAwakeInitialize();
            yield return null;
        }

        IsStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsStarted)
        {
            foreach (IManagerBase managerBase in managerList)
            {
                managerBase.Act();
            }
        }
    }

    void Clear()
    {
        managerList.Clear();
        IsStarted = false;
    }

    public void Initialize()
    {
        if (Instance != null)
        {
            Instance.Clear();
        }

        Instance = this;

        StartCoroutine(AwakeInitialize());
    }

    public void AddManager(IManagerBase managerBase)
    {
        if (IsStarted)
        {
            managerList.Add(managerBase);
            managerList.Sort(comparer);
        }
    }

    public void RemoveManager(IManagerBase managerBase)
    {
        if (IsStarted)
        {
            managerList.Remove(managerBase);
        }
    }

    public T MakeManager<T>() where T : Component, IManagerBase
    {
        T tmp = gameObject.AddComponent<T>();
        managerList.Add(tmp);
        return tmp;
    }
}
