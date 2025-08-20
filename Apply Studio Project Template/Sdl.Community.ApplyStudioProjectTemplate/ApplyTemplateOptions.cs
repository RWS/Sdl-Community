// -----------------------------------------------------------------------
// <copyright file="ApplyTemplateOptions.cs" company="SDL plc">
// © 2014 SDL plc
// </copyright>
// -----------------------------------------------------------------------

namespace Sdl.Community.ApplyStudioProjectTemplate
{
    /// <summary>
    /// The options for applying the template
    /// </summary>
    public enum ApplyTemplateOptions
    {
        /// <summary>
        /// Keep the current settings
        /// </summary>
        Keep = 0,

        Merge = 1,
        /// <summary>
        /// Merge the settings from the template by prepending them to the existing settings
        /// </summary>
        MergePrepend = 2,

        /// <summary>
        /// Merge the settings from the template by appending them to the existing settings
        /// </summary>
        MergeAppend = 3,

        /// <summary>
        /// Overwrite the current settings with the settings from the template
        /// </summary>
        Overwrite = 4,
    }
}