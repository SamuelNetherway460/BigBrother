using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using BigBrother.models;

namespace BigBrother.logging
{
    /// <summary>
    /// Controls the collection, formatting, writing and timing of log information.
    /// </summary>
    public class LogGenerator
    {
        public string Name { get; private set; }
        public int DueTime { get; private set; }
        public int Period { get; private set; }
        private Timer Timer { get; set; }

        public List<COMPort> COMPorts { get; private set; }
        public List<ProcessInfo> Processes { get; private set; }
        public List<USBDeviceInfo> USBDevices { get; private set; }
        public Diagnostics Diagnostics { get; private set; }

        public bool IncludeCOMPorts { get; private set; }
        public bool IncludeProcesses { get; private set; }
        public bool IncludeUSBDevices { get; private set; }
        public bool IncludeDiagnostics { get; private set; }
        public List<string> PrintThreadsForProcessesList { get; private set; }

        /// <summary>
        /// Constructor.
        ///
        /// Initializes a new LogGenerator ready to start logging.
        /// </summary>
        /// <param name="name">Name of the LogGenerator for identification.</param>
        /// <param name="dueTime">Time delay before logging starts.</param>
        /// <param name="period">How often logs are taken for the selected items.</param>
        /// <param name="includeCOMPorts">Whether or not to log COM Ports.</param>
        /// <param name="includeProcesses">Whether or not to log information on active processes.</param>
        /// <param name="includeUSBDevices">Whether or not to log information on connected USB devices.</param>
        /// <param name="includeDiagnostics">Whether or not to log general diagnostics information, i.e., CPU usage, RAM usage, etc.</param>
        /// <param name="printThreadsForProcessesList">List of .exe files indicating which processes to print thread IDs for.</param>
        public LogGenerator(string name, int dueTime, int period, bool includeCOMPorts, bool includeProcesses,
            bool includeUSBDevices, bool includeDiagnostics, List<string> printThreadsForProcessesList)
        {
            this.Name = name;
            this.DueTime = dueTime;
            this.Period = period;

            this.IncludeCOMPorts = includeCOMPorts;
            this.IncludeProcesses = includeProcesses;
            this.IncludeUSBDevices = includeUSBDevices;
            this.IncludeDiagnostics = includeDiagnostics;
            this.PrintThreadsForProcessesList = printThreadsForProcessesList;

            COMPorts = new List<COMPort>();
            Processes = new List<ProcessInfo>();
            USBDevices = new List<USBDeviceInfo>();
        }

        /// <summary>
        /// Starts writing log data to the log file.
        /// Will be delayed according to <see cref="DueTime"/>
        /// </summary>
        public void StartLogging()
        {
            try
            {
                logging.Logging.Debug(resources.Constants.LOG_GENERATOR_STARTED + Name + " | " + Period + "ms");
                Timer = new Timer(
                Log,
                null,
                DueTime,
                Period);
            }
            catch (Exception e)
            {
                Program.Tombstone.Epitaph(e.Message);
            }
        }

        /// <summary>
        /// Stops writing log data to the log file.
        /// </summary>
        public void StopLogging()
        {
            try
            {
                Timer.Dispose();
                logging.Logging.Debug(resources.Constants.LOG_GENERATOR_STOPPED + Name);
            }
            catch (Exception e)
            {
                Program.Tombstone.Epitaph(e.Message);
            }
        }

        /// <summary>
        /// Resets the LogGenerator parameters and starts the logging process.
        /// </summary>
        /// <param name="dueTime">Time delay before logging starts.</param>
        /// <param name="period">How often logs are taken for the selected items.</param>
        /// <param name="includeCOMPorts">Whether or not to log COM Ports.</param>
        /// <param name="includeProcesses">Whether or not to log information on active processes.</param>
        /// <param name="includeUSBDevices">Whether or not to log information on connected USB devices.</param>
        /// <param name="includeDiagnostics">Whether or not to log general diagnostics information, i.e., CPU usage, RAM usage, etc.</param>
        /// <param name="printThreadsForProcessesList">List of .exe files indicating which processes to print thread IDs for.</param>
        public void RestartLogging(int dueTime, int period, bool includeCOMPorts, bool includeProcesses,
            bool includeUSBDevices, bool includeDiagnostics, List<string> printThreadsForProcessesList)
        {
            try
            {
                ResetLogging(dueTime, period, includeCOMPorts, includeProcesses, includeUSBDevices, includeDiagnostics, printThreadsForProcessesList);
                StartLogging();
            }
            catch (Exception e)
            {
                Program.Tombstone.Epitaph(e.Message);
            }
        }

        /// <summary>
        /// Resets the LogGenerator parameters.
        /// </summary>
        /// <param name="dueTime">Time delay before logging starts.</param>
        /// <param name="period">How often logs are taken for the selected items.</param>
        /// <param name="includeCOMPorts">Whether or not to log COM Ports.</param>
        /// <param name="includeProcesses">Whether or not to log information on active processes.</param>
        /// <param name="includeDiagnostics">Whether or not to log general diagnostics information, i.e., CPU usage, RAM usage, etc.</param>
        /// <param name="printThreadsForProcessesList">List of .exe files indicating which processes to print thread IDs for.</param>
        public void ResetLogging(int dueTime, int period, bool includeCOMPorts, bool includeProcesses,
            bool includeUSBDevices, bool includeDiagnostics, List<string> printThreadsForProcessesList)
        {
            try
            {
                this.DueTime = dueTime;
                this.Period = period;

                this.IncludeCOMPorts = includeCOMPorts;
                this.IncludeProcesses = includeProcesses;
                this.IncludeUSBDevices = includeUSBDevices;
                this.IncludeDiagnostics = includeDiagnostics;
                this.PrintThreadsForProcessesList = printThreadsForProcessesList;

                COMPorts = new List<COMPort>();
                Processes = new List<ProcessInfo>();
                USBDevices = new List<USBDeviceInfo>();
            }
            catch (Exception e)
            {
                Program.Tombstone.Epitaph(e.Message);
            }
        }

        /// <summary>
        /// Logs all the selected data items to the log file.
        /// </summary>
        /// <param name="o"></param>
        private void Log(object o)
        {
            try
            {
                Update();
                if (IncludeCOMPorts) LogCOMPorts();
                if (IncludeProcesses) LogProcesses();
                if (IncludeUSBDevices) LogUSBDevices();
                if (IncludeDiagnostics) LogDiagnostics();
            }
            catch (Exception e)
            {
                Program.Tombstone.Epitaph(e.Message);
            }
        }

        /// <summary>
        /// Logs all the selected data items to the log file.
        /// </summary>
        public void Log()
        {
            try
            {
                Update();
                if (IncludeCOMPorts) LogCOMPorts();
                if (IncludeProcesses) LogProcesses();
                if (IncludeUSBDevices) LogUSBDevices();
                if (IncludeDiagnostics) LogDiagnostics();
            }
            catch (Exception e)
            {
                Program.Tombstone.Epitaph(e.Message);
            }
        }

        /// <summary>
        /// Logs all COM Port information to the log file.
        /// </summary>
        private void LogCOMPorts()
        {
            try
            {
                string logString = string.Empty;
                for (int i = 0; i < COMPorts.Count; i++)
                {
                    logString += COMPorts[i].ToString();
                    if (i != COMPorts.Count - 1) logString += " | ";
                }

                logging.Logging.Debug(logString);
            }
            catch (Exception e)
            {
                Program.Tombstone.Epitaph(e.Message);
            }
        }

        /// <summary>
        /// Logs information about active processes to the log file.
        /// </summary>
        private void LogProcesses()
        {
            try
            {
                foreach (ProcessInfo p in Processes)
                {
                    logging.Logging.Debug("\n" + p.ToString(PrintThreadsForProcessesList.Contains(p.Name)) + "\n");
                }
            }
            catch (Exception e)
            {
                Program.Tombstone.Epitaph(e.Message);
            }
        }

        /// <summary>
        /// Logs information about connected USB devices to the log file.
        /// </summary>
        private void LogUSBDevices()
        {
            try
            {
                string logString = string.Empty;
                foreach (USBDeviceInfo usb in USBDevices)
                {
                    logString += usb.ToString();
                }

                logging.Logging.Debug(logString);
            }
            catch (Exception e)
            {
                Program.Tombstone.Epitaph(e.Message);
            }
        }

        /// <summary>
        /// Logs general system information i.e., CPU usage, Memory usage, etc.
        /// </summary>
        private void LogDiagnostics()
        {
            try
            {
                logging.Logging.Debug("\n" + Diagnostics);
            }
            catch (Exception e)
            {
                Program.Tombstone.Epitaph(e.Message);
            }
        }

        /// <summary>
        /// Updates the log information of all selected data items.
        /// </summary>
        public void Update()
        {
            try
            {
                if (IncludeCOMPorts)
                {
                    UpdateCOMPorts();
                }

                if (IncludeProcesses)
                {
                    UpdateProcesses();
                }

                if (IncludeUSBDevices)
                {
                    UpdateUSBDevices();
                }

                if (IncludeDiagnostics)
                {
                    UpdateDiagnostics();
                }
            }
            catch (Exception e)
            {
                Program.Tombstone.Epitaph(e.Message);
            }
        }

        /// <summary>
        /// Updates log information on COM Ports.
        /// </summary>
        private void UpdateCOMPorts()
        {
            try
            {
                COMPorts = new List<COMPort>();

                string[] serialPortNames = SerialPort.GetPortNames();

                foreach (string com in serialPortNames)
                {
                    COMPorts.Add(new COMPort(com));
                }
            }
            catch (Exception e)
            {
                Program.Tombstone.Epitaph(e.Message);
            }
        }

        /// <summary>
        /// Updates log information about currently active processes.
        /// </summary>
        private void UpdateProcesses()
        {
            try
            {
                Processes = new List<ProcessInfo>();

                // Get all process and thread information from Terranova API
                Terranova.API.ProcessInfo[] processes = Terranova.API.ProcessCE.GetProcesses();
                Terranova.API.ThreadInfo[] allThreads = Terranova.API.ThreadCE.GetThreads();

                // Transfer process and thread information from Terranova API to custom objects
                foreach (Terranova.API.ProcessInfo pi in processes)
                {
                    List<ThreadInfo> threadInfos = new List<ThreadInfo>();
                    // Get all threads for the current process
                    foreach (Terranova.API.ThreadInfo ti in allThreads)
                    {
                        if (ti.ParentProcessID == pi.Pid) threadInfos.Add(new ThreadInfo(ti.Tid));
                    }
                    Processes.Add(new ProcessInfo(pi.Name, pi.FullPath, pi.Pid, pi.ThreadCount,
                        pi.ParentProcessID, threadInfos));
                }
            }
            catch (Exception e)
            { 
                Program.Tombstone.Epitaph(e.Message);
            }
        }

        //TODO DOCUMENTATION
        //TODO IMPLEMENT
        /// <summary>
        /// Updates log information about currently connected USB devices.
        /// </summary>
        private void UpdateUSBDevices()
        {
            try
            {

            }
            catch (Exception e)
            {
                Program.Tombstone.Epitaph(e.Message);
            }
        }

        //TODO DOCUMENTATION
        //TODO IMPLEMENT
        /// <summary>
        /// 
        /// </summary>
        private void UpdateDiagnostics()
        {
            try
            {

            }
            catch (Exception e)
            {
                Program.Tombstone.Epitaph(e.Message);
            }
        }
    }
}