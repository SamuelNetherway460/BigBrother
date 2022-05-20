using System;

namespace BigBrother.utilities.general
{
    /// <summary>
    /// General utility functions for naming files.
    /// </summary>
    static class FileNaming
    {
        /// <summary>
        /// Generates a random string using a GUID.
        /// </summary>
        /// <returns>The randomly generated string.</returns>
        public static string GenerateRandomString()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Generates a formatted datetime string adding leading zeros
        /// to the month, day, hour, minute and second if single digits.
        /// </summary>
        /// <returns>The formatted datetime string.</returns>
        public static string GenerateDateTimeString()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;

            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            int seconds = DateTime.Now.Second;

            string monthString = month < 10 ? "0" + month : month.ToString();
            string dayString = day < 10 ? "0" + day : day.ToString();

            string hourString = hour < 10 ? "0" + hour : hour.ToString();
            string minuteString = minute < 10 ? "0" + minute : minute.ToString();
            string secondsString = seconds < 10 ? "0" + seconds : seconds.ToString();

            return dayString + "-" + monthString + "-" + year + " " + hourString + "." + minuteString + "." + secondsString;
        }
    }
}
