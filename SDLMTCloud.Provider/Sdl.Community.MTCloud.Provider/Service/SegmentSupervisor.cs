using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service.Events;
using Sdl.Core.Globalization;
using Sdl.DesktopEditor.EditorApi;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class SegmentSupervisor : ISegmentSupervisor
	{
		private readonly EditorController _editorController;
		private Guid _docId;
		private ITranslationService _translationService;

		public SegmentSupervisor(EditorController editorController)
		{
			_editorController = editorController;

		}

		public event ConfirmationLevelChangedEventHandler SegmentConfirmed;

		private Document ActiveDocument => _editorController?.ActiveDocument;

		public Dictionary<SegmentId, TargetSegmentData> ActiveDocumentData
		{
			get
			{
				if (Data.ContainsKey(_docId)) return Data[_docId];
				SetIdAndActiveFile();

				return Data[_docId];
			}
		}

		public Dictionary<Guid, Dictionary<SegmentId, TargetSegmentData>> Data { get; set; } = new Dictionary<Guid, Dictionary<SegmentId, TargetSegmentData>>();

		public Feedback GetImprovement(SegmentId? segmentId = null)
		{
			var currentSegment = segmentId ?? ActiveDocument.ActiveSegmentPair?.Properties.Id;
			Feedback improvement = null;

			var segmentHasImprovement = currentSegment != null && ActiveDocumentData.ContainsKey(currentSegment.Value);
			if (segmentHasImprovement)
			{
				improvement = ActiveDocumentData[currentSegment.Value].Feedback;
			}
			return improvement;
		}

		public void StartSupervising(ITranslationService translationService)
		{
			_editorController.ActiveDocumentChanged -= EditorController_ActiveDocumentChanged;

			if (ActiveDocument != null)
			{
				ActiveDocument.ContentChanged -= ActiveDocument_ContentChanged;
				ActiveDocument.SegmentsConfirmationLevelChanged -= ActiveDocument_SegmentsConfirmationLevelChanged;
			}

			_translationService = translationService;
			if (_translationService != null)
			{
				_translationService.TranslationReceived -= TranslationService_TranslationReceived;
				_translationService.TranslationReceived += TranslationService_TranslationReceived;
			}

			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;

			if (ActiveDocument != null)
			{
				ActiveDocument.ContentChanged += ActiveDocument_ContentChanged;
				ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
			}
		}

		private void ActiveDocument_ContentChanged(object sender, DocumentContentEventArgs e)
		{
			AddToSegmentContextData();
		}

		private void ActiveDocument_SegmentsConfirmationLevelChanged(object sender, EventArgs e)
		{
			var segment = (ISegment)((ISegmentContainerNode)sender).Item;
			if (segment == null) return;

			var segmentId = segment.Properties.Id;
			var translationOrigin = segment.Properties.TranslationOrigin;
			var targetSegmentText = segment.ToString();

			if (IsImprovementToTpTranslation(translationOrigin, segmentId, segment))
			{
				AddImprovement(segmentId, targetSegmentText);
			}

			if (segment.Properties.ConfirmationLevel != ConfirmationLevel.Translated) return;
			SegmentConfirmed?.Invoke(segmentId);
		}

		public void AddImprovement(SegmentId segmentId, string improvement)
		{
			if (!ActiveDocumentData.ContainsKey(segmentId)) return;

			var item = ActiveDocumentData[segmentId].Feedback;
			if (item.Suggestion != improvement) item.Suggestion = improvement;
		}

		public void CreateFeedbackEntry(SegmentId segmentId, string originalTarget, string targetOrigin,
			string source)
		{
			if (targetOrigin != PluginResources.SDLMTCloudName) return;

			ActiveDocumentData.TryGetValue(segmentId, out var targetSegmentData);
			if (targetSegmentData == null)
			{
				ActiveDocumentData[segmentId] = new TargetSegmentData
				{
					Feedback = new Feedback(originalTarget, source),
				};
			}
			else
			{
				ActiveDocumentData[segmentId].Feedback = new Feedback(originalTarget, source);
			}
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			if (ActiveDocument == null) return;
			SetIdAndActiveFile();
			ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
			ActiveDocument.ContentChanged += ActiveDocument_ContentChanged;
		}

		private bool IsImprovementToTpTranslation(ITranslationOrigin translationOrigin, SegmentId segmentId, ISegment segment)
		{
			return translationOrigin?.OriginBeforeAdaptation?.OriginSystem == PluginResources.SDLMTCloudName &&
				   ActiveDocumentData.ContainsKey(segmentId) &&
				   ActiveDocumentData[segmentId].Feedback.OriginalMtCloudTranslation != segment.ToString() &&
				   segment.Properties?.ConfirmationLevel == ConfirmationLevel.Translated;
		}

		private void SetIdAndActiveFile()
		{
			_docId = ActiveDocument.ActiveFile.Id;
			if (!Data.ContainsKey(_docId))
			{
				Data[_docId] = new Dictionary<SegmentId, TargetSegmentData>();
			}
		}

		private void TranslationService_TranslationReceived(List<string> sources, TranslationData targetSegmentData)
		{
			if (ActiveDocument == null) return;
			for (var i = 0; i < sources.Count; i++)
			{
				var currentSegmentPair = ActiveDocument.SegmentPairs.FirstOrDefault(segPair => segPair.Source.ToString() == sources[i]);

				AddTargetSegmentMetaData(targetSegmentData.TranslationOriginInformation, currentSegmentPair);
				CreateFeedback(sources, targetSegmentData, i, currentSegmentPair);
			}
		}

		private void CreateFeedback(List<string> sources, TranslationData targetSegmentData, int i, ISegmentPair currentSegmentPair)
		{
			var currentSegmentId = currentSegmentPair?.Properties.Id;

			if (currentSegmentId != null)
			{
				CreateFeedbackEntry(currentSegmentId.Value, targetSegmentData.TargetSegments[i], PluginResources.SDLMTCloudName,
					sources[i]);
			}
			else
			{
				CreateFeedbackEntry(ActiveDocument.ActiveSegmentPair.Properties.Id, targetSegmentData.TargetSegments[i], PluginResources.SDLMTCloudName,
					ActiveDocument.ActiveSegmentPair.Source.ToString());
			}
		}

		private void AddTargetSegmentMetaData(TranslationOriginInformation translationOriginInformation, ISegmentPair currentSegmentPair)
		{
			currentSegmentPair = currentSegmentPair ?? ActiveDocument.ActiveSegmentPair;
			var segmentId = currentSegmentPair.Properties.Id;

			ActiveDocumentData.TryGetValue(segmentId, out var targetSegmentData);
			if (targetSegmentData == null)
			{
				ActiveDocumentData[segmentId] = new TargetSegmentData
				{
					TranslationOriginInformation = translationOriginInformation
				};
			}
			else
			{
				ActiveDocumentData[segmentId].TranslationOriginInformation = targetSegmentData.TranslationOriginInformation;
			}
		}

		private void AddToSegmentContextData()
		{
			var currentSegmentPair = ActiveDocument.ActiveSegmentPair;


			//var propFact = _editorController.ActiveDocument.PropertiesFactory;

			//var paragraphUnitProperties = currentSegmentPair.GetParagraphUnitProperties();
			//var contexts = paragraphUnitProperties.Contexts?.Contexts;

			//if (contexts == null)
			//{
			//	paragraphUnitProperties.Contexts = propFact.CreateContextProperties();
			//}

			//var contextInfo = contexts.FirstOrDefault(ci => ci.ContextType == "Translation Origin Information");
			//if (contextInfo == null)
			//{
			//	contextInfo = propFact.CreateContextInfo("Translation Origin Information");
			//	contexts.Add(contextInfo);
			//}

			ActiveDocumentData.TryGetValue(currentSegmentPair.Properties.Id, out var targetData);
			var qualityEstimation = targetData.TranslationOriginInformation.QualityEstimation;
			var model = targetData.TranslationOriginInformation.Model;

			currentSegmentPair.Properties.TranslationOrigin.SetMetaData("quality_estimation", qualityEstimation);
			currentSegmentPair.Properties.TranslationOrigin.SetMetaData("model", model);

			//contextInfo.DisplayColor = GetEstimationColorLabel(qualityEstimation);

			//contextInfo.DisplayName = $"{qualityEstimation}: Quality Estimation for SDL MT Cloud";
			//contextInfo.DisplayCode = "QE";
			//contextInfo.Description = RandomString();

			//ActiveDocument.UpdateParagraphUnitProperties(paragraphUnitProperties);
		}

		private Color GetEstimationColorLabel(string estimation)
		{
			switch (estimation)
			{
				case "Good":
					return Color.FromArgb(0, 128, 64);
				case "Adequate":
					return Color.FromArgb(0, 128, 255);
				case "Poor":
					return Color.FromArgb(255, 72, 72);
				default:
					return Color.FromArgb(183, 183, 219);
			}
		}

		private static Random random = new Random();
		public static string RandomString()
		{
			var strings = new List<string> { "Calitate exagerata", "Nu se poate mai bine, doar daca exageram", "Sub orice critica", "Cat de cat corect, daca esti de la tara", "Se pot aduce critici sub care sa nu fie" };
			return strings[random.Next(5)];
		}
	}
}