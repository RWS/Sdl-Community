// <copyright file="IStringFeeder.cs" company="SDL International">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Oleksandr Tkachenko</author>
// <email>otkachenko@sdl.com</email>
// <date>2010-06-10</date>
// <summary>IStringFeeder</summary>

namespace Sdl.Utilities.SplitSDLXLIFF.Lib
{
    using System;

    /// <summary>
    /// Interface for class that suplies TagParser with strings
    /// </summary>
    public interface IStringFeeder : IDisposable
    {
        /// <summary>
        /// Initialize class
        /// </summary>
        void Initialize();

        /// <summary>
        /// Provides string for parsing
        /// </summary>
        /// <returns>new string for parsing</returns>
        string FeedString();

        /// <summary>
        /// Indicates whether string cannot be supplied 
        /// </summary>
        /// <returns>True if string cannot be supplied</returns>
        bool EOF();

        /// <summary>
        /// Progress of processing source string
        /// </summary>
        /// <returns>Current progress</returns>
        double Progress();
    }
}
