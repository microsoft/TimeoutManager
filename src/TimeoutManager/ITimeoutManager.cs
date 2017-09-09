// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.TimeoutManager
{
    using System;

    /// <summary>
    /// A Thread safe manager for managing the timeout of objects of type T
    /// </summary>
    /// <typeparam name="T">The types of the objects to manage the timeout for</typeparam>
    public interface ITimeoutManager<T> : IDisposable
    {
        /// <summary>
        /// Starts the timeout count for a given object
        /// </summary>
        /// <param name="timeoutItem">The object to count the timeout for</param>
        /// <returns>True if successfully added timeout, false otherwise</returns>
        bool CountTimeout(T timeoutItem);

        /// <summary>
        /// Cancels the timeout count for the object if it has not already been timed out
        /// </summary>
        /// <param name="cancelledItem">The timeout-object to cancel</param>
        /// <returns>True if successfully cancelled, false otherwise</returns>
        bool TryCancelTimeout(T cancelledItem);

        /// <summary>
        /// An event which is invoked every time an object times out
        /// </summary>
        event ItemTimedOutEventHandler<T> ItemTimedOut;
    }
}
