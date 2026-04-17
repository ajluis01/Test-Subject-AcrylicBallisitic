using System;
using System.Collections.Generic;

// A simple Event System that can be used for remote systems communication
public static class EventManager
{
    static readonly Dictionary<Type, Action<GameEvent>> events = new Dictionary<Type, Action<GameEvent>>();
    static readonly Dictionary<Delegate, Action<GameEvent>> eventLookUps = new Dictionary<Delegate, Action<GameEvent>>();

    public static void AddListener<T>(Action<T> evt) where T : GameEvent
    {
        if (!eventLookUps.ContainsKey(evt))
        {
            Action<GameEvent> newAction = (e) => evt((T)e);
            eventLookUps[evt] = newAction;

            if (events.TryGetValue(typeof(T), out Action<GameEvent> internalAction))
                events[typeof(T)] = internalAction += newAction;
            else
                events[typeof(T)] = newAction;
        }
    }

    public static void RemoveListener<T>(Action<T> evt) where T : GameEvent
    {
        if (eventLookUps.TryGetValue(evt, out var action))
        {
            if (events.TryGetValue(typeof(T), out var tempAction))
            {
                tempAction -= action;
                if (tempAction == null)
                    events.Remove(typeof(T));
                else
                    events[typeof(T)] = tempAction;
            }

            eventLookUps.Remove(evt);
        }
    }

    public static void Broadcast(GameEvent evt)
    {
        if (events.TryGetValue(evt.GetType(), out var action))
            action.Invoke(evt);
    }

    public static void Clear()
    {
        events.Clear();
        eventLookUps.Clear();
    }
}