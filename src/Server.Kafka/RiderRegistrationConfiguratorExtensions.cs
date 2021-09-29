﻿using System;
using Confluent.Kafka;
using GreenPipes;
using MassTransit;
using MassTransit.Registration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Server.Kafka
{
    public static class RiderRegistrationConfiguratorExtensions
    {
        public static void UsingKafka(this IRiderRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<KafkaMessageConsumer>();
            configurator.UsingKafka((context, factoryConfigurator) =>
            {
                var kafkaOptions = context.GetRequiredService<IOptions<KafkaOptions>>().Value;
                factoryConfigurator.SecurityProtocol = SecurityProtocol.SaslPlaintext;
                factoryConfigurator.Host(kafkaOptions.Host, hostConfigurator =>
                    hostConfigurator.UseSasl(saslConfigurator =>
                    {
                        saslConfigurator.Mechanism = SaslMechanism.ScramSha512;
                        saslConfigurator.Username = kafkaOptions.UserName;
                        saslConfigurator.Password = kafkaOptions.Password;
                    }));

                factoryConfigurator.TopicEndpoint<string, KafkaMessage>(kafkaOptions.Topic, "group-1",
                    e =>
                    {
                        e.AutoOffsetReset = AutoOffsetReset.Earliest;
                        e.SetValueDeserializer(new KafkaMessageDeserializer());
                        e.ConfigureConsumer<KafkaMessageConsumer>(context);
                        e.SetOffsetsCommittedHandler(OffsetsCommittedHandler(context));
                        e.UseScheduledRedelivery(c => c.Incremental(144, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10)));
                        e.UseMessageRetry(c => c.Immediate(3));
                    });
            });
        }

        private static Action<IConsumer<string, KafkaMessage>, CommittedOffsets> OffsetsCommittedHandler(IConfigurationServiceProvider context)
        {
            return (_, offsets) =>
            {
                var logger = context.GetRequiredService<ILoggerFactory>().CreateLogger("SetOffsetsCommittedHandler");
                logger.LogInformation("Offsets.Count: {Count}", offsets.Offsets.Count);
            };
        }
    }
}