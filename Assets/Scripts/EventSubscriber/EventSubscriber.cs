using System;
using System.Collections.Generic;

public static class EventSubscriber<T>
{
    private static readonly Dictionary<GameEvent, Action<T>> Events = new();

    public static void Subscribe(GameEvent eventType, Action<T> callback)
    {
        if (callback == null) return;

        if (Events.TryGetValue(eventType, out var existing))
            Events[eventType] = existing + callback;
        else
            Events[eventType] = callback;
    }  
    


    public static void Unsubscribe(GameEvent eventType, Action<T> callback)
    {
        if (callback == null) return;

        if (Events.TryGetValue(eventType, out var existing))
        {
            existing -= callback;
            if (existing == null) Events.Remove(eventType);
            else Events[eventType] = existing;
        }
    } 
    


    public static void Publish(GameEvent eventType, T value)
    {
        if (Events.TryGetValue(eventType, out var callbacks))
            callbacks?.Invoke(value);
    }


    public static void ClearAll()
    {
        Events.Clear();
    }
}

public static class EventSubscriber
{
    private static readonly Dictionary<GameEvent, Action> Events = new();

    public static void Subscribe(GameEvent eventType, Action callback)
    {
        if (callback == null) return;

        if (Events.TryGetValue(eventType, out var existing))
            Events[eventType] = existing + callback;
        else
            Events[eventType] = callback;
    }  

    public static void Unsubscribe(GameEvent eventType, Action callback)
    {
        if (callback == null) return;

        if (Events.TryGetValue(eventType, out var existing))
        {
            existing -= callback;
            if (existing == null) Events.Remove(eventType);
            else Events[eventType] = existing;
        }
    } 
    

    public static void Publish(GameEvent eventType)
    {
        if (Events.TryGetValue(eventType, out var callbacks))
            callbacks?.Invoke();
    }



    public static void ClearAll()
    {
        Events.Clear();
    }
}
