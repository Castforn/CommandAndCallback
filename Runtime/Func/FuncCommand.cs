using System;
using System.Collections.Generic;

namespace Castforn.CommandAndCallback
{

    /// <summary>
    /// Represents a command that invokes a function delegate and returns a result.
    /// </summary>
    public sealed class FuncCommand<TResult> : ReadOnlyFuncCommand<TResult>, IEquatable<FuncCommand<TResult>>
    {
        #region Lifecycle
        FuncCommand(Func<TResult> callback) => _callback = callback;
        #endregion

        #region API
        /// <summary>
        /// Acquires a func command from the pool, creating a new instance if necessary.
        /// </summary>
        /// <returns>
        /// An acquired func command configured with the specified callback.
        /// </returns>
        public static FuncCommand<TResult> Get(Func<TResult> callback)
        {
            if (callback == null) return null;
            if (!_pool.TryPop(out var command)) return new(callback);

            command._callback = callback;
            command._hasReleased = false;
            return command;
        }

        

        public override bool Release()
        {
            if (_hasReleased) return false;
            _hasReleased = true;

            _callback = null;

            _pool.Push(this);
            return true;
        }

        public bool Equals(FuncCommand<TResult> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is FuncCommand<TResult> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(FuncCommand<TResult> left, FuncCommand<TResult> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(FuncCommand<TResult> left, FuncCommand<TResult> right) => !(left == right);

        public static bool operator ==(FuncCommand<TResult> left, ReadOnlyFuncCommand<TResult> right) => left?._callback == right;
        public static bool operator !=(FuncCommand<TResult> left, ReadOnlyFuncCommand<TResult> right) => !(left == right);

        public static bool operator ==(ReadOnlyFuncCommand<TResult> left, FuncCommand<TResult> right) => left == right?._callback;
        public static bool operator !=(ReadOnlyFuncCommand<TResult> left, FuncCommand<TResult> right) => !(left == right);

        public static bool operator ==(FuncCommand<TResult> left, Func<TResult> right) => left?._callback == right;
        public static bool operator !=(FuncCommand<TResult> left, Func<TResult> right) => !(left == right);

        public static bool operator ==(Func<TResult> left, FuncCommand<TResult> right) => left == right?._callback;
        public static bool operator !=(Func<TResult> left, FuncCommand<TResult> right) => !(left == right);

        public static implicit operator FuncCommand<TResult>(Func<TResult> value) => Get(value);
        public static explicit operator Func<TResult>(FuncCommand<TResult> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<FuncCommand<TResult>> _pool = new();
        #endregion
    }

    /// <summary>
    /// Represents a read-only command that invokes a function delegate and returns a result.
    /// </summary>
    public abstract class ReadOnlyFuncCommand<TResult> : BaseFuncCommand<TResult>, IEquatable<ReadOnlyFuncCommand<TResult>>
    {
        #region API
        public override bool HasReleased => _hasReleased;
        public override TResult Invoke() => _callback == null ? default : _callback.Invoke();



        public bool Equals(ReadOnlyFuncCommand<TResult> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ReadOnlyFuncCommand<TResult> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ReadOnlyFuncCommand<TResult> left, ReadOnlyFuncCommand<TResult> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ReadOnlyFuncCommand<TResult> left, ReadOnlyFuncCommand<TResult> right) => !(left == right);

        public static bool operator ==(ReadOnlyFuncCommand<TResult> left, Func<TResult> right) => left?._callback == right;
        public static bool operator !=(ReadOnlyFuncCommand<TResult> left, Func<TResult> right) => !(left == right);

        public static bool operator ==(Func<TResult> left, ReadOnlyFuncCommand<TResult> right) => left == right?._callback;
        public static bool operator !=(Func<TResult> left, ReadOnlyFuncCommand<TResult> right) => !(left == right);

        public static explicit operator Func<TResult>(ReadOnlyFuncCommand<TResult> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        protected Func<TResult> _callback;
        protected bool _hasReleased;
        #endregion
    }

}