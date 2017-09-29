// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.TimeoutManager
{
    public class TimeoutManagerFactory : ITimeoutManagerFactory
    {
        public ITimeoutManager<T> CreateTimeoutManager<T>(int timeoutMilliseconds, int checkForTimeoutIntervalMilliseconds = TimeoutManagerConstants.DefaultIntervalTimeoutCheckMilliseconds)
        {
            return new TimeoutManager<T>(timeoutMilliseconds, checkForTimeoutIntervalMilliseconds);
        }
    }
}
