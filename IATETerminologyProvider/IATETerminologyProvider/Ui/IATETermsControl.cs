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

			//get the entry by Id from all the source terms result and map the following values from the target language
			if (entrySourceLanguage != null)
			{
				SetSourceFields(languageFlags, entrySourceLanguage);
			}

			//get the entry by Id from all the target terms result and map the following values from the target language
			if (entryTargetLanguage != null)
			{
				SetTargetFields(languageFlags, entryTargetLanguage);
			}
			ShowFields();
		}
		#endregion

		#region Private Methods

		/// <summary>
		/// Set target fields values in the Termbase Viewer
		/// </summary>
		/// <param name="languageFlags">all languages flags</param>
		/// <param name="entryTargetLanguage">target term entry language</param>
		private void SetTargetFields(LanguageFlags languageFlags, IEntryLanguage entryTargetLanguage)
		{
			pictureBoxTarget.Load(languageFlags.GetImageStudioCodeByLanguageCode(entryTargetLanguage.Locale.Name));
			lblTargetLanguageText.Text = entryTargetLanguage.Name;
			var targetTerms = entryTargetLanguage.Terms;
			if (targetTerms.Count > 0)
			{
				lblTargetTermText.Text = Utils.UppercaseFirstLetter(targetTerms[0].Value);

				txtTargetDefinitionText.Text = targetTerms[0].Fields.Where(f => f.Name.Equals("Definition")).FirstOrDefault() != null
					? Utils.UppercaseFirstLetter(targetTerms[0].Fields.Where(f => f.Name.Equals("Definition")).FirstOrDefault().Value)
					: "-";

				lblTargetDomainText.Text = targetTerms[0].Fields.Where(f => f.Name.Equals("Domain")).FirstOrDefault() != null
					? Utils.UppercaseFirstLetter(targetTerms[0].Fields.Where(f => f.Name.Equals("Domain")).FirstOrDefault().Value.ToLower())
					: "-";

				lblTargetSubdomainText.Text = targetTerms[0].Fields.Where(f => f.Name.Equals("Subdomain")).FirstOrDefault() != null
					? targetTerms[0].Fields.Where(f => f.Name.Equals("Subdomain")).FirstOrDefault().Value
					: "-";
				lblTargetSubdomainText.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lblTargetSubdomainText.Text.ToLower());

				lblTargetTermTypeText.Text = targetTerms[0].Fields.Where(f => f.Name.Equals("TermType")).FirstOrDefault() != null
					? Utils.UppercaseFirstLetter(targetTerms[0].Fields.Where(f => f.Name.Equals("TermType")).FirstOrDefault().Value)
					: "-";
			}
		}
		
		/// <summary>
		/// Set source fields values in the Termbase Viewer
		/// </summary>
		/// <param name="languageFlags">all languages flags</param>
		/// <param name="entrySourceLanguage">source term entry language</param>
		private void SetSourceFields(LanguageFlags languageFlags, IEntryLanguage entrySourceLanguage)
		{
			pictureBoxSource.Load(languageFlags.GetImageStudioCodeByLanguageCode(entrySourceLanguage.Locale.Name));
			lblSourceLanguageText.Text = entrySourceLanguage.Name;
			var sourceTerms = entrySourceLanguage.Terms;
			if (sourceTerms.Count > 0)
			{
				lblSourceTermText.Text = Utils.UppercaseFirstLetter(sourceTerms[0].Value);

				txtSourceDefinitionText.Text = sourceTerms[0].Fields.Where(f => f.Name.Equals("Definition")).FirstOrDefault() != null
					? Utils.UppercaseFirstLetter(sourceTerms[0].Fields.Where(f => f.Name.Equals("Definition")).FirstOrDefault().Value)
					: "-";

				lblSourceDomainText.Text = sourceTerms[0].Fields.Where(f => f.Name.Equals("Domain")).FirstOrDefault() != null
					? Utils.UppercaseFirstLetter(sourceTerms[0].Fields.Where(f => f.Name.Equals("Domain")).FirstOrDefault().Value.ToLower())
					: "-";

				lblSourceSubdomainText.Text = sourceTerms[0].Fields.Where(f => f.Name.Equals("Subdomain")).FirstOrDefault() != null
					? sourceTerms[0].Fields.Where(f => f.Name.Equals("Subdomain")).FirstOrDefault().Value
					: "-";
				lblSourceSubdomainText.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lblSourceSubdomainText.Text.ToLower());

				lblSourceTermTypeText.Text = sourceTerms[0].Fields.Where(f => f.Name.Equals("TermType")).FirstOrDefault() != null
					? Utils.UppercaseFirstLetter(sourceTerms[0].Fields.Where(f => f.Name.Equals("TermType")).FirstOrDefault().Value)
					: "-";
			}
		}

		/// <summary>
		/// Show/Hide the fields in the Termbase Viewer based on value existence.
		/// </summary>
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