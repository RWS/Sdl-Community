using System;
using System.Diagnostics;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.FileType.Preview
{
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	[System.Runtime.InteropServices.ComVisibleAttribute(true)]
	public partial class InternalPreviewControl : UserControl
	{
		private string _activeSegId = string.Empty;
		private string _jumpParagraphId = string.Empty;
		private string _jumpSegmentId = string.Empty;
		private bool _segmentSelectedFromBrowser;
		private Timer _timer;

		public event PreviewControlHandler WindowSelectionChanged;

		[DebuggerHidden]
		public InternalPreviewControl()
		{
			InitializeComponent();
			
			//set the properties of the web-browser component
			webBrowserControl.AllowWebBrowserDrop = false;
			webBrowserControl.IsWebBrowserContextMenuEnabled = false;
			webBrowserControl.WebBrowserShortcutsEnabled = false;
			webBrowserControl.ScriptErrorsSuppressed = true;
			webBrowserControl.AllowNavigation = false;
			webBrowserControl.ObjectForScripting = this;
			webBrowserControl.DocumentCompleted += webBrowserControl_DocumentCompleted;
			
			_timer = new Timer();
			_timer.Tick += TimerTick;
			_timer.Interval = 500;
		}

		void webBrowserControl_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			ScrollToElement(_activeSegId);

			//set the CSS style for the currently selected segment
			if (webBrowserControl.Document != null)
				webBrowserControl.Document.InvokeScript("setActiveStyle", new object[] { _activeSegId });
		}

		/// <summary>
		/// construct a segment reference from _jumpparagraphId and _jumpsegmentId, 
		/// which is returned when user clicks the corresponding segment in the preview control
		/// </summary>
		/// <returns></returns>
		[DebuggerHidden]
		public SegmentReference GetSelectedSegment()
		{
			if (!string.IsNullOrEmpty(_jumpSegmentId))
			{
				var segRef = new SegmentReference(default, new ParagraphUnitId(_jumpParagraphId), new SegmentId(_jumpSegmentId));
				return segRef;
			}
			return null;
		}

		/// <summary>
		/// open file for preview
		/// </summary>
		/// <param name="fileName"></param>
		[DebuggerHidden]
		public void OpenTarget(string fileName)
		{
			if (InvokeRequired)
			{
				Invoke(new Action<string>(OpenTarget), fileName);
			}
			else
			{
				webBrowserControl.Navigate(fileName);
				webBrowserControl.Refresh();
			}
		}

		/// <summary>
		/// scroll to and highlight active segment in the preview control
		/// </summary>
		/// <param name="segment"></param>
		[DebuggerHidden]
		public void ScrollToSegment(SegmentReference segment)
		{
			if (InvokeRequired)
			{
				Invoke(new Action<SegmentReference>(ScrollToSegment), segment);
			}
			else
			{
				if (!_segmentSelectedFromBrowser)
				{
					ScrollToElement(segment.SegmentId.Id);

					// handle situations in which the document was opened 
					// and no active segment has been set yet.
					if (string.IsNullOrEmpty(_activeSegId))
					{
						_activeSegId = segment.SegmentId.Id;
						// select the CSS style for the currently selected segment
						webBrowserControl?.Document?.InvokeScript("setActiveStyle", new object[] { segment.SegmentId.Id });
					}
				}

				if (_activeSegId != segment.SegmentId.Id)
				{
					// reset the CSS style back from active to normal for the previously selected segment
					if (_activeSegId != null || _activeSegId == "")
					{
						webBrowserControl?.Document?.InvokeScript("setNormalStyle", new object[] { _activeSegId });
					}

					// set the CSS style for the currently selected segment
					webBrowserControl?.Document?.InvokeScript("setActiveStyle", new object[] { segment.SegmentId.Id });
				}

				// set the active segment id
				_activeSegId = segment.SegmentId.Id;

				if (_segmentSelectedFromBrowser)
				{
					_segmentSelectedFromBrowser = false;
				}
			}
		}

		[DebuggerHidden]
		public void Close()
		{
			// The Filter Framework takes care of cleaning up temporary files.
		}

		/// <summary>
		/// public method that is called from the preview control 
		/// when a segment has been selected
		/// </summary>
		[DebuggerHidden]
		public void SelectSegment(string paragraphUnitId, string segmentId)
		{
			// set global variables for jumping into clicked segment
			_jumpParagraphId = paragraphUnitId;
			_jumpSegmentId = segmentId;

			_segmentSelectedFromBrowser = true;
			FireWindowSelectionChanged();
		}

		[DebuggerHidden]
		protected void FireWindowSelectionChanged()
		{
			WindowSelectionChanged?.Invoke(null);
		}

		/// <summary>
		/// called when segment is confirmed and Trados Studio jumps into next segment
		/// </summary>
		[DebuggerHidden]
		public void JumpToActiveElement()
		{
			if (InvokeRequired)
			{
				Invoke(new MethodInvoker(JumpToActiveElement));
			}
			else
			{
				_timer.Start();
			}
		}


		[DebuggerHidden]
		private void ScrollAndJumpToActiveElement()
		{
			if (InvokeRequired)
			{
				Invoke(new MethodInvoker(ScrollAndJumpToActiveElement));
			}
			else
			{
				if (webBrowserControl.ReadyState != WebBrowserReadyState.Complete)
				{
					return;
				}

				_timer.Stop();

				ScrollToElement(_activeSegId);
				webBrowserControl?.Document?.InvokeScript("setActiveStyle", new object[] { _activeSegId });
			}
		}

		[DebuggerHidden]
		private void TimerTick(object sender, EventArgs e)
		{
			ScrollAndJumpToActiveElement();
		}

		/// <summary>
		/// scroll to the active segment inside the control
		/// </summary>
		/// <param name="elemName"></param>
		[DebuggerHidden]
		private void ScrollToElement(string elemName)
		{
			if (webBrowserControl.Document != null)
			{
				var doc = webBrowserControl.Document;
				var elem = doc.GetElementById(elemName);
				if (elem == null)
				{
					return;
				}

				elem.ScrollIntoView(true);
				if (webBrowserControl.Document.Body != null)
				{
					ScrollBy(-webBrowserControl.Document.Body.ClientRectangle.Bottom / 2, webBrowserControl);
				}
			}
		}

		[DebuggerHidden]
		private static void ScrollBy(int scrollBy, WebBrowser toScroll)
		{
			if (toScroll?.Document?.Body == null || toScroll.Document.Body.ScrollTop < 5)
			{
				return;
			}

			var newLocation = new Point(toScroll.Document.Body.ScrollLeft,
				toScroll.Document.Body.ScrollTop + scrollBy);

			toScroll.Document?.Window?.ScrollTo(newLocation);
		}
	}
}
