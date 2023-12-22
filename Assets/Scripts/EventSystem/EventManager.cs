using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Events
{
    [DefaultExecutionOrder(-10)]
    public class EventManager : Singleton<EventManager>
    {
        private Dictionary<Type, List<Action<object>>> _eventListeners = new();

        public void AddListener<T>(Action<object> action)
        {
            if (_eventListeners.ContainsKey(typeof(T)))
            {
                _eventListeners[typeof(T)].Add(action);
            }
            else
            {
                _eventListeners.Add(typeof(T), new List<Action<object>>() { action });
            }
        }

        public void RemoveListener<T>(Action<object> action)
        {
            if (_eventListeners.ContainsKey(typeof(T)))
            {
                if (_eventListeners[typeof(T)].Contains(action))
                    _eventListeners[typeof(T)].Remove(action);
            }
        }

        public void TriggerEvent<T>(object data = null)
        {
            if (_eventListeners.ContainsKey(typeof(T)))
            {
                var listeners = _eventListeners[typeof(T)];
                foreach (var listener in listeners)
                {
                    listener?.Invoke(data);
                }
            }
        }
    }
}