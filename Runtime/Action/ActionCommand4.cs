using System;
using System.Collections.Generic;
using UnityEngine;

namespace Castforn.CommandAndCallback
{

    /// <inheritdoc cref="ActionCommand"/>
    public sealed class ActionCommand<T1, T2, T3, T4> : ReadOnlyActionCommand<T1, T2, T3, T4>, IEquatable<ActionCommand<T1, T2, T3, T4>>
    {
        #region Lifecycle
        ActionCommand(Action<T1, T2, T3, T4> callback) => _callback = callback;
        #endregion

        #region API
        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4> Get(Action<T1, T2, T3, T4> callback)
        {
            if (callback == null) return null;
            if (!_pool.TryPop(out var command)) return new(callback);

            command._callback = callback;
            command._hasReleased = false;
            return command;
        }

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4> Get(Action<T1, T2, T3, T4> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (callback == null) return null;
            var command = Get(callback);

            command.Arg1 = arg1;
            command.Arg2 = arg2;
            command.Arg3 = arg3;
            command.Arg4 = arg4;
            return command;
        }



        public new T1 Arg1
        {
            get => base.Arg1;
            set => base.Arg1 = value;
        }

        public new T2 Arg2
        {
            get => base.Arg2;
            set => base.Arg2 = value;
        }

        public new T3 Arg3
        {
            get => base.Arg3;
            set => base.Arg3 = value;
        }

        public new T4 Arg4
        {
            get => base.Arg4;
            set => base.Arg4 = value;
        }

        /// <inheritdoc cref="ActionCommand{T1}.SetArgs(T1)"/>
        public void SetArgs(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }



        public override bool Release()
        {
            if (_hasReleased) return false;
            _hasReleased = true;

            _callback = null;
            Arg1 = default;
            Arg2 = default;
            Arg3 = default;
            Arg4 = default;

            _pool.Push(this);
            return true;
        }



        public bool Equals(ActionCommand<T1, T2, T3, T4> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ActionCommand<T1, T2, T3, T4> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ActionCommand<T1, T2, T3, T4> left, ActionCommand<T1, T2, T3, T4> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ActionCommand<T1, T2, T3, T4> left, ActionCommand<T1, T2, T3, T4> right) => !(left == right);

        public static bool operator ==(ActionCommand<T1, T2, T3, T4> left, ReadOnlyActionCommand<T1, T2, T3, T4> right) => left?._callback == right;
        public static bool operator !=(ActionCommand<T1, T2, T3, T4> left, ReadOnlyActionCommand<T1, T2, T3, T4> right) => !(left == right);

        public static bool operator ==(ReadOnlyActionCommand<T1, T2, T3, T4> left, ActionCommand<T1, T2, T3, T4> right) => left == right?._callback;
        public static bool operator !=(ReadOnlyActionCommand<T1, T2, T3, T4> left, ActionCommand<T1, T2, T3, T4> right) => !(left == right);

        public static bool operator ==(ActionCommand<T1, T2, T3, T4> left, Action<T1, T2, T3, T4> right) => left?._callback == right;
        public static bool operator !=(ActionCommand<T1, T2, T3, T4> left, Action<T1, T2, T3, T4> right) => !(left == right);

        public static bool operator ==(Action<T1, T2, T3, T4> left, ActionCommand<T1, T2, T3, T4> right) => left == right?._callback;
        public static bool operator !=(Action<T1, T2, T3, T4> left, ActionCommand<T1, T2, T3, T4> right) => !(left == right);

        public static implicit operator ActionCommand<T1, T2, T3, T4>(Action<T1, T2, T3, T4> value) => Get(value);
        public static explicit operator Action<T1, T2, T3, T4>(ActionCommand<T1, T2, T3, T4> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<ActionCommand<T1, T2, T3, T4>> _pool = new();
        #endregion
    }

    /// <inheritdoc cref="ReadOnlyActionCommand"/>
    public abstract class ReadOnlyActionCommand<T1, T2, T3, T4> : BaseActionCommand, IEquatable<ReadOnlyActionCommand<T1, T2, T3, T4>>
    {
        #region API
        public T1 Arg1 { get; protected set; }
        public T2 Arg2 { get; protected set; }
        public T3 Arg3 { get; protected set; }
        public T4 Arg4 { get; protected set; }



        public override bool HasReleased => _hasReleased;

        /// <inheritdoc cref="ReadOnlyActionCommand{T1}.Invoke(T1)"/>
        [HideInCallstack]
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => _callback?.Invoke(arg1, arg2, arg3, arg4);
        public override void Invoke() => _callback?.Invoke(Arg1, Arg2, Arg3, Arg4);

        /// <inheritdoc cref="ReadOnlyActionCommand{T1}.Execute(T1)"/>
        [HideInCallstack]
        public void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            try
            {
                Invoke(arg1, arg2, arg3, arg4);
            }
            finally
            {
                Release();
            }
        }



        public bool Equals(ReadOnlyActionCommand<T1, T2, T3, T4> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ReadOnlyActionCommand<T1, T2, T3, T4> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ReadOnlyActionCommand<T1, T2, T3, T4> left, ReadOnlyActionCommand<T1, T2, T3, T4> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ReadOnlyActionCommand<T1, T2, T3, T4> left, ReadOnlyActionCommand<T1, T2, T3, T4> right) => !(left == right);

        public static bool operator ==(ReadOnlyActionCommand<T1, T2, T3, T4> left, Action<T1, T2, T3, T4> right) => left?._callback == right;
        public static bool operator !=(ReadOnlyActionCommand<T1, T2, T3, T4> left, Action<T1, T2, T3, T4> right) => !(left == right);

        public static bool operator ==(Action<T1, T2, T3, T4> left, ReadOnlyActionCommand<T1, T2, T3, T4> right) => left == right?._callback;
        public static bool operator !=(Action<T1, T2, T3, T4> left, ReadOnlyActionCommand<T1, T2, T3, T4> right) => !(left == right);

        public static explicit operator Action<T1, T2, T3, T4>(ReadOnlyActionCommand<T1, T2, T3, T4> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        protected Action<T1, T2, T3, T4> _callback;
        protected bool _hasReleased;
        #endregion
    }

}