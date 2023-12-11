using Eventualize.Core;
using Eventualize.Core.StorageAdapters.SQLServerStorageAdapter;
using Eventualize.Core.Tests;
using System.Data;
using static Eventualize.Core.Tests.TestHelper;

namespace CoreTests.StorageAdapterTests.SQLServerStorageAdapterTests
{
    public static class SQLServerStorageAdapterTestsSteps
    {
        public static async Task<Aggregate<TestState>> StoreAggregateTwice(SQLServerStorageAdapter storageAdapter)
        {
            var aggregate = await PrepareAggregateWithPendingEvents();
            await storageAdapter.SaveAsync(aggregate, true);
            var aggregate2 = new Aggregate<TestState>(aggregate.AggregateType, aggregate.Id, aggregate.MinEventsBetweenSnapshots, aggregate.PendingEvents);
            foreach (var pendingEvet in aggregate.PendingEvents)
                aggregate2.AddPendingEvent(pendingEvet);
            await storageAdapter.SaveAsync(aggregate2, true);
            return aggregate2;
        }

        public static void AssertEventsAreEqual(List<EventEntity> events1, List<EventEntity> events2)
        {
            Assert.Equal(events1.Count, events2.Count);
            events1.Select((e, index) =>
            {
                Assert.Equal(e, events2[index]);
                return true;
            });
        }
    }

}