using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Collections.Generic;
using NLog;
using SDLCopyTags.Helpers;

namespace SDLCopyTags
{
    [Action("CopyTagAction", Icon = "CopyTags_appstore", Name = "Copy Tags to Target")]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 1, DisplayType.Large)]
    public class CopyTagAction : AbstractViewControllerAction<EditorController>
    {
	    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

	    protected override void Execute()
        {
	        Log.Setup();
			try
			{
				_logger.Info("Log initialized");
				var currentSegment = Controller?.ActiveDocument?.GetActiveSegmentPair();

				if (currentSegment != null && currentSegment.Target.Count == 0)
				{
					var source = currentSegment.Source;
					var tags = GetTags(source);

					foreach (var tag in tags)
					{
						currentSegment.Target.Add((IAbstractMarkupData)tag.Clone());
					}
				}
				Controller?.ActiveDocument?.UpdateSegmentPair(currentSegment);
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
			}
		}

        private IList<IAbstractMarkupData> GetTags(ISegment source)
        {
			try
			{
				var tagVisitor = new TagVisitor();
				tagVisitor.VisitSegment((ISegment)source.Clone());

				return tagVisitor?.Tags;
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
				return new List<IAbstractMarkupData>();
			}
		}
    }
}