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
    public class CommElementBase
    {
        public AssetMgr AssetMgr { get; internal set; }

        public Asset Asset { get; internal set; }

        public Submodel Submodel { get; internal set; }

        internal virtual void populate(AssetMgr mgr, Asset asset, Submodel submodel)
        {
            AssetMgr = mgr;
            Asset = asset;
            Submodel = submodel;
        }

        internal static string buildTopic(params string[] elements)
        {
            return string.Join("/", elements);
        }
    }
}
