using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using SDLCopyTags.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SDLCopyTags
{
    [Action("CopyTagAction", Icon = "CopyTags_appstore", Name = "Copy Tags to Target")]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 1, DisplayType.Large)]
    [Shortcut(Keys.Control | Keys.T)]
    public class CopyTagAction : AbstractViewControllerAction<EditorController>
    {
		public static readonly Log Log = Log.Instance;

		protected override void Execute()
        {
			try
			{
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
				Log.Logger.Error($"{"CopyTagAction Execute method: "} {ex.Message}\n {ex.StackTrace}");
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
				Log.Logger.Error($"{"GetTags method: "} {ex.Message}\n {ex.StackTrace}");
				return new List<IAbstractMarkupData>();
			}
		}
    }
}