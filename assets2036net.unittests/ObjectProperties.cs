﻿// Copyright (c) 2021 - for information on the respective copyright owner
// see the NOTICE file and/or the repository github.com/boschresearch/assets2036net.
//
// SPDX-License-Identifier: Apache-2.0

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Xunit;

namespace assets2036net.unittests
{
    public class ComplexType
    {
        public string name { get; set; }
        public float x { get; set; }
        public float y { get; set; }
    }

    public class ObjectProperties : UnitTestBase
    {
        [Fact]
        public void WriteAndReadObject()
        {
            string location = this.GetType().Assembly.Location;
            location = Path.GetDirectoryName(location);
            location = Path.Combine(location, "resources/properties.json");
            Uri uri = new Uri(location);


            using AssetMgr mgr = new AssetMgr(Settings.BrokerHost, Settings.BrokerPort, Settings.Namespace, Settings.EndpointName);

            using Asset assetOwner = mgr.CreateAsset(Settings.Namespace, "ObjectProperties", uri);
            using Asset assetConsumer = mgr.CreateAssetProxy(Settings.Namespace, "ObjectProperties", uri);

            var complexValue = new Dictionary<string, object>()
            {
                {"name", "Testname" },
                {"x", 77.77},
                {"y", 99.99},
                {"encaps_object", new Dictionary<string, object>()
                    {
                        {"name", "TestnameEncaps" },
                        {"x", 55.55},
                        {"y", 44.44},
                    }
                }
            };

            assetOwner.Submodel("properties").Property("an_object").Value = complexValue;


            Assert.Throws<Exception>(() => assetOwner.Submodel("properties").Property("an_object").Value); 

            //Thread.Sleep(Settings.WaitTime);

            Thread.Sleep(300); 

            Assert.True(waitForCondition(() =>
            {
                if (assetConsumer.Submodel("properties").Property("an_object").Value == null)
                    return false; 
                Thread.Sleep(300); 

                // Console.WriteLine(
                //     complexValue.ToString()); 
                // Console.WriteLine(
                //     assetConsumer.Submodel("properties").Property("an_object").GetValueAs<Dictionary<string, object>>().ToString()); 

                return complexValue.ToString().Equals(
                    assetConsumer.Submodel("properties").Property("an_object").ValueAs<Dictionary<string, object>>().ToString()); 

            }, Settings.WaitTime));

            var test2 = assetConsumer.Submodel("properties").Property("an_object").ValueAs<ComplexType>();

        }
    }
}
