﻿using System.Diagnostics;

namespace EvDb.Core.Adapters;


[DebuggerDisplay("EventType:{EventType}, Offset:{Offset}, StreamId:{StreamId}")]
public struct EvDbEventRecord
{
    public Guid Id { get; init; }
    public string Domain { get; init; }
    public string Partition { get; init; }
    public string StreamId { get; init; }
    public long Offset { get; init; }
    public string EventType { get; init; }
    public string? TraceId { get; init; }
    public string? SpanId { get; init; }
    public byte[] Payload { get; init; }
    public string CapturedBy { get; init; }
    public DateTimeOffset CapturedAt { get; init; }

    #region Casting Overloads

    public static implicit operator EvDbEvent(EvDbEventRecord entity)
    {
        EvDbStreamCursor StreamCursor = new(
                                            entity.Domain,
                                            entity.Partition,
                                            entity.StreamId,
                                            entity.Offset);
        return new EvDbEvent(
                    entity.EventType,
                    entity.CapturedAt,
                    entity.CapturedBy,
                    StreamCursor,
                    entity.Payload);
    }

    public static implicit operator EvDbEventRecord(EvDbEvent e)
    {
        Activity? activity = Activity.Current;
        return new EvDbEventRecord
        {
            Id = Guid.NewGuid(), // TODO: GuidV7
            Domain = e.StreamCursor.Domain,
            Partition = e.StreamCursor.Partition,
            StreamId = e.StreamCursor.StreamId,
            Offset = e.StreamCursor.Offset,
            EventType = e.EventType,
            Payload = e.Payload,
            CapturedBy = e.CapturedBy,
            CapturedAt = e.CapturedAt,
            SpanId = activity?.SpanId.ToHexString(),
            TraceId = activity?.TraceId.ToHexString()
        };
    }

    #endregion //  Casting Overloads
}
