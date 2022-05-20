using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using BigBrother.utilities.general;

namespace BigBrother.logging
{
    /// <summary>
    /// Class providing basic logging functionality.
    /// </summary>
    public static class Logging
    {
        private const string TRACE_CATEGORY = "BB-TRACE";
        private const string AUDIT_CATEGORY = "BB-AUDIT";
        private const string ERROR_CATEGORY = "BB-ERROR";
        private const string DEBUG_CATEGORY = "BB-DEBUG";

        private const string REMOVABLE_STORAGE_DIRECTORY = @"\emmc";
        private const string WRITABLE_ROOT_DIRECTORY = @"\emmc\App\Logs\BigBrother";

        private static ILogger _logger = new SystemDebugLogger();
        private static bool _traceEnabled = true;

        /// <summary>
        /// Enables/disables logging of debug info (only in debug builds).
        /// </summary>
        private static bool _debugEnabled = true;

        /// <summary>
        /// Gets/Sets the core logger implementation.
        /// </summary>
        public static ILogger Logger
        {
            get
            {
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }

        /// <summary>
        /// Enables/disables logging of trace info.
        /// </summary>
        public static bool TraceEnabled
        {
            get
            {
                return _traceEnabled;
            }
            set
            {
                _traceEnabled = value;
            }
        }

        /// <summary>
        /// Enables/disables logging of debug info (only in debug builds).
        /// </summary>
        public static bool DebugEnabled
        {
            get
            {
                return _debugEnabled;
            }
            set
            {
                _debugEnabled = value;
            }
        }

        /// <summary>
        /// Logs a formatted trace message suitable for diagnostics/debug.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Trace(string format, params object[] args)
        {
            if (TraceEnabled)
            {
                WriteMessage(TRACE_CATEGORY, format, args);
            }
        }

        /// <summary>
        /// Logs a formatted debug message suitable for diagnostics/debug.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [Conditional("DEBUG")]
        public static void Debug(string format, params object[] args)
        {
            if (DebugEnabled)
            {
                WriteMessage(DEBUG_CATEGORY, format, args);
            }
        }

        /// <summary>
        /// Logs a formatted audit message.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Audit(string format, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(string.Format(format, args));
            WriteMessage(AUDIT_CATEGORY, format, args);
        }

        /// <summary>
        /// Logs a formatted error message.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Error(string format, params object[] args)
        {
            WriteMessage(ERROR_CATEGORY, format, args);
        }

        /// <summary>
        /// Logs a formatted error message.
        /// </summary>
        public static void Exception(Exception exception)
        {
            lock (_logger.Lock) // keep all lines together
            {
                var level = 0;
                var inner = "";
                while (exception != null)
                {
                    WriteMessage(ERROR_CATEGORY, "{0}{1}: {2}", inner, exception.GetType().Name, exception.Message);
                    if (exception.StackTrace != null)
                    {
                        foreach (var line in exception.StackTrace.Split('\n'))
                        {
                            WriteMessage(ERROR_CATEGORY, "... {0}", line);
                        }
                    }
                    exception = exception.InnerException;
                    if (exception != null)
                    {
                        inner = string.Format("Inner({0}) ", ++level);
                    }
                }
            }
        }

        /// <summary>
        /// Actually logs the message.
        /// </summary>
        private static void WriteMessage(string category, string format, params object[] args)
        {
            Logger.LogMessage(category, format, args);
        }

        /// <summary>
        /// Built in logger.
        /// </summary>
        private class SystemDebugLogger : ILogger
        {
            private readonly object _lock = new object();

            public object Lock
            {
                get
                {
                    return _lock;
                }
            }

            /// <summary>
            /// Logs a message using the built in OS logger.
            /// </summary>
            /// <param name="category"></param>
            /// <param name="format"></param>
            /// <param name="args"></param>
            public void LogMessage(string category, string format, params object[] args)
            {
                var msg = DefaultMessageLogger.FormatLine(category, format, args);
                lock (_lock)
                {
                    System.Diagnostics.Debug.Write(msg);
                }
            }
        }

        /// <summary>
        /// Implements the default message logging.
        /// </summary>
        public class DefaultMessageLogger : ILogger
        {
            private readonly object _lock = new object();
            private Stream _logFileStream;
            private string _tempFilename;
            private string _filename;

            /// <summary>
            /// Default Constructor.
            ///
            /// Initializes a new logger.
            /// This variation creates the log file named using a datetime string.
            /// </summary>
            public DefaultMessageLogger()
            {
                _filename = Path.Combine(WRITABLE_ROOT_DIRECTORY, "BB " + FileNaming.GenerateDateTimeString());
                _filename += ".txt";
                Open(_filename);
            }

            /// <summary>
            /// Constructor.
            ///
            /// Initializes a new logger.
            /// This variation creates a temporary randomly named log file and after <see cref="delay"/>
            /// renames the log file to a file name made up using a datetime string.
            /// </summary>
            /// <param name="delay">The time delay in milliseconds before the log file is renamed.</param>
            public DefaultMessageLogger(int delay)
            {
                System.Threading.Timer renameFileTimer = null;
                renameFileTimer = new System.Threading.Timer((obj) =>
                {
                    Debug("**************BIG BROTHER ABOUT TO SWITCH FROM TEMP RANDOM FILE NAME TO PERMANENT DATETIME FILENAME**************");
                    ChangeFileName(Path.Combine(WRITABLE_ROOT_DIRECTORY, "BB " + FileNaming.GenerateDateTimeString()) + ".txt");
                    renameFileTimer.Dispose();
                    Debug("**************BIG BROTHER FINISHED SWITCHING FROM TEMP RANDOM FILE NAME TO PERMANENT DATETIME FILENAME**************");
                }, null, delay, System.Threading.Timeout.Infinite);

                _tempFilename = Path.Combine(WRITABLE_ROOT_DIRECTORY, "BBT " + FileNaming.GenerateRandomString());
                _tempFilename += ".txt";
                Open(_tempFilename);
            }

            /// <summary>
            /// Changes the _filename of the log file.
            /// </summary>
            /// <param name="newFilename">The new _filename for the log file.</param>
            private void ChangeFileName(string newFilename)
            {
                FileInfo sourceFileInfo = new FileInfo(_tempFilename);
                if (sourceFileInfo.Exists)
                {
                    _filename = newFilename;
                    sourceFileInfo.MoveTo(newFilename);
                    _logFileStream.Close();
                    Open(_filename);
                }
            }

            /// <summary>
            /// Opens a new file stream for logging to the log file.
            /// </summary>
            private void Open(string filename)
            {
                Directory.CreateDirectory(WRITABLE_ROOT_DIRECTORY);
                _logFileStream = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                _logFileStream.Seek(0, SeekOrigin.End);
            }

            /// <summary>
            /// Logs a new message to the log file.
            /// </summary>
            /// <param name="category">Category of the log message.</param>
            /// <param name="format">Format of the log message.</param>
            /// <param name="args"></param>
            public void LogMessage(string category, string format, params object[] args)
            {
                var msg = FormatLine(category, format, args);
                var bytes = Encoding.UTF8.GetBytes(msg);
                lock (_lock)
                {
                    _logFileStream.Write(bytes, 0, bytes.Length);
                    _logFileStream.Flush();
                }
                WriteSystem(msg);
            }

            /// <summary>
            /// General lock file for when logging to the log file.
            /// </summary>
            public object Lock
            {
                get
                {
                    return _lock;
                }
            }

            /// <summary>
            /// Formats a log message.
            /// </summary>
            /// <param name="category"></param>
            /// <param name="format"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public static string FormatLine(string category, string format, object[] args)
            {
                var msg = string.Format(format, args);
                var time = DateTime.Now.ToString("s");
                var text = new StringBuilder(100)
                    .Append(time)
                    .Append(' ')
                    .Append(category)
                    .Append(' ')
                    .Append(msg)
                    .AppendLine()
                    .ToString();
                return text;
            }

            /// <summary>
            /// Writes <paramref name="msg"/> to the windows logging functions for output to the debug port and debugger.
            /// </summary>
            private static void WriteSystem(string msg)
            {
                System.Diagnostics.Debug.Write(msg);
            }
        }
    }

    /// <summary>
    /// Logging implementation interface
    /// </summary>
    public interface ILogger
    {
        void LogMessage(string category, string format, params object[] args);

        /// <summary>
        /// Gets a lock object for multiple consecutive log entries that need grouping.
        /// </summary>
        object Lock { get; }
    }
}