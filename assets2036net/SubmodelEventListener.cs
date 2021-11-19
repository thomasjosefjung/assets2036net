// Copyright (c) 2021 - for information on the respective copyright owner
// see the NOTICE file and/or the repository github.com/boschresearch/assets2036net.
//
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text;

namespace assets2036net
{
    /// <summary>
    /// Used in the submodel consumer implementation to listen to events. 
    /// </summary>
    /// <param name="eventMessage">the event's parameters</param>
    public delegate void SubmodelEventListener(SubmodelEventMessage eventMessage);
}
