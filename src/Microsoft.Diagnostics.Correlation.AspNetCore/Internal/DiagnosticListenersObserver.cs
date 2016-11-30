// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Diagnostics.Correlation.AspNetCore.Internal
{
    internal class DiagnosticListenersObserver : IObserver<DiagnosticListener>
    {
        private readonly IDictionary<string, IObserver<KeyValuePair<string, object>>> observers;

        public DiagnosticListenersObserver(IDictionary<string, IObserver<KeyValuePair<string, object>>> observers)
        {
            this.observers = observers;
        }

        public void OnNext(DiagnosticListener value)
        {
            if (observers.ContainsKey(value.Name))
                value.Subscribe(observers[value.Name]);
        }

        public void OnCompleted() { }

        public void OnError(Exception error) { }
    }
}