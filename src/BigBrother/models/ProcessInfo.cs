using System;
using System.Collections.Generic;

namespace BigBrother.models
{
    /// <summary>
    /// Class containing information about a process.
    /// </summary>
    public class ProcessInfo
    {
        public string Name { get; private set; }
        public string FullPath { get; private set; }
        public IntPtr ProcessID { get; private set; }
        public int ThreadCount { get; private set; }
        public int ParentProcessID { get; private set; }
        public List<ThreadInfo> Threads { get; private set; }

        /// <summary>
        /// Constructor.
        ///
        /// Instantiates a new ProcessInfo object.
        /// </summary>
        /// <param name="name">Name of the process .exe</param>
        /// <param name="fullPath">Full path to the process .exe</param>
        /// <param name="processID">PID of the process.</param>
        /// <param name="threadCount">The number of threads in the process.</param>
        /// <param name="parentProcessID">The PID of the parent process.</param>
        public ProcessInfo(string name, string fullPath, IntPtr processID, int threadCount, int parentProcessID, List<ThreadInfo> threads)
        {
            this.Name = name;
            this.FullPath = fullPath;
            this.ProcessID = processID;
            this.ThreadCount = threadCount;
            this.ParentProcessID = parentProcessID;
            this.Threads = threads;
        }

        /// <summary>
        /// Generates a formatted string representation of the ProcessInfo object.
        /// </summary>
        /// <returns>The formatted string representation of the ProcessInfo object.</returns>
        public string ToString(bool listThreads)
        {
            string processInfoString = "Name: " + Name +
                                       "\nFull Path: " + FullPath +
                                       "\nPID: " + ProcessID +
                                       "\nThread Count: " + ThreadCount +
                                       "\nParent Process PID: " + ParentProcessID;

            if (listThreads)
            {
                processInfoString += "\nThread TIDs: ";

                for (int i = 0; i < Threads.Count; i++)
                {
                    if (i == Threads.Count - 1)
                    {
                        processInfoString += Threads[i].ToString();
                    }
                    else
                    {
                        processInfoString += Threads[i] + " | ";
                    }
                }
            }

            return processInfoString;
        }
    }
}
