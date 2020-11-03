using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Sdl.Community.DeeplAddon.Infrastructure
{
    /// <summary>
    /// A simple class that implements the CRC32 algorithm.
    /// </summary>
    internal class Crc32 : HashAlgorithm
    {
        /// <summary>
        /// Contains the default Polynomial.
        /// </summary>
        private const uint DefaultPolynomial = 0xedb88320u;

        /// <summary>
        /// Contains the default seed.
        /// </summary>
        private const uint DefaultSeed = 0xffffffffu;

        /// <summary>
        /// A value for the hash.
        /// </summary>
        private uint hash;

        /// <summary>
        /// A value for the seed.
        /// </summary>
        private readonly uint seed;

        /// <summary>
        /// A table of Unsigned Integers.
        /// </summary>
        private readonly uint[] table;

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc32"/> class.
        /// </summary>
        public Crc32()
        {
            this.table = Crc32.InitializeTable(Crc32.DefaultPolynomial);
            this.seed = Crc32.DefaultSeed;
            this.hash = this.seed;
        }

        /// <summary>
        /// Gets the size, in bits, of the computed hash code.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The size, in bits, of the computed hash code.
        /// </returns>
        public override int HashSize => 32;

        /// <summary>
        /// Gets the CRC.
        /// </summary>
        /// <param name="data">The string to compute the CRC for.</param>
        /// <returns>The CRC of the supplied string.</returns>
        public static int GetCrc(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return 0;
            }

            byte[] srcbytes = Encoding.Unicode.GetBytes(data);
            return Crc32.GetCrc(srcbytes);
        }

        /// <summary>
        /// Gets the CRC.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The CRC value for the supplied data.</returns>
        public static int GetCrc(byte[] data)
        {
            if (data == null)
            {
                return 0;
            }

            string srchash = string.Empty;
            using (Crc32 crc = new Crc32())
            {
                srchash = crc.ComputeHash(data).Aggregate(srchash, (current, b) => current + b.ToString("x2").ToLower(CultureInfo.InvariantCulture));
            }

            return int.Parse(srchash, NumberStyles.HexNumber);
        }

        /// <summary>
        /// Initializes an implementation of the <see cref="T:System.Security.Cryptography.HashAlgorithm"/> class.
        /// </summary>
        public override void Initialize()
        {
            this.hash = this.seed;
        }

        /// <summary>
        /// Hashes the core.
        /// </summary>
        /// <param name="array">The buffer.</param>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        protected override void HashCore(byte[] array, int start, int length)
        {
            this.hash = Crc32.CalculateHash(this.table, this.hash, array, start, length);
        }

        /// <summary>
        /// When overridden in a derived class, finalizes the hash computation after the last data is processed by the cryptographic stream object.
        /// </summary>
        /// <returns>The computed hash code.</returns>
        protected override byte[] HashFinal()
        {
            byte[] hashBuffer = Crc32.UIntToBigEndianBytes(~this.hash);
            this.HashValue = hashBuffer;
            return hashBuffer;
        }

        /// <summary>
        /// Initializes the table.
        /// </summary>
        /// <param name="polynomial">The polynomial.</param>
        /// <returns>The table of unsigned integers.</returns>
        private static uint[] InitializeTable(uint polynomial)
        {
            uint[] createTable = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                uint entry = (uint)i;
                for (int j = 0; j < 8; j++)
                {
                    if ((entry & 1) == 1)
                    {
                        entry = (entry >> 1) ^ polynomial;
                    }
                    else
                    {
                        entry = entry >> 1;
                    }
                }

                createTable[i] = entry;
            }

            return createTable;
        }

        /// <summary>
        /// Calculates the hash.
        /// </summary>
        /// <param name="table">The table with which to compute the checksum.</param>
        /// <param name="seed">The seed to use.</param>
        /// <param name="buffer">The buffer of data for which the checksum should be calculated.</param>
        /// <param name="start">The start point in the buffer.</param>
        /// <param name="size">The number of bytes with which to compute the checksum.</param>
        /// <returns>An unsigned integer representing the hash value.</returns>
        private static uint CalculateHash(uint[] table, uint seed, byte[] buffer, int start, int size)
        {
            uint crc = seed;
            for (int i = start; i < size; i++)
            {
                unchecked
                {
                    crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
                }
            }

            return crc;
        }

        /// <summary>
        /// Hashes the core.
        /// </summary>
        /// <param name="array">The buffer.</param>
        public uint CalculateHash2(byte[] array)
        {
            return Crc32.CalculateHash(this.table, this.hash, array, 0, array.Length);
        }

        /// <summary>
        /// Converts an unsigned integer to an array of bytes.
        /// </summary>
        /// <param name="x">The value to convert.</param>
        /// <returns>An array of BigEndian bytes.</returns>
        private static byte[] UIntToBigEndianBytes(uint x) => new[]
                                                              {
                                                                  (byte) ((x >> 24) & 0xff),
                                                                  (byte) ((x >> 16) & 0xff),
                                                                  (byte) ((x >> 8) & 0xff),
                                                                  (byte) (x & 0xff)
                                                              };
    }
}
