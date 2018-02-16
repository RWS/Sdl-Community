//-----------------------------------------------------------------------
// <copyright file="TmPenalties.cs" company="SDL plc">
//  Copyright (c) SDL plc. All rights reserved.
// </copyright>
// <summary>
//  A collection of penalties that have been applied to a match from a TM.
// </summary>
//-----------------------------------------------------------------------
namespace Sdl.Sdk.SdlxTmTranslationProvider.SdlxTm
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// A collection of penalties that have been applied to a match from a TM.
    /// </summary>
    public class TmPenalties
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TmPenalties"/> class.
        /// </summary>
        public TmPenalties()
        {
            this.AutomatchPenalty = 0;
            this.TextPenalty = 0;
            this.PunctuationPenalty = 0;
            this.ContextPenalty = 0;
            this.FilterPenalty = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TmPenalties"/> class.
        /// </summary>
        /// <param name="translationUnit">The translation unit.</param>
        public TmPenalties(XmlNode translationUnit)
            : this()
        {
            this.AutomatchPenalty = this.GetAttributeValue(translationUnit, "automatch_penalty", this.AutomatchPenalty);
            this.TextPenalty = this.GetAttributeValue(translationUnit, "textdiff_penalty", this.TextPenalty);
            this.PunctuationPenalty = this.GetAttributeValue(translationUnit, "punctuation_penalty", this.PunctuationPenalty);
            this.ContextPenalty = this.GetAttributeValue(translationUnit, "context_penalty", this.ContextPenalty);
            this.FilterPenalty = this.GetAttributeValue(translationUnit, "filter_penalty", this.FilterPenalty);
        }

        /// <summary>
        /// Gets or sets the automatch penalty.
        /// </summary>
        /// <value>The automatch penalty.</value>
        public double AutomatchPenalty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text penalty.
        /// </summary>
        /// <value>The text penalty.</value>
        public double TextPenalty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the punctuation penalty.
        /// </summary>
        /// <value>The punctuation penalty.</value>
        public double PunctuationPenalty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the context penalty.
        /// </summary>
        /// <value>The context penalty.</value>
        public double ContextPenalty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filter penalty.
        /// </summary>
        /// <value>The filter penalty.</value>
        public double FilterPenalty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the apply penalty.
        /// </summary>
        /// <value>The apply penalty.</value>
        public double ApplyPenalty
        {
            get;
            set;
        }

        /// <summary>
        /// Converts a string to a double.
        /// </summary>
        /// <param name="numberValue">The number value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The double value
        /// </returns>
        public static double ConvertToDouble(string numberValue, double defaultValue)
        {
            double returnValue = 0;
            NumberFormatInfo formatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            if (numberValue.Contains(","))
            {
                formatInfo.NumberGroupSeparator = ".";
                formatInfo.NumberDecimalSeparator = ",";
            }

            try
            {
                returnValue = Convert.ToDouble(numberValue, formatInfo);
            }
            catch (FormatException)
            {
                returnValue = defaultValue;
            }

            return returnValue;
        }

        /// <summary>
        /// Gets an attribute value from the translation unit.
        /// </summary>
        /// <param name="translationUnit">The translation unit.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value of the attribute.</returns>
        private double GetAttributeValue(XmlNode translationUnit, string attributeName, double defaultValue)
        {
            XmlNode attributeNode = translationUnit.SelectSingleNode("@" + attributeName);
            if (attributeNode == null)
            {
                return defaultValue;
            }

            return TmPenalties.ConvertToDouble(attributeNode.Value, defaultValue);
        }
    }
}
