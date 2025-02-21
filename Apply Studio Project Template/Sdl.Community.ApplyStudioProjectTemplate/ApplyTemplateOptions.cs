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

        /// <summary>
        /// Merge the settings from the template with the existing settings
        /// </summary>
        Merge = 1,

        /// <summary>
        /// Overwrite the current settings with the settings from the template
        /// </summary>
        Overwrite = 2,
    }
}
