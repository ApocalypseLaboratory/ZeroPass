using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroPass.Monitor
{
    public class CellChangeMonitor : Singleton<CellChangeMonitor>
    {
        private readonly List<CellChangedEntry.Handler> cellChangedCallbacksToRun = new();

        private readonly Dictionary<int, CellChangedEntry> cellChangedHandlers = new();

        private HashSet<int> dirtyTransforms = new();

        private int gridWidth;

        private readonly List<Action<Transform, bool>> moveChangedCallbacksToRun = new();

        private readonly Dictionary<int, MovementStateChangedEntry> movementStateChangedHandlers = new();

        private HashSet<int> movingTransforms = new();

        private HashSet<int> pendingDirtyTransforms = new();

        private HashSet<int> previouslyMovingTransforms = new();

        private readonly Dictionary<int, int> transformLastKnownCell = new();

        public void MarkDirty(Transform transform)
        {
            if (gridWidth != 0)
            {
                pendingDirtyTransforms.Add(transform.GetInstanceID());
                var childCount = transform.childCount;
                for (var i = 0; i < childCount; i++) MarkDirty(transform.GetChild(i));
            }
        }

        public bool IsMoving(Transform transform)
        {
            return movingTransforms.Contains(transform.GetInstanceID());
        }

        public void RegisterMovementStateChanged(Transform transform, Action<Transform, bool> handler)
        {
            var instanceID = transform.GetInstanceID();
            var value = default(MovementStateChangedEntry);
            if (!movementStateChangedHandlers.TryGetValue(instanceID, out value))
            {
                value = default;
                value.handlers = new List<Action<Transform, bool>>();
                value.transform = transform;
            }

            value.handlers.Add(handler);
            movementStateChangedHandlers[instanceID] = value;
        }

        public void UnregisterMovementStateChanged(int instance_id, Action<Transform, bool> callback)
        {
            var value = default(MovementStateChangedEntry);
            if (movementStateChangedHandlers.TryGetValue(instance_id, out value))
            {
                value.handlers.Remove(callback);
                if (value.handlers.Count == 0) movementStateChangedHandlers.Remove(instance_id);
            }
        }

        public void UnregisterMovementStateChanged(Transform transform, Action<Transform, bool> callback)
        {
            UnregisterMovementStateChanged(transform.GetInstanceID(), callback);
        }

        public int RegisterCellChangedHandler(Transform transform, Action callback, string debug_name)
        {
            var instanceID = transform.GetInstanceID();
            var value = default(CellChangedEntry);
            if (!cellChangedHandlers.TryGetValue(instanceID, out value))
            {
                value = default;
                value.transform = transform;
                value.handlers = new List<CellChangedEntry.Handler>();
            }

            var handler = default(CellChangedEntry.Handler);
            handler.name = debug_name;
            handler.callback = callback;
            var item = handler;
            value.handlers.Add(item);
            cellChangedHandlers[instanceID] = value;
            return instanceID;
        }

        public void UnregisterCellChangedHandler(int instance_id, Action callback)
        {
            var value = default(CellChangedEntry);
            if (cellChangedHandlers.TryGetValue(instance_id, out value))
            {
                for (var i = 0; i < value.handlers.Count; i++)
                {
                    var handler = value.handlers[i];
                    if (!(handler.callback != callback))
                    {
                        value.handlers.RemoveAt(i);
                        break;
                    }
                }

                if (value.handlers.Count == 0) cellChangedHandlers.Remove(instance_id);
            }
        }

        public void UnregisterCellChangedHandler(Transform transform, Action callback)
        {
            UnregisterCellChangedHandler(transform.GetInstanceID(), callback);
        }

        public int PosToCell(Vector3 pos)
        {
            var x = pos.x;
            var num = pos.y + 0.05f;
            var num2 = (int)num;
            var num3 = (int)x;
            return num2 * gridWidth + num3;
        }

        public void SetGridSize(int grid_width, int grid_height)
        {
            gridWidth = grid_width;
        }

        public void RenderEveryTick()
        {
            var hashSet = pendingDirtyTransforms;
            pendingDirtyTransforms = dirtyTransforms;
            dirtyTransforms = hashSet;
            pendingDirtyTransforms.Clear();
            previouslyMovingTransforms.Clear();
            hashSet = previouslyMovingTransforms;
            previouslyMovingTransforms = movingTransforms;
            movingTransforms = hashSet;
            foreach (var dirtyTransform in dirtyTransforms)
            {
                var value = default(CellChangedEntry);
                if (cellChangedHandlers.TryGetValue(dirtyTransform, out value))
                {
                    if (value.transform == null) continue;
                    var value2 = -1;
                    transformLastKnownCell.TryGetValue(dirtyTransform, out value2);
                    var num = PosToCell(value.transform.GetPosition());
                    if (value2 != num)
                    {
                        cellChangedCallbacksToRun.Clear();
                        cellChangedCallbacksToRun.AddRange(value.handlers);
                        foreach (var item in cellChangedCallbacksToRun)
                        {
                            var current2 = item;
                            foreach (var handler in value.handlers)
                            {
                                var current3 = handler;
                                if (current3.callback == current2.callback)
                                {
                                    current3.callback();
                                    break;
                                }
                            }
                        }

                        transformLastKnownCell[dirtyTransform] = num;
                    }
                }

                movingTransforms.Add(dirtyTransform);
                if (!previouslyMovingTransforms.Contains(dirtyTransform))
                    RunMovementStateChangedCallbacks(dirtyTransform, true);
            }

            foreach (var previouslyMovingTransform in previouslyMovingTransforms)
                if (!movingTransforms.Contains(previouslyMovingTransform))
                    RunMovementStateChangedCallbacks(previouslyMovingTransform, false);
            dirtyTransforms.Clear();
        }

        private void RunMovementStateChangedCallbacks(int instance_id, bool state)
        {
            var value = default(MovementStateChangedEntry);
            if (movementStateChangedHandlers.TryGetValue(instance_id, out value))
            {
                moveChangedCallbacksToRun.Clear();
                moveChangedCallbacksToRun.AddRange(value.handlers);
                foreach (var item in moveChangedCallbacksToRun)
                    if (value.handlers.Contains(item))
                        item(value.transform, state);
            }
        }

        private void Validate()
        {
        }

        private struct CellChangedEntry
        {
            public struct Handler
            {
                public string name;

                public Action callback;
            }

            public Transform transform;

            public List<Handler> handlers;
        }

        private struct MovementStateChangedEntry
        {
            public Transform transform;

            public List<Action<Transform, bool>> handlers;
        }
    }
}