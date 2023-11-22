using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using CustomViewExample.Model;
using CustomViewExample.Services;
using RwsAppStore.UsefulTipsService;
using RwsAppStore.UsefulTipsService.Model;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace CustomViewExample.Ribbon.Actions
{

	[Action(Id = "CustomViewExample_SelectSegmentAction_Id", Name = "CustomViewExample_SelectSegmentAction_Name", Description = "CustomViewExample_SelectSegmentAction_Description",
		Icon = "wordLight_yellow", ContextByType = typeof(EditorController))]
	[ActionLayout(typeof(RibbonGroups.EditorActionsGroup), 3, DisplayType.Large, "CustomViewExample_SelectSegmentAction_Name", true)]
	internal class SelectSegmentAction : AbstractAction
	{
		protected override void Execute()
		{
			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			if (editorController.ActiveDocument == null)
			{
				return;
			}
			var activeFile = editorController.ActiveDocument.ActiveFile;

			// return false and nothing happens (with any file with any segment id)
			var result = editorController.ActiveDocument.SetActiveSegmentPair(activeFile, "3", true);


			MessageBox.Show("SelectSegmentAction Result:" + result);
			//Debug.WriteLine("SelectSegmentAction Result:" + result);
		}
	}
}
