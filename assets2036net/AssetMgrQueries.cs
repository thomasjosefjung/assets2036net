﻿//// Copyright (c) 2021 - for information on the respective copyright owner
//// see the NOTICE file and/or the repository github.com/boschresearch/assets2036net.
////
//// SPDX-License-Identifier: Apache-2.0

//using System;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace assets2036net
{
    public partial class AssetMgr : IDisposable
    {
        public List<Submodel> GetSupportedSubmodels(string @namespace, string name)
        {
            var submodels = new Dictionary<string, Submodel>();

            var factory = new MqttFactory();

            using (var mqttClient = factory.CreateMqttClient())
            {
                DateTime latest = DateTime.Now;

                mqttClient.ApplicationMessageReceivedAsync += (MqttApplicationMessageReceivedEventArgs eventArgs) => 
                {
                    latest = DateTime.Now;
                    var topic = eventArgs.ApplicationMessage.Topic;

                    string message = System.Text.Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment.Array);

                    var metaTag = System.Text.Json.JsonSerializer.Deserialize<MetaPropertyValue>(
                        message, 
                        Tools.JsonSerializerOptions);

                    if (metaTag == null)
                    {
                        return Task.CompletedTask;
                    }

                    try
                    {
                        if (metaTag.SubmodelDefinition == null)
                        // if (!metaTag.TryGetValue(StringConstants.PropertyNameMetaSubmodelSchema, out object oSchema))
                        {
                            return Task.CompletedTask;
                        }

                        var stringSchema = JsonSerializer.Serialize(
                            metaTag.SubmodelDefinition, 
                            Tools.JsonSerializerOptions);

                        submodels.Add(metaTag.SubmodelDefinition.Name, metaTag.SubmodelDefinition);
                    }
                    catch (Exception exc)
                    {
                        log.ErrorFormat("Error during parsing message at meta topic {0}: \\n {1}", topic, exc.ToString());
                    }


                    return Task.CompletedTask;
                }; 

                mqttClient.ConnectedAsync += (MqttClientConnectedEventArgs evtArgs) => 
                {
                    var topics = new MqttClientSubscribeOptionsBuilder()
                        .WithTopicFilter(new MqttTopicFilter()
                        {
                            Topic = string.Format("{0}/{1}/+/_meta", @namespace, name),
                            QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce
                        });

                    return mqttClient.SubscribeAsync(topics.Build(), CancellationToken.None);
                };

                var options = new MqttClientOptionsBuilder()
                    .WithClientId(Guid.NewGuid().ToString())
                    .WithTcpServer(BrokerHost, BrokerPort)
                    .WithCleanSession();

                mqttClient.ConnectAsync(options.Build(), CancellationToken.None).Wait();

                // wait until three second no new message arrived, then return what we have 
                do
                {
                    Thread.Sleep(10);
                }
                while (DateTime.Now.Subtract(latest) < TimeSpan.FromSeconds(1));

                return new List<Submodel>(submodels.Values);
            }
        }

        public List<Tuple<string, string>> GetAvailableAssetNames()
        {
            var foundAssets = new Dictionary<string, HashSet<string>>();
            DateTime latest = DateTime.Now;

            var factory = new MqttFactory();
            using (var mqttClient = factory.CreateMqttClient())
            {

                mqttClient.ApplicationMessageReceivedAsync += (MqttApplicationMessageReceivedEventArgs eventArgs) => 
                {
                    latest = DateTime.Now;

                    var topic = new Topic(eventArgs.ApplicationMessage.Topic);

                    if (foundAssets.TryGetValue(topic.GetRootTopicName(), out HashSet<string> hs))
                    {
                        hs.Add(topic.GetAssetName());
                    }
                    else
                    {
                        hs = new HashSet<string>
                        {
                            topic.GetAssetName()
                        };
                        foundAssets.Add(topic.GetRootTopicName(), hs);
                    }

                    return Task.CompletedTask;
                };

                mqttClient.ConnectedAsync += (MqttClientConnectedEventArgs evtArgs) => 
                {
                    var topics = new MqttClientSubscribeOptionsBuilder()
                        .WithTopicFilter(new MqttTopicFilter()
                        {
                            Topic = "+/+/+/_meta",
                            QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce
                        });

                    return mqttClient.SubscribeAsync(topics.Build(), CancellationToken.None);
                };

                var options = new MqttClientOptionsBuilder()
                    .WithClientId(Guid.NewGuid().ToString())
                    .WithTcpServer(BrokerHost, BrokerPort)
                    .WithCleanSession();

                mqttClient.ConnectAsync(options.Build(), CancellationToken.None).Wait();

                // wait until three second no new message arrived, then return what we have 
                do
                {
                    Thread.Sleep(10);
                }
                while (DateTime.Now.Subtract(latest) < TimeSpan.FromSeconds(1));

                var result = new List<Tuple<string, string>>();
                foreach (var kvp in foundAssets)
                {
                    string ns = kvp.Key;
                    foreach (var assetName in kvp.Value)
                    {
                        result.Add(Tuple.Create(ns, assetName));
                    }
                }

                return result;
            }
        }
    }
}
