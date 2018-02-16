//-----------------------------------------------------------------------
// <copyright file="TmTranslationVariant.cs" company="SDL plc">
//  Copyright (c) SDL plc. All rights reserved.
// </copyright>
// <summary>
//  A translation unit variant (TUV) for a segment match from the TM.
// </summary>
//-----------------------------------------------------------------------
namespace Sdl.Sdk.SdlxTmTranslationProvider.SdlxTm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// A translation unit variant (TUV) for a segment match from the TM.
    /// </summary>
    public class TmTranslationVariant
    {
        /// <summary>
        /// The properties for this variant
        /// </summary>
        private List<TmField> properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="TmTranslationVariant"/> class.
        /// </summary>
        /// <param name="variantNode">The variant node.</param>
        public TmTranslationVariant(XmlNode variantNode)
        {
            // Extract basic information
            XmlNode node = variantNode.SelectSingleNode("@xml:lang", null);
            if (node != null)
            {
                this.Language = node.Value;
            }

            node = variantNode.SelectSingleNode("@langindex");
            if (node != null)
            {
                this.LanguageIndex = Convert.ToInt32(node.Value);
            }

            node = variantNode.SelectSingleNode("seg");
            if (node != null && node.HasChildNodes)
            {
                this.Text = node.FirstChild.Value;
            }
            else
            {
                this.Text = string.Empty;
            }
            
            // Now get the properties
            this.properties = new List<TmField>();
            foreach (XmlNode propertyNode in variantNode.SelectNodes("prop"))
            {
                TmField newProperty = new TmField(propertyNode);
                this.properties.Add(newProperty);
            }
        }

        /// <summary>
        /// Gets the properties for this match.
        /// </summary>
        /// <value>The properties for this match.</value>
        public List<TmField> Properties
        {
            get { return this.properties; }
        }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language for this variant.</value>
        public string Language
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the index of the language.
        /// </summary>
        /// <value>The index of the language.</value>
        public int LanguageIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text for this variant.</value>
        public string Text
        {
            get;
            set;
        }
    }
}
