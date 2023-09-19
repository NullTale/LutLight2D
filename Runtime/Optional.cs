using System;
using UnityEngine;

// LutLight2D © NullTale - https://twitter.com/NullTale/
namespace LutLight2D
{
    [Serializable]
    public sealed class Optional<T>
    {
        [SerializeField]
        internal bool enabled;

        [SerializeField]
        internal T value = default!;
    
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public T    Value
        {
            get => value;
            set => this.value = value;
        }

        // =======================================================================
        public Optional(bool enabled)
        {
            this.enabled = enabled;
        }

        public Optional(bool enabled, T value)
        {
            this.enabled = enabled;
            this.value   = value;
        }
        
        public bool TryGetValue(out T val)
        {
            val = enabled ? value : default;
            return enabled;
        }

        public T GetValue(T disabledValue)
        {
            return enabled ? value : disabledValue;
        }
        
        public static implicit operator bool(Optional<T> opt)
        {
            return opt.enabled;
        }

        public static implicit operator T(Optional<T> opt)
        {
            return opt.value;
        }
    }
}