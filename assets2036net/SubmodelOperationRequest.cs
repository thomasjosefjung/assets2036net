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
    /// Instances of class SubmodelOperationRequest encapsulate one request to an asset operation.
    /// Its serialization is used as payload on the wire. When you implement operations for your ohwn asset
    /// or submodel, all parameters to an operation will be encapsulated inside an instance of 
    /// SubmodelOperationRequest. 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)] 
    public class SubmodelOperationRequest : CommElementBase
    {
        /// <summary>
        /// The unique request id for an operation call. Used to map request and answer on the client side. 
        /// </summary>
        [JsonProperty("req_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// a dictionary containig the request's corresponding parameters. 
        /// </summary>
        [JsonProperty("params")]
        public Dictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Referehnce to the submodel operation which will be used to handle this request. 
        /// </summary>
        public SubmodelOperation Operation { get; set; }

        /// <summary>
        /// When implementing your own submodel provider, the operation handler needs to return 
        /// a SubmodelOperationRequest-object. This operation builds one correspondung to this
        /// SubmodelOperationRequest, so that e.g. the request-id is automatically set for the response. 
        /// </summary>
        /// <returns>a prefilled SubmodelOperationResponse corresponding to this request. You only need 
        /// to set the return value using <seealso cref="SubmodelOperationResponse.Value"/></returns>
        public SubmodelOperationResponse CreateResponseObj()
        {
            return new SubmodelOperationResponse()
            {
                AssetMgr = this.AssetMgr,
                Asset = this.Asset,
                Operation = this.Operation, 
                //Name = SubmodelElement.buildTopic(Operation.Name, StringConstants.StringConstant_RESP),
                RequestId = this.RequestId,
                Submodel = this.Submodel
            };
        }


        private static log4net.ILog log = Config.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        internal SubmodelOperationRequest()
        {
            Parameters = new Dictionary<string, object>(); 
        }

        internal SubmodelOperationRequest(SubmodelOperation operation)
            : this()
        {
            Operation = operation; 
            //Name = SubmodelElement.buildTopic(operation.Name, StringConstants.StringConstant_REQ);
            Parameters = new Dictionary<string, object>(); 
        }

        internal void Publish()
        {
            string message = JsonConvert.SerializeObject(this);

            var topic = buildTopic(Operation.Topic, StringConstants.StringConstant_REQ); 
            log.DebugFormat("Send: {0} @ {1}", message, topic);
            Asset.publish(
                topic,
                message,
                false);
        }

    }
}
