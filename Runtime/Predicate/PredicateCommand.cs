using System;
using System.Collections.Generic;
using UnityEngine;

namespace Castforn.CommandAndCallback
{

    /// <summary>
    /// Represents a command that invokes a predicate delegate and returns a bool.
    /// </summary>
    public sealed class PredicateCommand<T> : ReadOnlyPredicateCommand<T>, IEquatable<PredicateCommand<T>>
    {
        #region Lifecycle
        PredicateCommand(Predicate<T> callback) => _callback = callback;
        #endregion

        #region API
        /// <summary>
        /// Acquires a predicate command from the pool, creating a new instance if necessary.
        /// </summary>
        /// <returns>
        /// An acquired predicate command configured with the specified callback.
        /// </returns>
        public static PredicateCommand<T> Get(Predicate<T> callback)
        {
            if (callback == null) return null;
            if (!_pool.TryPop(out var command)) return new(callback);

            command._callback = callback;
            command._hasReleased = false;
            return command;
        }

        /// <inheritdoc cref="Get(Predicate{T})"/>
        public static PredicateCommand<T> Get(Predicate<T> callback, T arg)
        {
            if (callback == null) return null;
            var command = Get(callback);

            command.Arg = arg;
            return command;
        }



        public new T Arg
        {
            get => base.Arg;
            set => base.Arg = value;
        }

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
        /// Invokes the command using the supplied arguments and returns a bool then releases it afterward.
        /// </summary>
        [HideInCallstack]
        public bool Execute(T arg)
        {
            bool result;
            try
            {
                result = Invoke(arg);
            }
            finally
            {
                Release();
            }
            return result;
        }

        /// <summary>
        /// Invokes the command and returns a bool then releases it afterward.
        /// </summary>
        [HideInCallstack]
        public bool Execute()
        {
            bool result;
            try
            {
                result = Invoke(Arg);
            }
            finally
            {
                Release();
            }
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

        public static bool operator ==(PredicateCommand<T> left, ReadOnlyPredicateCommand<T> right) => left?._callback == right;
        public static bool operator !=(PredicateCommand<T> left, ReadOnlyPredicateCommand<T> right) => !(left == right);

        public static bool operator ==(ReadOnlyPredicateCommand<T> left, PredicateCommand<T> right) => left == right?._callback;
        public static bool operator !=(ReadOnlyPredicateCommand<T> left, PredicateCommand<T> right) => !(left == right);

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
        #endregion
    }

    /// <summary>
    /// Represents a read-only command that invokes a predicate delegate and returns a bool.
    /// </summary>
    public abstract class ReadOnlyPredicateCommand<T> : Command, IEquatable<ReadOnlyPredicateCommand<T>>
    {
        #region API
        public T Arg { get; protected set; }



        public override bool HasReleased => _hasReleased;

        /// <summary>
        /// Invokes the command and returns a bool.
        /// </summary>
        [HideInCallstack]
        public bool Invoke() => _callback != null && _callback.Invoke(Arg);


        public bool Equals(ReadOnlyPredicateCommand<T> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ReadOnlyPredicateCommand<T> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ReadOnlyPredicateCommand<T> left, ReadOnlyPredicateCommand<T> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ReadOnlyPredicateCommand<T> left, ReadOnlyPredicateCommand<T> right) => !(left == right);

        public static bool operator ==(ReadOnlyPredicateCommand<T> left, Predicate<T> right) => left?._callback == right;
        public static bool operator !=(ReadOnlyPredicateCommand<T> left, Predicate<T> right) => !(left == right);

        public static bool operator ==(Predicate<T> left, ReadOnlyPredicateCommand<T> right) => left == right?._callback;
        public static bool operator !=(Predicate<T> left, ReadOnlyPredicateCommand<T> right) => !(left == right);

        public static explicit operator Predicate<T>(ReadOnlyPredicateCommand<T> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        protected Predicate<T> _callback;
        protected bool _hasReleased;
        #endregion
    }

}