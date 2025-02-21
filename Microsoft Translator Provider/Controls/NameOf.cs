using System;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;

namespace MicrosoftTranslatorProvider.Controls
{
	[ContentProperty(nameof(Member))]
	public class NameOf : MarkupExtension
	{
		public Type Type { get; set; }

		public string Member { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			if (Type == null || string.IsNullOrEmpty(Member) || Member.Contains("."))
			{
				throw new ArgumentException("x");
			}

			var propertyInfo = Type.GetRuntimeProperties().FirstOrDefault(x => x.Name == Member);
			var fieldInfo = Type.GetRuntimeFields().FirstOrDefault(x => x.Name == Member);
			if (propertyInfo == null && fieldInfo == null)
			{
				var errorMessage = string.Format(
					"x",
					Member,
					Type);
				throw new ArgumentException(errorMessage);
			}

			return Member;
		}
	}
}