using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eventualize.Core.Abstractions.Stream;

namespace Eventualize.Core;
public record EventualizeStoredEvent(string EventType,
DateTime CapturedAt,
string CapturedBy,
string JsonData,
DateTime StoredAt,
EventualizeStreamAddress StreamAddress,
long SequenceId)
: EventualizeEvent(EventType, CapturedAt, CapturedBy, JsonData)
{
}