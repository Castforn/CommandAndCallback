using System;
using System.Collections.Generic;
using UnityEngine;

namespace Castforn.CommandAndCallback
{

    /// <inheritdoc cref="ActionCommand"/>
    public sealed class ActionCommand<T1> : ReadOnlyActionCommand<T1>, IEquatable<ActionCommand<T1>>
    {
        #region Lifecycle
        ActionCommand(Action<T1> callback) => _callback = callback;
        #endregion

        #region API
        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1> Get(Action<T1> callback)
        {
            if (callback == null) return null;
            if (!_pool.TryPop(out var command)) return new(callback);

            command._callback = callback;
            command._hasReleased = false;
            return command;
        }

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1> Get(Action<T1> callback, T1 arg1)
        {
            if (callback == null) return null;
            var command = Get(callback);

            command.Arg1 = arg1;
            return command;
        }



        public new T1 Arg1
        {
            get => base.Arg1;
            set => base.Arg1 = value;
        }

        /// <summary>
        /// Sets the stored arguments of the command.
        /// </summary>
        public void SetArgs(T1 arg1)
        {
            Arg1 = arg1;
        }



        public override bool Release()
        {
            if (_hasReleased) return false;
            _hasReleased = true;

            _callback = null;
            Arg1 = default;

            _pool.Push(this);
            return true;
        }



        public bool Equals(ActionCommand<T1> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ActionCommand<T1> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ActionCommand<T1> left, ActionCommand<T1> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ActionCommand<T1> left, ActionCommand<T1> right) => !(left == right);

        public static bool operator ==(ActionCommand<T1> left, ReadOnlyActionCommand<T1> right) => left?._callback == right;
        public static bool operator !=(ActionCommand<T1> left, ReadOnlyActionCommand<T1> right) => !(left == right);

        public static bool operator ==(ReadOnlyActionCommand<T1> left, ActionCommand<T1> right) => left == right?._callback;
        public static bool operator !=(ReadOnlyActionCommand<T1> left, ActionCommand<T1> right) => !(left == right);

        public static bool operator ==(ActionCommand<T1> left, Action<T1> right) => left?._callback == right;
        public static bool operator !=(ActionCommand<T1> left, Action<T1> right) => !(left == right);

        public static bool operator ==(Action<T1> left, ActionCommand<T1> right) => left == right?._callback;
        public static bool operator !=(Action<T1> left, ActionCommand<T1> right) => !(left == right);

        public static implicit operator ActionCommand<T1>(Action<T1> value) => Get(value);
        public static explicit operator Action<T1>(ActionCommand<T1> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<ActionCommand<T1>> _pool = new();
        #endregion
    }

    /// <inheritdoc cref="ReadOnlyActionCommand"/>
    public abstract class ReadOnlyActionCommand<T1> : BaseActionCommand, IEquatable<ReadOnlyActionCommand<T1>>
    {
        #region API
        public T1 Arg1 { get; protected set; }



        public override bool HasReleased => _hasReleased;

        /// <summary>
        /// Invokes the command using the supplied arguments.
        /// </summary>
        [HideInCallstack]
        public void Invoke(T1 arg1) => _callback?.Invoke(arg1);
        public override void Invoke() => _callback?.Invoke(Arg1);

        /// <summary>
        /// Invokes the command using the supplied arguments then releases it afterward.
        /// </summary>
        [HideInCallstack]
        public void Execute(T1 arg1)
        {
            try
            {
                Invoke(arg1);
            }
            finally
            {
                Release();
            }
        }



        public bool Equals(ReadOnlyActionCommand<T1> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ReadOnlyActionCommand<T1> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ReadOnlyActionCommand<T1> left, ReadOnlyActionCommand<T1> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ReadOnlyActionCommand<T1> left, ReadOnlyActionCommand<T1> right) => !(left == right);

        public static bool operator ==(ReadOnlyActionCommand<T1> left, Action<T1> right) => left?._callback == right;
        public static bool operator !=(ReadOnlyActionCommand<T1> left, Action<T1> right) => !(left == right);

        public static bool operator ==(Action<T1> left, ReadOnlyActionCommand<T1> right) => left == right?._callback;
        public static bool operator !=(Action<T1> left, ReadOnlyActionCommand<T1> right) => !(left == right);

        public static explicit operator Action<T1>(ReadOnlyActionCommand<T1> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        protected Action<T1> _callback;
        protected bool _hasReleased;
        #endregion
    }

}