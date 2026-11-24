using System;
using System.Collections.Generic;

namespace Castforn.CommandAndCallback
{

    /// <summary>
    /// Represents a command that invokes an action delegate.
    /// </summary>
    public sealed class ActionCommand : ReadOnlyActionCommand, IEquatable<ActionCommand>
    {
        #region Lifecycle
        ActionCommand(Action callback) => _callback = callback;
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

        public static bool operator ==(ActionCommand left, ReadOnlyActionCommand right) => left?._callback == right;
        public static bool operator !=(ActionCommand left, ReadOnlyActionCommand right) => !(left == right);

        public static bool operator ==(ReadOnlyActionCommand left, ActionCommand right) => left == right?._callback;
        public static bool operator !=(ReadOnlyActionCommand left, ActionCommand right) => !(left == right);

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
        #endregion
    }

    /// <summary>
    /// Represents a read only command that invokes an action delegate.
    /// </summary>
    public abstract class ReadOnlyActionCommand : BaseActionCommand, IEquatable<ReadOnlyActionCommand>
    {
        #region API
        public override bool HasReleased => _hasReleased;
        public override void Invoke() => _callback?.Invoke();



        public bool Equals(ReadOnlyActionCommand other) => other is not null && _callback == other._callback;
        public override bool Equals(object obj) => obj is ReadOnlyActionCommand other && Equals(other);
        public override int GetHashCode() => _callback?.GetHashCode() ?? 0;

        public static bool operator ==(ReadOnlyActionCommand left, ReadOnlyActionCommand right) => ReferenceEquals(left, right) || left is not null && left.Equals(right);
        public static bool operator !=(ReadOnlyActionCommand left, ReadOnlyActionCommand right) => !(left == right);

        public static bool operator ==(ReadOnlyActionCommand left, Action right) => left?._callback == right;
        public static bool operator !=(ReadOnlyActionCommand left, Action right) => !(left == right);

        public static bool operator ==(Action left, ReadOnlyActionCommand right) => left == right?._callback;
        public static bool operator !=(Action left, ReadOnlyActionCommand right) => !(left == right);

        public static explicit operator Action(ReadOnlyActionCommand value)
        {
            if (value is null) return null;
            var callback = value._callback;
            return value.Release() ? callback : null;
        }
        #endregion

        #region Implementation
        protected Action _callback;
        protected bool _hasReleased;
        #endregion
    }

}