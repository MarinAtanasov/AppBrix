﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using AppBrix.Events.Schedule.Contracts;
using System.Collections.Generic;

namespace AppBrix.Events.Schedule.Impl;

internal sealed class PriorityQueue
{
    #region Public and overriden methods
    public void Push(PriorityQueueItem item)
    {
        this.queue.Add(item);
        this.BubbleUp(this.queue.Count - 1);
    }

    public PriorityQueueItem? Peek() => this.queue.Count > 0 ? this.queue[0] : null;

    public PriorityQueueItem Pop() => this.RemoveAt(0);

    public void ReprioritizeHead() => this.BubbleDown(0);

    public void Remove<T>(IScheduledEvent<T> args) where T : IEvent
    {
        for (var i = this.queue.Count - 1; i >= 0; i--)
        {
            if (object.ReferenceEquals(this.queue[i].ScheduledEvent, args))
            {
                this.RemoveAt(i);
                return;
            }
        }
    }

    public void Clear() => this.queue.Clear();
    #endregion

    #region Private methods
    private PriorityQueueItem RemoveAt(int index)
    {
        var item = this.queue[index];
        var lastIndex = this.queue.Count - 1;
        if (index < lastIndex)
        {
            this.queue[index] = this.queue[lastIndex];
            this.queue.RemoveAt(lastIndex);
            this.BubbleUp(index);
            this.BubbleDown(index);
        }
        else
        {
            this.queue.RemoveAt(index);
        }
        return item;
    }

    private void BubbleUp(int index)
    {
        while (index > 0)
        {
            var parent = this.GetParent(index);
            if (this.queue[parent].Occurrence <= this.queue[index].Occurrence)
                return;

            this.Swap(parent, index);
            index = parent;
        }
    }

    private void BubbleDown(int index)
    {
        var child = this.GetFirstChild(index);
        while (child < this.queue.Count)
        {
            if (child < this.queue.Count - 1 && this.queue[child].Occurrence >= this.queue[child + 1].Occurrence)
                child++;

            if (this.queue[index].Occurrence <= this.queue[child].Occurrence)
                return;

            this.Swap(index, child);
            index = child;
            child = this.GetFirstChild(index);
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
    private readonly List<PriorityQueueItem> queue = new List<PriorityQueueItem>();
    #endregion
}
