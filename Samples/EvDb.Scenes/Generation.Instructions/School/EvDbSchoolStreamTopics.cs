﻿// Ignore Spelling: TopicProducer Topic

using EvDb.Core;
using EvDb.Scenes;

namespace EvDb.UnitTests;

[EvDbAttachTopicTables<TopicTables>]
[EvDbMessageTypes<AvgMessage>]
[EvDbMessageTypes<StudentPassedMessage>]
[EvDbMessageTypes<StudentFailedMessage>]
[EvDbTopics<SchoolStreamFactory>]
public partial class EvDbSchoolStreamTopics // TODO: MessageRouter / Outbox
{
    protected override TopicTablesPreferences[] TopicToTables(EvDbSchoolStreamTopicOptions topic) =>
        topic switch
        {
            EvDbSchoolStreamTopicOptions.Topic1 => [TopicTablesPreferences.Commands],
            EvDbSchoolStreamTopicOptions.Topic2 => [
                                                    TopicTablesPreferences.Messaging],
            EvDbSchoolStreamTopicOptions.Topic3 => [
                                                    TopicTablesPreferences.MessagingVip,
                                                    TopicTablesPreferences.Messaging],
            _ => []
        };

    protected override void ProduceTopicMessages(EvDb.Scenes.StudentReceivedGradeEvent payload,
                                                 IEvDbEventMeta meta,
                                                 EvDbSchoolStreamViews views,
                                                 EvDbSchoolStreamTopicsContext topics)
    {
        Stats state = views.ALL;
        AvgMessage avg = new(state.Sum / (double)state.Count);
        topics.Add(avg);
        var studentName = views.StudentStats.Students
            .First(m => m.StudentId == payload.StudentId)
            .StudentName;
        if (payload.Grade >= 60)
        {
            var pass = new StudentPassedMessage(payload.StudentId,
                                             studentName,
                                             meta.CapturedAt,
                                             payload.Grade);
            topics.Add(pass, TopicsOfStudentPassedMessage.Topic2);
            topics.Add(pass, TopicsOfStudentPassedMessage.Topic3);
        }
        else
        {
            var fail = new StudentFailedMessage(payload.StudentId,
                                             studentName,
                                             meta.CapturedAt,
                                             payload.Grade);
            topics.Add(fail);
        }
    }
}

