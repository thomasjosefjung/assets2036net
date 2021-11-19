// Copyright (c) 2021 - for information on the respective copyright owner
// see the NOTICE file and/or the repository github.com/boschresearch/assets2036net.
//
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace assets2036net.unittests
{
    public class AssetMgrQueries : UnitTestBase
    {
        private new readonly ITestOutputHelper log;

        public AssetMgrQueries(ITestOutputHelper outp)
        {
            log = outp; 
        }

        [Fact]
        public void GetAvailableAssetNames()
        {
            var mgr = new assets2036net.AssetMgr(Settings.BrokerHost, Settings.BrokerPort, "arena2036", "Assets2036Aasx");

            var names = mgr.GetAvailableAssetNames();

            foreach (var tuple in names)
            {
                log.WriteLine(tuple.Item1, tuple.Item2);
            }
        }

        [Fact]
        public void GetSupportedSubmodels()
        {
            var mgr = new assets2036net.AssetMgr(Settings.BrokerHost, Settings.BrokerPort, "arena2036", "Assets2036Aasx");

            var submodels = mgr.GetSupportedSubmodels("arena2036", "SimCISS"); 

            foreach (var sm in submodels)
            {
                log.WriteLine(string.Format("{0} from {1}", sm.Name, sm.SubmodelUrl)); 
            }
        }

        [Fact]
        public void RemoveAssetTrace()
        {
            var assetName = "WillBeDeleted";
            var @namespace = "assets2036net"; 

            var mgr = new assets2036net.AssetMgr(Settings.BrokerHost, Settings.BrokerPort, @namespace, assetName);

            try
            {
                var submodelUrl = "https://arena2036-infrastructure.saz.bosch-si.com/arena2036_public/assets2036_submodels/-/raw/master/testmodel.json";
                var asset = mgr.CreateAsset(@namespace, assetName, new Uri(submodelUrl));

                var proxy = mgr.CreateFullAssetProxy(@namespace, assetName);

                double value = 1.99;

                asset.Submodel("testmodel").Property("number").Value = value;
                Thread.Sleep(1000);
                Assert.Equal(value, proxy.Submodel("testmodel").Property("number").ValueDouble);
            }
            finally
            {
                mgr.Dispose(); 
            }


            mgr = new assets2036net.AssetMgr(Settings.BrokerHost, Settings.BrokerPort, @namespace, assetName);
            Tools.RemoveAssetTrace(Settings.BrokerHost, Settings.BrokerPort, @namespace, assetName);

            var client = new uPLibrary.Networking.M2Mqtt.MqttClient(Settings.BrokerHost, Settings.BrokerPort, false, null, null, uPLibrary.Networking.M2Mqtt.MqttSslProtocols.None);

            try
            {
                client.Connect("assets2036net");

                client.MqttMsgPublishReceived += (object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e) =>
                {
                    throw new Exception(string.Format("Received unexpected message on topic {0}", e.Topic));
                };

                client.Subscribe(new string[] { string.Format("{0}/{1}/#", @namespace, assetName) }, new byte[] { 2 });

                Thread.Sleep(1000);
            }
            finally
            {
                client.Disconnect(); 
            }
        }

    }
}
