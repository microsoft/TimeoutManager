// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.TimeoutManager
{
    using System.Collections.Generic;
    using System.Threading;

    public class QueueWithRemove<T>
    {
        private readonly ReaderWriterLockSlim queueLock = new ReaderWriterLockSlim();
        private readonly LinkedList<T> list = new LinkedList<T>();
        private readonly Dictionary<T, LinkedListNode<T>> dictionary;

        public QueueWithRemove()
        {
            this.dictionary = new Dictionary<T, LinkedListNode<T>>();
        }

        public QueueWithRemove(IEqualityComparer<T> comparer)
        {
            this.dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
        }

        public bool TryEnqueueIfNotExists(T t)
        {
            queueLock.EnterUpgradeableReadLock();
            try
            {
                if (dictionary.ContainsKey(t))
                {
                    return false;
                }

                queueLock.EnterWriteLock();
                try
                {
                    LinkedListNode<T> addedNode = list.AddLast(t);
                    dictionary.Add(t, addedNode);
                    return true;
                }
                finally
                {
                    queueLock.ExitWriteLock();
                }
            }
            finally
            {
                queueLock.ExitUpgradeableReadLock();
            }
        }

        public bool TryDequeue(out T result)
        {
            queueLock.EnterUpgradeableReadLock();
            try
            {
                if (list.Count == 0)
                {
                    result = default(T);
                    return false;
                }

                result = list.First.Value;

                queueLock.EnterWriteLock();
                try
                {
                    list.RemoveFirst();
                    dictionary.Remove(result);
                }
                finally
                {
                    queueLock.ExitWriteLock();
                }

                return true;
            }
            finally
            {
                queueLock.ExitUpgradeableReadLock();
            }
        }

        public bool TryPeek(out T result)
        {
            queueLock.EnterReadLock();
            try
            {
                if (list.Count == 0)
                {
                    result = default(T);
                    return false;
                }

                result = list.First.Value;
                return true;
            }
            finally
            {
                queueLock.ExitReadLock();
            }
        }

        public bool TryRemove(T t)
        {
            queueLock.EnterUpgradeableReadLock();
            try
            {
                if (!dictionary.ContainsKey(t))
                {
                    return false;
                }

                LinkedListNode<T> nodeToRemove = dictionary[t];
                queueLock.EnterWriteLock();

                try
                {
                    dictionary.Remove(t);
                    list.Remove(nodeToRemove);
                }
                finally
                {
                    queueLock.ExitWriteLock();    
                }

                return true;
            }
            finally
            {
                queueLock.ExitUpgradeableReadLock();
            }
        }

        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }
    }
}
