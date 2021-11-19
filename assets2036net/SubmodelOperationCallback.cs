// Copyright (c) 2021 - for information on the respective copyright owner
// see the NOTICE file and/or the repository github.com/boschresearch/assets2036net.
//
// SPDX-License-Identifier: Apache-2.0

namespace assets2036net
{
    /// <summary>
    /// Signature of the local operation you have to provide, when you implement the provider 
    /// of a <seealso cref="SubmodelOperation"/> 
    /// </summary>
    /// <param name="req">The request in an encapsulated form</param>
    /// <returns>a response object. See <seealso cref="SubmodelOperationResponse"/></returns>
    public delegate SubmodelOperationResponse SubmodelOperationCallback(SubmodelOperationRequest req);
}
