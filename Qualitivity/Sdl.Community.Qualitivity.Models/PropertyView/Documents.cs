using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Text;

namespace Sdl.Community.Structures.PropertyView
{
    public class DocumentConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Document) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) &&
                value is Document)
            {

                var document = (Document)value;

                var str = document.DocumentName;
                return str;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {

            return base.ConvertFrom(context, culture, value);
        }
    }

    public class DocumentCollectionEditor : CollectionEditor
    {
        public DocumentCollectionEditor(Type type)
            : base(type)
        {
        }

        protected override bool CanSelectMultipleInstances()
        {
            return false;
        }

        protected override Type CreateCollectionItemType()
        {
            return typeof(Document);
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    //[TypeConverterAttribute(typeof(DocumentConverter)),
    //DescriptionAttribute("Expand to see the Document")]     
     [ReadOnly(true)]
    public class Document
    {
        private string _documentName;
        private string _sourceLanguage;
        private string _targetLanguage;

         public Document()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }

         public Document(string documentName, string sourceLanguage, string targetLanguage)
        {
            _documentName = documentName;
            _sourceLanguage = sourceLanguage;
            _targetLanguage = targetLanguage;
        }


        [DisplayName("Name")]
        [Description("The document name")]
        [ReadOnly(true)]
        [DefaultValue("")]
        [NotifyParentProperty(true)]
         public string DocumentName
         {
             get
             {
                 return _documentName;
             }
             set
             {
                 _documentName = value;
             }
         }

        [DisplayName("Source")]
        [Description("The source language")]
        [ReadOnly(true)]
        [DefaultValue("")]
        [NotifyParentProperty(true)]
        public string SourceLanguage
        {
            get
            {
                return _sourceLanguage;
            }
            set
            {
                _sourceLanguage = value;
            }
        }

        [DisplayName("Target")]
        [Description("The target language")]
        [ReadOnly(true)]
        [DefaultValue("")]
        [NotifyParentProperty(true)]
        public string TargetLanguage
        {
            get
            {
                return _targetLanguage;
            }
            set
            {
                _targetLanguage = value;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(DocumentName);
            sb.Append("(");
            sb.Append(SourceLanguage);
            sb.Append(" - ");
            sb.Append(TargetLanguage);
            sb.Append(")");
            return sb.ToString();
        }
    }

    
}
