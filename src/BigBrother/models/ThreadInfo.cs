using System;

namespace BigBrother.models
{
    /// <summary>
    /// Class containing information about a thread.
    /// </summary>
    public class ThreadInfo
    {
        public IntPtr ThreadID { get; private set; }

        /// <summary>
        /// Constructor.
        ///
        /// Instantiates a new ProcessInfo object.
        /// </summary>
        /// <param name="threadID">TID of the thread.</param>
        public ThreadInfo(IntPtr threadID)
        {
            this.ThreadID = threadID;
        }

        /// <summary>
        /// Generates a formatted string representation of the ThreadInfo object.
        /// </summary>
        /// <returns>The formatted string representation of the ThreadInfo object.</returns>
        public override string ToString()
        {
            string threadInfoString = ThreadID.ToString();
            return threadInfoString;
        }
    }
}
