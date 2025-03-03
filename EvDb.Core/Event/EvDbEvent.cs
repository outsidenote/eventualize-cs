﻿using Generator.Equals;
using System.Diagnostics;
using System.Text.Json;

namespace EvDb.Core;

[Equatable]
[DebuggerDisplay("{EventType}: {Payload}")]
public partial record struct EvDbEvent(string EventType,
                                [property: IgnoreEquality] DateTimeOffset CapturedAt,
                                string CapturedBy,
                                EvDbStreamCursor StreamCursor,
                                byte[] Payload) :
                                            IEvDbEventConverter,
                                            IEvDbEventMeta
{
    public static readonly EvDbEvent Empty = new EvDbEvent();

    T IEvDbEventConverter.GetData<T>(JsonSerializerOptions? options)
    {
        var json = JsonSerializer.Deserialize<T>(Payload, options) ?? throw new InvalidCastException(typeof(T).Name);
        return json;
    }
}
