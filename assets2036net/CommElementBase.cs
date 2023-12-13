﻿// Copyright (c) 2021 - for information on the respective copyright owner
// see the NOTICE file and/or the repository github.com/boschresearch/assets2036net.
//
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;

namespace assets2036net
{
    /// <summary>
    /// Base class for submodel elements AND message classes like SubmodelOperationRequest, SubmodelEventMessage and others. 
    /// Offers access to the related <seealso cref="Asset" />, <seealso cref="AssetMgr" /> and <seealso cref="Submodel" /> instances. 
    /// </summary>
    public class CommElementBase
    {
        /// <summary>
        /// The related AssetMgr instance. 
        /// </summary>
        [JsonIgnore]
        public AssetMgr AssetMgr { get; internal set; }

        /// <summary>
        /// The related Asset instance. 
        /// </summary>
        [JsonIgnore]
        public Asset Asset { get; internal set; }

        /// <summary>
        /// The related Submodel instance. 
        /// </summary>
        [JsonIgnore]
        public Submodel Submodel { get; internal set; }

        internal virtual void Populate(AssetMgr mgr, Asset asset, Submodel submodel)
        {
            AssetMgr = mgr;
            Asset = asset;
            Submodel = submodel;
        }

        internal static string BuildTopic(params string[] elements)
        {
            return string.Join("/", elements);
        }
    }
}
