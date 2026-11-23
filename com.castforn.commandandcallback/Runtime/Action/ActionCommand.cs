using System;
using System.Collections.Generic;
using UnityEngine;

namespace Castforn.CommandAndCallback
{

    /// <summary>
    /// Represents a command that invokes an action delegate.
    /// </summary>
    public sealed class ActionCommand : BaseActionCommand, IEquatable<ActionCommand>
    {
        #region Lifecycle
        ActionCommand(Action callback)
        {
            _callback = callback;
        }
        #endregion

        #region API
        /// <summary>
        /// Acquires an action command from the pool, creating a new instance if necessary.
        /// </summary>
        /// <returns>
        /// An acquired action command configured with the specified callback.
        /// </returns>
        public static ActionCommand Get(Action callback)
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
        public override void Invoke() => _callback?.Invoke();

        public override bool Release()
        {
            if (_hasReleased) return false;
            _hasReleased = true;

            _callback = null;

            _pool.Push(this);
            return true;
        }

        public bool Equals(ActionCommand other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ActionCommand other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ActionCommand left, ActionCommand right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ActionCommand left, ActionCommand right) => !(left == right);

        public static bool operator ==(ActionCommand left, Action right) => left?._callback == right;
        public static bool operator !=(ActionCommand left, Action right) => !(left == right);

        public static bool operator ==(Action left, ActionCommand right) => left == right?._callback;
        public static bool operator !=(Action left, ActionCommand right) => !(left == right);

        public static implicit operator ActionCommand(Action value) => Get(value);
        public static explicit operator Action(ActionCommand value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<ActionCommand> _pool = new();
        Action _callback;
        bool _hasReleased;
        #endregion
    }

    /// <inheritdoc cref="ActionCommand"/>
    public sealed class ActionCommand<T1> : BaseActionCommand, IEquatable<ActionCommand<T1>>
    {
        #region Lifecycle
        ActionCommand(Action<T1> callback, T1 arg1)
        {
            _callback = callback;
            Arg1 = arg1;
        }
        #endregion

        #region API
        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1> Get(Action<T1> callback) => Get(callback, default);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1> Get(Action<T1> callback, T1 arg1)
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
#if UNITY_EDITOR
            try { Invoke(arg1); }
            finally { Release(); }
#else
            Invoke(arg1);
            Release();
#endif
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
        Action<T1> _callback;
        bool _hasReleased;
        #endregion
    }

    /// <inheritdoc cref="ActionCommand"/>
    public sealed class ActionCommand<T1, T2> : BaseActionCommand, IEquatable<ActionCommand<T1, T2>>
    {
        #region Lifecycle
        ActionCommand(Action<T1, T2> callback, T1 arg1, T2 arg2)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
        }
        #endregion

        #region API
        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2> Get(Action<T1, T2> callback) => Get(callback, default, default);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2> Get(Action<T1, T2> callback, T1 arg1, T2 arg2)
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

        /// <inheritdoc cref="ActionCommand{T1}.SetArgs(T1)"/>
        public void SetArgs(T1 arg1, T2 arg2)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }

        /// <inheritdoc cref="ActionCommand{T1}.Invoke(T1)"/>
        [HideInCallstack]
        public void Invoke(T1 arg1, T2 arg2) => _callback?.Invoke(arg1, arg2);
        public override void Invoke() => _callback?.Invoke(Arg1, Arg2);

        /// <inheritdoc cref="ActionCommand{T1}.Execute(T1)"/>
        [HideInCallstack]
        public void Execute(T1 arg1, T2 arg2)
        {
#if UNITY_EDITOR
            try { Invoke(arg1, arg2); }
            finally { Release(); }
#else
            Invoke(arg1, arg2);
            Release();
#endif
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

        public bool Equals(ActionCommand<T1, T2> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ActionCommand<T1, T2> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ActionCommand<T1, T2> left, ActionCommand<T1, T2> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ActionCommand<T1, T2> left, ActionCommand<T1, T2> right) => !(left == right);

        public static bool operator ==(ActionCommand<T1, T2> left, Action<T1, T2> right) => left?._callback == right;
        public static bool operator !=(ActionCommand<T1, T2> left, Action<T1, T2> right) => !(left == right);

        public static bool operator ==(Action<T1, T2> left, ActionCommand<T1, T2> right) => left == right?._callback;
        public static bool operator !=(Action<T1, T2> left, ActionCommand<T1, T2> right) => !(left == right);

        public static implicit operator ActionCommand<T1, T2>(Action<T1, T2> value) => Get(value);
        public static explicit operator Action<T1, T2>(ActionCommand<T1, T2> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<ActionCommand<T1, T2>> _pool = new();
        Action<T1, T2> _callback;
        bool _hasReleased;
        #endregion
    }

    /// <inheritdoc cref="ActionCommand"/>
    public sealed class ActionCommand<T1, T2, T3> : BaseActionCommand, IEquatable<ActionCommand<T1, T2, T3>>
    {
        #region Lifecycle
        ActionCommand(Action<T1, T2, T3> callback, T1 arg1, T2 arg2, T3 arg3)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }
        #endregion

        #region API
        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3> Get(Action<T1, T2, T3> callback) => Get(callback, default, default, default);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3> Get(Action<T1, T2, T3> callback, T1 arg1, T2 arg2, T3 arg3)
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

        /// <inheritdoc cref="ActionCommand{T1}.SetArgs(T1)"/>
        public void SetArgs(T1 arg1, T2 arg2, T3 arg3)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }

        /// <inheritdoc cref="ActionCommand{T1}.Invoke(T1)"/>
        [HideInCallstack]
        public void Invoke(T1 arg1, T2 arg2, T3 arg3) => _callback?.Invoke(arg1, arg2, arg3);
        public override void Invoke() => _callback?.Invoke(Arg1, Arg2, Arg3);

        /// <inheritdoc cref="ActionCommand{T1}.Execute(T1)"/>
        [HideInCallstack]
        public void Execute(T1 arg1, T2 arg2, T3 arg3)
        {
#if UNITY_EDITOR
            try { Invoke(arg1, arg2, arg3); }
            finally { Release(); }
#else
            Invoke(arg1, arg2, arg3);
            Release();
#endif
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

        public bool Equals(ActionCommand<T1, T2, T3> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ActionCommand<T1, T2, T3> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ActionCommand<T1, T2, T3> left, ActionCommand<T1, T2, T3> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ActionCommand<T1, T2, T3> left, ActionCommand<T1, T2, T3> right) => !(left == right);

        public static bool operator ==(ActionCommand<T1, T2, T3> left, Action<T1, T2, T3> right) => left?._callback == right;
        public static bool operator !=(ActionCommand<T1, T2, T3> left, Action<T1, T2, T3> right) => !(left == right);

        public static bool operator ==(Action<T1, T2, T3> left, ActionCommand<T1, T2, T3> right) => left == right?._callback;
        public static bool operator !=(Action<T1, T2, T3> left, ActionCommand<T1, T2, T3> right) => !(left == right);

        public static implicit operator ActionCommand<T1, T2, T3>(Action<T1, T2, T3> value) => Get(value);
        public static explicit operator Action<T1, T2, T3>(ActionCommand<T1, T2, T3> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<ActionCommand<T1, T2, T3>> _pool = new();
        Action<T1, T2, T3> _callback;
        bool _hasReleased;
        #endregion
    }

    /// <inheritdoc cref="ActionCommand"/>
    public sealed class ActionCommand<T1, T2, T3, T4> : BaseActionCommand, IEquatable<ActionCommand<T1, T2, T3, T4>>
    {
        #region Lifecycle
        ActionCommand(Action<T1, T2, T3, T4> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }
        #endregion

        #region API
        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4> Get(Action<T1, T2, T3, T4> callback) => Get(callback, default, default, default, default);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4> Get(Action<T1, T2, T3, T4> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
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

        /// <inheritdoc cref="ActionCommand{T1}.SetArgs(T1)"/>
        public void SetArgs(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }

        /// <inheritdoc cref="ActionCommand{T1}.Invoke(T1)"/>
        [HideInCallstack]
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => _callback?.Invoke(arg1, arg2, arg3, arg4);
        public override void Invoke() => _callback?.Invoke(Arg1, Arg2, Arg3, Arg4);

        /// <inheritdoc cref="ActionCommand{T1}.Execute(T1)"/>
        [HideInCallstack]
        public void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
#if UNITY_EDITOR
            try { Invoke(arg1, arg2, arg3, arg4); }
            finally { Release(); }
#else
            Invoke(arg1, arg2, arg3, arg4);
            Release();
#endif
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
        Action<T1, T2, T3, T4> _callback;
        bool _hasReleased;
        #endregion
    }

    /// <inheritdoc cref="ActionCommand"/>
    public sealed class ActionCommand<T1, T2, T3, T4, T5> : BaseActionCommand, IEquatable<ActionCommand<T1, T2, T3, T4, T5>>
    {
        #region Lifecycle
        ActionCommand(Action<T1, T2, T3, T4, T5> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
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
        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5> Get(Action<T1, T2, T3, T4, T5> callback) => Get(callback, default, default, default, default, default);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5> Get(Action<T1, T2, T3, T4, T5> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
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

        /// <inheritdoc cref="ActionCommand{T1}.SetArgs(T1)"/>
        public void SetArgs(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
        }

        /// <inheritdoc cref="ActionCommand{T1}.Invoke(T1)"/>
        [HideInCallstack]
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => _callback?.Invoke(arg1, arg2, arg3, arg4, arg5);
        public override void Invoke() => _callback?.Invoke(Arg1, Arg2, Arg3, Arg4, Arg5);

        /// <inheritdoc cref="ActionCommand{T1}.Execute(T1)"/>
        [HideInCallstack]
        public void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
#if UNITY_EDITOR
            try { Invoke(arg1, arg2, arg3, arg4, arg5); }
            finally { Release(); }
#else
            Invoke(arg1, arg2, arg3, arg4, arg5);
            Release();
#endif
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

        public bool Equals(ActionCommand<T1, T2, T3, T4, T5> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ActionCommand<T1, T2, T3, T4, T5> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ActionCommand<T1, T2, T3, T4, T5> left, ActionCommand<T1, T2, T3, T4, T5> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ActionCommand<T1, T2, T3, T4, T5> left, ActionCommand<T1, T2, T3, T4, T5> right) => !(left == right);

        public static bool operator ==(ActionCommand<T1, T2, T3, T4, T5> left, Action<T1, T2, T3, T4, T5> right) => left?._callback == right;
        public static bool operator !=(ActionCommand<T1, T2, T3, T4, T5> left, Action<T1, T2, T3, T4, T5> right) => !(left == right);

        public static bool operator ==(Action<T1, T2, T3, T4, T5> left, ActionCommand<T1, T2, T3, T4, T5> right) => left == right?._callback;
        public static bool operator !=(Action<T1, T2, T3, T4, T5> left, ActionCommand<T1, T2, T3, T4, T5> right) => !(left == right);

        public static implicit operator ActionCommand<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> value) => Get(value);
        public static explicit operator Action<T1, T2, T3, T4, T5>(ActionCommand<T1, T2, T3, T4, T5> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<ActionCommand<T1, T2, T3, T4, T5>> _pool = new();
        Action<T1, T2, T3, T4, T5> _callback;
        bool _hasReleased;
        #endregion
    }

    /// <inheritdoc cref="ActionCommand"/>
    public sealed class ActionCommand<T1, T2, T3, T4, T5, T6> : BaseActionCommand, IEquatable<ActionCommand<T1, T2, T3, T4, T5, T6>>
    {
        #region Lifecycle
        ActionCommand(Action<T1, T2, T3, T4, T5, T6> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
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
        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5, T6> Get(Action<T1, T2, T3, T4, T5, T6> callback) => Get(callback, default, default, default, default, default, default);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5, T6> Get(Action<T1, T2, T3, T4, T5, T6> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
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

        /// <inheritdoc cref="ActionCommand{T1}.SetArgs(T1)"/>
        public void SetArgs(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
        }

        /// <inheritdoc cref="ActionCommand{T1}.Invoke(T1)"/>
        [HideInCallstack]
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => _callback?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
        public override void Invoke() => _callback?.Invoke(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6);

        /// <inheritdoc cref="ActionCommand{T1}.Execute(T1)"/>
        [HideInCallstack]
        public void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
#if UNITY_EDITOR
            try { Invoke(arg1, arg2, arg3, arg4, arg5, arg6); }
            finally { Release(); }
#else
            Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
            Release();
#endif
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

        public bool Equals(ActionCommand<T1, T2, T3, T4, T5, T6> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ActionCommand<T1, T2, T3, T4, T5, T6> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ActionCommand<T1, T2, T3, T4, T5, T6> left, ActionCommand<T1, T2, T3, T4, T5, T6> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ActionCommand<T1, T2, T3, T4, T5, T6> left, ActionCommand<T1, T2, T3, T4, T5, T6> right) => !(left == right);

        public static bool operator ==(ActionCommand<T1, T2, T3, T4, T5, T6> left, Action<T1, T2, T3, T4, T5, T6> right) => left?._callback == right;
        public static bool operator !=(ActionCommand<T1, T2, T3, T4, T5, T6> left, Action<T1, T2, T3, T4, T5, T6> right) => !(left == right);

        public static bool operator ==(Action<T1, T2, T3, T4, T5, T6> left, ActionCommand<T1, T2, T3, T4, T5, T6> right) => left == right?._callback;
        public static bool operator !=(Action<T1, T2, T3, T4, T5, T6> left, ActionCommand<T1, T2, T3, T4, T5, T6> right) => !(left == right);

        public static implicit operator ActionCommand<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> value) => Get(value);
        public static explicit operator Action<T1, T2, T3, T4, T5, T6>(ActionCommand<T1, T2, T3, T4, T5, T6> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<ActionCommand<T1, T2, T3, T4, T5, T6>> _pool = new();
        Action<T1, T2, T3, T4, T5, T6> _callback;
        bool _hasReleased;
        #endregion
    }

    /// <inheritdoc cref="ActionCommand"/>
    public sealed class ActionCommand<T1, T2, T3, T4, T5, T6, T7> : BaseActionCommand, IEquatable<ActionCommand<T1, T2, T3, T4, T5, T6, T7>>
    {
        #region Lifecycle
        ActionCommand(Action<T1, T2, T3, T4, T5, T6, T7> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
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
        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5, T6, T7> Get(Action<T1, T2, T3, T4, T5, T6, T7> callback) => Get(callback, default, default, default, default, default, default, default);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5, T6, T7> Get(Action<T1, T2, T3, T4, T5, T6, T7> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
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

        /// <inheritdoc cref="ActionCommand{T1}.SetArgs(T1)"/>
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

        /// <inheritdoc cref="ActionCommand{T1}.Invoke(T1)"/>
        [HideInCallstack]
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => _callback?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        public override void Invoke() => _callback?.Invoke(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7);

        /// <inheritdoc cref="ActionCommand{T1}.Execute(T1)"/>
        [HideInCallstack]
        public void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
#if UNITY_EDITOR
            try { Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7); }
            finally { Release(); }
#else
            Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            Release();
#endif
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

        public bool Equals(ActionCommand<T1, T2, T3, T4, T5, T6, T7> other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ActionCommand<T1, T2, T3, T4, T5, T6, T7> other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ActionCommand<T1, T2, T3, T4, T5, T6, T7> left, ActionCommand<T1, T2, T3, T4, T5, T6, T7> right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ActionCommand<T1, T2, T3, T4, T5, T6, T7> left, ActionCommand<T1, T2, T3, T4, T5, T6, T7> right) => !(left == right);

        public static bool operator ==(ActionCommand<T1, T2, T3, T4, T5, T6, T7> left, Action<T1, T2, T3, T4, T5, T6, T7> right) => left?._callback == right;
        public static bool operator !=(ActionCommand<T1, T2, T3, T4, T5, T6, T7> left, Action<T1, T2, T3, T4, T5, T6, T7> right) => !(left == right);

        public static bool operator ==(Action<T1, T2, T3, T4, T5, T6, T7> left, ActionCommand<T1, T2, T3, T4, T5, T6, T7> right) => left == right?._callback;
        public static bool operator !=(Action<T1, T2, T3, T4, T5, T6, T7> left, ActionCommand<T1, T2, T3, T4, T5, T6, T7> right) => !(left == right);

        public static implicit operator ActionCommand<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> value) => Get(value);
        public static explicit operator Action<T1, T2, T3, T4, T5, T6, T7>(ActionCommand<T1, T2, T3, T4, T5, T6, T7> value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        static readonly Stack<ActionCommand<T1, T2, T3, T4, T5, T6, T7>> _pool = new();
        Action<T1, T2, T3, T4, T5, T6, T7> _callback;
        bool _hasReleased;
        #endregion
    }

    /// <inheritdoc cref="ActionCommand"/>
    public sealed class ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> : BaseActionCommand, IEquatable<ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8>>
    {
        #region Lifecycle
        ActionCommand(Action<T1, T2, T3, T4, T5, T6, T7, T8> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
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
        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> Get(Action<T1, T2, T3, T4, T5, T6, T7, T8> callback) => Get(callback, default, default, default, default, default, default, default, default);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> Get(Action<T1, T2, T3, T4, T5, T6, T7, T8> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
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

        /// <inheritdoc cref="ActionCommand{T1}.Invoke(T1)"/>
        [HideInCallstack]
        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => _callback?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        public override void Invoke() => _callback?.Invoke(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8);

        /// <inheritdoc cref="ActionCommand{T1}.Execute(T1)"/>
        [HideInCallstack]
        public void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
#if UNITY_EDITOR
            try { Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8); }
            finally { Release(); }
#else
            Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            Release();
#endif
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
        Action<T1, T2, T3, T4, T5, T6, T7, T8> _callback;
        bool _hasReleased;
        #endregion
    }

}