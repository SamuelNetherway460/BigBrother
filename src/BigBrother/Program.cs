using System.Collections.Generic;
using BigBrother.logging;

namespace BigBrother
{
    /// <summary>
    /// Main program class.
    /// Simply sets off the gathering and logging of data.
    /// </summary>
    class Program
    {
        public static Tombstone Tombstone;
        private const int RENAME_FILE_DELAY = 20000;

        private const string SUPER_FREQUENT_LOG_GENERATOR_NAME = "SF";
        private const int SUPER_FREQUENT_LOG_GENERATOR_PERIOD = 1000;//ms
        private const int SUPER_FREQUENT_LOG_GENERATOR_DUE_TIME = 0;//ms

        private const string FREQUENT_LOG_GENERATOR_NAME = "F";
        private const int FREQUENT_LOG_GENERATOR_PERIOD = 5000;//ms
        private const int FREQUENT_LOG_GENERATOR_DUE_TIME = 1;//ms

        private const string INFREQUENT_LOG_GENERATOR_NAME = "IF";
        private const int INFREQUENT_LOG_GENERATOR_PERIOD = 30000;//ms
        private const int INFREQUENT_LOG_GENERATOR_DUE_TIME = 2;//ms

        private static LogGenerator _superFrequentLogGenerator;
        private static LogGenerator _frequentLogGenerator;
        private static LogGenerator _infrequentLogGenerator;

        /// <summary>
        /// Main method.
        ///
        /// Sets parameters for three different LogGenerator's:
        /// <see cref="_superFrequentLogGenerator"/>
        /// <see cref="_frequentLogGenerator"/>
        /// <see cref="_infrequentLogGenerator"/>
        /// Starts each of the LogGenerators.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Tombstone = new Tombstone("\\emmc\\App\\Logs\\BigBrother\\BB-TOMBSTONE.txt");
            Logging.Logger = new Logging.DefaultMessageLogger(RENAME_FILE_DELAY);
            LaunchApplication();

            Logging.Debug(resources.Constants.APP_NAME + " " + resources.Constants.VERSION_PREFIX + resources.Constants.VERSION + resources.Constants.VERSION_POSTFIX);

            List<string> printThreadsForProcessesList = new List<string>();

            _superFrequentLogGenerator = new LogGenerator(
                SUPER_FREQUENT_LOG_GENERATOR_NAME,
                SUPER_FREQUENT_LOG_GENERATOR_DUE_TIME,
                SUPER_FREQUENT_LOG_GENERATOR_PERIOD,
                false,
                false,
                false,
                false,
                printThreadsForProcessesList);

            _frequentLogGenerator = new LogGenerator(
                FREQUENT_LOG_GENERATOR_NAME,
                FREQUENT_LOG_GENERATOR_DUE_TIME,
                FREQUENT_LOG_GENERATOR_PERIOD,
                true,
                false,
                false,
                false,
                printThreadsForProcessesList);

            _infrequentLogGenerator = new LogGenerator(
                INFREQUENT_LOG_GENERATOR_NAME,
                INFREQUENT_LOG_GENERATOR_DUE_TIME,
                INFREQUENT_LOG_GENERATOR_PERIOD,
                false,
                true,
                false,
                false,
                printThreadsForProcessesList);

            _frequentLogGenerator.StartLogging();
            _infrequentLogGenerator.StartLogging();

            while(true){}
        }

        /// <summary>
        /// Launches required applications. Often the process under test.
        /// </summary>
        private static void LaunchApplication()
        {

        }
    }
}
