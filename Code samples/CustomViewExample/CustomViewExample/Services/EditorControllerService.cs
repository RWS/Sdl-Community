using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Desktop.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace CustomViewExample.Services
{
	/// <summary>
	/// Persists a single <see cref="EditorController"/> instance for the lifetime of the
	/// application and tracks the <see cref="IStudioDocument"/> instances raised through its
	/// events, as a workaround for GetDocuments() returning a list of null entries.
	/// The documents captured here are the actual instances Studio raised, so they can be
	/// passed back to <see cref="EditorController.Activate"/>.
	/// </summary>
	public sealed class EditorControllerService
	{
		private static readonly Lazy<EditorControllerService> LazyInstance =
			new Lazy<EditorControllerService>(() => new EditorControllerService());

		private readonly List<IStudioDocument> _documents = new List<IStudioDocument>();

		private readonly Dictionary<string, int> _eventCounts = new Dictionary<string, int>
		{
			{ "Opened", 0 },
			{ "Closed", 0 },
			{ "ActiveDocumentChanged", 0 },
			{ "ActivationChanged", 0 }
		};

		private EditorControllerService()
		{
			EditorController = SdlTradosStudio.Application.GetController<EditorController>();

			// Known issue: Opened is never raised by the EditorController; the subscription is
			// kept so the EventCounts diagnostic can demonstrate it. Document tracking relies
			// on ActiveDocumentChanged/ActivationChanged instead.
			EditorController.Opened += EditorController_Opened;
			EditorController.Closed += EditorController_Closed;
			EditorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
			EditorController.ActivationChanged += EditorController_ActivationChanged;

			// Seed with whatever is already open when the singleton is first touched,
			// filtering out the null entries reported from GetDocuments()
			var existingDocuments = EditorController.GetDocuments();
			if (existingDocuments != null)
			{
				foreach (var document in existingDocuments.Where(document => document != null))
				{
					TrackDocument(document);
				}
			}

			ActiveDocument = EditorController.ActiveDocument;
			TrackDocument(ActiveDocument);
		}

		public static EditorControllerService Instance => LazyInstance.Value;

		public EditorController EditorController { get; }

		/// <summary>The last document instance received from the controller events.</summary>
		public IStudioDocument ActiveDocument { get; private set; }

		/// <summary>The document instances captured from the Opened/ActiveDocumentChanged events.</summary>
		public IReadOnlyList<IStudioDocument> Documents => _documents;

		/// <summary>How many times each EditorController event has fired since the singleton was created.</summary>
		public IReadOnlyDictionary<string, int> EventCounts => _eventCounts;

		/// <summary>Raised after the tracked active document is updated from a controller event.</summary>
		public event EventHandler<DocumentEventArgs> ActiveDocumentChanged;

		/// <summary>Re-raised from EditorController.ActivationChanged, after ActiveDocument is refreshed.</summary>
		public event EventHandler<ActivationChangedEventArgs> EditorActivationChanged;

		public void Activate(IStudioDocument document)
		{
			if (document == null)
			{
				return;
			}

			System.Diagnostics.Trace.WriteLine("[CustomViewExample] Requesting activation of \""
				+ document.Files?.FirstOrDefault()?.Name + "\"");

			EditorController.Activate(document);
		}

		private void EditorController_Opened(object sender, DocumentEventArgs e)
		{
			CountEvent("Opened");

			TrackDocument(e.Document);
			ActiveDocument = e.Document ?? EditorController.ActiveDocument;
		}

		private void EditorController_Closed(object sender, DocumentEventArgs e)
		{
			CountEvent("Closed");

			if (e.Document != null)
			{
				_documents.Remove(e.Document);
			}

			if (ReferenceEquals(ActiveDocument, e.Document))
			{
				ActiveDocument = EditorController.ActiveDocument;
			}
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			CountEvent("ActiveDocumentChanged");

			TrackDocument(e.Document);
			ActiveDocument = e.Document ?? EditorController.ActiveDocument;

			ActiveDocumentChanged?.Invoke(this, e);
		}

		private void EditorController_ActivationChanged(object sender, ActivationChangedEventArgs e)
		{
			CountEvent("ActivationChanged");

			if (e.Active)
			{
				// The editor view became active; the controller's ActiveDocument is the
				// actual document instance that can be passed to Activate(document)
				var document = EditorController.ActiveDocument;
				TrackDocument(document);
				ActiveDocument = document;
			}

			EditorActivationChanged?.Invoke(this, e);
		}

		private void CountEvent(string eventName)
		{
			_eventCounts[eventName]++;
			System.Diagnostics.Trace.WriteLine("[CustomViewExample] EditorController." + eventName
				+ " fired (count: " + _eventCounts[eventName] + ")");
		}

		private void TrackDocument(IStudioDocument document)
		{
			if (document == null || _documents.Contains(document))
			{
				return;
			}

			_documents.Add(document);
			System.Diagnostics.Trace.WriteLine("[CustomViewExample] Tracking document \""
				+ document.Files?.FirstOrDefault()?.Name + "\" (total: " + _documents.Count + ")");
		}
	}
}
