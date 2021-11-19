// Copyright (c) 2021 - for information on the respective copyright owner
// see the NOTICE file and/or the repository github.com/boschresearch/assets2036net.
//
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;

namespace assets2036net
{
    public partial class AssetMgr : IDisposable
    {
        public List<Submodel> GetSupportedSubmodels(string @namespace, string name)
        {
            var submodels = new Dictionary<string, Submodel>();

            var mqttClient = new MqttClient(BrokerHost, BrokerPort, false, null, null, MqttSslProtocols.None);

            try
            {
                DateTime latest = DateTime.Now;

                mqttClient.MqttMsgPublishReceived += (object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e) =>
                {
                    latest = DateTime.Now;

                    var topic = new Topic(e.Topic);

                    string message = System.Text.Encoding.UTF8.GetString(e.Message);
                    var metaTag = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(message);

                    if (metaTag == null)
                    {
                        return; 
                    }

                    try
                    {
                        object oSchema;
                        if (!metaTag.TryGetValue(StringConstants.PropertyNameMetaSubmodelSchema, out oSchema))
                        {
                            return;
                        }

                        var stringSchema = Newtonsoft.Json.JsonConvert.SerializeObject(oSchema);
                        var schema = Newtonsoft.Json.JsonConvert.DeserializeObject<Submodel>(stringSchema);
                        schema.SubmodelUrl = metaTag["submodel_url"] as string;
                        submodels.Add(schema.Name, schema);
                    }
                    catch (Exception exc)
                    {
                        log.ErrorFormat("Error during parsing message at meta topic {0}: \\n {1}", e.Topic, exc.ToString());
                    }
                };

                mqttClient.Connect(string.Format("AssetMgr_Queries_{0}", this._endpointName));
                mqttClient.Subscribe(
                    new string[] { string.Format("{0}/{1}/+/_meta", @namespace, name) },
                    new byte[] { uPLibrary.Networking.M2Mqtt.Messages.MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                // wait until one second no new message arrived, then return what we have 
                do
                {
                    Thread.Sleep(10);
                }
                while (DateTime.Now.Subtract(latest) < TimeSpan.FromSeconds(3));
            }
            finally
            {
                //mqttClient.Disconnect(); 
            }

            return new List<Submodel>(submodels.Values);
        }

        public List<Tuple<string, string>> GetAvailableAssetNames()
        {
            var foundAssets = new Dictionary<string, HashSet<string>>();
            var mqttClient = new MqttClient(BrokerHost, BrokerPort, false, null, null, MqttSslProtocols.None);

            DateTime latest = DateTime.Now; 

            mqttClient.MqttMsgPublishReceived += (object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e) =>
            {
                latest = DateTime.Now; 

                var topic = new Topic(e.Topic);

                HashSet<string> hs;
                if (foundAssets.TryGetValue(topic.GetRootTopicName(), out hs))
                {
                    hs.Add(topic.GetAssetName()); 
                }
                else
                {
                    hs = new HashSet<string>();
                    hs.Add(topic.GetAssetName());
                    foundAssets.Add(topic.GetRootTopicName(), hs); 
                }
            };

            mqttClient.Connect(string.Format("AssetMgr_Queries_{0}", this._endpointName));
            mqttClient.Subscribe(
                new string[] { "+/+/+/_meta" },
                new byte[] { uPLibrary.Networking.M2Mqtt.Messages.MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 

            // wait until one second no new message arrived, then return what we have 
            do
            {
                Thread.Sleep(100); 
            }
            while (DateTime.Now.Subtract(latest) < TimeSpan.FromSeconds(1)); 

            var result = new List<Tuple<string, string>>();
            foreach(var kvp in foundAssets)
            {
                string ns = kvp.Key; 
                foreach(var assetName in kvp.Value)
                {
                    result.Add(Tuple.Create(ns, assetName)); 
                }
            }

            return result; 
        }

    }
}
