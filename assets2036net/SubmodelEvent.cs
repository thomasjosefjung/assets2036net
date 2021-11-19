// Copyright (c) 2021 - for information on the respective copyright owner
// see the NOTICE file and/or the repository github.com/boschresearch/assets2036net.
//
// SPDX-License-Identifier: Apache-2.0

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using uPLibrary.Networking.M2Mqtt;

namespace assets2036net
{
    /// <summary>
    /// The SubmodelElement representing an event. 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SubmodelEvent : SubmodelElement
    {
        private static log4net.ILog log = Config.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        internal SubmodelEvent()
        {
        }

        private Dictionary<string, Parameter> _parameters; 

        /// <summary>
        /// The event parameters
        /// </summary>
        [JsonProperty("parameters")]
        public Dictionary<string, Parameter> Parameters
        {
            get
            {
                return _parameters; 
            }
            internal set
            {
                _parameters = value; 
                foreach(var kvp in _parameters)
                {
                    kvp.Value.Name = kvp.Key; 
                }
            }
        }

        /// <summary>
        /// Used by the submodel provider to actually emit the event via MQTT. 
        /// </summary>
        /// <param name="parameters">The event parameter values. </param>
        public void Emit(Dictionary<string, object> parameters)
        {
            var emission = new SubmodelEventMessage()
            {
                Timestamp = DateTime.Now,
                Parameters = parameters
            };

            Asset.publish(
                Topic,      
                JsonConvert.SerializeObject(emission),
                false);
        }


        /// <summary>
        /// To be used by the submodel consumer to listen to this event. 
        /// </summary>
        public event SubmodelEventListener Emission; 

        internal void EmitEmission(SubmodelEventMessage emission)
        {
            Emission?.Invoke(emission); 
        }

        internal override void createSubscriptions(MqttClient mqttClient, Mode mode)
        {
            if (mode == Mode.Consumer)
            {
                var topic = Topic;
                log.InfoFormat("{0} subscribes to {1}", Name, topic);
                mqttClient.Subscribe(
                    new string[] { topic },
                    new byte[] { uPLibrary.Networking.M2Mqtt.Messages.MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
        }
    }
}
