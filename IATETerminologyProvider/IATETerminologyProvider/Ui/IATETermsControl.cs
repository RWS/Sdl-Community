using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model;
using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;

namespace IATETerminologyProvider.Ui
{
	public partial class IATETermsControl : UserControl
	{
		private readonly IATETerminologyProvider _iateTerminologyProvider;
		private readonly PathInfo _pathInfo;
		private readonly object _lockObject = new object();

		public IATETermsControl()
		{
			InitializeComponent();
		}

		public IATETermsControl(IATETerminologyProvider iateTerminologyProvider) : this()
		{
			_iateTerminologyProvider = iateTerminologyProvider;
			_pathInfo = new PathInfo();

			ReleaseSubscribers();

			_iateTerminologyProvider.TermEntriesChanged += OnTermEntriesChanged;
			listBox1.SelectedIndexChanged += ListBoxSelectedIndexChanged;

			CreateReportTemplate(Path.Combine(_pathInfo.TemporaryStorageFullPath, "report.xslt"));
		}

		public void JumpToTerm(IEntry entry)
		{
			SelectEntryItem(entry);
		}

		public void ReleaseSubscribers()
		{
			if (_iateTerminologyProvider != null)
			{
				_iateTerminologyProvider.TermEntriesChanged -= OnTermEntriesChanged;
			}

			if (listBox1 != null)
			{
				listBox1.SelectedIndexChanged -= ListBoxSelectedIndexChanged;
			}
		}

		public IEnumerable<IEntry> GetEntries()
		{
			return listBox1.Items.Cast<EntryModelItem>().Select(item => item.Entry).Cast<IEntry>().ToList();
		}

		public IEntry GetSelectedEntry()
		{
			return ((EntryModelItem) listBox1.SelectedItem)?.Entry;
		}

		public void UpdateEntriesInView(IEnumerable<IEntry> entries, Language sourceLanguage, IEntry selectedEntry)
		{
			UpdateEntriesInViewInternal(entries.Cast<EntryModel>(), sourceLanguage, selectedEntry);
		}

		private void SelectEntryItem(IEntry entry)
		{
			if (!(entry is EntryModel entryModel))
			{
				return;
			}

			foreach (var item in listBox1.Items.Cast<EntryModelItem>())
			{
				if (item.Entry.ItemId == entryModel.ItemId || item.Entry.Id == entryModel.Id)
				{
					listBox1.SelectedItem = item;
					break;
				}
			}
		}

		private void SelectEntry(IEntry entry)
		{
			var reportXmlFulPath = Path.Combine(_pathInfo.TemporaryStorageFullPath, "report.xml");

			var xmlTxtWriter = new XmlTextWriter(reportXmlFulPath, Encoding.UTF8)
			{
				Formatting = Formatting.None,
				Indentation = 3,
				Namespaces = false
			};
			xmlTxtWriter.WriteStartDocument(true);

			xmlTxtWriter.WriteProcessingInstruction("xml-stylesheet", "type='text/xsl' href='" + "report.xslt" + "'");
			xmlTxtWriter.WriteComment("IATETerminologyProvider by SDL Community Developers, 2019");

			WriteReport(entry, xmlTxtWriter);

			xmlTxtWriter.WriteEndDocument();
			xmlTxtWriter.Flush();
			xmlTxtWriter.Close();

			webBrowser1.Url = new Uri(Path.Combine("file://", reportXmlFulPath));
			webBrowser1.Refresh();
		}

		private void OnTermEntriesChanged(object sender, EventArgs.TermEntriesChangedEventArgs e)
		{
			UpdateEntriesInViewInternal(e.EntryModels, e.SourceLanguage, null);
		}

		private void UpdateEntriesInViewInternal(IEnumerable<EntryModel> entryModels, Language sourceLanguage, IEntry selectedEntry)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new Action<IEnumerable<EntryModel>, Language, IEntry>(UpdateEntriesInViewInternal), entryModels, sourceLanguage, selectedEntry);
				return;
			}

			lock (_lockObject)
			{
				var items = new List<EntryModelItem>();
				var indexes = new List<string>();

				foreach (var entryModel in entryModels)
				{
					var sourceTerms = entryModel.Languages
						.Where(a => a.Locale.TwoLetterISOLanguageName == sourceLanguage.CultureInfo.TwoLetterISOLanguageName).ToList();
					foreach (var sourceTerm in sourceTerms)
					{
						foreach (var entryTerm in sourceTerm.Terms)
						{
							if (!indexes.Contains(entryTerm.Value))
							{
								items.Add(new EntryModelItem
								{
									Entry = entryModel,
									Text = entryTerm.Value
								});

								indexes.Add(entryTerm.Value);
							}
						}
					}
				}

				listBox1.BeginUpdate();
				listBox1.Items.Clear();
				listBox1.Items.AddRange(items.ToArray());

				if (listBox1.Items.Count > 0)
				{
					if (selectedEntry != null)
					{
						SelectEntryItem(selectedEntry);
					}
					else
					{
						listBox1.SelectedItem = listBox1.Items[0];
					}
				}

				listBox1.EndUpdate();
			}
		}

		private static void CreateReportTemplate(string fullFilePath)
		{
			if (File.Exists(fullFilePath))
			{
				File.Delete(fullFilePath);
			}

			var reportTemplate = "IATETerminologyProvider.Resources.report.xslt";

			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(reportTemplate))
			{
				if (stream != null)
				{
					using (var reader = new BinaryReader(stream))
					{
						using (Stream writer = File.Create(fullFilePath))
						{
							var buffer = new byte[2048];
							while (true)
							{
								var current = reader.Read(buffer, 0, buffer.Length);
								if (current == 0)
								{
									break;
								}

								writer.Write(buffer, 0, current);
							}
						}
					}
				}
			}
		}

		private static void WriteReport(IEntry entry, XmlWriter xmlTxtWriter)
		{
			xmlTxtWriter.WriteStartElement("Report");
			xmlTxtWriter.WriteAttributeString("xml:space", "preserve");
			xmlTxtWriter.WriteAttributeString("ItemId", ((EntryModel)entry).ItemId);

			WriteFields(entry.Fields, xmlTxtWriter);

			WriteLanguages(entry, xmlTxtWriter);

			xmlTxtWriter.WriteEndElement(); //report
		}

		private static void WriteLanguages(IEntry entry, XmlWriter xmlTxtWriter)
		{
			xmlTxtWriter.WriteStartElement("Languages");

			for (var i = 0; i < entry.Languages.Count; i++)
			{
				WriteLanguage(entry.Languages[i], i == 0, xmlTxtWriter);
			}

			xmlTxtWriter.WriteEndElement(); //Languages
		}

		private static void WriteLanguage(IEntryLanguage language, bool isSource, XmlWriter xmlTxtWriter)
		{
			var languageFlags = new LanguageFlags();
			var fullPath = languageFlags.GetImageStudioCodeByLanguageCode(language.Locale.Name);

			xmlTxtWriter.WriteStartElement("Language");
			xmlTxtWriter.WriteAttributeString("Name", language.Name);
			xmlTxtWriter.WriteAttributeString("CultureInfo", language.Locale.Name);
			xmlTxtWriter.WriteAttributeString("TwoLetterISOLanguageName", language.Locale.TwoLetterISOLanguageName);
			xmlTxtWriter.WriteAttributeString("IsSource", isSource.ToString());
			xmlTxtWriter.WriteAttributeString("FlagFullPath", File.Exists(fullPath) ? fullPath : string.Empty);

			WriteFields(language.Fields, xmlTxtWriter);

			WriteTerms(language, xmlTxtWriter);

			xmlTxtWriter.WriteEndElement(); //Language
		}

		private static void WriteTerms(IEntryLanguage language, XmlWriter xmlTxtWriter)
		{
			xmlTxtWriter.WriteStartElement("Terms");

			if (language.Terms.Count > 0)
			{
				var fields = language.Terms[0].Fields.Where(entryField =>
					entryField.Name == "Definition" || entryField.Name == "Domain" || entryField.Name == "Subdomain").ToList();

				WriteFields(fields, xmlTxtWriter);
			}

			foreach (var term in language.Terms)
			{
				WriteTerm(term, xmlTxtWriter);
			}

			xmlTxtWriter.WriteEndElement(); //Terms
		}

		private static void WriteTerm(IEntryTerm term, XmlWriter xmlTxtWriter)
		{
			xmlTxtWriter.WriteStartElement("Term");

			xmlTxtWriter.WriteAttributeString("Value", term.Value);

			var fields = term.Fields.Where(entryField =>
				entryField.Name == "Type" || entryField.Name == "Status").ToList();

			WriteFields(fields, xmlTxtWriter);

			xmlTxtWriter.WriteEndElement(); //Term
		}

		private static void WriteFields(IEnumerable<IEntryField> fields, XmlWriter xmlTxtWriter)
		{
			xmlTxtWriter.WriteStartElement("Fields");
			foreach (var field in fields)
			{
				xmlTxtWriter.WriteStartElement("Field");

				xmlTxtWriter.WriteAttributeString("Name", field.Name);
				xmlTxtWriter.WriteAttributeString("Value", field.Value);

				xmlTxtWriter.WriteEndElement(); //field
			}

			xmlTxtWriter.WriteEndElement(); //fields
		}

		private void ListBoxSelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (listBox1.SelectedItem != null && listBox1.SelectedItem is EntryModelItem item)
			{
				SelectEntry(item.Entry);
			}
		}
	}
}