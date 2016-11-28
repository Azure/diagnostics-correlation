// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Diagnostics.Context
{
    /// <summary>
    /// Provides CorrelationContext implementation holding following properties:
    ///  - 'correlation id' as common identifier of the whole operation (or flow) potentially involving multiple services and HTTP calls
    ///  - 'request id' as identifier of this particular request. This is could be passed from the upstream service. Optional
    ///  - 'child request id' as identifier of outgoing request. For the downstream service, it becomes 'request id'. Optional
    ///  - dictionary of any custom fields
    /// </summary>
    public class CorrelationContext : IDictionary<string, object>, ICorrelationContext<CorrelationContext>
    {
        /// <summary>
        /// Current CorrelationId,  common identifier of the whole operation (or flow) potentially involving multiple services and HTTP calls
        /// </summary>
        public string CorrelationId => (string)contextDict[CorrelationIdKey];

        /// <summary>
        /// Parent RequestId, identifier of this particular request. This is could be passed from the upstream service
        /// </summary>
        public string RequestId => (string)contextDict[RequestIdKey];

        /// <summary>
        /// Child request id, identifier of outgoing request. For the downstream service, it becomes 'request id'
        /// </summary>
        public string ChildRequestId => (string) (contextDict.ContainsKey(ChildRequestIdKey) ? contextDict[ChildRequestIdKey] : null);

        /// <summary>
        /// Creates CorrelationContext instance from correlation and request ids
        /// </summary>
        /// <param name="correlationId">correlation id</param>
        /// <param name="requestId">request id</param>
        public CorrelationContext(string correlationId, string requestId)
        {
            if (correlationId == null)
                throw new ArgumentNullException(nameof(correlationId));
            contextDict = new Dictionary<string, object>
            {
                [CorrelationIdKey] = correlationId,
                [RequestIdKey] = requestId
            };
        }

        /// <summary>
        /// Creates CorrelationContext instance from correlation id
        /// </summary>
        /// <param name="correlationId">correlation id</param>
        public CorrelationContext(string correlationId) : this(correlationId, null)
        {
        }

        private CorrelationContext(string correlationId, string requestId, string childRequestId)
        {
            if (correlationId == null)
                throw new ArgumentNullException(nameof(correlationId));
            contextDict = new Dictionary<string, object>
            {
                [CorrelationIdKey] = correlationId,
                [RequestIdKey] = requestId,
                [ChildRequestIdKey] = childRequestId
            };
        }

        private readonly IDictionary<string, object> contextDict;
        private const string CorrelationIdKey = "correlationId";
        private const string RequestIdKey = "requestId";
        private const string ChildRequestIdKey = "childRequestId";

        /// <summary>
        /// Inherited from Dictionary
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return contextDict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Inherited from Dictionary
        /// </summary>
        public void Add(KeyValuePair<string, object> item)
        {
            switch (item.Key)
            {
                case CorrelationIdKey:
                    throw new ArgumentException($"{nameof(item.Key)} could not add {CorrelationIdKey}");
                case RequestIdKey:
                    throw new ArgumentException($"{nameof(item.Key)} could not add {RequestIdKey}");
                case ChildRequestIdKey:
                    throw new ArgumentException($"{nameof(item.Key)} could not add {ChildRequestId}");

            }

            contextDict.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Inherited from Dictionary
        /// </summary>
        public void Clear()
        {
            var correlation = CorrelationId;
            var request = RequestId;
            var childId = ChildRequestId;
            contextDict.Clear();
            contextDict.Add(CorrelationIdKey, correlation);
            contextDict.Add(RequestIdKey, request);
            contextDict.Add(ChildRequestId, childId);
        }

        /// <summary>
        /// Inherited from Dictionary
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<string, object> item)
        {
            return contextDict.Contains(item);
        }

        /// <summary>
        /// Inherited from Dictionary
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            contextDict.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Inherited from Dictionary
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<string, object> item)
        {
            switch (item.Key)
            {
                case CorrelationIdKey:
                    throw new ArgumentException($"{nameof(item.Key)} could not remove {CorrelationIdKey}");
                case RequestIdKey:
                    throw new ArgumentException($"{nameof(item.Key)} could not remove {RequestIdKey}");
                case ChildRequestIdKey:
                    throw new ArgumentException($"{nameof(item.Key)} could not remove {ChildRequestIdKey}");

            }
            return contextDict.Remove(item);
        }

        /// <summary>
        /// Inherited from Dictionary
        /// </summary>
        public int Count => contextDict.Count;

        /// <summary>
        /// Inherited from Dictionary
        /// </summary>
        public bool IsReadOnly => contextDict.IsReadOnly;

        /// <summary>
        /// Inherited from Dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return contextDict.ContainsKey(key);
        }

        /// <summary>
        /// Inherited from Dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            switch (key)
            {
                case CorrelationIdKey:
                    throw new ArgumentException($"{nameof(key)} could not add {CorrelationIdKey}");
                case RequestIdKey:
                    throw new ArgumentException($"{nameof(key)} could not add {RequestIdKey}");
            }

            contextDict.Add(key, value);
        }

        /// <summary>
        /// Inherited from Dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            switch (key)
            {
                case CorrelationIdKey:
                    throw new ArgumentException($"{nameof(key)} could not remove {CorrelationIdKey}");
                case RequestIdKey:
                    throw new ArgumentException($"{nameof(key)} could not remove {RequestIdKey}");
            }
            return contextDict.Remove(key);
        }

        /// <summary>
        /// Inherited from Dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out object value)
        {
            return contextDict.TryGetValue(key, out value);
        }

        /// <summary>
        /// Inherited from Dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get { return contextDict[key]; }
            set { Add(key, value); }
        }

        /// <summary>
        /// Inherited from Dictionary
        /// </summary>
        public ICollection<string> Keys => contextDict.Keys;

        /// <summary>
        /// Inherited from Dictionary
        /// </summary>
        public ICollection<object> Values => contextDict.Values;

        /// <summary>
        /// Implements <see cref="ICorrelationContext{TContext}"/>
        /// Creates copy of 'parent' context for outgoing request with child request id
        /// </summary>
        /// <param name="childRequestId">>Unique id for outgoing request</param>
        /// <returns>Child <see cref="CorrelationContext"/></returns>
        public CorrelationContext GetChildRequestContext(string childRequestId)
        {
            if (childRequestId == null)
                return this;

            var ctxCopy = new CorrelationContext(CorrelationId, RequestId, childRequestId);
            foreach (var pair in this.Where(pair => pair.Key != RequestIdKey && pair.Key != CorrelationIdKey))
            {
                ctxCopy.Add(pair);
            }
            return ctxCopy;
        }
    }
}
