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

public class ManagerParent : MonoBehaviour
{
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

    public static ManagerParent Instance { get; private set; }

    readonly List<IManagerBase> managerList = new List<IManagerBase>();

    readonly ManagerBaseComparer comparer = new ManagerBaseComparer();

    public bool IsStarted { get; private set; } = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        Initialize();
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

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Clear()
    {
        managerList.Clear();
        IsStarted = false;
    }

    public void Initialize()
    {
        Instance.Clear();
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
        AddManager(tmp);
        return tmp;
    }

    IEnumerator AwakeInitialize()
    {
        gameObject.GetComponents<IManagerBase>(managerList);
        managerList.Sort(comparer);

        foreach (IManagerBase managerBase in managerList)
        {
            managerBase.AwakeInitialize();
        }

        yield return null;

        foreach (IManagerBase managerBase in managerList)
        {
            managerBase.LateAwakeInitialize();
        }

        IsStarted = true;
    }
}
