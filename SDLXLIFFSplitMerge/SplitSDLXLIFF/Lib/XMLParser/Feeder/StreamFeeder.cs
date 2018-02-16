// <copyright file="StreamFeeder.cs" company="SDL International">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Oleksandr Tkachenko</author>
// <email>otkachenko@sdl.com</email>
// <date>2010-06-10</date>
// <summary>StreamFeeder</summary>

namespace Sdl.Utilities.SplitSDLXLIFF.Lib
{
    using System;
    using System.IO;

    /// <summary>
    /// On progress delegate
    /// </summary>
    /// <param name="progress">Represents a value of finishing in percents</param>
    public delegate void OnProgressDelegate(double progress);

    /// <summary>
    /// Class that suplies TagParser with strings
    /// </summary>
    public class StreamFeeder : IStringFeeder
    {
        /// <summary>
        /// Path to the file
        /// </summary>
        internal readonly string FilePath;
        
        /// <summary>
        /// Size of the source
        /// </summary>
        private long sourceSize;

        /// <summary>
        /// Stream reader
        /// </summary>
        private StreamReader reader;

        /// <summary>
        /// End of file
        /// </summary>
        private bool eof;

        /// <summary>
        /// Buffer of chars
        /// </summary>
        private char[] buffer;
        
        /// <summary>
        /// Initializes a new instance of the StreamFeeder class
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        public StreamFeeder(string filePath)
        {
            this.FilePath = filePath;
            this.eof = false;
            this.BytesToRead = 4096;
        }

        /// <summary>
        /// On progress event
        /// </summary>
        public event OnProgressDelegate OnProgress;
        
        /// <summary>
        /// Gets bytest to read
        /// </summary>
        public int BytesToRead { get; private set; }

        /// <summary>
        /// Gets read count
        /// </summary>
        public long ReadCount { get; private set; }

        /// <summary>
        /// Initializing variables
        /// </summary>
        public void Initialize()
        {
            this.reader = new StreamReader(this.FilePath);

            this.eof = false;
            this.buffer = new char[this.BytesToRead];

            this.ReadCount = 0;

            FileInfo info = new FileInfo(this.FilePath);
            this.sourceSize = info.Length;
        }

        /// <summary>
        /// Feeds the string
        /// </summary>
        /// <returns>String, generates from reader</returns>
        public string FeedString()
        {
            if (!this.eof)
            {
                if (!this.reader.EndOfStream)
                {
                    for (int j = 0; j < this.BytesToRead; j++)
                    {
                        this.buffer[j] = '\0';
                    }

                    this.reader.Read(this.buffer, 0, this.BytesToRead); // read small amount of first butes

                    this.ReadCount += this.BytesToRead;

                    if (this.OnProgress != null)
                    {
                        this.OnProgress(this.Progress());
                    }

                    return new string(this.buffer);
                }
                else
                {
                    this.eof = true;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns true if end of file
        /// </summary>
        /// <returns>True if end of file</returns>
        public bool EOF()
        {
            return this.eof;
        }

        /// <summary>
        /// Dispose the reader
        /// </summary>
        public void Dispose()
        {
            this.reader.Close();
            this.reader.Dispose();
        }

        /// <summary>
        /// Returns progress in percents
        /// </summary>
        /// <returns>Progress in percents</returns>
        public double Progress()
        {
            double progress = Convert.ToDouble(this.ReadCount) / this.sourceSize * 100;

            if (progress > 100)
            {
                return 100;
            }

            return progress;
        }
    }
}
