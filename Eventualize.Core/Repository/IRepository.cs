﻿
namespace Eventualize.Core;

public interface IRepository
{
    Task<EventualizeAggregate<T>> GetAsync<T>(EventualizeAggregateFactory<T> aggregateFactory, string streamId, CancellationToken cancellation = default) where T : notnull, new();
    Task SaveAsync<T>(EventualizeAggregate<T> aggregate, CancellationToken cancellation = default) where T : notnull, new();
}