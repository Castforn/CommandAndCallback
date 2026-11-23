using System;
using System.Collections.Generic;
using UnityEngine;

namespace Castforn.CommandAndCallback
{

    /// <summary>
    /// Represents a command that invokes a function delegate and returns a result.
    /// </summary>
    public sealed class FuncCommand<TResult> : BaseFuncCommand<TResult>, IEquatable<FuncCommand<TResult>>
    {
        #region Lifecycle
        FuncCommand(Func<TResult> callback)
        {
            _callback = callback;
        }
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
            while (_pool.TryPop(out var command))
            {
                command._callback = callback;
                command._hasReleased = false;
                return command;
            }
            return new(callback);
        }

        public override bool HasReleased => _hasReleased;
        public override TResult Invoke() => _callback == null ? default : _callback.Invoke();

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
        Func<TResult> _callback;
        bool _hasReleased;
        #endregion
    }

    /// <inheritdoc cref="FuncCommand{TResult}"/>
    public sealed class FuncCommand<T1, TResult> : BaseFuncCommand<TResult>, IEquatable<FuncCommand<T1, TResult>>
    {
        #region Lifecycle
        FuncCommand(Func<T1, TResult> callback, T1 arg1)
        {
            _callback = callback;
            Arg1 = arg1;
        }
        #endregion

        #region API
        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, TResult> Get(Func<T1, TResult> callback) => Get(callback, default);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, TResult> Get(Func<T1, TResult> callback, T1 arg1)
        {
            if (callback == null) return null;
            while (_pool.TryPop(out var command))
            {
                command.Arg1 = arg1;
                command._callback = callback;
                command._hasReleased = false;
                return command;
            }
            return new(callback, arg1);
        }

        public override bool HasReleased => _hasReleased;
        public T1 Arg1;

        /// <summary>
        /// Sets the stored arguments of the command.
        /// </summary>
        public void SetArgs(T1 arg1)
        {
            Arg1 = arg1;
        }

        /// <summary>
        /// Invokes the command using the supplied arguments and returns it's result.
        /// </summary>
        [HideInCallstack]
        public TResult Invoke(T1 arg1) => _callback == null ? default : _callback.Invoke(arg1);
        public override TResult Invoke() => _callback == null ? default : _callback.Invoke(Arg1);

        /// <summary>
        /// Invokes the command using the supplied arguments and returns it's result then releases it afterward.
        /// </summary>
        [HideInCallstack]
        public TResult Execute(T1 arg1)
        {
            TResult result;
#if UNITY_EDITOR
            try { result = Invoke(arg1); }
            finally { Release(); }
#else
            result = Invoke(arg1);
            Release();
#endif
            return result;
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

        public bool Equals(FuncCommand<T1, TResult> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is FuncCommand<T1, TResult> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(FuncCommand<T1, TResult> left, FuncCommand<T1, TResult> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(FuncCommand<T1, TResult> left, FuncCommand<T1, TResult> right) => !(left == right);

        public static bool operator ==(FuncCommand<T1, TResult> left, Func<T1, TResult> right) => left?._callback == right;
        public static bool operator !=(FuncCommand<T1, TResult> left, Func<T1, TResult> right) => !(left == right);

        public static bool operator ==(Func<T1, TResult> left, FuncCommand<T1, TResult> right) => left == right?._callback;
        public static bool operator !=(Func<T1, TResult> left, FuncCommand<T1, TResult> right) => !(left == right);

        public static implicit operator FuncCommand<T1, TResult>(Func<T1, TResult> value) => Get(value);
        public static explicit operator Func<T1, TResult>(FuncCommand<T1, TResult> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<FuncCommand<T1, TResult>> _pool = new();
        Func<T1, TResult> _callback;
        bool _hasReleased;
        #endregion
    }

    /// <inheritdoc cref="FuncCommand{TResult}"/>
    public sealed class FuncCommand<T1, T2, TResult> : BaseFuncCommand<TResult>, IEquatable<FuncCommand<T1, T2, TResult>>
    {
        #region Lifecycle
        FuncCommand(Func<T1, T2, TResult> callback, T1 arg1, T2 arg2)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
        }
        #endregion

        #region API
        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, TResult> Get(Func<T1, T2, TResult> callback) => Get(callback, default, default);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, TResult> Get(Func<T1, T2, TResult> callback, T1 arg1, T2 arg2)
        {
            if (callback == null) return null;
            while (_pool.TryPop(out var command))
            {
                command.Arg1 = arg1;
                command.Arg2 = arg2;
                command._callback = callback;
                command._hasReleased = false;
                return command;
            }
            return new(callback, arg1, arg2);
        }

        public override bool HasReleased => _hasReleased;
        public T1 Arg1;
        public T2 Arg2;

        /// <inheritdoc cref="FuncCommand{T1, TResult}.SetArgs(T1)"/>
        public void SetArgs(T1 arg1, T2 arg2)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }

        /// <inheritdoc cref="FuncCommand{T1, TResult}.Invoke(T1)"/>
        [HideInCallstack]
        public TResult Invoke(T1 arg1, T2 arg2) => _callback == null ? default : _callback.Invoke(arg1, arg2);
        public override TResult Invoke() => _callback == null ? default : _callback.Invoke(Arg1, Arg2);

        /// <inheritdoc cref="FuncCommand{T1, TResult}.Execute(T1)"/>
        [HideInCallstack]
        public TResult Execute(T1 arg1, T2 arg2)
        {
            TResult result;
#if UNITY_EDITOR
            try { result = Invoke(arg1, arg2); }
            finally { Release(); }
#else
            result = Invoke(arg1, arg2);
            Release();
#endif
            return result;
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
        Func<T1, T2, TResult> _callback;
        bool _hasReleased;
        #endregion
    }

    /// <inheritdoc cref="FuncCommand{TResult}"/>
    public sealed class FuncCommand<T1, T2, T3, TResult> : BaseFuncCommand<TResult>, IEquatable<FuncCommand<T1, T2, T3, TResult>>
    {
        #region Lifecycle
        FuncCommand(Func<T1, T2, T3, TResult> callback, T1 arg1, T2 arg2, T3 arg3)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }
        #endregion

        #region API
        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, TResult> Get(Func<T1, T2, T3, TResult> callback) => Get(callback, default, default, default);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, TResult> Get(Func<T1, T2, T3, TResult> callback, T1 arg1, T2 arg2, T3 arg3)
        {
            if (callback == null) return null;
            while (_pool.TryPop(out var command))
            {
                command.Arg1 = arg1;
                command.Arg2 = arg2;
                command.Arg3 = arg3;
                command._callback = callback;
                command._hasReleased = false;
                return command;
            }
            return new(callback, arg1, arg2, arg3);
        }

        public override bool HasReleased => _hasReleased;
        public T1 Arg1;
        public T2 Arg2;
        public T3 Arg3;

        /// <inheritdoc cref="FuncCommand{T1, TResult}.SetArgs(T1)"/>
        public void SetArgs(T1 arg1, T2 arg2, T3 arg3)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }

        /// <inheritdoc cref="FuncCommand{T1, TResult}.Invoke(T1)"/>
        [HideInCallstack]
        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3) => _callback == null ? default : _callback.Invoke(arg1, arg2, arg3);
        public override TResult Invoke() => _callback == null ? default : _callback.Invoke(Arg1, Arg2, Arg3);

        /// <inheritdoc cref="FuncCommand{T1, TResult}.Execute(T1)"/>
        [HideInCallstack]
        public TResult Execute(T1 arg1, T2 arg2, T3 arg3)
        {
            TResult result;
#if UNITY_EDITOR
            try { result = Invoke(arg1, arg2, arg3); }
            finally { Release(); }
#else
            result = Invoke(arg1, arg2, arg3);
            Release();
#endif
            return result;
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
        Func<T1, T2, T3, TResult> _callback;
        bool _hasReleased;
        #endregion
    }

    /// <inheritdoc cref="FuncCommand{TResult}"/>
    public sealed class FuncCommand<T1, T2, T3, T4, TResult> : BaseFuncCommand<TResult>, IEquatable<FuncCommand<T1, T2, T3, T4, TResult>>
    {
        #region Lifecycle
        FuncCommand(Func<T1, T2, T3, T4, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }
        #endregion

        #region API
        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, TResult> Get(Func<T1, T2, T3, T4, TResult> callback) => Get(callback, default, default, default, default);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, TResult> Get(Func<T1, T2, T3, T4, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (callback == null) return null;
            while (_pool.TryPop(out var command))
            {
                command.Arg1 = arg1;
                command.Arg2 = arg2;
                command.Arg3 = arg3;
                command.Arg4 = arg4;
                command._callback = callback;
                command._hasReleased = false;
                return command;
            }
            return new(callback, arg1, arg2, arg3, arg4);
        }

        public override bool HasReleased => _hasReleased;
        public T1 Arg1;
        public T2 Arg2;
        public T3 Arg3;
        public T4 Arg4;

        /// <inheritdoc cref="FuncCommand{T1, TResult}.SetArgs(T1)"/>
        public void SetArgs(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }

        /// <inheritdoc cref="FuncCommand{T1, TResult}.Invoke(T1)"/>
        [HideInCallstack]
        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => _callback == null ? default : _callback.Invoke(arg1, arg2, arg3, arg4);
        public override TResult Invoke() => _callback == null ? default : _callback.Invoke(Arg1, Arg2, Arg3, Arg4);

        /// <inheritdoc cref="FuncCommand{T1, TResult}.Execute(T1)"/>
        [HideInCallstack]
        public TResult Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            TResult result;
#if UNITY_EDITOR
            try { result = Invoke(arg1, arg2, arg3, arg4); }
            finally { Release(); }
#else
            result = Invoke(arg1, arg2, arg3, arg4);
            Release();
#endif
            return result;
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

        public bool Equals(FuncCommand<T1, T2, T3, T4, TResult> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is FuncCommand<T1, T2, T3, T4, TResult> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(FuncCommand<T1, T2, T3, T4, TResult> left, FuncCommand<T1, T2, T3, T4, TResult> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(FuncCommand<T1, T2, T3, T4, TResult> left, FuncCommand<T1, T2, T3, T4, TResult> right) => !(left == right);

        public static bool operator ==(FuncCommand<T1, T2, T3, T4, TResult> left, Func<T1, T2, T3, T4, TResult> right) => left?._callback == right;
        public static bool operator !=(FuncCommand<T1, T2, T3, T4, TResult> left, Func<T1, T2, T3, T4, TResult> right) => !(left == right);

        public static bool operator ==(Func<T1, T2, T3, T4, TResult> left, FuncCommand<T1, T2, T3, T4, TResult> right) => left == right?._callback;
        public static bool operator !=(Func<T1, T2, T3, T4, TResult> left, FuncCommand<T1, T2, T3, T4, TResult> right) => !(left == right);

        public static implicit operator FuncCommand<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> value) => Get(value);
        public static explicit operator Func<T1, T2, T3, T4, TResult>(FuncCommand<T1, T2, T3, T4, TResult> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<FuncCommand<T1, T2, T3, T4, TResult>> _pool = new();
        Func<T1, T2, T3, T4, TResult> _callback;
        bool _hasReleased;
        #endregion
    }

    /// <inheritdoc cref="FuncCommand{TResult}"/>
    public sealed class FuncCommand<T1, T2, T3, T4, T5, TResult> : BaseFuncCommand<TResult>, IEquatable<FuncCommand<T1, T2, T3, T4, T5, TResult>>
    {
        #region Lifecycle
        FuncCommand(Func<T1, T2, T3, T4, T5, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
        }
        #endregion

        #region API
        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, T5, TResult> Get(Func<T1, T2, T3, T4, T5, TResult> callback) => Get(callback, default, default, default, default, default);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, T5, TResult> Get(Func<T1, T2, T3, T4, T5, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (callback == null) return null;
            while (_pool.TryPop(out var command))
            {
                command.Arg1 = arg1;
                command.Arg2 = arg2;
                command.Arg3 = arg3;
                command.Arg4 = arg4;
                command.Arg5 = arg5;
                command._callback = callback;
                command._hasReleased = false;
                return command;
            }
            return new(callback, arg1, arg2, arg3, arg4, arg5);
        }

        public override bool HasReleased => _hasReleased;
        public T1 Arg1;
        public T2 Arg2;
        public T3 Arg3;
        public T4 Arg4;
        public T5 Arg5;

        /// <inheritdoc cref="FuncCommand{T1, TResult}.SetArgs(T1)"/>
        public void SetArgs(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
        }

        /// <inheritdoc cref="FuncCommand{T1, TResult}.Invoke(T1)"/>
        [HideInCallstack]
        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => _callback == null ? default : _callback.Invoke(arg1, arg2, arg3, arg4, arg5);
        public override TResult Invoke() => _callback == null ? default : _callback.Invoke(Arg1, Arg2, Arg3, Arg4, Arg5);

        /// <inheritdoc cref="FuncCommand{T1, TResult}.Execute(T1)"/>
        [HideInCallstack]
        public TResult Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            TResult result;
#if UNITY_EDITOR
            try { result = Invoke(arg1, arg2, arg3, arg4, arg5); }
            finally { Release(); }
#else
            result = Invoke(arg1, arg2, arg3, arg4, arg5);
            Release();
#endif
            return result;
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

            _pool.Push(this);
            return true;
        }

        public bool Equals(FuncCommand<T1, T2, T3, T4, T5, TResult> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is FuncCommand<T1, T2, T3, T4, T5, TResult> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(FuncCommand<T1, T2, T3, T4, T5, TResult> left, FuncCommand<T1, T2, T3, T4, T5, TResult> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(FuncCommand<T1, T2, T3, T4, T5, TResult> left, FuncCommand<T1, T2, T3, T4, T5, TResult> right) => !(left == right);

        public static bool operator ==(FuncCommand<T1, T2, T3, T4, T5, TResult> left, Func<T1, T2, T3, T4, T5, TResult> right) => left?._callback == right;
        public static bool operator !=(FuncCommand<T1, T2, T3, T4, T5, TResult> left, Func<T1, T2, T3, T4, T5, TResult> right) => !(left == right);

        public static bool operator ==(Func<T1, T2, T3, T4, T5, TResult> left, FuncCommand<T1, T2, T3, T4, T5, TResult> right) => left == right?._callback;
        public static bool operator !=(Func<T1, T2, T3, T4, T5, TResult> left, FuncCommand<T1, T2, T3, T4, T5, TResult> right) => !(left == right);

        public static implicit operator FuncCommand<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> value) => Get(value);
        public static explicit operator Func<T1, T2, T3, T4, T5, TResult>(FuncCommand<T1, T2, T3, T4, T5, TResult> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<FuncCommand<T1, T2, T3, T4, T5, TResult>> _pool = new();
        Func<T1, T2, T3, T4, T5, TResult> _callback;
        bool _hasReleased;
        #endregion
    }

    /// <inheritdoc cref="FuncCommand{TResult}"/>
    public sealed class FuncCommand<T1, T2, T3, T4, T5, T6, TResult> : BaseFuncCommand<TResult>, IEquatable<FuncCommand<T1, T2, T3, T4, T5, T6, TResult>>
    {
        #region Lifecycle
        FuncCommand(Func<T1, T2, T3, T4, T5, T6, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
        }
        #endregion

        #region API
        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, T5, T6, TResult> Get(Func<T1, T2, T3, T4, T5, T6, TResult> callback) => Get(callback, default, default, default, default, default, default);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, T5, T6, TResult> Get(Func<T1, T2, T3, T4, T5, T6, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            if (callback == null) return null;
            while (_pool.TryPop(out var command))
            {
                command.Arg1 = arg1;
                command.Arg2 = arg2;
                command.Arg3 = arg3;
                command.Arg4 = arg4;
                command.Arg5 = arg5;
                command.Arg6 = arg6;
                command._callback = callback;
                command._hasReleased = false;
                return command;
            }
            return new(callback, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public override bool HasReleased => _hasReleased;
        public T1 Arg1;
        public T2 Arg2;
        public T3 Arg3;
        public T4 Arg4;
        public T5 Arg5;
        public T6 Arg6;

        /// <inheritdoc cref="FuncCommand{T1, TResult}.SetArgs(T1)"/>
        public void SetArgs(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
        }

        /// <inheritdoc cref="FuncCommand{T1, TResult}.Invoke(T1)"/>
        [HideInCallstack]
        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => _callback == null ? default : _callback.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
        public override TResult Invoke() => _callback == null ? default : _callback.Invoke(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6);

        /// <inheritdoc cref="FuncCommand{T1, TResult}.Execute(T1)"/>
        [HideInCallstack]
        public TResult Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            TResult result;
#if UNITY_EDITOR
            try { result = Invoke(arg1, arg2, arg3, arg4, arg5, arg6); }
            finally { Release(); }
#else
            result = Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
            Release();
#endif
            return result;
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

            _pool.Push(this);
            return true;
        }

        public bool Equals(FuncCommand<T1, T2, T3, T4, T5, T6, TResult> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is FuncCommand<T1, T2, T3, T4, T5, T6, TResult> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(FuncCommand<T1, T2, T3, T4, T5, T6, TResult> left, FuncCommand<T1, T2, T3, T4, T5, T6, TResult> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(FuncCommand<T1, T2, T3, T4, T5, T6, TResult> left, FuncCommand<T1, T2, T3, T4, T5, T6, TResult> right) => !(left == right);

        public static bool operator ==(FuncCommand<T1, T2, T3, T4, T5, T6, TResult> left, Func<T1, T2, T3, T4, T5, T6, TResult> right) => left?._callback == right;
        public static bool operator !=(FuncCommand<T1, T2, T3, T4, T5, T6, TResult> left, Func<T1, T2, T3, T4, T5, T6, TResult> right) => !(left == right);

        public static bool operator ==(Func<T1, T2, T3, T4, T5, T6, TResult> left, FuncCommand<T1, T2, T3, T4, T5, T6, TResult> right) => left == right?._callback;
        public static bool operator !=(Func<T1, T2, T3, T4, T5, T6, TResult> left, FuncCommand<T1, T2, T3, T4, T5, T6, TResult> right) => !(left == right);

        public static implicit operator FuncCommand<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> value) => Get(value);
        public static explicit operator Func<T1, T2, T3, T4, T5, T6, TResult>(FuncCommand<T1, T2, T3, T4, T5, T6, TResult> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<FuncCommand<T1, T2, T3, T4, T5, T6, TResult>> _pool = new();
        Func<T1, T2, T3, T4, T5, T6, TResult> _callback;
        bool _hasReleased;
        #endregion
    }

    /// <inheritdoc cref="FuncCommand{TResult}"/>
    public sealed class FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult> : BaseFuncCommand<TResult>, IEquatable<FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult>>
    {
        #region Lifecycle
        FuncCommand(Func<T1, T2, T3, T4, T5, T6, T7, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
        }
        #endregion

        #region API
        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult> Get(Func<T1, T2, T3, T4, T5, T6, T7, TResult> callback) => Get(callback, default, default, default, default, default, default, default);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult> Get(Func<T1, T2, T3, T4, T5, T6, T7, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            if (callback == null) return null;
            while (_pool.TryPop(out var command))
            {
                command.Arg1 = arg1;
                command.Arg2 = arg2;
                command.Arg3 = arg3;
                command.Arg4 = arg4;
                command.Arg5 = arg5;
                command.Arg6 = arg6;
                command.Arg7 = arg7;
                command._callback = callback;
                command._hasReleased = false;
                return command;
            }
            return new(callback, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public override bool HasReleased => _hasReleased;
        public T1 Arg1;
        public T2 Arg2;
        public T3 Arg3;
        public T4 Arg4;
        public T5 Arg5;
        public T6 Arg6;
        public T7 Arg7;

        /// <inheritdoc cref="FuncCommand{T1, TResult}.SetArgs(T1)"/>
        public void SetArgs(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
        }

        /// <inheritdoc cref="FuncCommand{T1, TResult}.Invoke(T1)"/>
        [HideInCallstack]
        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => _callback == null ? default : _callback.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        public override TResult Invoke() => _callback == null ? default : _callback.Invoke(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7);

        /// <inheritdoc cref="FuncCommand{T1, TResult}.Execute(T1)"/>
        [HideInCallstack]
        public TResult Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            TResult result;
#if UNITY_EDITOR
            try { result = Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7); }
            finally { Release(); }
#else
            result = Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            Release();
#endif
            return result;
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

            _pool.Push(this);
            return true;
        }

        public bool Equals(FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult> left, FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult> left, FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult> right) => !(left == right);

        public static bool operator ==(FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult> left, Func<T1, T2, T3, T4, T5, T6, T7, TResult> right) => left?._callback == right;
        public static bool operator !=(FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult> left, Func<T1, T2, T3, T4, T5, T6, T7, TResult> right) => !(left == right);

        public static bool operator ==(Func<T1, T2, T3, T4, T5, T6, T7, TResult> left, FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult> right) => left == right?._callback;
        public static bool operator !=(Func<T1, T2, T3, T4, T5, T6, T7, TResult> left, FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult> right) => !(left == right);

        public static implicit operator FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> value) => Get(value);
        public static explicit operator Func<T1, T2, T3, T4, T5, T6, T7, TResult>(FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult>> _pool = new();
        Func<T1, T2, T3, T4, T5, T6, T7, TResult> _callback;
        bool _hasReleased;
        #endregion
    }

    /// <inheritdoc cref="FuncCommand{TResult}"/>
    public sealed class FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult> : BaseFuncCommand<TResult>, IEquatable<FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>
    {
        #region Lifecycle
        FuncCommand(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
        }
        #endregion

        #region API
        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Get(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> callback) => Get(callback, default, default, default, default, default, default, default, default);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Get(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            if (callback == null) return null;
            while (_pool.TryPop(out var command))
            {
                command.Arg1 = arg1;
                command.Arg2 = arg2;
                command.Arg3 = arg3;
                command.Arg4 = arg4;
                command.Arg5 = arg5;
                command.Arg6 = arg6;
                command.Arg7 = arg7;
                command.Arg8 = arg8;
                command._callback = callback;
                command._hasReleased = false;
                return command;
            }
            return new(callback, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public override bool HasReleased => _hasReleased;
        public T1 Arg1;
        public T2 Arg2;
        public T3 Arg3;
        public T4 Arg4;
        public T5 Arg5;
        public T6 Arg6;
        public T7 Arg7;
        public T8 Arg8;

        /// <inheritdoc cref="FuncCommand{T1, TResult}.SetArgs(T1)"/>
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

        /// <inheritdoc cref="FuncCommand{T1, TResult}.Invoke(T1)"/>
        [HideInCallstack]
        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => _callback == null ? default : _callback.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        public override TResult Invoke() => _callback == null ? default : _callback.Invoke(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8);

        /// <inheritdoc cref="FuncCommand{T1, TResult}.Execute(T1)"/>
        [HideInCallstack]
        public TResult Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            TResult result;
#if UNITY_EDITOR
            try { result = Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8); }
            finally { Release(); }
#else
            result = Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            Release();
#endif
            return result;
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

        public bool Equals(FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult> left, FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult> left, FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult> right) => !(left == right);

        public static bool operator ==(FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult> left, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> right) => left?._callback == right;
        public static bool operator !=(FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult> left, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> right) => !(left == right);

        public static bool operator ==(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> left, FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult> right) => left == right?._callback;
        public static bool operator !=(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> left, FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult> right) => !(left == right);

        public static implicit operator FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> value) => Get(value);
        public static explicit operator Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> _pool = new();
        Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> _callback;
        bool _hasReleased;
        #endregion
    }

}