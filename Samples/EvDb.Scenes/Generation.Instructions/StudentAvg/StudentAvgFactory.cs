﻿using EvDb.Core;
using EvDb.Scenes;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;


namespace EvDb.UnitTests;

//[EvDbAggregateView<IEnumerable<StudentAvg>>]
//[EvDbAggregateView<IEnumerable<double>>]
//[EvDbAggregateFactory<IStudentFlowEventTypes>]
[EvDbAggregateFactory<IEnumerable<StudentAvg>, IStudentFlowEventTypes>]
public partial class StudentAvgFactory
{
    private readonly ConcurrentDictionary<int, StudentCalc> _students = new ();

    public StudentAvgFactory(IEvDbStorageAdapter storageAdapter) : base(storageAdapter)
    {
        var def = StudentFlowEventTypesContext.Default;
        JsonSerializerOptions = def.Options;
    }

    protected override IEnumerable<StudentAvg> DefaultState { get; } = [];

    public override string Kind { get; } = "student-avg";

    protected override JsonSerializerOptions? JsonSerializerOptions { get; }

    public override EvDbPartitionAddress Partition { get; } = new EvDbPartitionAddress("school-records", "students");

    protected override IEnumerable<StudentAvg> Fold(
        IEnumerable<StudentAvg> state,
        StudentEnlistedEvent enlisted,
        IEvDbEventMeta meta)
    {
        int id = enlisted.Student.Id;
        string name = enlisted.Student.Name;
        _students.TryAdd(id, 
            new StudentCalc(id, name, 0, 0));
        return state;
    }

    protected override IEnumerable<StudentAvg> Fold(
        IEnumerable<StudentAvg> state,
        StudentReceivedGradeEvent receivedGrade,
        IEvDbEventMeta meta)
    {
        if (!_students.TryGetValue(receivedGrade.StudentId, out StudentCalc entity))
            throw new Exception("It's broken");

        _students[receivedGrade.StudentId] = entity with 
                {
                    Count = entity.Count + 1, 
                    Sum = entity.Sum + receivedGrade.Grade,
                };


        var result = _students.Values
                            .Where(m => m.Count != 0)
                            .Select(m =>
                                new StudentAvg(m.StudentName, m.Sum / m.Count));
        return result.ToArray();
    }
}