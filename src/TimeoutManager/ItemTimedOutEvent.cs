// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.TimeoutManager
{
    using System;

    public class ItemTimedOutEventArgs<T> : EventArgs
    {
        public ItemTimedOutEventArgs(T timedOutItem)
        {
            this.TimedOutItem = timedOutItem;
        }

        public T TimedOutItem
        {
            get;
            private set;
        }
    }

    public delegate void ItemTimedOutEventHandler<T>(ItemTimedOutEventArgs<T> e);
}
