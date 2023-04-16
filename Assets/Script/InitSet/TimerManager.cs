using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour, IManagerBase
{
    public delegate void TimerOnceEventHandler(int timerID);
    public delegate void TimerUpdateEventHandler(int timerID, float elapsedTime, float dt);

    public static TimerManager Instance { get; private set; }

    enum TimerState
    {
        Ready,
        Active,
        Stop,
        Inactive,
    }

    class Timer
    {
        public event TimerOnceEventHandler OnStartEvent;
        public event TimerOnceEventHandler OnEndEvent;

        public event TimerUpdateEventHandler OnUpdateEvent;

        public TimerState State { get; protected set; }
        public int TimerID { get; protected set; } = 0;

        float elapsedTime = 0.0f;
        float endTime = 0.0f;

        float preUpdateTime = 0.0f;
        float updateTime = 0.0f;

        float lateStartTime = 0.0f;

        public void Initialize(int timerID, float endTime, float updateTime = -1.0f, TimerOnceEventHandler startEvent = null, TimerUpdateEventHandler updateEvent = null, TimerOnceEventHandler endEvent = null, float lateStartTime = 0.0f)
        {
            this.TimerID = timerID;

            this.elapsedTime = 0.0f;
            this.endTime = endTime;

            this.updateTime = updateTime;
            this.preUpdateTime = 0.0f;

            this.lateStartTime = lateStartTime;
            this.State = TimerState.Ready;

            this.OnStartEvent = startEvent;
            this.OnUpdateEvent = updateEvent;
            this.OnEndEvent = endEvent;
        }

        public void Stop()
        {
            if (State == TimerState.Ready)
            {
                elapsedTime = 0.0f;
            }
            State = TimerState.Stop;
        }

        public void Restart()
        {
            State = TimerState.Active;
        }

        public void End()
        {
            OnEndEvent?.Invoke(TimerID);
            State = TimerState.Inactive;
        }

        public void OnUpdate(float dt)
        {
            if (State == TimerState.Stop || State == TimerState.Inactive)
            {
                return;
            }

            elapsedTime += dt;

            if (State == TimerState.Active)
            {
                if (elapsedTime > endTime)
                {
                    End();
                    return;
                }

                if (updateTime > 0.0f && elapsedTime - preUpdateTime > updateTime)
                {
                    OnUpdateEvent?.Invoke(TimerID, elapsedTime, elapsedTime - preUpdateTime);
                    preUpdateTime = elapsedTime;
                }
            }
            else if (State == TimerState.Ready)
            {
                if (elapsedTime > lateStartTime)
                {
                    elapsedTime = 0.0f;
                    State = TimerState.Active;
                    OnStartEvent?.Invoke(TimerID);
                }
            }
        }
    }

    Dictionary<int, Timer> timerDictionary = new Dictionary<int, Timer>();
    List<Timer> inactiveTimerList = new List<Timer>();
    List<Timer> tmpTimerList = new List<Timer>();

    int timerNums = 0;

    public int ActPriority { get; } = 0;
    public void AwakeInitialize()
    {
        if (Instance != null && Instance != this)
        {
            Instance.timerDictionary.Clear();
            Instance.inactiveTimerList.Clear();
            Instance.tmpTimerList.Clear();
        }

        Instance = this;
        timerDictionary.Clear();
        inactiveTimerList.Clear();
        tmpTimerList.Clear();
    }

    public void LateAwakeInitialize()
    {

    }

    public int CreateTimer(float endTime, float updateTime = 999.0f, TimerOnceEventHandler startEvent = null, TimerUpdateEventHandler updateEvent = null, TimerOnceEventHandler endEvent = null, float lateStartTime = 0.0f)
    {
        Timer timer;

        if (inactiveTimerList.Count > 0)
        {
            timer = inactiveTimerList[0];
            inactiveTimerList.RemoveAt(0);
        }
        else
        {
            timer = new Timer();
        }

        timer.Initialize(timerNums, endTime, updateTime, startEvent, updateEvent, endEvent, lateStartTime);
        tmpTimerList.Add(timer);

        return timerNums++;
    }

    public bool StopTimer(int timerID)
    {
        timerDictionary.TryGetValue(timerID, out Timer timer);

        if (timer == null || timer.State == TimerState.Inactive)
        {
            return false;
        }

        timer.Stop();
        return true;
    }

    public bool RestartTimer(int timerID)
    {
        timerDictionary.TryGetValue(timerID, out Timer timer);

        if (timer == null || timer.State == TimerState.Inactive)
        {
            return false;
        }

        timer.Restart();
        return true;
    }

    public bool EndTimer(int timerID)
    {
        timerDictionary.TryGetValue(timerID, out Timer timer);

        if (timer == null || timer.State == TimerState.Inactive)
        {
            return false;
        }

        timer.End();
        return true;
    }

    public void Act()
    {
        foreach (Timer timer in tmpTimerList)
        {
            if (timer.State == TimerState.Ready)
            {
                timerDictionary.Add(timer.TimerID, timer);
            }
            else if (timer.State == TimerState.Inactive)
            {
                timerDictionary.Remove(timer.TimerID);
                inactiveTimerList.Add(timer);
            }
        }

        tmpTimerList.Clear();

        foreach (int timerID in timerDictionary.Keys)
        {
            Timer timer = timerDictionary[timerID];
            timer.OnUpdate(Time.deltaTime);

            if (timer.State == TimerState.Inactive)
            {
                tmpTimerList.Add(timer);
            }
        }
    }
}
