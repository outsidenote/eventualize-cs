﻿using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using System.Transactions;

// TODO [bnaya 2023-12-13] consider to encapsulate snapshot object with Snapshot<T> which is a wrapper of T that holds T and snapshotOffset 

namespace EvDb.Core;


//[DebuggerDisplay("")]
public class EvDbStream :
        IEvDbStreamStore,
        IEvDbStreamStoreData
{
    // [bnaya 2023-12-11] Consider what is the right data type (thread safe)
    protected internal IImmutableList<IEvDbEvent> _pendingEvents = ImmutableList<IEvDbEvent>.Empty;

    protected static readonly SemaphoreSlim _dirtyLock = new SemaphoreSlim(1);
    private static readonly AssemblyName ASSEMBLY_NAME = Assembly.GetExecutingAssembly()?.GetName() ?? throw new NotSupportedException("GetExecutingAssembly");
    private static readonly string DEFAULT_CAPTURE_BY = $"{ASSEMBLY_NAME.Name}-{ASSEMBLY_NAME.Version}";

    #region Ctor

    public EvDbStream(
        IEvDbStreamConfig factory,
        IImmutableList<IEvDbView> views,
        IEvDbStorageAdapter storageAdapter,
        string streamId,
        long lastStoredOffset)
    {
        _views = views;
        _storageAdapter = storageAdapter;
        StreamAddress = new EvDbStreamAddress(factory.PartitionAddress, streamId);
        LastStoredOffset = lastStoredOffset;
        Options = factory.JsonSerializerOptions;
    }


    #endregion // Ctor

    #region Views

    protected IImmutableList<IEvDbView> _views;
    IEnumerable<IEvDbView> IEvDbStreamStoreData.Views => _views;

    private readonly IEvDbStorageAdapter _storageAdapter;

    #endregion // Views

    #region GetNextOffset

    private EvDbStreamCursor GetNextOffset()
    {
        if (_pendingEvents.Count == 0)
        { 
            var empty = new EvDbStreamCursor(StreamAddress, 0);
            return empty;
        }
        IEvDbEvent e = _pendingEvents[_pendingEvents.Count - 1];
        long offset = e.StreamCursor.Offset;
        var result = new EvDbStreamCursor(StreamAddress, offset + 1);
        return result;
    }

    #endregion // GetNextOffset

    #region AddEvent

    protected void AddEvent<T>(T payload, string? capturedBy = null)
        where T : IEvDbEventPayload
    {
        capturedBy = capturedBy ?? DEFAULT_CAPTURE_BY;
        var json = JsonSerializer.Serialize(payload, Options);
        try
        {
            EvDbStreamCursor cursor = GetNextOffset();
            IEvDbEvent e = new EvDbEvent(payload.EventType, DateTime.UtcNow, capturedBy, cursor, json);
            _dirtyLock.Wait(); // TODO: [bnaya 2024-01-09] re-consider the lock solution (ToImmutable?, custom object with length and state [hopefully immutable] that implement IEnumerable)
            _pendingEvents = _pendingEvents.Add(e); 

            foreach (IEvDbView folding in _views)
                folding.FoldEvent(e);
        }
        finally
        {
            _dirtyLock.Release();
        }
    }

    #endregion // AddEvent

    public void ClearLocalEvents()
    {
        LastStoredOffset += _pendingEvents.Count;
        _pendingEvents.Clear();
    }

    async Task IEvDbStreamStore.SaveAsync(CancellationToken cancellation)
    {
        IEvDbStreamStoreData self = this;
        try
        {
            await _dirtyLock.WaitAsync();// TODO: [bnaya 2024-01-09] re-consider the lock solution
            {
                if (self.IsEmpty)
                {
                    await Task.FromResult(true);
                    return;
                }

                await _storageAdapter.SaveAsync(this, cancellation);
                IEvDbEvent ev = _pendingEvents[_pendingEvents.Count - 1];
                LastStoredOffset = ev.StreamCursor.Offset;
                _pendingEvents = ImmutableList<IEvDbEvent>.Empty;
                foreach (var view in _views)
                {
                    view.OnSaved();
                }
            }
        }
        finally
        {
            _dirtyLock.Release();
        }
    }

    #region StreamId

    public EvDbStreamAddress StreamAddress { get; init; }

    #endregion // StreamAddress

    public int EventsCount => _pendingEvents.Count;

    #region LastStoredOffset

    public long LastStoredOffset { get; protected set; } = -1;

    #endregion // LastStoredOffset

    #region IsEmpty

    bool IEvDbStreamStoreData.IsEmpty => _pendingEvents.Count == 0;

    #endregion // IsEmpty

    public JsonSerializerOptions? Options { get; }

    IEnumerable<IEvDbEvent> IEvDbStreamStoreData.Events => _pendingEvents;
}

