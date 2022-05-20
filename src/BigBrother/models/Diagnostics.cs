namespace BigBrother.models
{
    //TODO DOCUMENTATION
    /// <summary>
    /// 
    /// </summary>
    public class Diagnostics
    {
        public string CPUUsage { get; private set; }
        public string TotalMemoryUsed { get; private set; }

        //TODO DOCUMENTATION
        //TODO IMPLEMENT
        //TODO TEST
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpuUsage"></param>
        /// <param name="totalMemoryUsed"></param>
        public Diagnostics(string cpuUsage, string totalMemoryUsed)
        {
            this.CPUUsage = cpuUsage;
            this.TotalMemoryUsed = totalMemoryUsed;
        }

        //TODO DOCUMENTATION
        //TODO IMPLEMENT
        //TODO TEST
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string toString = "CPU Usage: " + CPUUsage + "%" +
                              "\nMemory Usage: " + TotalMemoryUsed;
            return toString;
        }
    }
}