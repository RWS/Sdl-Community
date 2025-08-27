using System;
using System.Collections.Generic;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.Verification.Api;

namespace GlobalVerifierSample
{
	/// <summary>
	/// Required annotation for declaring the extension class.
	/// </summary>

	[GlobalVerifier("Identical Segments Verifier", "Plugin_Name", "Plugin_Description")]
	public class IdenticalVerifierMain : AbstractBilingualFileTypeComponent, IGlobalVerifier, IBilingualVerifier, ISharedObjectsAware
	{
		private ISharedObjects _sharedObjects;
		private IdenticalVerifierSettings _verificationSettings;

        public IdenticalVerifierMain()
        {
            
        }

		/// <summary>
		/// Initializes the settings bundle object from which to retrieve the setting(s)
		/// to be used in the verification logic, e.g. the context display code to
		/// which the verification should be applied.
		/// </summary>

		internal IdenticalVerifierSettings VerificationSettings
		{
			get
			{
				if (_verificationSettings == null && _sharedObjects != null)
				{
					ISettingsBundle bundle = _sharedObjects.GetSharedObject<ISettingsBundle>("SettingsBundle");
					if (bundle != null)
					{
						_verificationSettings = bundle.GetSettingsGroup<IdenticalVerifierSettings>();
					}
				}
				return _verificationSettings;
			}
		}

		public void SetSharedObjects(ISharedObjects sharedObjects)
		{
			_sharedObjects = sharedObjects;
		}

		/// <summary>
		/// The following members set some general properties of the verification plug-in,
		/// e.g. the plug-in name and the icon that are displayed in the user interface of SDL Trados Studio.
		/// </summary>

		public string Description
		{
			get { return PluginResources.Verifier_Description; }
		}

		public System.Drawing.Icon Icon
		{
			get { return PluginResources.Checked; }
		}

		public string Name
		{
			get { return PluginResources.Plugin_Name; }
		}

		public string HelpTopic
		{
			get { return String.Empty; }
		}

		public string SettingsId
		{
			get { return "Identical Verifier"; }
		}

		public Type SettingsType
		{
			get { return typeof(IdenticalVerifierSettings); }
		}

		public IList<string> GetSettingsPageExtensionIds()
		{
			IList<string> list = new List<string>();

			list.Add("Identical Settings Definition ID");

			return list;
		}

		public IDocumentItemFactory ItemFactory
		{
			get;
			set;
		}

		/// <summary>
		/// This member is used to output any verification messages in the user interface of SDL Trados Studio.
		/// </summary>

		public IBilingualContentMessageReporter MessageReporter
		{
			get;
			set;
		}

		public BilingualContentMessageReporterProxy MessageReporterProxy
		{
			get => MessageReporter != null ? new BilingualContentMessageReporterProxy(MessageReporter) : null; }

		public void Complete()
		{
			// Not required for this implementation.
		}

		public void FileComplete()
		{
			// Not required for this implementation.
		}

		public void Initialize(IDocumentProperties documentInfo)
		{
			// Not required for this implementation.
		}

		public void SetFileProperties(IFileProperties fileInfo)
		{
			// Not required for this implementation.
		}

		public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			// Apply the verification logic.
			CheckParagraphUnit(paragraphUnit);
		}

		/// <summary>
		/// The following member performs the actual verification. It traverses the segment pairs of the current document,
		/// and checks whether a particular segment has any context information (count > 0). It then determines whether
		/// the display code is identical to the display code entered in the plug-in settings.
		/// If this is the case, it determines whether the target segment is actually identical to the source segment.
		/// If not, a warning message will be generated, which is then displayed between the source and target segments,
		/// and in the Messages window of SDL Trados Studio.
		/// </summary>
		/// <param name="paragraphUnit"></param>

		private void CheckParagraphUnit(IParagraphUnit paragraphUnit)
		{
			// loop through the whole paragraph unit
			foreach (ISegmentPair segmentPair in paragraphUnit.SegmentPairs)
			{
				// Determine if context information is available,
				// and if the context equals the one specified in the user interface.
				if (paragraphUnit.Properties.Contexts.Contexts.Count > 0 &&
					paragraphUnit.Properties.Contexts.Contexts[1].DisplayCode == VerificationSettings.CheckContext.Value)
				{
					// Check whether target differs from source.
					// If this is the case, then output a warning message
					if (segmentPair.Source.ToString() != segmentPair.Target.ToString())
					{
						MessageReporterProxy.ReportMessage(this, PluginResources.Plugin_Name,
							ErrorLevel.Warning, PluginResources.Error_NotIdentical,
							new TextLocation(new Location(segmentPair.Target, true), 0),
							new TextLocation(new Location(segmentPair.Target, true), segmentPair.Target.ToString().Length - 1));
					}
				}
			}
		}
	}
}