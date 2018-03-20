// ---------------------------------
// <copyright file="TranslationMemoryAccessMode.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-07</date>
// ---------------------------------
namespace Sdl.Community.Trados2007
{
    /// <summary>
    /// Transaltion Memory access mode levels.
    /// </summary>
    public enum TranslationMemoryAccessMode
    {
        /// <summary>
        /// Read only
        /// </summary>
        Read,

        /// <summary>
        /// Read and write
        /// </summary>
        Write,

        /// <summary>
        /// Unknown mode
        /// </summary>
        Maintenance,

        /// <summary>
        /// Prevents other from accessing this tm.
        /// </summary>
        Exclusive
    }
}