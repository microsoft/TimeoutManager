// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.TimeoutManager
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
   
    public class MultiTimeoutManager<T> : IMultiTimeoutManager<T>
    {
        protected readonly ConcurrentDictionary<int, ITimeoutManager<T>> timeoutManagers;
        protected readonly ITimeoutManagerFactory timeoutManagerFactory;
        
        public event ItemTimedOutEventHandler<T> ItemTimedOut;

        public MultiTimeoutManager(ITimeoutManagerFactory timeoutManagerFactory)
        {
            this.timeoutManagers = new ConcurrentDictionary<int, ITimeoutManager<T>>();
            this.timeoutManagerFactory = timeoutManagerFactory;
        }

        public bool CountTimeout(T timeoutItem, TimeSpan timeoutTimeSpan)
        {
            ITimeoutManager<T> timeoutManager = GetOrCreateTimeoutManager((int)timeoutTimeSpan.TotalMilliseconds);

            return timeoutManager.CountTimeout(timeoutItem);
        }

        public bool TryCancelTimeout(T cancelledItem)
        {
            bool timeoutCancelled = false;
            foreach (ITimeoutManager<T> timeoutManager in this.timeoutManagers.Values)
            {
                if (timeoutManager.TryCancelTimeout(cancelledItem))
                {
                    timeoutCancelled = true;
                }
            }

            return timeoutCancelled;
        }

        /// <summary>
        /// We look at the current dictionary for a key with value timeout + offset and return that value
        /// If the value is not present, we create a timeout manager with that value, add it to the dictionary and return that value
        /// </summary>
        protected virtual ITimeoutManager<T> GetOrCreateTimeoutManager(int timeoutMilliseconds)
        {
            return this.timeoutManagers.GetOrAdd(timeoutMilliseconds, CreateTimeoutManager);
        }

        protected virtual ITimeoutManager<T> CreateTimeoutManager(int timeoutValue)
        {
            ITimeoutManager<T> timeoutManager = this.timeoutManagerFactory.CreateTimeoutManager<T>(timeoutValue);

            timeoutManager.ItemTimedOut += RaiseTimeoutEvents;

            Trace.TraceInformation("Adding timeout manager with a timeout value of '{0}'", timeoutValue);
       
            return timeoutManager;
        }

        private void RaiseTimeoutEvents(ItemTimedOutEventArgs<T> timedOut)
        {
            // For thread-safety (delegates are immutable)
            ItemTimedOutEventHandler<T> tempDelegate = this.ItemTimedOut;
            if (tempDelegate != null)
            {
                tempDelegate.Invoke(timedOut);
            }
        }

        public void Dispose()
        {
            foreach (ITimeoutManager<T> timeoutManager in this.timeoutManagers.Values)
            {
                timeoutManager.ItemTimedOut -= RaiseTimeoutEvents;
                timeoutManager.Dispose();
            };
        }
    }
}
