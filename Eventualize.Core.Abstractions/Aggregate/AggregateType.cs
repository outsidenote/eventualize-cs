﻿namespace Eventualize.Core;


public class AggregateType<StateType> where StateType : notnull, new()
{
    // [bnaya 2023-12-11] Consider what is the right data type (thread safe)
    public Dictionary<string, EventType> RegisteredEventTypes { get; private set; } = new();

    // [bnaya 2023-12-11] Consider what is the right data type (thread safe)
    public Dictionary<string, IFoldingFunction<StateType>> FoldingLogic = new();

    public readonly string Name;

    public readonly int MinEventsBetweenSnapshots;

    public AggregateType(string name)
    {
        Name = name;
        MinEventsBetweenSnapshots = 0;
    }

    public AggregateType(string name, int minEventsBetweenSnapshots)
    {
        Name = name;
        MinEventsBetweenSnapshots = minEventsBetweenSnapshots;
    }

    public Aggregate<StateType> CreateAggregate(string id)
    {
        return new Aggregate<StateType>(this, id, MinEventsBetweenSnapshots);
    }

    public Aggregate<StateType> CreateAggregate(string id, List<EventEntity> events)
    {
        return new Aggregate<StateType>(this, id, MinEventsBetweenSnapshots, events);
    }

    public Aggregate<StateType> CreateAggregate(string id, StateType snapshot, long lastStoredSequenceId, List<EventEntity> events)
    {
        return new Aggregate<StateType>(this, id, MinEventsBetweenSnapshots, snapshot, lastStoredSequenceId, events);
    }

    public void AddEventType(EventType eventType)
    {
        RegisteredEventTypes.Add(eventType.EventTypeName, eventType);
    }

    public void AddFoldingFunction(string eventTypeName, IFoldingFunction<StateType> foldingFunction)
    {
        EventType? eventType;
        if (!RegisteredEventTypes.TryGetValue(eventTypeName, out eventType))
            throw new KeyNotFoundException($"Event type name {eventTypeName} was not found.");
        FoldingLogic.Add(eventTypeName, foldingFunction);

    }

    public void AddEventType(EventType eventType, IFoldingFunction<StateType> foldingFunction)
    {
        AddEventType(eventType);
        AddFoldingFunction(eventType.EventTypeName, foldingFunction);
    }

    public StateType FoldEvents(StateType oldState, IEnumerable<EventEntity> events)
    {
        StateType currentState = oldState;
        foreach (var e in events)
        {
            currentState = FoldEvent(currentState, e);
        }
        return currentState;
    }

    public StateType FoldEvent(StateType oldState, EventEntity someEvent)
    {
        StateType currentState = oldState;
        IFoldingFunction<StateType>? foldingFunction;
        if (!FoldingLogic.TryGetValue(someEvent.EventType, out foldingFunction)) throw new ArgumentNullException(nameof(someEvent));
        currentState = foldingFunction.Fold(currentState, someEvent);
        return currentState;
    }
}
