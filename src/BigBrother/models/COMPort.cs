namespace BigBrother.models
{
    /// <summary>
    /// Encapsulates all information about a single COM Port.
    /// </summary>
    public class COMPort
    {
        public string Name { get; private set; }

        /// <summary>
        /// Constructor.
        ///
        /// Initializes a new single COM Port.
        /// </summary>
        /// <param name="name">The name of the COM Port, i.e., COM1, COM4, COM6, ect.</param>
        public COMPort(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Generates a formatted string containing all COM Port information.
        /// </summary>
        /// <returns>The formatted string with COM Port information.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}