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
    /// Represents a parameter of a SubmodelOperation and of SubmodelEvents. Used only 
    /// for JSON serialization of submodel descriptions. Usually not needed when implementing 
    /// submodel providers or proxies. 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Parameter
    {
        /// <summary>
        /// The parameter type. Allowed are the JSON schema types. 
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Name of the parameter. 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
