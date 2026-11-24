using UnityEngine;

namespace Castforn.CommandAndCallback
{

    /// <summary>
    /// Represents a command that performs an action without returning a result.
    /// </summary>
    public abstract class BaseActionCommand : Command
    {
        /// <summary>
        /// Invokes the command.
        /// </summary>
        [HideInCallstack]
        public abstract void Invoke();

        /// <summary>
        /// Invokes the command then releases it afterward.
        /// </summary>
        [HideInCallstack]
        public void Execute()
        {
            try
            {
                Invoke();
            }
            finally
            {
                Release();
            }
        }
    }

}