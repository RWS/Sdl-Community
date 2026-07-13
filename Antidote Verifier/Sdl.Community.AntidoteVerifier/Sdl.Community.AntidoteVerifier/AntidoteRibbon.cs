using NLog;
using Sdl.Community.AntidoteVerifier.Connectix;
using Sdl.Community.AntidoteVerifier.Connectix.Protocol;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System.Windows.Forms;

namespace Sdl.Community.AntidoteVerifier
{
	[RibbonGroup("Sdl.Community.AntidoteVerifier", Name ="Antidote Verifier", ContextByType = typeof(EditorController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class AntidoteVerifierRibbon: AbstractRibbonGroup
	{

	}

	/// <summary>
	/// Shared launch path for the ribbon actions: guards against no open document and starts a Connectix
	/// session (WebSocket API) for the requested <see cref="AntidoteTool"/> id.
	/// </summary>
	internal static class AntidoteLauncher
	{
		public static void Launch(string tool)
		{
			var activeDocument = ApplicationContext.EditorController?.ActiveDocument;
			if (activeDocument == null)
			{
				MessageBox.Show(PluginResources.CorrectorAction_NoFileOpened, PluginResources.Error,
					MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			var editorService = new EditorService(activeDocument);
			ConnectixSession.Launch(editorService, tool);
		}
	}

	[Action("Sdl.Community.AntidoteVerifier.CorrectorAction",
		Name ="Corrector",
		Icon = "boutonCorrecteur",
		Description ="Run Antidote verification")]
	[ActionLayout(typeof(AntidoteVerifierRibbon),40, DisplayType.Large)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 3, DisplayType.Default)]

	public class AntidoteCorrectorAction: AbstractAction
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		protected override void Execute()
		{
			_logger.Info("Starting Antidote for correction!");
			AntidoteLauncher.Launch(AntidoteTool.Corrector);
		}
	}

	[Action("Sdl.Community.AntidoteVerifier.DictionaryAction", Name = "Dictionaries", Icon = "dictionary", Description = "Run Antidote dictionary")]
	[ActionLayout(typeof(AntidoteVerifierRibbon), 10, DisplayType.Normal)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation),
		1,
		DisplayType = DisplayType.Default,
		Name = "Dictionaries",
		IsSeparator = false)]
	public class AntidoteDictionaryAction : AbstractAction
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		protected override void Execute()
		{
			_logger.Info("Starting Antidote for dictionary!");
			AntidoteLauncher.Launch(AntidoteTool.Dictionaries);
		}
	}

	[Action("Sdl.Community.AntidoteVerifier.GuideAction", Name = "Guides", Icon = "guide", Description = "Run Antidote guide")]
	[ActionLayout(typeof(AntidoteVerifierRibbon), 10, DisplayType.Normal)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation),
		2,
		Name = "Guides",
		IsSeparator = false)]
	public class AntidoteGuideAction : AbstractAction
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		protected override void Execute()
		{
			_logger.Info("Starting Antidote for guide!");
			AntidoteLauncher.Launch(AntidoteTool.Guides);
		}
	}

}
