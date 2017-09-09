// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.TimeoutManager
{
    using System.Collections.Generic;

    internal class TimeoutManagerItemWrapperComparer<T> : IEqualityComparer<TimeoutManagerItemWrapper<T>>
    {
        public bool Equals(TimeoutManagerItemWrapper<T> x, TimeoutManagerItemWrapper<T> y)
        {
            return x.Item.Equals(y.Item);
        }

        public int GetHashCode(TimeoutManagerItemWrapper<T> obj)
        {
            return obj.Item.GetHashCode();
        }
    }
}
