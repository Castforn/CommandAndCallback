using System;

namespace Castforn.CommandAndCallback
{

    /// <summary>
    /// Represents a command that can be invoked and released.
    /// </summary>
    public abstract class Command
    {
        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand GetAction(Action callback) => ActionCommand.Get(callback);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1> GetAction<T1>(Action<T1> callback) => ActionCommand<T1>.Get(callback);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2> GetAction<T1, T2>(Action<T1, T2> callback) => ActionCommand<T1, T2>.Get(callback);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3> GetAction<T1, T2, T3>(Action<T1, T2, T3> callback) => ActionCommand<T1, T2, T3>.Get(callback);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4> GetAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback) => ActionCommand<T1, T2, T3, T4>.Get(callback);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5> GetAction<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> callback) => ActionCommand<T1, T2, T3, T4, T5>.Get(callback);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5, T6> GetAction<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> callback) => ActionCommand<T1, T2, T3, T4, T5, T6>.Get(callback);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5, T6, T7> GetAction<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> callback) => ActionCommand<T1, T2, T3, T4, T5, T6, T7>.Get(callback);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> GetAction<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> callback) => ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8>.Get(callback);


        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1> GetAction<T1>(Action<T1> callback, T1 arg1) => ActionCommand<T1>.Get(callback, arg1);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2> GetAction<T1, T2>(Action<T1, T2> callback, T1 arg1, T2 arg2) => ActionCommand<T1, T2>.Get(callback, arg1, arg2);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3> GetAction<T1, T2, T3>(Action<T1, T2, T3> callback, T1 arg1, T2 arg2, T3 arg3) => ActionCommand<T1, T2, T3>.Get(callback, arg1, arg2, arg3);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4> GetAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => ActionCommand<T1, T2, T3, T4>.Get(callback, arg1, arg2, arg3, arg4);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5> GetAction<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => ActionCommand<T1, T2, T3, T4, T5>.Get(callback, arg1, arg2, arg3, arg4, arg5);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5, T6> GetAction<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => ActionCommand<T1, T2, T3, T4, T5, T6>.Get(callback, arg1, arg2, arg3, arg4, arg5, arg6);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5, T6, T7> GetAction<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => ActionCommand<T1, T2, T3, T4, T5, T6, T7>.Get(callback, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

        /// <inheritdoc cref="ActionCommand.Get(Action)"/>
        public static ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8> GetAction<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => ActionCommand<T1, T2, T3, T4, T5, T6, T7, T8>.Get(callback, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);



        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<TResult> GetFunc<TResult>(Func<TResult> callback) => FuncCommand<TResult>.Get(callback);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, TResult> GetFunc<T1, TResult>(Func<T1, TResult> callback) => FuncCommand<T1, TResult>.Get(callback);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, TResult> GetFunc<T1, T2, TResult>(Func<T1, T2, TResult> callback) => FuncCommand<T1, T2, TResult>.Get(callback);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, TResult> GetFunc<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> callback) => FuncCommand<T1, T2, T3, TResult>.Get(callback);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, TResult> GetFunc<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> callback) => FuncCommand<T1, T2, T3, T4, TResult>.Get(callback);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, T5, TResult> GetFunc<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> callback) => FuncCommand<T1, T2, T3, T4, T5, TResult>.Get(callback);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, T5, T6, TResult> GetFunc<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> callback) => FuncCommand<T1, T2, T3, T4, T5, T6, TResult>.Get(callback);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult> GetFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> callback) => FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult>.Get(callback);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult> GetFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> callback) => FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult>.Get(callback);


        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, TResult> GetFunc<T1, TResult>(Func<T1, TResult> callback, T1 arg1) => FuncCommand<T1, TResult>.Get(callback, arg1);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, TResult> GetFunc<T1, T2, TResult>(Func<T1, T2, TResult> callback, T1 arg1, T2 arg2) => FuncCommand<T1, T2, TResult>.Get(callback, arg1, arg2);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, TResult> GetFunc<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> callback, T1 arg1, T2 arg2, T3 arg3) => FuncCommand<T1, T2, T3, TResult>.Get(callback, arg1, arg2, arg3);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, TResult> GetFunc<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => FuncCommand<T1, T2, T3, T4, TResult>.Get(callback, arg1, arg2, arg3, arg4);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, T5, TResult> GetFunc<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => FuncCommand<T1, T2, T3, T4, T5, TResult>.Get(callback, arg1, arg2, arg3, arg4, arg5);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, T5, T6, TResult> GetFunc<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => FuncCommand<T1, T2, T3, T4, T5, T6, TResult>.Get(callback, arg1, arg2, arg3, arg4, arg5, arg6);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult> GetFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => FuncCommand<T1, T2, T3, T4, T5, T6, T7, TResult>.Get(callback, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

        /// <inheritdoc cref="FuncCommand{TResult}.Get(Func{TResult})"/>
        public static FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult> GetFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => FuncCommand<T1, T2, T3, T4, T5, T6, T7, T8, TResult>.Get(callback, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);



        /// <inheritdoc cref="PredicateCommand{T}.Get(Predicate{T})"/>
        public static PredicateCommand<T> GetPredicate<T>(Predicate<T> callback) => PredicateCommand<T>.Get(callback);


        /// <inheritdoc cref="PredicateCommand{T}.Get(Predicate{T})"/>
        public static PredicateCommand<T> GetPredicate<T>(Predicate<T> callback, T arg) => PredicateCommand<T>.Get(callback, arg);



        /// <summary>
        /// Whether the command has already been released back to its pool.
        /// </summary>
        public abstract bool HasReleased { get; }

        /// <summary>
        /// Releases the command back to its pool.
        /// </summary>
        /// <returns>
        /// True if the command was successfully released.
        /// </returns>
        public abstract bool Release();
    }

}