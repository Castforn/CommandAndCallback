using System;
using System.Collections.Generic;
using UnityEngine;

namespace Castforn.CommandAndCallback
{

    /// <inheritdoc cref="ActionCommand"/>
    public sealed class ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> : ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8>, IEquatable<ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8>>
    {
        #region Lifecycle
        ActionCommand(Action<T1, T2, T3, T4, T5, T6, T7, T8> callback) => _callback = callback;
        #endregion

        #region API
        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> Get(Action<T1, T2, T3, T4, T5, T6, T7, T8> callback)
        {
            if (callback == null) return null;
            while (_pool.TryPop(out var command))
            {
                command._callback = callback;
                command._hasReleased = false;
                return command;
            }
            return new(callback);
        }

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> Get(Action<T1, T2, T3, T4, T5, T6, T7, T8> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            if (callback == null) return null;
            var command = Get(callback);

            command.Arg1 = arg1;
            command.Arg2 = arg2;
            command.Arg3 = arg3;
            command.Arg4 = arg4;
            command.Arg5 = arg5;
            command.Arg6 = arg6;
            command.Arg7 = arg7;
            command.Arg8 = arg8;
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

        public new T5 Arg5
        {
            get => base.Arg5;
            set => base.Arg5 = value;
        }

        public new T6 Arg6
        {
            get => base.Arg6;
            set => base.Arg6 = value;
        }

        public new T7 Arg7
        {
            get => base.Arg7;
            set => base.Arg7 = value;
        }

        public new T8 Arg8
        {
            get => base.Arg8;
            set => base.Arg8 = value;
        }

        /// <inheritdoc cref="ActionCommand{T1}.SetArgs(T1)"/>
        public void SetArgs(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
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
            Arg5 = default;
            Arg6 = default;
            Arg7 = default;
            Arg8 = default;

            _pool.Push(this);
            return true;
        }



        public bool Equals(ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> left, ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> left, ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> right) => !(left == right);

        public static bool operator ==(ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> left, ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> right) => left?._callback == right;
        public static bool operator !=(ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> left, ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> right) => !(left == right);

        public static bool operator ==(ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> left, ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> right) => left == right?._callback;
        public static bool operator !=(ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> left, ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> right) => !(left == right);

        public static bool operator ==(ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> left, Action<T1, T2, T3, T4, T5, T6, T7, T8> right) => left?._callback == right;
        public static bool operator !=(ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> left, Action<T1, T2, T3, T4, T5, T6, T7, T8> right) => !(left == right);

        public static bool operator ==(Action<T1, T2, T3, T4, T5, T6, T7, T8> left, ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> right) => left == right?._callback;
        public static bool operator !=(Action<T1, T2, T3, T4, T5, T6, T7, T8> left, ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> right) => !(left == right);

        public static implicit operator ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> value) => Get(value);
        public static explicit operator Action<T1, T2, T3, T4, T5, T6, T7, T8>(ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8>> _pool = new();
        #endregion
    }

    /// <inheritdoc cref="ReadOnlyActionCommand"/>
    public abstract class ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> : BaseActionCommand, IEquatable<ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8>>
    {
        #region API
        public T1 Arg1 { get; protected set; }
        public T2 Arg2 { get; protected set; }
        public T3 Arg3 { get; protected set; }
        public T4 Arg4 { get; protected set; }
        public T5 Arg5 { get; protected set; }
        public T6 Arg6 { get; protected set; }
        public T7 Arg7 { get; protected set; }
        public T8 Arg8 { get; protected set; }



        public override bool HasReleased => _hasReleased;

        /// <inheritdoc cref="ReadOnlyActionCommand{T1}.Invoke(T1)"/>
        [HideInCallstack]
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => _callback?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        public override void Invoke() => _callback?.Invoke(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8);

        /// <inheritdoc cref="ReadOnlyActionCommand{T1}.Execute(T1)"/>
        [HideInCallstack]
        public void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            try
            {
                Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            }
            finally
            {
                Release();
            }
        }



        public bool Equals(ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> left, ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> left, ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> right) => !(left == right);

        public static bool operator ==(ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> left, Action<T1, T2, T3, T4, T5, T6, T7, T8> right) => left?._callback == right;
        public static bool operator !=(ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> left, Action<T1, T2, T3, T4, T5, T6, T7, T8> right) => !(left == right);

        public static bool operator ==(Action<T1, T2, T3, T4, T5, T6, T7, T8> left, ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> right) => left == right?._callback;
        public static bool operator !=(Action<T1, T2, T3, T4, T5, T6, T7, T8> left, ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> right) => !(left == right);

        public static explicit operator Action<T1, T2, T3, T4, T5, T6, T7, T8>(ReadOnlyActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        protected Action<T1, T2, T3, T4, T5, T6, T7, T8> _callback;
        protected bool _hasReleased;
        #endregion
    }

}