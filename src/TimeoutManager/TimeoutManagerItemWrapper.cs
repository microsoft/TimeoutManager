// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.TimeoutManager
{
    using System;

    internal class TimeoutManagerItemWrapper<T>
    {
        public TimeoutManagerItemWrapper(T itemToWrap)
        {
            Item = itemToWrap;
        }

        public TimeoutManagerItemWrapper(T itemToWrap, DateTime entryTime)
        {
            Item = itemToWrap;
            Time = entryTime;
        }

        public T Item
        {
            get;
            private set;
        }

        public DateTime Time
        {
            get;
            private set;
        }
    }
}
