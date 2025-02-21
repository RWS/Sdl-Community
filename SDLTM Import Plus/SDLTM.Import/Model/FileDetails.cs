using System.Drawing;
using System.Globalization;
using System.Xml;
using Sdl.Core.Globalization;
using SDLTM.Import.Helpers;

namespace SDLTM.Import.Model
{
    public class FileDetails: BaseModel
    {
		private bool _importStarted;
	    private bool _importCompleted;

		public bool ImportStarted
		{
			get => _importStarted;
			set
			{
				if (_importStarted == value)
				{
					return;
				}
				_importStarted = value;
				OnPropertyChanged(nameof(ImportStarted));
			}
		}

	    public bool ImportCompleted
	    {
		    get => _importCompleted;
		    set
		    {
			    if (_importCompleted == value) return;
			    _importCompleted = value;
			    OnPropertyChanged(nameof(ImportCompleted));
		    }
	    }

	    public string Name { get; set; }
	    public string Path { get; set; }
	    public CultureInfo SourceLanguage { get; set; }
	    public CultureInfo TargetLanguage { get; set; }
	    public Image SourceFlag { get; set; }
	    public Image TargetFlag { get; set; }	
	    public string Id { get; set; }
	    public bool IsXliff { get; set; }
	    public FileTypes FileType{ get; set; }

	    public void SetLanguagePairDetails()
	    {
		    if (FileType == FileTypes.Xliff)
		    {
				ReadLanguagesFromXliff();
			}
		    if (FileType == FileTypes.Tmx)
		    {
			    ReadLanguagesFromTmx();
		    }
			SetFlags();
	    }

	    private void ReadLanguagesFromTmx()
	    {
		    var xmlDoc = new XmlDocument();
		    xmlDoc.Load(Path);

		    var fileNodes = xmlDoc.DocumentElement?.ChildNodes;
		    if (fileNodes == null) return;
		    foreach (XmlNode fileNode in fileNodes)
		    {
			    if (fileNode.Name.Equals("header"))
				{
					if (fileNode.Attributes == null) continue;
					foreach (XmlAttribute attribute in fileNode.Attributes)
				    {
					    if (attribute.Name.Equals("srclang"))
					    {
							SourceLanguage = new CultureInfo(attribute.Value);
						}
					}
			    }
			    if (fileNode.Name.Equals("body"))
			    {
				    foreach (XmlNode tuNode in fileNode.ChildNodes)
				    {
					    foreach (XmlNode tuvNode in tuNode.ChildNodes)
					    {
						    if(tuvNode.Attributes==null)continue;
						    foreach (XmlAttribute languageAttribute in tuvNode.Attributes)
						    {
							    if (languageAttribute.Name.Equals("xml:lang"))
							    {
								    var languageCultureInfo = new CultureInfo(languageAttribute.Value);
								    if (!SourceLanguage.TwoLetterISOLanguageName.Equals(languageCultureInfo.TwoLetterISOLanguageName))
								    {
									    TargetLanguage = languageCultureInfo;
										break;
								    }
							    }
						    }
					    }
						break;
				    }
				}
			}
		}

	    private void ReadLanguagesFromXliff()
	    {
			var xmlDoc = new XmlDocument();
		    xmlDoc.Load(Path);

		    var fileNodes = xmlDoc.DocumentElement?.ChildNodes;
		    if (fileNodes == null) return;
		    foreach (XmlNode fileNode in fileNodes)
		    {
			    if (!fileNode.Name.Equals("file")) continue;
			    if (fileNode.Attributes == null) continue;
			    foreach (XmlAttribute attribute in fileNode.Attributes)
			    {
				    if (attribute.Name.Equals("source-language"))
				    {
					    SourceLanguage = new CultureInfo(attribute.Value);
				    }
				    if (attribute.Name.Equals("target-language"))
				    {
						TargetLanguage = new CultureInfo(attribute.Value);
						break;
					}
				}
		    }
	    }

	    private void SetFlags()
	    {
		    SourceFlag = new Language(SourceLanguage).GetFlagImage();
		    TargetFlag = new Language(TargetLanguage).GetFlagImage();
	    }
	}
}
