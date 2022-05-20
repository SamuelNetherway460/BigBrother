namespace BigBrother.models
{
    //TODO DOCUMENTATION
    //TODO IMPLEMENT
    /// <summary>
    /// Class that encapsulates information about a single USB device
    /// that is currently connected at the time of logging.
    /// </summary>
    public class USBDeviceInfo
    {
        public string DeviceID { get; private set; }
        public string PnpDeviceID { get; private set; }
        public string Description { get; private set; }

        //TODO DOCUMENTATION
        //TODO IMPLEMENT
        //TODO TEST
        /// <summary>
        /// Constructor.
        ///
        /// Initializes a new USB Device that is currently connected.
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="pnpDeviceID"></param>
        /// <param name="description"></param>
        public USBDeviceInfo(string deviceID, string pnpDeviceID, string description)
        {
            this.DeviceID = deviceID;
            this.PnpDeviceID = pnpDeviceID;
            this.Description = description;
        }

        //TODO DOCUMENTATION
        //TODO IMPLEMENT
        //TODO TEST
        /// <summary>
        /// Generates a formatted string containing all USB Device information.
        /// </summary>
        /// <returns>The formatted string with USB Device information.</returns>
        public override string ToString()
        {
            string usbDeviceInfoString = "Device ID: " + DeviceID +
                                         "\nPnp Device ID: " + PnpDeviceID +
                                         "\nDescription: " + Description;

            return usbDeviceInfoString;
        }
    }
}
