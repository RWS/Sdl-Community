using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sdl.Community.XLIFF.Manager.Controls
{
	public enum EllipsisPlacement
	{
		Left,
		Center,
		Right
	}

	/// <summary>
	/// This is a subclass of TextBox with the ability to show an ellipsis 
	/// when the Text doesn't fit in the visible area.
	/// </summary>
	public class TextBoxWithEllipsis : TextBox
	{		
		public TextBoxWithEllipsis()
		{
			// Initialize inherited stuff as desired.
			IsReadOnlyCaretVisible = true;

			// Initialize stuff added by this class
			IsEllipsisEnabled = true;
			UseLongTextForToolTip = true;
			FudgePix = 3.0;

			AllowDrop = true;
			PreviewDragOver += TextBoxWithEllipsis_DragOver;

			_placement = EllipsisPlacement.Right;
			_internalEnabled = true;

			LayoutUpdated += TextBoxWithEllipsis_LayoutUpdated;
			SizeChanged += TextBoxWithEllipsis_SizeChanged;
		}

		private void TextBoxWithEllipsis_DragOver(object sender, DragEventArgs e)
		{
			e.Effects = DragDropEffects.Copy | DragDropEffects.Move;			
			e.Handled = true;
		}

		public static readonly DependencyProperty LongTextProperty =
			DependencyProperty.Register("LongText", typeof(string), typeof(TextBoxWithEllipsis),
				new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					LongTextPropertyChangedCallback));

		private static void LongTextPropertyChangedCallback(DependencyObject dependencyObject, 
			DependencyPropertyChangedEventArgs de)
		{
			if (!(dependencyObject is TextBoxWithEllipsis control))
			{
				return;
			}
			
			control.LongText = de.NewValue?.ToString() ?? string.Empty;
			control.PrepareForLayout();
		}

		/// <summary>
		/// The underlying text that gets truncated with ellipsis if it doesn't fit.
		/// Setting this and setting Text has the same effect, but getting Text may
		/// get a truncated version of LongText.
		/// </summary>
		public string LongText
		{
			get => (string)GetValue(LongTextProperty);
			set => SetValue(LongTextProperty, value);
		}



		public EllipsisPlacement EllipsisPlacement
		{
			get => _placement;

			set
			{
				if (_placement != value)
				{
					_placement = value;

					if (DoEllipsis)
					{
						PrepareForLayout();
					}
				}
			}
		}

		/// <summary>
		/// If true, Text/LongText will be truncated with ellipsis
		/// to fit in the visible area of the TextBox
		/// (except when it has the focus).
		/// </summary>
		public bool IsEllipsisEnabled
		{
			get => _externalEnabled;

			set
			{
				_externalEnabled = value;
				PrepareForLayout();

				if (DoEllipsis)
				{
					// Since we didn't change Text or Size, layout wasn't performed 
					// as a side effect.  Pretend that it was.
					TextBoxWithEllipsis_LayoutUpdated(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// If true, ToolTip will be set to LongText whenever
		/// LongText doesn't fit in the visible area.  
		/// If false, ToolTip will be set to null unless
		/// the user sets it to something other than LongText.
		/// </summary>
		public bool UseLongTextForToolTip
		{
			get => _useLongTextForToolTip;

			set
			{
				if (_useLongTextForToolTip != value)
				{
					_useLongTextForToolTip = value;

					if (value)
					{
						// When turning it on, set ToolTip to
						// _longText if the current Text is too long.
						if (ExtentWidth > ViewportWidth || Text != LongText)
						{
							ToolTip = LongText;
						}
					}
					else
					{
						// When turning it off, set ToolTip to null
						// unless user has set it to something other
						// than _longText;
						if (LongText.Equals(ToolTip))
						{
							ToolTip = null;
						}
					}
				}
			}
		}

		public double FudgePix
		{
			get;
			set;
		}

		// Last length of substring of LongText known to fit.
		// Used while calculating the correct length to fit.
		private int _lastFitLen;

		// Last length of substring of LongText known to be too long.
		// Used while calculating the correct length to fit.
		private int _lastLongLen;

		// Length of substring of LongText currently assigned to the Text property.
		// Used while calculating the correct length to fit.
		private int _curLen;

		// Used to detect whether the OnTextChanged event occurs due to an
		// external change vs. an internal one.
		private bool _externalChange = true;

		// Used to disable ellipsis internally (primarily while
		// the control has the focus).
		private bool _internalEnabled = true;

		// Backer for LongText.
		//private string _longText = "";

		// Backer for IsEllipsisEnabled
		private bool _externalEnabled = true;

		// Backer for UseLongTextForToolTip.
		private bool _useLongTextForToolTip;

		// Backer for EllipsisPlacement
		private EllipsisPlacement _placement;

		// OnTextChanged is overridden so we can avoid 
		// raising the TextChanged event when we change 
		// the Text property internally while searching 
		// for the longest substring that fits.
		// If Text is changed externally, we copy the
		// new Text into LongText before we overwrite Text 
		// with the truncated version (if IsEllipsisEnabled).
		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			if (_externalChange)
			{
				LongText = Text ?? "";
				if (UseLongTextForToolTip) ToolTip = LongText;
				PrepareForLayout();
				base.OnTextChanged(e);
			}
		}

		// Makes the entire text available for editing, selecting, and scrolling
		// until focus is lost.
		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			_internalEnabled = false;
			SetText(LongText);
			base.OnGotKeyboardFocus(e);
		}

		// Returns to trimming and showing ellipsis.
		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			_internalEnabled = true;
			PrepareForLayout();
			base.OnLostKeyboardFocus(e);
		}

		// Sets the Text property without raising the TextChanged event.
		private void SetText(string text)
		{
			if (Text != text)
			{
				_externalChange = false;
				Text = text; // Will trigger Layout event.
				_externalChange = true;
			}

		}

		// Arranges for the next LayoutUpdated event to trim _longText and add ellipsis.
		// Also triggers layout by setting Text.
		private void PrepareForLayout()
		{
			_lastFitLen = 0;
			_lastLongLen = LongText.Length;
			_curLen = LongText.Length;

			// This raises the LayoutUpdated event, whose
			// handler does the ellipsis.
			SetText(LongText);
		}

		private void TextBoxWithEllipsis_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (DoEllipsis && e.NewSize.Width != e.PreviousSize.Width)
			{
				// We need to recalculate the longest substring of LongText that will fit (with ellipsis).
				// Prepare for the LayoutUpdated event, which does the recalc and is raised after this.
				PrepareForLayout();
			}
		}

		private bool DoEllipsis => IsEllipsisEnabled && _internalEnabled;

		// Called when Text or Size changes (and maybe at other times we don't care about).
		private void TextBoxWithEllipsis_LayoutUpdated(object sender, EventArgs e)
		{
			if (DoEllipsis)
			{
				// This does a binary search (bisection) to determine the maximum substring
				// of _longText that will fit in visible area.  Instead of a loop, it
				// uses a type of recursion that happens because this event is raised
				// again if we set the Text property in here.

				if (ViewportWidth + FudgePix < ExtentWidth)
				{
					// The current Text (whose length without ellipsis is _curLen) is too long.
					_lastLongLen = _curLen;
				}
				else
				{
					// The current Text is not too long.
					_lastFitLen = _curLen;
				}

				// Try a new substring whose length is halfway between the last length
				// known to fit and the last length known to be too long.
				int newLen = (_lastFitLen + _lastLongLen) / 2;

				if (_curLen == newLen)
				{
					// We're done! Usually, _lastLongLen is _lastFitLen + 1.

					if (UseLongTextForToolTip)
					{
						ToolTip = Text == LongText ? null : LongText;
					}
				}
				else
				{
					_curLen = newLen;

					// This sets the Text property without raising the TextChanged event.
					// However it does raise the LayoutUpdated event again, though
					// not recursively.
					CalcText();
				}
			}
			else if (UseLongTextForToolTip)
			{
				ToolTip = ViewportWidth < ExtentWidth ? LongText : null;
			}
		}

		// Sets Text to a substring of _longText based on _placement and _curLen.
		private void CalcText()
		{
			switch (_placement)
			{
				case EllipsisPlacement.Right:
					SetText(LongText.Substring(0, _curLen) + "\u2026");
					break;

				case EllipsisPlacement.Center:
					var firstLen = _curLen / 2;
					var secondLen = _curLen - firstLen;
					SetText(LongText.Substring(0, firstLen) + "\u2026" + LongText.Substring(LongText.Length - secondLen));
					break;

				case EllipsisPlacement.Left:
					var start = LongText.Length - _curLen;
					SetText("\u2026" + LongText.Substring(start));
					break;

				default:
					throw new Exception("Unexpected switch value: " + _placement);
			}
		}
	}
}
