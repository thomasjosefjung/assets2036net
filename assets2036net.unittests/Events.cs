// Copyright (c) 2021 - for information on the respective copyright owner
// see the NOTICE file and/or the repository github.com/boschresearch/assets2036net.
//
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Xunit;

namespace assets2036net.unittests
{
    public class Events: UnitTestBase
    {
        [Fact]
        public void EventWithComplexParameters()
        {
            string location = this.GetType().Assembly.Location;
            location = Path.GetDirectoryName(location);
            location = Path.Combine(location, "resources/events.json");
            Uri uri = new Uri(location);

            AssetMgr mgrOwner = new AssetMgr(Settings.BrokerHost, Settings.BrokerPort, Settings.RootTopic, Settings.EndpointName);
            AssetMgr mgrConsumer = new AssetMgr(Settings.BrokerHost, Settings.BrokerPort, Settings.RootTopic, Settings.EndpointName);

            Asset assetOwner = mgrOwner.CreateAsset(Settings.RootTopic, "EventWithComplexParameters", uri);
            Asset assetConsumer = mgrConsumer.CreateAssetProxy(Settings.RootTopic, "EventWithComplexParameters", uri);

            // bind local eventlistener to event
            assetConsumer.Submodel("events").Event("anevent").Emission += this.handleEvent; 

            this.a = null;
            this.b = null;
            this.c = null;

            object a = "Parameter_a";
            object b = "Parameter_b";
            object c = new Newtonsoft.Json.Linq.JObject()
            {
                {"eins", 1 },
                {"zwei", 2 }, 
                {"drei", 3 }, 
                {"vier", "4" }
            };

            Thread.Sleep(1000);

            assetOwner.Submodel("events").Event("anevent").Emit(new Dictionary<string, object>()
            {
                {"aaa", a },
                {"bbb", b },
                {"objectparameter", c }
            });

            //Thread.Sleep(Settings.WaitTime);

            Assert.True(waitForCondition(() =>
            {
                return a.Equals(this.a);
            }, Settings.WaitTime));

            Assert.True(waitForCondition(() =>
            {
                return b.Equals(this.b);
            }, Settings.WaitTime));

            Assert.True(waitForCondition(() =>
            {
                if (this.c == null)
                {
                    return false; 
                }

                return c.ToString().Equals(this.c.ToString());
            }, Settings.WaitTime));

        }

        object a = null;
        object b = null;
        object c = null;

        private void handleEvent(SubmodelEventMessage emission)
        {
            this.a = emission.Parameters["aaa"];
            this.b = emission.Parameters["bbb"];
            this.c = emission.Parameters["objectparameter"];
        }
    }
}
