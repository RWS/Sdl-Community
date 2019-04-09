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
			treeView1.AfterSelect += TreeView1_AfterSelect;

			CreateReportTemplate(Path.Combine(_pathInfo.TemporaryStorageFullPath, "report.xslt"));
		}

		private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node?.Tag != null && e.Node?.Tag is EntryModel item)
			{
				SelectEntry(item);
			}
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

			if (treeView1 != null)
			{
				treeView1.AfterSelect -= TreeView1_AfterSelect;
			}
		}

		public IEnumerable<IEntry> GetEntries()
		{
			var entries = new List<IEntry>();
			foreach (TreeNode node in treeView1.Nodes)
			{
				entries.Add(node.Tag as EntryModel);

				if (node.Nodes.Count > 0)
				{
					foreach (TreeNode childNode in node.Nodes)
					{
						entries.Add(childNode.Tag as EntryModel);
					}
				}
			}

			return entries;
		}

		public IEntry GetSelectedEntry()
		{
			return (EntryModel)treeView1.SelectedNode?.Tag;
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

			foreach (TreeNode node in treeView1.Nodes)
			{
				var foundNode = FoundNode(node, entryModel);

				if (foundNode)
				{
					treeView1.SelectedNode = node;
				}
				else if (node.Nodes.Count > 0)
				{
					foreach (TreeNode childNode in node.Nodes)
					{
						foundNode = FoundNode(childNode, entryModel);
						if (foundNode)
						{
							treeView1.SelectedNode = childNode;
							break;
						}
					}
				}

				if (foundNode)
				{
					break;
				}
			}

			treeView1.SelectedNode?.EnsureVisible();
		}

		private static bool FoundNode(TreeNode node, EntryModel entryModel)
		{
			if (node.Tag is EntryModel item)
			{
				if (item.ItemId == entryModel.ItemId || item.Id == entryModel.Id)
				{
					return true;
				}
			}

			return false;
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
			if (e == null)
			{
				UpdateEntriesInViewInternal(new List<EntryModel>(), null, null);
			}
			else
			{
				UpdateEntriesInViewInternal(e.EntryModels ?? new List<EntryModel>(), e.SourceLanguage, null);
			}
		}

		private void UpdateEntriesInViewInternal(IEnumerable<EntryModel> entryModels, Language sourceLanguage, IEntry selectedEntry)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new Action<IEnumerable<EntryModel>, Language, IEntry>(UpdateEntriesInViewInternal), entryModels, sourceLanguage, selectedEntry);
				return;
			}

			var items = GetEntryModelItems(entryModels, sourceLanguage);

			treeView1.BeginUpdate();
			treeView1.Nodes.Clear();
			foreach (var item in items)
			{
				var node = treeView1.Nodes.Add(item.Value[0].Guid.ToString(), item.Value[0].Text);
				node.Tag = item.Value[0].Entry;
				if (item.Value.Count > 1)
				{
					for (var i = 1; i < item.Value.Count; i++)
					{
						var subNode = node.Nodes.Add(item.Value[i].Guid.ToString(), item.Value[i].Text);
						subNode.Tag = item.Value[i].Entry;
					}
				}
			}

			treeView1.Sort();
			if (treeView1.Nodes.Count > 0)
			{
				if (selectedEntry != null)
				{
					SelectEntryItem(selectedEntry);
				}
				else
				{
					SelectEntryItem(treeView1.Nodes[0].Tag as EntryModel);
				}
			}
			else
			{
				SelectEntry(null);
			}

			treeView1.EndUpdate();
		}

		private static Dictionary<string, List<EntryModelItem>> GetEntryModelItems(IEnumerable<EntryModel> entryModels, Language sourceLanguage)
		{
			var items = new Dictionary<string, List<EntryModelItem>>();
			var index = new List<string>();

			foreach (var entryModel in entryModels)
			{
				var sourceTerms = entryModel.Languages
					.Where(a => a.Locale.TwoLetterISOLanguageName == sourceLanguage.CultureInfo.TwoLetterISOLanguageName).ToList();

				foreach (var sourceTerm in sourceTerms)
				{
					foreach (var entryTerm in sourceTerm.Terms)
					{
						var indexItem = $"Source.{entryTerm.Value}.ItemId.{entryModel.ItemId}";

						if (!index.Contains(indexItem))
						{
							var item = new EntryModelItem
							{
								Guid = Guid.NewGuid(),
								Entry = entryModel,
								Text = entryTerm.Value
							};

							if (items.ContainsKey(item.Text))
							{
								items[item.Text].Add(item);
							}
							else
							{
								items.Add(item.Text, new List<EntryModelItem> { item });
							}

							index.Add(indexItem);
						}
					}
				}
			}

			return items;
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
			if (entry != null)
			{
				xmlTxtWriter.WriteAttributeString("ItemId", ((EntryModel)entry).ItemId);

				WriteFields(entry.Fields, xmlTxtWriter);

				WriteLanguages(entry, xmlTxtWriter);
			}

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
	}
}