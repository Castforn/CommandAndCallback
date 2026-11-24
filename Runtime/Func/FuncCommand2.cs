using System;
using System.Collections.Generic;
using UnityEngine;

namespace Castforn.CommandAndCallback
{

    /// <inheritdoc cref="FuncCommand{TResult}"/>
    public sealed class FuncCommand<T1, T2, TResult> : ReadOnlyFuncCommand<T1, T2, TResult>, IEquatable<FuncCommand<T1, T2, TResult>>
    {
        #region Lifecycle
        FuncCommand(Func<T1, T2, TResult> callback) => _callback = callback;
        #endregion

        #region API
        /// <inheritdoc cref="FuncCommand{T1, TResult}.Get(Func{T1, TResult})"/>
        public static FuncCommand<T1, T2, TResult> Get(Func<T1, T2, TResult> callback)
        {
            if (callback == null) return null;
            if (!_pool.TryPop(out var command)) return new(callback);

            command._callback = callback;
            command._hasReleased = false;
            return command;
        }

        /// <inheritdoc cref="FuncCommand{T1, T2, TResult}.Get(Func{T1, T2, TResult})"/>
        public static FuncCommand<T1, T2, TResult> Get(Func<T1, T2, TResult> callback, T1 arg1, T2 arg2)
        {
            if (callback == null) return null;
            var command = Get(callback);

            command.Arg1 = arg1;
            command.Arg2 = arg2;
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

        /// <inheritdoc cref="FuncCommand{T1, TResult}.SetArgs(T1)"/>
        public void SetArgs(T1 arg1, T2 arg2)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }



        public override bool Release()
        {
            if (_hasReleased) return false;
            _hasReleased = true;

            _callback = null;
            Arg1 = default;
            Arg2 = default;

            _pool.Push(this);
            return true;
        }



        public bool Equals(FuncCommand<T1, T2, TResult> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is FuncCommand<T1, T2, TResult> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(FuncCommand<T1, T2, TResult> left, FuncCommand<T1, T2, TResult> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(FuncCommand<T1, T2, TResult> left, FuncCommand<T1, T2, TResult> right) => !(left == right);

        public static bool operator ==(FuncCommand<T1, T2, TResult> left, ReadOnlyFuncCommand<T1, T2, TResult> right) => left?._callback == right;
        public static bool operator !=(FuncCommand<T1, T2, TResult> left, ReadOnlyFuncCommand<T1, T2, TResult> right) => !(left == right);

        public static bool operator ==(ReadOnlyFuncCommand<T1, T2, TResult> left, FuncCommand<T1, T2, TResult> right) => left == right?._callback;
        public static bool operator !=(ReadOnlyFuncCommand<T1, T2, TResult> left, FuncCommand<T1, T2, TResult> right) => !(left == right);

        public static bool operator ==(FuncCommand<T1, T2, TResult> left, Func<T1, T2, TResult> right) => left?._callback == right;
        public static bool operator !=(FuncCommand<T1, T2, TResult> left, Func<T1, T2, TResult> right) => !(left == right);

        public static bool operator ==(Func<T1, T2, TResult> left, FuncCommand<T1, T2, TResult> right) => left == right?._callback;
        public static bool operator !=(Func<T1, T2, TResult> left, FuncCommand<T1, T2, TResult> right) => !(left == right);

        public static implicit operator FuncCommand<T1, T2, TResult>(Func<T1, T2, TResult> value) => Get(value);
        public static explicit operator Func<T1, T2, TResult>(FuncCommand<T1, T2, TResult> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<FuncCommand<T1, T2, TResult>> _pool = new();
        #endregion
    }

    /// <inheritdoc cref="ReadOnlyFuncCommand{TResult}"/>
    public abstract class ReadOnlyFuncCommand<T1, T2, TResult> : BaseFuncCommand<TResult>, IEquatable<ReadOnlyFuncCommand<T1, T2, TResult>>
    {
        #region API
        public T1 Arg1 { get; protected set; }
        public T2 Arg2 { get; protected set; }



        public override bool HasReleased => _hasReleased;

        /// <inheritdoc cref="ReadOnlyFuncCommand{T1, TResult}.Invoke(T1)"/>
        [HideInCallstack]
        public TResult Invoke(T1 arg1, T2 arg2) => _callback == null ? default : _callback.Invoke(arg1, arg2);
        public override TResult Invoke() => _callback == null ? default : _callback.Invoke(Arg1, Arg2);

        /// <inheritdoc cref="ReadOnlyFuncCommand{T1, TResult}.Execute(T1)"/>
        [HideInCallstack]
        public TResult Execute(T1 arg1, T2 arg2)
        {
            TResult result = default;
            try
            {
                result = Invoke(arg1, arg2);
            }
            finally
            {
                Release();
            }
            return result;
        }



        public bool Equals(ReadOnlyFuncCommand<T1, T2, TResult> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ReadOnlyFuncCommand<T1, T2, TResult> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ReadOnlyFuncCommand<T1, T2, TResult> left, ReadOnlyFuncCommand<T1, T2, TResult> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ReadOnlyFuncCommand<T1, T2, TResult> left, ReadOnlyFuncCommand<T1, T2, TResult> right) => !(left == right);

        public static bool operator ==(ReadOnlyFuncCommand<T1, T2, TResult> left, Func<T1, T2, TResult> right) => left?._callback == right;
        public static bool operator !=(ReadOnlyFuncCommand<T1, T2, TResult> left, Func<T1, T2, TResult> right) => !(left == right);

        public static bool operator ==(Func<T1, T2, TResult> left, ReadOnlyFuncCommand<T1, T2, TResult> right) => left == right?._callback;
        public static bool operator !=(Func<T1, T2, TResult> left, ReadOnlyFuncCommand<T1, T2, TResult> right) => !(left == right);

        public static explicit operator Func<T1, T2, TResult>(ReadOnlyFuncCommand<T1, T2, TResult> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        protected Func<T1, T2, TResult> _callback;
        protected bool _hasReleased;
        #endregion
    }

}