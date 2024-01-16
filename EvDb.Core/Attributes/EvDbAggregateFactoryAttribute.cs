﻿namespace EvDb.Core;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EvDbAggregateFactoryAttribute<TState, TEventType> : Attribute
    where TEventType : IEvDbEventTypes
{
}