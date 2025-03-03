﻿using EvDb.Core;
using EvDb.StructuresValidation.Abstractions;
using EvDb.StructuresValidation.Abstractions.Events;

namespace EvDb.StructuresValidation.Repositories;

[EvDbMessageTypes<PersonChangedMultiChannelsMessage>]
[EvDbMessageTypes<PersonChangedMessage>]
[EvDbOutbox<CustomerEntityModelMultiChannelsStreamFactory>]
public partial class CustomerEntityModelMultiChannelsOutbox
{
    protected override void ProduceOutboxMessages(EmailValidatedEvent payload, IEvDbEventMeta meta, EvDbCustomerEntityModelMultiChannelsStreamViews views, CustomerEntityModelMultiChannelsOutboxContext outbox)
    {
        var personChanged = new PersonChangedMultiChannelsMessage
        {
            Id = meta.StreamCursor.StreamId,
            Email = payload.Email,
            EmailIsValid = payload.IsValid
        };
        outbox.Add(personChanged, PersonChangedMultiChannelsMessage.Channels.Channel1);
        outbox.Add(personChanged, PersonChangedMultiChannelsMessage.Channels.Channel2);
    }
}