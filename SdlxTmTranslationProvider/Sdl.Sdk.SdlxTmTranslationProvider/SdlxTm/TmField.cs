//-----------------------------------------------------------------------
// <copyright file="TmField.cs" company="SDL plc">
//  Copyright (c) SDL plc. All rights reserved.
// </copyright>
// <summary>
//  A field from a segment match from the TM.
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
    /// A field from a segment match from the TM.
    /// </summary>
    public class TmField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TmField"/> class.
        /// </summary>
        public TmField()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TmField"/> class.
        /// </summary>
        /// <param name="propertyNode">The property node.</param>
        public TmField(XmlNode propertyNode)
        {
            // Determine whether this is a user-defined field
            this.UserDefined = false;
            XmlNode attributeNode = propertyNode.SelectSingleNode("@user_defined");
            if (attributeNode != null)
            {
                if (attributeNode.Value == "yes")
                {
                    this.UserDefined = true;
                }
            }

            // Determine the field type
            this.Type = TmFieldType.TmFieldTypeText;
            this.AllowMultiple = false;
            string propertyType = propertyNode.SelectSingleNode("@type").Value;
            string fieldType = propertyType.Substring(2, 3);
            switch (fieldType)
            {
                case "ALM":
                    this.Type = TmFieldType.TmFieldTypeAttribute;
                    this.AllowMultiple = true;
                    break;
                case "ALS":
                    this.Type = TmFieldType.TmFieldTypeAttribute;
                    break;
                case "DAT":
                    this.Type = TmFieldType.TmFieldTypeDate;
                    break;
                case "NUM":
                    this.Type = TmFieldType.TmFieldTypeNumber;
                    break;
            }

            // Now get the name and value
            this.Name = propertyType.Substring(6).Trim();
            string fieldValue = string.Empty;
            if (propertyNode.HasChildNodes)
            {
                fieldValue = propertyNode.FirstChild.Value;
            }

            // Convert dates as necessary
            if (this.Type == TmFieldType.TmFieldTypeDate)
            {
                int year = Convert.ToInt32(fieldValue.Substring(0, 4));
                int month = Convert.ToInt32(fieldValue.Substring(4, 2));
                int day = Convert.ToInt32(fieldValue.Substring(6, 2));
                int hour = Convert.ToInt32(fieldValue.Substring(9, 2));
                int minute = Convert.ToInt32(fieldValue.Substring(11, 2));
                int second = Convert.ToInt32(fieldValue.Substring(13, 2));
                this.Value = new DateTime(year, month, day, hour, minute, second);
            }
            else
            {
                this.Value = fieldValue;
            }
        }

        /// <summary>
        /// Gets or sets the field name.
        /// </summary>
        /// <value>The field name.</value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the field type.
        /// </summary>
        /// <value>The field type.</value>
        public TmFieldType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow multiple values.
        /// </summary>
        /// <value><c>true</c> if we allow multiple values; otherwise, <c>false</c>.</value>
        public bool AllowMultiple
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the field value.
        /// </summary>
        /// <value>The field value.</value>
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the field is user-defined.
        /// </summary>
        /// <value><c>true</c> if the field is user-defined; otherwise, <c>false</c>.</value>
        public bool UserDefined
        {
            get;
            set;
        }
    }
}