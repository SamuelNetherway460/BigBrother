using System.IO;
using System.Text;
using BigBrother.utilities.general;

namespace BigBrother.logging
{
    /// <summary>
    /// Class for logging to a tombstone file for when the big
    /// brother app encounters exceptions or dies completely.
    /// </summary>
    public class Tombstone
    {
        public string GraveAddress { get; private set; }
        private Stream _tombstoneFileStream;

        /// <summary>
        /// Constructor.
        ///
        /// Digs a new grave and plants the tombstone.
        /// </summary>
        /// <param name="graveAddress">Where the tombstone is planted.</param>
        public Tombstone(string graveAddress)
        {
            this.GraveAddress = graveAddress;
        }

        /// <summary>
        /// Writes the <see cref="lastWords"/> on the tombstone.
        /// </summary>
        /// <param name="lastWords">Description of death.</param>
        public void Epitaph(string lastWords)
        {
            this._tombstoneFileStream = File.Open(GraveAddress, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                FileShare.ReadWrite);
            this._tombstoneFileStream.Seek(0, SeekOrigin.End);
            var bytes = Encoding.UTF8.GetBytes("\nHere lies the departed " + resources.Constants.APP_NAME + " " + 
                                               FileNaming.GenerateDateTimeString() + ": " + lastWords);
            _tombstoneFileStream.Write(bytes, 0, bytes.Length);
            _tombstoneFileStream.Flush();
            _tombstoneFileStream.Close();
        }
    }
}