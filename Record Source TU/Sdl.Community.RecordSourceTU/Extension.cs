using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoreLinq;
using Sdl.DesktopEditor.EditorApi;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.RecordSourceTU
{
	public static class Extension
	{
		public static Uri GetInnerProviderUri(this Uri addSourceTmProviderUri)
		{
			if (addSourceTmProviderUri.AbsoluteUri.StartsWith(RecordSourceTuTmTranslationProvider.ProviderUriScheme,
				StringComparison.InvariantCultureIgnoreCase))
			{
				return
					new Uri(
						addSourceTmProviderUri.AbsoluteUri.Substring(
							RecordSourceTuTmTranslationProvider.ProviderUriScheme.Length));
			}

			return addSourceTmProviderUri;
		}

		public static string GetFilePath(this TranslationUnit translationUnit)
		{
			string filePath = string.Empty;
			if (translationUnit.FileProperties != null)
			{
				filePath = translationUnit.FileProperties.FileConversionProperties.OriginalFilePath;
			}
			else
			{
				var editorController = SdlTradosStudio.Application.GetController<EditorController>();
				if (editorController == null) return filePath;
				if (editorController.ActiveDocument == null) return filePath;

				var activeFile = editorController.ActiveDocument.ActiveFile;

				filePath = activeFile.LocalFilePath;
			}

			return filePath;
		}

		public static string GetProjectName(this TranslationUnit translationUnit)
		{
			var pController = SdlTradosStudio.Application.GetController<ProjectsController>();
			return pController != null ? pController.CurrentProject.GetProjectInfo().Name : "";
		}

		public static ProjectFile TryGetActiveFile(this Document document)
		{
			if (document.ActiveFile != null) return document.ActiveFile;
			//at the time of implementation using ActiveFile property is not working always for the virtual merged files - for some reasons in the editor
			//there more file nodes for one file. This is a workaround to try and resolve it.
			var segmentPair = document.ActiveSegmentPair;
			var type = document.GetType();
			var method = type.GetMethod("GetTargetSegmentContainerNodeById",
				BindingFlags.NonPublic | BindingFlags.Instance);

			var result = method.Invoke(document, new object[] { segmentPair.Target.Properties.Id.Id });

			var targetSegmentContainer = result as ISegmentContainerNode;

			if (targetSegmentContainer == null) return null;

			IAbstractContainerNode node = targetSegmentContainer;
			while (node != null && !(node is IFileContainerNode))
			{
				node = node.Parent;
			}
			var fileContainerNode = node as IFileContainerNode;
			if (fileContainerNode == null) return null;

			var assembly = Assembly.LoadFrom("Sdl.TranslationStudio.Common.dll");
			var filterFramweworkUtilitiesType =
				assembly.GetType("Sdl.TranslationStudio.Common.Editing.FilterFrameworkUtilities");
			var internalDocument =
				type.GetProperty("InternalDocument", BindingFlags.NonPublic | BindingFlags.Instance)
					.GetValue(document, null);
			var nativeBillingualDocument =
				internalDocument.GetType()
					.GetProperty("NativeBilingualDocument")
					.GetValue(internalDocument, null);
			var target = nativeBillingualDocument.GetType()
				.GetProperty("Target")
				.GetValue(nativeBillingualDocument, null);
			var rootContainer = target.GetType()
				.GetProperty("RootContainer")
				.GetValue(target, null);
			var getFileContainerNodesMethod = filterFramweworkUtilitiesType.GetMethod("GetFileContainerNodes",
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
			var fileContainers = getFileContainerNodesMethod.Invoke(null, new[] { rootContainer, null });
			var fileContainerNodes = fileContainers as List<IFileContainerNode>;
			if (fileContainerNodes == null) return null;

			var distinctFileContainerNodes = fileContainerNodes.DistinctBy(x => x.FileProperties.FileConversionProperties.FileId).ToList();

			var index =
				distinctFileContainerNodes.FindIndex(
					f =>
						f.FileProperties.FileConversionProperties.FileId.Equals(
							fileContainerNode.FileProperties.FileConversionProperties.FileId));
			if (0 <= index && index < document.Files.Count())
			{
				return document.Files.ElementAt(index);
			}
			return null;
		}
	}
}