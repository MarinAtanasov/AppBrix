// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Events.Schedule.Impl
{
    internal class PriorityQueue : IApplicationLifecycle
    {
        #region Public and overriden methods
        public void Initialize(IInitializeContext context)
        {
            this.app = context.App;
        }

        public void Uninitialize()
        {
            this.queue.Clear();
            this.app = null;
        }
        
        public void Push(PriorityQueueItem item)
        {
            this.queue.Add(item);
            this.BubbleUp(this.queue.Count - 1);
        }

        public PriorityQueueItem Peek()
        {
            return this.queue.Count > 0 ? this.queue[0] : null;
        }

        public PriorityQueueItem Pop()
        {
            return this.RemoveAt(0);
        }

        public void Remove<T>(IScheduledEvent<T> args) where T : IEvent
        {
            for (int i = 0; i < this.queue.Count; i++)
            {
                if (object.ReferenceEquals(this.queue[i].Event, args))
                {
                    this.RemoveAt(i);
                    return;
                }
            }
        }
        #endregion

        #region Private methods
        private PriorityQueueItem RemoveAt(int index)
        {
            var item = this.queue[index];
            var lastIndex = this.queue.Count - 1;
            this.queue[index] = this.queue[lastIndex];
            this.queue.RemoveAt(lastIndex);
            this.BubbleUp(index);
            this.BubbleDown(index);
            return item;
        }

        private void BubbleUp(int index)
        {
            if (index == 0)
                return;

            var parent = this.GetParent(index);
            if (this.queue[parent].Occurrence > this.queue[index].Occurrence)
            {
                this.Swap(parent, index);
                this.BubbleUp(parent);
            }
        }

        private void BubbleDown(int index)
        {
            var firstChild = this.GetFirstChild(index);
            if (firstChild >= this.queue.Count)
                return;

            var secondChild = firstChild + 1;
            var child = secondChild == this.queue.Count || this.queue[firstChild].Occurrence < this.queue[secondChild].Occurrence ?
                firstChild : secondChild;

            if (this.queue[index].Occurrence > this.queue[child].Occurrence)
            {
                this.Swap(index, child);
                this.BubbleDown(child);
            }
        }

        private void Swap(int i1, int i2)
        {
            var item = this.queue[i1];
            this.queue[i1] = this.queue[i2];
            this.queue[i2] = item;
        }

        private int GetParent(int index) => (index - 1) / 2;

        private int GetFirstChild(int index) => index * 2 + 1;
        #endregion

        #region Private fields and constants
        private readonly IList<PriorityQueueItem> queue = new List<PriorityQueueItem>();
        private IApp app;
        #endregion
    }
}
