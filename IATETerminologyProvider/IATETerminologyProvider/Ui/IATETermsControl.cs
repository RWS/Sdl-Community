using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using IATETerminologyProvider.Helpers;
using Sdl.Terminology.TerminologyProvider.Core;

namespace IATETerminologyProvider.Ui
{
	public partial class IATETermsControl : UserControl
	{
		#region Private Fields
		private readonly IATETerminologyProvider _iateTerminologyProvider;
		#endregion

		#region Constructors
		public IATETermsControl()
		{
			InitializeComponent();
		}

		public IATETermsControl(IATETerminologyProvider iateTerminologyProvider) : this()
		{
			ShowFields();
			_iateTerminologyProvider = iateTerminologyProvider;
		}
		#endregion

		#region Public Methods
		public void JumpToTerm(IEntry entry)
		{
			var languageFlags = new LanguageFlags();
			var entrySourceLanguage = entry.Languages[0];
			var entryTargetLanguage = entry.Languages[1];

			if (entrySourceLanguage != null)
			{
				pictureBoxSource.Load(languageFlags.GetImageStudioCodeByLanguageCode(entrySourceLanguage.Locale.Name));
				lblSourceLanguageText.Text = entrySourceLanguage.Name;
				var sourceTerms = entrySourceLanguage.Terms;
				if (sourceTerms.Count > 0)
				{
					SetSourceFields(sourceTerms);
				}
			}

			//get the entry by Id from all the target terms result and map the following values from the target language
			if (entryTargetLanguage != null)
			{
				pictureBoxTarget.Load(languageFlags.GetImageStudioCodeByLanguageCode(entryTargetLanguage.Locale.Name));
				lblTargetLanguageText.Text = entryTargetLanguage.Name;
				var targetTerms = entryTargetLanguage.Terms;
				if (targetTerms.Count > 0)
				{
					SetTargetFields(targetTerms);
				}
				ShowFields();
			}
		}
		#endregion

		#region Private Methods
		private void SetTargetFields(IList<IEntryTerm> terms)
		{
			lblTargetTermText.Text = Utils.UppercaseFirstLetter(terms[0].Value);

			txtTargetDefinitionText.Text = terms[0].Fields.Where(f => f.Name.Equals("Definition")).FirstOrDefault() != null
				? Utils.UppercaseFirstLetter(terms[0].Fields.Where(f => f.Name.Equals("Definition")).FirstOrDefault().Value)
				: string.Empty;

			lblTargetDomainText.Text = terms[0].Fields.Where(f => f.Name.Equals("Domain")).FirstOrDefault() != null
				? Utils.UppercaseFirstLetter(terms[0].Fields.Where(f => f.Name.Equals("Domain")).FirstOrDefault().Value.ToLower())
				: string.Empty;

			lblTargetSubdomainText.Text = terms[0].Fields.Where(f => f.Name.Equals("Subdomain")).FirstOrDefault() != null
				? terms[0].Fields.Where(f => f.Name.Equals("Subdomain")).FirstOrDefault().Value
				: string.Empty;
			lblTargetSubdomainText.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lblTargetSubdomainText.Text.ToLower());

			lblTargetTermTypeText.Text = terms[0].Fields.Where(f => f.Name.Equals("TermType")).FirstOrDefault() != null
				? Utils.UppercaseFirstLetter(terms[0].Fields.Where(f => f.Name.Equals("TermType")).FirstOrDefault().Value)
				: string.Empty;
		}
		
		private void SetSourceFields(IList<IEntryTerm> terms)
		{
			lblSourceTermText.Text = Utils.UppercaseFirstLetter(terms[0].Value);

			txtSourceDefinitionText.Text = terms[0].Fields.Where(f => f.Name.Equals("Definition")).FirstOrDefault() != null
				? Utils.UppercaseFirstLetter(terms[0].Fields.Where(f => f.Name.Equals("Definition")).FirstOrDefault().Value)
				: string.Empty;

			lblSourceDomainText.Text = terms[0].Fields.Where(f => f.Name.Equals("Domain")).FirstOrDefault() != null
				? Utils.UppercaseFirstLetter(terms[0].Fields.Where(f => f.Name.Equals("Domain")).FirstOrDefault().Value.ToLower())
				: string.Empty;

			lblSourceSubdomainText.Text = terms[0].Fields.Where(f => f.Name.Equals("Subdomain")).FirstOrDefault() != null
				? terms[0].Fields.Where(f => f.Name.Equals("Subdomain")).FirstOrDefault().Value
				: string.Empty;
			lblSourceSubdomainText.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lblTargetSubdomainText.Text.ToLower());

			lblSourceTermTypeText.Text = terms[0].Fields.Where(f => f.Name.Equals("TermType")).FirstOrDefault() != null
				? Utils.UppercaseFirstLetter(terms[0].Fields.Where(f => f.Name.Equals("TermType")).FirstOrDefault().Value)
				: string.Empty;
		}

		private void ShowFields()
		{
			lblSourceLanguageText.Visible = !string.IsNullOrEmpty(lblSourceLanguageText.Text) ? true : false;
			lblSourceTerm.Visible = !string.IsNullOrEmpty(lblSourceTermText.Text) ? true : false;
			lblSourceDefinition.Visible = !string.IsNullOrEmpty(txtSourceDefinitionText.Text) ? true : false;
			txtSourceDefinitionText.Visible = !string.IsNullOrEmpty(txtSourceDefinitionText.Text) ? true : false;
			lblSourceDomain.Visible = !string.IsNullOrEmpty(lblSourceDomainText.Text) ? true : false;
			lblSourceSubdomain.Visible = !string.IsNullOrEmpty(lblSourceSubdomainText.Text) ? true : false;
			lblSourceTermType.Visible = !string.IsNullOrEmpty(lblSourceTermTypeText.Text) ? true : false;

			lblTargetLanguageText.Visible = !string.IsNullOrEmpty(lblTargetLanguageText.Text) ? true : false;
			lblTargetTerm.Visible = !string.IsNullOrEmpty(lblTargetTermText.Text) ? true : false;
			lblTargetDefinition.Visible = !string.IsNullOrEmpty(txtTargetDefinitionText.Text) ? true : false;
			txtTargetDefinitionText.Visible = !string.IsNullOrEmpty(txtTargetDefinitionText.Text) ? true : false;
			lblTargetDomain.Visible = !string.IsNullOrEmpty(lblTargetDomainText.Text) ? true : false;
			lblTargetSubDomain.Visible  = !string.IsNullOrEmpty(lblTargetSubdomainText.Text) ? true : false;
			lblTargetTermType.Visible = !string.IsNullOrEmpty(lblTargetTermTypeText.Text) ? true : false;
		}
		#endregion
	}
}