/* Copyright © 2009, Frank van de Ven, all rights reserved.       ]
 * email: vandevenator@gmail.com
 *
 * This is my first contribution to "The Code Project." I am not much of an article writer, but The Code Project (www.codeproject.com) has
 * been very useful to me over the past years, it is time to give something back.
 * 
 * This code is based on several code snippets and examples I found on the Internet.
 * I combined all this information to create a very useful Windows CE process enumeration and manipulation class.
 *
 * This code is for use with WINDOWS CE only.
 * 
 * The code was only tested on Windows Mobile 6.1 (Windows CE 5.2). Windows CE 4 should be no problem.
 *  
 * This source code is licensed under the Code Project Open License (CPOL).
 * Check out http://www.codeproject.com/info/cpol10.aspx for further details.
 * 
 * Some examples:
 * 
 *   ThreadInfo[] list = ThreadCE.GetThreads();
 *          
 *   foreach (ThreadInfo item in list)
 *   {
 *       Debug.WriteLine("Thread item: " + item.FullPath);
 *       if (item.FullPath == @"\Windows\iexplore.exe")
 *           item.Kill();
 *   }
 *
 *   bool result = ThreadCE.IsRunning(@"\Windows\iexplore.exe");
 *
 *   IntPtr pid = ThreadCE.FindThreadPID(@"\Windows\iexplore.exe");
 *   
 *   if (pid == IntPtr.Zero)
 *       throw new Exception("Thread not found.");
 *
 *   result = ThreadCE.FindAndKill(@"\Windows\iexplore.exe");
 * 
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;

namespace Terranova.API
{
    /// <summary>
    /// Contains information about a process.
    /// This information is collected by ThreadCE.GetThreads().
    /// </summary>
    public class ThreadInfo
    {
        private IntPtr _tid;
        private IntPtr _parentProcessID;

        internal ThreadInfo(IntPtr pid, IntPtr parentid)
        {
            _tid = pid;
            _parentProcessID = parentid;

        }

        /// <summary>
        /// Returns the Thread Id.
        /// </summary>
        public IntPtr Tid
        {
            get { return _tid; }
        }


        public IntPtr ParentProcessID
        {
            get { return _parentProcessID; }
        }

    }
    
    /// <summary>
    /// Static class that provides Windows CE process information and manipulation.
    /// The biggest difference with the Compact Framework's Thread class is that this
    /// class works with the full path to the .EXE file. And not the pathless .EXE file name.
    /// </summary>
    public static class ThreadCE
    {
        private const int MAX_PATH = 260;
        private const int TH32CS_SNAPTHREAD = 0x00000004;
        private const int TH32CS_SNAPNOHEAPS = 0x40000000;
        private const int INVALID_HANDLE_VALUE = -1;
        private const int THREAD_TERMINATE = 1;

        /// <summary>
        /// Returns an array with information about running processes.
        /// </summary>
        ///<exception cref="Win32Exception">Thrown when enumerating the processes fails.</exception>
        public static ThreadInfo[] GetThreads()
        {
            List<ThreadInfo> procList = new List<ThreadInfo>();

            IntPtr handle = CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD | TH32CS_SNAPNOHEAPS, 0);

            if ((Int32)handle == INVALID_HANDLE_VALUE)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateToolhelp32Snapshot error.");

            try
            {
                THREADENTRY processentry = new THREADENTRY();
                processentry.dwSize = (uint)Marshal.SizeOf(processentry);

                //Get the first process
                int retval = Thread32First(handle, ref processentry);

                while (retval == 1)
                {
                    procList.Add(new ThreadInfo(new IntPtr((int)processentry.th32ThreadID), new IntPtr((int)processentry.th32OwnerProcessID)));
                    retval = Thread32Next(handle, ref processentry);
                }
            }
            finally
            {
                CloseToolhelp32Snapshot(handle);
            }

            return procList.ToArray();
        }

        /// <summary>
        /// Checks if the specified .EXE is running.
        /// </summary>
        /// <param name="fullpath">The full path to an .EXE file.</param>
        /// <returns>Returns true is the process is running.</returns>
        /// <exception cref="Win32Exception">Thrown when taking a system snapshot fails.</exception>
        public static bool IsRunning(string fullpath)
        {
            return (FindThreadPID(fullpath) != IntPtr.Zero);
        }

        /// <summary>
        /// Finds and kills if the process for the specified .EXE file is running.
        /// </summary>
        /// <param name="fullpath">The full path to an .EXE file.</param>
        /// <returns>True if the process was terminated. False if the process was not found.</returns>
        /// <exception cref="Win32Exception">Thrown when opening or killing the process fails.</exception>
        public static bool FindAndKill(string fullpath)
        {
            IntPtr pid = FindThreadPID(fullpath);

            if (pid == IntPtr.Zero)
                return false;

            Kill(pid);

            return true;
        }

        /// <summary>
        /// Terminates the process with the specified Thread Id.
        /// </summary>
        /// <param name="pid">The Thread Id of the process to kill.</param>
        /// <exception cref="Win32Exception">Thrown when opening or killing the process fails.</exception>
        internal static void Kill(IntPtr pid)
        {

            IntPtr process_handle = OpenThread(THREAD_TERMINATE, false, (int)pid);

            if (process_handle == (IntPtr)INVALID_HANDLE_VALUE)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "OpenThread failed.");

            try
            {
                bool result = TerminateThread(process_handle, 0);

                if (result == false)
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "TerminateThread failed.");

            }
            finally
            {
                CloseHandle(process_handle);
            }
        }

        /// <summary>
        /// Finds the Thread Id of the specified .EXE file.
        /// </summary>
        /// <param name="fullpath">The full path to an .EXE file.</param>
        /// <returns>The Thread Id to the process found. Return IntPtr.Zero if the process is not running.</returns>
        ///<exception cref="Win32Exception">Thrown when taking a system snapshot fails.</exception>
        public static IntPtr FindThreadPID(string fullpath)
        {
            fullpath = fullpath.ToLower();

            IntPtr snapshot_handle = CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD | TH32CS_SNAPNOHEAPS, 0);

            if ((Int32)snapshot_handle == INVALID_HANDLE_VALUE)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateToolhelp32Snapshot failed.");

            try
            {
                THREADENTRY processentry = new THREADENTRY();
                processentry.dwSize = (uint)Marshal.SizeOf(processentry);
                StringBuilder fullexepath = new StringBuilder(1024);

                int retval = Thread32First(snapshot_handle, ref processentry);

                while (retval == 1)
                {
                    IntPtr pid = new IntPtr((int)processentry.th32ThreadID);

                    // Writes the full path to the process into a StringBuilder object.
                    // Note: If first parameter is IntPtr.Zero it returns the path to the current process.
                    GetModuleFileName(pid, fullexepath, fullexepath.Capacity);

                    if (fullexepath.ToString().ToLower() == fullpath)
                        return pid;

                    retval = Thread32Next(snapshot_handle, ref processentry);
                }
            }
            finally
            {
                CloseToolhelp32Snapshot(snapshot_handle);
            }

            return IntPtr.Zero;
        }

        [DllImport("toolhelp.dll", SetLastError = true)]
        private static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processID);

        [DllImport("toolhelp.dll")]
        private static extern int CloseToolhelp32Snapshot(IntPtr snapshot);

        [DllImport("toolhelp.dll")]
        private static extern int Thread32First(IntPtr snapshot, ref THREADENTRY processEntry);

        [DllImport("toolhelp.dll")]
        private static extern int Thread32Next(IntPtr snapshot, ref THREADENTRY processEntry);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern int GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, int nSize);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern bool TerminateThread(IntPtr hThread, uint ExitCode);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern IntPtr OpenThread(int flags, bool fInherit, int PID);

        [DllImport("coredll.dll")]
        private static extern bool CloseHandle(IntPtr handle);

        private struct THREADENTRY
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ThreadID;
            public uint th32OwnerProcessID;
            public long tpBasePri;
            public long tpDeltaPri;
            public uint dwFlags;
        }

    } // end of class
} // end of namespace 
