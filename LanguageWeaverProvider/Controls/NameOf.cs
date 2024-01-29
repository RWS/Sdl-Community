using System;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;

namespace LanguageWeaverProvider.Controls
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
				throw new ArgumentException(PluginResources.NameOf_CommandParameter_Syntax);
			}

			var propertyInfo = Type.GetRuntimeProperties().FirstOrDefault(x => x.Name == Member);
			var fieldInfo = Type.GetRuntimeFields().FirstOrDefault(x => x.Name == Member);
			if (propertyInfo == null && fieldInfo == null)
			{
				var errorMessage = string.Format(
					PluginResources.NameOf_CommandParameterI_MissingComponent,
					Member,
					Type);
				throw new ArgumentException(errorMessage);
			}

			return Member;
		}
	}
}