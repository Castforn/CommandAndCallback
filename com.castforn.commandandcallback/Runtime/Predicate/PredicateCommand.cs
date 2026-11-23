using System;
using System.Collections.Generic;
using UnityEngine;

namespace Castforn.CommandAndCallback
{

    /// <summary>
    /// Represents a command that invokes a predicate delegate and returns a bool.
    /// </summary>
    public sealed class PredicateCommand<T> : Command, IEquatable<PredicateCommand<T>>
    {
        #region Lifecycle
        PredicateCommand(Predicate<T> callback, T arg)
        {
            _callback = callback;
            Arg = arg;
        }
        #endregion

        #region API
        /// <summary>
        /// Acquires a predicate command from the pool, creating a new instance if necessary.
        /// </summary>
        /// <returns>
        /// An acquired predicate command configured with the specified callback.
        /// </returns>
        public static PredicateCommand<T> Get(Predicate<T> callback) => Get(callback, default);

        /// <inheritdoc cref="Get(Predicate{T})"/>
        public static PredicateCommand<T> Get(Predicate<T> callback, T arg)
        {
            if (callback == null) return null;
            while (_pool.TryPop(out var command))
            {
                command.Arg = arg;
                command._callback = callback;
                command._hasReleased = false;
                return command;
            }
            return new(callback, arg);
        }

        public override bool HasReleased => _hasReleased;
        public T Arg;

        /// <summary>
        /// Sets the stored arguments of the command.
        /// </summary>
        public void SetArgs(T arg)
        {
            Arg = arg;
        }

        /// <summary>
        /// Invokes the command using the supplied arguments and returns a bool.
        /// </summary>
        [HideInCallstack]
        public bool Invoke(T arg) => _callback != null && _callback.Invoke(arg);

        /// <summary>
        /// Invokes the command and returns a bool.
        /// </summary>
        [HideInCallstack]
        public bool Invoke() => _callback != null && _callback.Invoke(Arg);

        /// <summary>
        /// Invokes the command using the supplied arguments and returns a bool then releases it afterward.
        /// </summary>
        [HideInCallstack]
        public bool Execute(T arg)
        {
            bool result;
#if UNITY_EDITOR
            try { result = Invoke(arg); }
            finally { Release(); }
#else
            result = Invoke(arg);
            Release();
#endif
            return result;
        }

        public override bool Release()
        {
            if (_hasReleased) return false;
            _hasReleased = true;

            _callback = null;
            Arg = default;

            _pool.Push(this);
            return true;
        }

        public bool Equals(PredicateCommand<T> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is PredicateCommand<T> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(PredicateCommand<T> left, PredicateCommand<T> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(PredicateCommand<T> left, PredicateCommand<T> right) => !(left == right);

        public static bool operator ==(PredicateCommand<T> left, Predicate<T> right) => left?._callback == right;
        public static bool operator !=(PredicateCommand<T> left, Predicate<T> right) => !(left == right);

        public static bool operator ==(Predicate<T> left, PredicateCommand<T> right) => left == right?._callback;
        public static bool operator !=(Predicate<T> left, PredicateCommand<T> right) => !(left == right);

        public static implicit operator PredicateCommand<T>(Predicate<T> value) => Get(value);
        public static explicit operator Predicate<T>(PredicateCommand<T> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<PredicateCommand<T>> _pool = new();
        Predicate<T> _callback;
        bool _hasReleased;
        #endregion
    }

}