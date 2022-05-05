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

		/// <summary>
		/// Alternative Merge of settings from the template with the existing settings
		/// </summary>
		/// <remarks>
		/// This is to not to break older versions of settings file ASPT.xml.
		/// ApplyTemplateForm actually uses Append and Prepend for Merge/AltMerge.
		/// For translation providers the old Merge matches Prepend, 
		/// for Termbases the old Merge matches Append.
		/// </remarks>
		AltMerge = 3
    }
}
