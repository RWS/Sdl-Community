﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Sdl.Community.NumberVerifier;
using Sdl.Community.NumberVerifier.Helpers;
using Sdl.Community.NumberVerifier.MessageUI;
using Sdl.DesktopEditor.BasicControls;
using Sdl.DesktopEditor.EditorApi;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.Verification.Api;

namespace Sdl.Community.Extended.MessageUI
{
	public partial class NumberVerifierMessageUI : UserControl, ISuggestionProvider, ISegmentChangedAware
	{
		#region Create Edit Controls

		private readonly BasicSegmentEditControl _sourceSegmentControl = new BasicSegmentEditControl();
		private readonly BasicSegmentEditControl _targetSegmentControl = new BasicSegmentEditControl();

		#endregion

		private bool _hasSegmentChanged = false;

		private Suggestion _suggestion;

		#region Constructor
		public NumberVerifierMessageUI(MessageEventArgs messageEventArgs, IBilingualDocument bilingualDocument, ISegment sourceSegment, ISegment targetSegment)
		{
			MessageEventArgs = messageEventArgs;
			BilingualDocument = bilingualDocument;
			SourceSegment = sourceSegment;
			TargetSegment = targetSegment;
			InitializeComponent();

			_sourceSegmentControl.Dock = DockStyle.Fill;
			_sourceSegmentControl.IsReadOnly = false;
			_sourceSegmentControl.ReplaceDocumentSegment(sourceSegment.Clone() as ISegment);
			panel_Source.Controls.Add(_sourceSegmentControl);
			_sourceSegmentControl.ReplaceDocumentSegment(sourceSegment);

			_targetSegmentControl.Dock = DockStyle.Fill;
			_targetSegmentControl.IsReadOnly = false;
			_targetSegmentControl.ReplaceDocumentSegment(targetSegment.Clone() as ISegment);
			panel_Target.Controls.Add(_targetSegmentControl);
			_targetSegmentControl.ReplaceDocumentSegment((ISegment)targetSegment.Clone());

			_targetSegmentControl.SegmentContentChanged += OnSegmentContentChanged;

			//set up the target and source rich box which will be used to identify the issued text(s)
			var sourceText = new StringBuilder();
			var targetText = new StringBuilder();

			foreach (var item in targetSegment?.AllSubItems)
			{
				targetText.AppendFormat($"{item.ToString()} ");
			}			
			foreach (var item in sourceSegment?.AllSubItems)
			{
				sourceText.AppendFormat($"{item.ToString()} ");
			}
			target_richTextBox.Text = targetText.ToString();
			source_richTextBox.Text = sourceText.ToString();

			_hasSegmentChanged = false;

			UpdateMessage(messageEventArgs);
		}
		#endregion

		public MessageEventArgs MessageEventArgs
		{
			get;
			private set;
		}

		public IBilingualDocument BilingualDocument
		{
			get;
			private set;
		}

		public ISegment SourceSegment
		{
			get;
			private set;
		}

		/// <summary>
		/// TargetSegment property represents the target segment.
		/// </summary>
		public ISegment TargetSegment
		{
			get;
			private set;
		}


		#region ISegmentChangedAware implementation
		/// <summary>
		/// Returns true if object was manually edited
		/// </summary>
		public bool HasSegmentChanged
		{
			get { return _hasSegmentChanged; }
		}

		/// <summary>
		/// Returns edited segment
		/// </summary>
		public ISegment EditedSegment
		{
			get { return _targetSegmentControl.GetDocumentSegment(); }
		}

		/// <summary>
		/// The paragraph unit ID for edited segment in the original document.
		/// Note: The segment may not reference the original document so the paragraph ID may be null.
		/// </summary>
		public ParagraphUnitId? TargetParagraphId
		{
			get { return MessageEventArgs.FromLocation.ParagrahUnitId; }
		}

		/// <summary>
		/// The segment ID for the edited segment in the original document.
		/// </summary>
		public SegmentId? TargetSegmentId
		{
			get { return MessageEventArgs.FromLocation.SegmentId; }
		}

		/// <summary>
		/// Reset target segment content to original value 
		/// </summary>
		public void ResetSegment()
		{
			// don't listen for events when contents are reset
			_targetSegmentControl.SegmentContentChanged -= OnSegmentContentChanged;

			// show target segment in segment control
			_targetSegmentControl.ReplaceDocumentSegment((ISegment)TargetSegment.Clone());

			_hasSegmentChanged = false;

			// start listening to changes again 
			_targetSegmentControl.SegmentContentChanged += OnSegmentContentChanged;

		}

		public event EventHandler<EventArgs> SegmentChanged;

		#endregion


		#region Private members
		/// <summary>
		/// Updates the message from the given message event arguments.
		/// </summary>
		/// <param name="messageEventArgs">message event arguments</param>
		private void UpdateMessage(MessageEventArgs messageEventArgs)
		{

			if (messageEventArgs.ExtendedData is NumberVerifierMessageData messageData)
			{
				tb_ErrorDetails.Text = messageEventArgs.Level + Environment.NewLine + messageEventArgs.Message;
				if (messageData.MessageType.Equals(Constants.AlphanumericIssue) ||
				    messageData.MessageType.Equals(Constants.HindiIssue))
				{
					tb_SourceIssues.Text = messageData.InitialSourceIssues;
					tb_TargetIssues.Text = messageData.InitialTargetIssues;
				}
				else
				{
					tb_SourceIssues.Text = messageData.SourceIssues;
					tb_TargetIssues.Text = messageData.TargetIssues;
				}
				if (!string.IsNullOrEmpty(source_richTextBox.Text))
				{
					ColorTextIssues(tb_SourceIssues.Text, source_richTextBox);
				}
				if (!string.IsNullOrEmpty(target_richTextBox.Text))
				{
					ColorTextIssues(tb_TargetIssues.Text, target_richTextBox);
				}
			}


			if (messageEventArgs.ExtendedData is AlignmentErrorExtendedData alignmentData)
			{
				var sourceStartIndex = alignmentData.SourceRange.StartIndex;
				var sourceRangeLength = alignmentData.SourceRange.Length;
				var targetStartIndex = alignmentData.TargetRange.StartIndex;
				var targetRangeLength = alignmentData.TargetRange.Length;

				if (sourceRangeLength != 0)
				{
					source_richTextBox.Select(sourceStartIndex, sourceRangeLength);
					source_richTextBox.SelectionBackColor = Color.Gold;
				}
				
				if (targetRangeLength != 0)
				{
					target_richTextBox.Select(targetStartIndex, targetRangeLength);
					target_richTextBox.SelectionBackColor = Color.Gold;
				}

				tb_ErrorDetails.Text = messageEventArgs.Message;
				tb_SourceIssues.Text = alignmentData.SourceIssues;
				tb_TargetIssues.Text = alignmentData.TargetIssues;
				return;
			}


		}

		/// <summary>
		/// Handle content changed event
		/// </summary>
		/// <param name="sender">sender</param>
		/// <param name="args">Always null</param>
		private void OnSegmentContentChanged(object sender, EventArgs args)
		{
			_hasSegmentChanged = true;
			SegmentChanged?.Invoke(this, null);
		}

		/// <summary>
		/// Color the text(s) (from the Message Details window) which was found as an issue after the number verification process executed.
		/// </summary>
		/// <param name="textIssue">text issue</param>
		/// <param name="richTextBox">source/target rich box which contains the entire source and target segment text</param>
		private void ColorTextIssues(string textIssue, RichTextBox richTextBox)
		{
			var formatedTexts = new List<string>();
			//Check if textIssue is Hindi numbers
			var numberVerifierMain = new NumberVerifierMain();
			var easternArabicNumbers = numberVerifierMain.GetEasternArabicNumbers();
			var textIssueChars = textIssue.ToCharArray();
			if(easternArabicNumbers.Any(h=>textIssueChars.Any(t=>t.ToString().Equals(h.Value))))
			{
				formatedTexts = Regex.Replace(textIssue, @"\r\n ? |\n ? |\r", " ")?.Split(' ')?.ToList();
			}

			// remove the special chars from the text in order to be identified correctly	
			if (!formatedTexts.Any())
			{
				formatedTexts = Regex.Replace(textIssue, @"[^0-9a-zA-Z\._]", " ")?.Split(' ')?.ToList();
			}

			foreach (var formatedText in formatedTexts)
			{
				if (!string.IsNullOrEmpty(formatedText))
				{
					// if the text with issue matched with the text from the source/target segment, then color the text
					var matchIssue = Regex.Match(richTextBox.Text, formatedText);
					if (matchIssue.Success)
					{
						int endIndex = formatedText.Length;
						richTextBox.Select(matchIssue.Index, endIndex);
						richTextBox.SelectionBackColor = Color.Gold;
					}
				}
			}			
		}
		#endregion
		#region ISuggestionProvider
		public Suggestion GetSuggestion()
		{
			return _suggestion;
		}

		public bool HasSuggestion()
		{
			return false;
		}

		public event EventHandler SuggestionChanged;
		#endregion
	}
}