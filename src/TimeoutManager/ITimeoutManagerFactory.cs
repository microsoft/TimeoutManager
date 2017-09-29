// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.TimeoutManager
{
    public interface ITimeoutManagerFactory
    {
        /// <summary>
        /// Factory method for creating a timeout manager
        /// </summary>
        /// <typeparam name="T">The type of items to count timeout for</typeparam>
        /// <param name="timeoutMilliseconds">The timeout span in milliseconds</param>
        /// <param name="checkForTimeoutIntervalMilliseconds">The interval between each time-out check</param>
        /// <returns>The instance of TimeoutManager created</returns>
        ITimeoutManager<T> CreateTimeoutManager<T>(int timeoutMilliseconds, int checkForTimeoutIntervalMilliseconds = TimeoutManagerConstants.DefaultIntervalTimeoutCheckMilliseconds);
    }
}
