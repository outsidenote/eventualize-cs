namespace EvDb.Core.Tests;

using EvDb.Scenes;
using EvDb.UnitTests;
using Xunit.Abstractions;

public class SqlServerStreamTests : IntegrationTests
{
    public SqlServerStreamTests(ITestOutputHelper output) :
        base(output, StoreType.SqlServer)
    {
    }

    [Fact]
    public async Task Stream_WhenStoringWithoutSnapshotting_Succeed()
    {
        IEvDbSchoolStream stream = await _storageAdapter
                            .GivenLocalStreamWithPendingEvents(_output)
                            .WhenStreamIsSavedAsync();

        ThenStreamSavedWithoutSnapshot();

        void ThenStreamSavedWithoutSnapshot()
        {
            Assert.Equal(3, stream.StoreOffset);
            Assert.All(stream.Views.ToMetadata(), v => Assert.Equal(-1, v.StoreOffset));
            Assert.All(stream.Views.ToMetadata(), v => Assert.Equal(3, v.FoldOffset));

            Assert.Equal(180, stream.Views.ALL.Sum);
            Assert.Equal(3, stream.Views.ALL.Count);
            Assert.Single(stream.Views.StudentStats.Students);

            StudentStats studentStat = stream.Views.StudentStats.Students[0];
            var student = Steps.CreateStudentEntity();
            Assert.Equal(student.Id, studentStat.StudentId);
            Assert.Equal(student.Name, studentStat.StudentName);
            Assert.Equal(180, studentStat.Sum);
            Assert.Equal(3, studentStat.Count);
        }
    }

    [Fact]
    public async Task Stream_WhenStoringWithSnapshotting_Succeed()
    {
        IEvDbSchoolStream stream = await _storageAdapter
                            .GivenLocalStreamWithPendingEvents(_output, 6)
                            .WhenStreamIsSavedAsync();

        ThenStreamSavedWithSnapshot(stream);

        void ThenStreamSavedWithSnapshot(IEvDbSchoolStream aggregate)
        {
            Assert.Equal(6, stream.StoreOffset);
            Assert.All(stream.Views.ToMetadata(), v => Assert.Equal(6, v.StoreOffset));
            Assert.All(stream.Views.ToMetadata(), v => Assert.Equal(6, v.FoldOffset));

            Assert.Equal(630, stream.Views.ALL.Sum);
            Assert.Equal(6, stream.Views.ALL.Count);
            Assert.Single(stream.Views.StudentStats.Students);

            StudentStats studentStat = stream.Views.StudentStats.Students[0];
            var student = Steps.CreateStudentEntity();
            Assert.Equal(student.Id, studentStat.StudentId);
            Assert.Equal(student.Name, studentStat.StudentName);
            Assert.Equal(630, studentStat.Sum);
            Assert.Equal(6, studentStat.Count);
        }
    }

    [Fact]
    public async Task Stream_WhenStoringWithSnapshottingWhenStoringTwice_Succeed()
    {
        IEvDbSchoolStream stream = await _storageAdapter
                            .GivenLocalStreamWithPendingEvents(_output)
                            .GivenStreamIsSavedAsync()
                            .GivenAddingPendingEventsAsync()
                            .WhenStreamIsSavedAsync();

        ThenStreamSavedWithSnapshot(stream);

        void ThenStreamSavedWithSnapshot(IEvDbSchoolStream aggregate)
        {
            Assert.Equal(6, stream.StoreOffset);
            Assert.All(stream.Views.ToMetadata(), v => Assert.Equal(6, v.StoreOffset));
            Assert.All(stream.Views.ToMetadata(), v => Assert.Equal(6, v.FoldOffset));

            Assert.Equal(360, stream.Views.ALL.Sum);
            Assert.Equal(6, stream.Views.ALL.Count);
            Assert.Single(stream.Views.StudentStats.Students);

            StudentStats studentStat = stream.Views.StudentStats.Students[0];
            var student = Steps.CreateStudentEntity();
            Assert.Equal(student.Id, studentStat.StudentId);
            Assert.Equal(student.Name, studentStat.StudentName);
            Assert.Equal(360, studentStat.Sum);
            Assert.Equal(6, studentStat.Count);
        }
    }


    [Fact(Skip = "OCC state should be generated")]
    public async Task Stream_WhenStoringStaleStream_ThrowException()
    {
        throw new NotImplementedException();
        //IEvDbSchoolStream stream = _storageAdapter
        //            .GivenStreamWithStaleEvents(_output);

        //await Assert.ThrowsAsync<OCCException>(async () => await stream.SaveAsync(default));
    }
}