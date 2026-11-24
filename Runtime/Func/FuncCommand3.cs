using System;
using System.Collections.Generic;
using UnityEngine;

namespace Castforn.CommandAndCallback
{

    /// <inheritdoc cref="FuncCommand{TResult}"/>
    public sealed class FuncCommand<T1, T2, T3, TResult> : ReadOnlyFuncCommand<T1, T2, T3, TResult>, IEquatable<FuncCommand<T1, T2, T3, TResult>>
    {
        #region Lifecycle
        FuncCommand(Func<T1, T2, T3, TResult> callback) => _callback = callback;
        #endregion

        #region API
        /// <inheritdoc cref="FuncCommand{T1, TResult}.Get(Func{T1, TResult})"/>
        public static FuncCommand<T1, T2, T3, TResult> Get(Func<T1, T2, T3, TResult> callback)
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

        /// <inheritdoc cref="FuncCommand{T1, T2, T3, TResult}.Get(Func{T1, T2, T3, TResult})"/>
        public static FuncCommand<T1, T2, T3, TResult> Get(Func<T1, T2, T3, TResult> callback, T1 arg1, T2 arg2, T3 arg3)
        {
            if (callback == null) return null;
            var command = Get(callback);

            command.Arg1 = arg1;
            command.Arg2 = arg2;
            command.Arg3 = arg3;
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

        /// <inheritdoc cref="FuncCommand{T1, TResult}.SetArgs(T1)"/>
        public void SetArgs(T1 arg1, T2 arg2, T3 arg3)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }



        public override bool Release()
        {
            if (_hasReleased) return false;
            _hasReleased = true;

            _callback = null;
            Arg1 = default;
            Arg2 = default;
            Arg3 = default;

            _pool.Push(this);
            return true;
        }



        public bool Equals(FuncCommand<T1, T2, T3, TResult> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is FuncCommand<T1, T2, T3, TResult> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(FuncCommand<T1, T2, T3, TResult> left, FuncCommand<T1, T2, T3, TResult> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(FuncCommand<T1, T2, T3, TResult> left, FuncCommand<T1, T2, T3, TResult> right) => !(left == right);

        public static bool operator ==(FuncCommand<T1, T2, T3, TResult> left, ReadOnlyFuncCommand<T1, T2, T3, TResult> right) => left?._callback == right;
        public static bool operator !=(FuncCommand<T1, T2, T3, TResult> left, ReadOnlyFuncCommand<T1, T2, T3, TResult> right) => !(left == right);

        public static bool operator ==(ReadOnlyFuncCommand<T1, T2, T3, TResult> left, FuncCommand<T1, T2, T3, TResult> right) => left == right?._callback;
        public static bool operator !=(ReadOnlyFuncCommand<T1, T2, T3, TResult> left, FuncCommand<T1, T2, T3, TResult> right) => !(left == right);

        public static bool operator ==(FuncCommand<T1, T2, T3, TResult> left, Func<T1, T2, T3, TResult> right) => left?._callback == right;
        public static bool operator !=(FuncCommand<T1, T2, T3, TResult> left, Func<T1, T2, T3, TResult> right) => !(left == right);

        public static bool operator ==(Func<T1, T2, T3, TResult> left, FuncCommand<T1, T2, T3, TResult> right) => left == right?._callback;
        public static bool operator !=(Func<T1, T2, T3, TResult> left, FuncCommand<T1, T2, T3, TResult> right) => !(left == right);

        public static implicit operator FuncCommand<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> value) => Get(value);
        public static explicit operator Func<T1, T2, T3, TResult>(FuncCommand<T1, T2, T3, TResult> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<FuncCommand<T1, T2, T3, TResult>> _pool = new();
        #endregion
    }

    /// <inheritdoc cref="ReadOnlyFuncCommand{TResult}"/>
    public abstract class ReadOnlyFuncCommand<T1, T2, T3, TResult> : BaseFuncCommand<TResult>, IEquatable<ReadOnlyFuncCommand<T1, T2, T3, TResult>>
    {
        #region API
        public T1 Arg1 { get; protected set; }
        public T2 Arg2 { get; protected set; }
        public T3 Arg3 { get; protected set; }



        public override bool HasReleased => _hasReleased;

        /// <inheritdoc cref="ReadOnlyFuncCommand{T1, TResult}.Invoke(T1)"/>
        [HideInCallstack]
        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3) => _callback == null ? default : _callback.Invoke(arg1, arg2, arg3);
        public override TResult Invoke() => _callback == null ? default : _callback.Invoke(Arg1, Arg2, Arg3);

        /// <inheritdoc cref="ReadOnlyFuncCommand{T1, TResult}.Execute(T1)"/>
        [HideInCallstack]
        public void Execute(T1 arg1, T2 arg2, T3 arg3)
        {
            try
            {
                Invoke(arg1, arg2, arg3);
            }
            finally
            {
                Release();
            }
        }



        public bool Equals(ReadOnlyFuncCommand<T1, T2, T3, TResult> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ReadOnlyFuncCommand<T1, T2, T3, TResult> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ReadOnlyFuncCommand<T1, T2, T3, TResult> left, ReadOnlyFuncCommand<T1, T2, T3, TResult> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ReadOnlyFuncCommand<T1, T2, T3, TResult> left, ReadOnlyFuncCommand<T1, T2, T3, TResult> right) => !(left == right);

        public static bool operator ==(ReadOnlyFuncCommand<T1, T2, T3, TResult> left, Func<T1, T2, T3, TResult> right) => left?._callback == right;
        public static bool operator !=(ReadOnlyFuncCommand<T1, T2, T3, TResult> left, Func<T1, T2, T3, TResult> right) => !(left == right);

        public static bool operator ==(Func<T1, T2, T3, TResult> left, ReadOnlyFuncCommand<T1, T2, T3, TResult> right) => left == right?._callback;
        public static bool operator !=(Func<T1, T2, T3, TResult> left, ReadOnlyFuncCommand<T1, T2, T3, TResult> right) => !(left == right);

        public static explicit operator Func<T1, T2, T3, TResult>(ReadOnlyFuncCommand<T1, T2, T3, TResult> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        protected Func<T1, T2, T3, TResult> _callback;
        protected bool _hasReleased;
        #endregion
    }

}