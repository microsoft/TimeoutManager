// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.TimeoutManager
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    public class TimeoutManager<T> : ITimeoutManager<T>
    {
        private int timeoutMilliseconds;
        private readonly int timeoutCheckIntervalMilliseconds;
        private readonly QueueWithRemove<TimeoutManagerItemWrapper<T>> timedItemsQueue;
        private readonly Timer timer;

        public event ItemTimedOutEventHandler<T> ItemTimedOut;

        public TimeoutManager(int timeoutMilliseconds, int checkForTimeoutIntervalMilliseconds)
        {
            ValidateTimeoutParameter(timeoutMilliseconds);
            ValidateCheckForTimeoutIntervalParameter(checkForTimeoutIntervalMilliseconds);

            this.timeoutMilliseconds = timeoutMilliseconds;
            this.timeoutCheckIntervalMilliseconds = checkForTimeoutIntervalMilliseconds;
            this.timedItemsQueue = new QueueWithRemove<TimeoutManagerItemWrapper<T>>(new TimeoutManagerItemWrapperComparer<T>());

            // Sets the timer to tick only once after specified interval (The timer will be set to tick only once again in the OnIntervalElapsed callback)
            this.timer = new Timer(OnIntervalElapsed, state: null, dueTime: checkForTimeoutIntervalMilliseconds, period: Timeout.Infinite);
        }

        public bool CountTimeout(T timeoutItem)
        {
            if (timeoutItem == null)
            {
                return false;
            }

            TimeoutManagerItemWrapper<T> timeWrapper = new TimeoutManagerItemWrapper<T>(timeoutItem, DateTime.UtcNow);
            return this.timedItemsQueue.TryEnqueueIfNotExists(timeWrapper);
        }

        public bool TryCancelTimeout(T cancelledItem)
        {
            if (cancelledItem != null)
            {
                return this.timedItemsQueue.TryRemove(new TimeoutManagerItemWrapper<T>(cancelledItem));
            }

            return false;
        }

        private void OnIntervalElapsed(object state)
        {
            ClearTimedOutItems();

            // Sets the timer to tick only once after specified interval
            this.timer.Change(this.timeoutCheckIntervalMilliseconds, Timeout.Infinite);
        }

        private void ClearTimedOutItems()
        {
            TimeoutManagerItemWrapper<T> timedOutItemWrapper;
            while (this.timedItemsQueue.TryPeek(out timedOutItemWrapper) && IsTimedOut(timedOutItemWrapper))
            {
                // This will fail in case the item was externally removed while the current thread entered the while scope.
                // We are using TryRemove here instead of TryDequeue, because the item peeked at could be removed by another thread before we managed to dequeue it
                if (this.timedItemsQueue.TryRemove(timedOutItemWrapper))
                {
                    RaiseTimeOutEvent(timedOutItemWrapper.Item);
                }
            }
        }

        private bool IsTimedOut(TimeoutManagerItemWrapper<T> itemWrapperToCheck)
        {
            TimeSpan itemAge = DateTime.UtcNow.Subtract(itemWrapperToCheck.Time);
            return itemAge.TotalMilliseconds >= this.timeoutMilliseconds;
        }

        private void RaiseTimeOutEvent(T timedOutItem)
        {
            // For thread-safety (delegates are immutable)
            ItemTimedOutEventHandler<T> tempDelegate = ItemTimedOut;
            if (tempDelegate != null)
            {
                tempDelegate.Invoke(new ItemTimedOutEventArgs<T>(timedOutItem));
            }
        }

        private static void ValidateTimeoutParameter(int timeoutMilliseconds)
        {
            if (timeoutMilliseconds < 0)
            {
                throw new ArgumentOutOfRangeException("timeoutMilliseconds", "Timeout must be non-negative integer");
            }
        }

        private static void ValidateCheckForTimeoutIntervalParameter(int checkForTimeoutIntervalMilliseconds)
        {
            if (checkForTimeoutIntervalMilliseconds <= 0)
            {
                throw new ArgumentOutOfRangeException("checkForTimeoutIntervalMilliseconds", "Timeout check interval must be a positive integer greater than zero");
            }
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}
