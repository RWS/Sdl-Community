//-----------------------------------------------------------------------
// <copyright file="TmFieldType.cs" company="SDL plc">
//  Copyright (c) SDL plc. All rights reserved.
// </copyright>
// <summary>
//  Translation memory field types.
// </summary>
//-----------------------------------------------------------------------
namespace Sdl.Sdk.SdlxTmTranslationProvider.SdlxTm
{
    /// <summary>
    /// Translation memory field types.
    /// </summary>
    public enum TmFieldType
    {
        /// <summary>
        /// Text field
        /// </summary>
        TmFieldTypeText,

        /// <summary>
        /// Attribute field
        /// </summary>
        TmFieldTypeAttribute,

        /// <summary>
        /// Date field
        /// </summary>
        TmFieldTypeDate,

        /// <summary>
        /// Number field
        /// </summary>
        TmFieldTypeNumber
    }
}
