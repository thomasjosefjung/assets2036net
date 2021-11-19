// Copyright (c) 2021 - for information on the respective copyright owner
// see the NOTICE file and/or the repository github.com/boschresearch/assets2036net.
//
// SPDX-License-Identifier: Apache-2.0

using log4net;
using log4net.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace assets2036net
{
    public static class Tools
    {
        /// <summary>
        /// Helper method to erase all retained messages produced by a specific asset. 
        /// </summary>
        /// <param name="host">host name of the MQTT broker</param>
        /// <param name="port">port of the MQTT broker (typical: 1883)</param>
        /// <param name="namespace">asset's namespace</param>
        /// <param name="name">asset's name</param>
        public static void RemoveAssetTrace(string host, int port, string @namespace, string name)
        {
            var client = new MqttClient(host, port, false, null, null, MqttSslProtocols.None);

            try
            {
                DateTime latest = DateTime.Now;

                string topic = string.Format("{0}/{1}/#", @namespace, name);

                var topicsToDelete = new List<string>();

                client.MqttMsgPublishReceived += (object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e) =>
                {
                    if (e.Message.Length > 0)
                    {
                        latest = DateTime.Now;
                        if (e.Retain)
                        {
                            topicsToDelete.Add(e.Topic);
                        }
                    }
                };

                client.Connect("assets2036net");
                client.Subscribe(new string[] { topic }, new byte[] { 2 });

                while (DateTime.Now.Subtract(latest) <= TimeSpan.FromSeconds(1))
                {
                    System.Threading.Thread.Sleep(10);
                }

                ConcurrentDictionary<ushort, byte> ids = new ConcurrentDictionary<ushort, byte>();

                client.MqttMsgPublished += (object sender, MqttMsgPublishedEventArgs e) => 
                {
                    byte value; 
                    ids.TryRemove(e.MessageId, out value); 
                };

                foreach (var t in topicsToDelete)
                {
                    ids.TryAdd(client.Publish(t, new byte[] { }, 2, true), 0);
                }

                while (ids.Count > 0)
                {
                    System.Threading.Thread.Sleep(100); 
                }
            }
            finally
            {
                client.Disconnect(); 
            }
        }

        /// <summary>
        /// Helper method to clean all! retained messages matching the given root topic 
        /// from the broker (e.g. [mynamespace/myAsset]. Use with care!!! All retained 
        /// message at the topics mynamespace/myAsset/# will be reset. 
        /// </summary>
        /// <param name="broker">hostname of the MQTT broker</param>
        /// <param name="port">port of the MQTT Broker. Typical: 1883</param>
        /// <param name="rootTopic"></param>
        public static void CleanAllRetainedMessages(string broker, int port, string rootTopic)
        {
            _broker = broker;
            _port = port;
            _rootTopic = rootTopic;

            _mqttClient = new MqttClient(broker, port, false, null, null, MqttSslProtocols.None);
            _mqttClient.MqttMsgPublishReceived += _mqttClient_MqttMsgPublishReceived;

            _mqttClient.Subscribe(new string[] { rootTopic+"/#" }, new byte[] { 2 });
            _mqttClient.Connect(Guid.NewGuid().ToString());
        }



        private static void _mqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Retain)
            {
                log.Info("CLEAN: " + e.Topic);
                bool sent = false; 
                try
                {
                    while (!sent)
                    {
                        _mqttClient.Publish(e.Topic, new byte[0], 2, true);
                        sent = true;
                    }
                }
                catch(Exception)
                {
                    _mqttClient.Disconnect();

                    _mqttClient = new MqttClient(_broker, _port, false, null, null, MqttSslProtocols.None);
                    _mqttClient.MqttMsgPublishReceived += _mqttClient_MqttMsgPublishReceived;

                    _mqttClient.Subscribe(new string[] { _rootTopic+"/#" }, new byte[] { 2 });
                    _mqttClient.Connect(Guid.NewGuid().ToString());
                }
            }
        }

        private static log4net.ILog log = Config.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        private static string _broker;
        private static int _port;

        private static MqttClient _mqttClient;
        private static string _rootTopic;
    }
}
