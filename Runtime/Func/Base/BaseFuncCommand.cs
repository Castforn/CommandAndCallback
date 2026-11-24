using UnityEngine;

namespace Castforn.CommandAndCallback
{

    /// <summary>
    /// Represents a command that performs an action and returns it's result.
    /// </summary>
    public abstract class BaseFuncCommand<TResult> : Command
    {
        /// <summary>
        /// Invokes the command and returns a result.
        /// </summary>
        [HideInCallstack]
        public abstract TResult Invoke();

        /// <summary>
        /// Invokes the command and returns a result then releases it afterward.
        /// </summary>
        [HideInCallstack]
        public TResult Execute()
        {
            TResult result;
            try
            {
                result = Invoke();
            }
            finally
            {
                Release();
            }
            return result;
        }
    }

}