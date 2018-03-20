// ---------------------------------
// <copyright file="Extensions.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-07</date>
// ---------------------------------
namespace Sdl.Community.Trados2007
{
    using System;

    using Trados.Interop.TMAccess;

    /// <summary>
    /// Extension methods definitions
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Converts <see cref="TranslationMemoryAccessMode"/> to its unmanaged analog.
        /// </summary>
        /// <param name="accessMode">The access mode.</param>
        /// <returns>Unmanaged access mode.</returns>
        internal static tmaTmAccessMode ToUnmanaged(this TranslationMemoryAccessMode accessMode)
        {
            switch (accessMode)
            {
                case TranslationMemoryAccessMode.Read:
                    return tmaTmAccessMode.tmaTmAccessRead;

                case TranslationMemoryAccessMode.Write:
                    return tmaTmAccessMode.tmaTmAccessWrite;

                case TranslationMemoryAccessMode.Maintenance:
                    return tmaTmAccessMode.tmaTmAccessMaintenance;

                case TranslationMemoryAccessMode.Exclusive:
                    return tmaTmAccessMode.tmaTmAccessExclusive;

                default:
                    return tmaTmAccessMode.tmaTmAccessRead;
            }
        }

        /// <summary>
        /// Converts string to <see cref="TranslationMemoryAccessMode"/>.
        /// </summary>
        /// <param name="translationMemoryAccessMode">The translation memory access mode.</param>
        /// <returns><see cref="TranslationMemoryAccessMode"/> mode.</returns>
        internal static TranslationMemoryAccessMode FromString(string translationMemoryAccessMode)
        {
            if (translationMemoryAccessMode.Equals(@"Read"))
            {
                return TranslationMemoryAccessMode.Read;
            }

            if (translationMemoryAccessMode.Equals(@"Write"))
            {
                return TranslationMemoryAccessMode.Write;
            }

            if (translationMemoryAccessMode.Equals(@"Exclusive"))
            {
                return TranslationMemoryAccessMode.Exclusive;
            }

            if (translationMemoryAccessMode.Equals(@"Maintenance"))
            {
                return TranslationMemoryAccessMode.Maintenance;
            }

            throw new NotSupportedException("Only TranslationMemoryAccessMode is supported.");
        }
    }
}