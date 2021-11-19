// Copyright (c) 2021 - for information on the respective copyright owner
// see the NOTICE file and/or the repository github.com/boschresearch/assets2036net.
//
// SPDX-License-Identifier: Apache-2.0

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace assets2036net
{
    /// <summary>
    /// Class represents the payload of an event. Used for payload JSON serialization. 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SubmodelEventMessage : CommElementBase
    {
        /// <summary>
        /// The event's timestamp. 
        /// </summary>
        public DateTime Timestamp { get; internal set; }

        [JsonProperty("timestamp")]
        internal string TimestampeString
        {
            get
            {
                return Timestamp.ToString("s", System.Globalization.CultureInfo.InvariantCulture); 
            }
            set
            {
                Timestamp = DateTime.Parse(value); 
            }
        }

        /// <summary>
        /// The event's parameter values
        /// </summary>
        [JsonProperty("params")]
        public Dictionary<string, object> Parameters{ get; set; }
    }
}
