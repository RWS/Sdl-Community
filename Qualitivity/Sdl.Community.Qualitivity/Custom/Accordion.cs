using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Custom {

// Version 2015.04.05
// -Removed unused line of code: bool hit = bounds.Contains(pt);
// -Fixed bug when the mouse was clicked on a drag area which
//  was on top of a scrollbar. The scrollbar consumed the mouse
//  events and needed to have the capture released.
// -Fixed bug. Mouse would turn into the grab cursor when it
//  was over a grab bar that belonged to an Accordion in a
//  background window, or when the mouse was on top of a
//  child window that was covering the accordion.
//
// Version 2015.01.16
// -Rewrote the mouse drag behavior using IMessageFilter. In
//  addition to cleaner code, this also fixed the mouse cursor
//  staying as the SizeNS cursor bug.
// -Added GrabWidth property that specifies when the mouse turns
//  into the GrabCursor cursor.
// -Added GrabCursor property, default value of Cursors.SizeNS.
// -Added <summary> comments to the properties and input parameters.
// -Fixed layout issue that happens when minimizing and restoring
//  a window. Some of the controls would appear invisible because
//  as the form is minimized, its height changes to zero.
//  For more info, see the Accordion.OnSizeChanged method and the
//  AccordionLayoutProblemForm at the bottom.
// -Fixed the bug when the accordion font changes, the tool box's
//  sub-menu fonts weren't updated.
//  Before the font size would stay the same for the sub-menus.
// -Fixed fill all from being greyed out. It was using the
//  wrong size.
// -Overrode the Accordion's GetPreferredSize method to return
//  the FLP host's preferred size.
// -Added method CheckBoxForControl(Control c) which returns the
//  header checkbox for a given nested control, or null if the
//  control is not found.
//
// v2014.11.12
// -Initial Release

///<summary>Use the Add(Control, "Title") method to add controls to the accordion.
///
///<para>Brief explation of 'Extra Height': Extra height is calculated to be the difference
///of the accordion's height and the of the sum of all heights of the accordion's contents:
///header checkboxes, margins, paddings, and preferred size heights of added controls.
///If the difference is less than zero then there is no extra height. E.g. Math.Max(0, ...).</para>
///
///<para>There are two options how Extra Height can be allocated.</para>
///<para>1) Allocate all the extra height to the last opened checkbox with a fill weight > 0, or</para>
///<para>2) Allocate the extra height between the open controls based on their fill weights.</para>
///
///<para>The #2 option is likely better for most situations, and is the default value.
///The boolean property 'FillLastOpened' determines which mode is used.</para>
///
///<para>Another feature of this Accordion is that it allows a control to be expanded by
///clicking and dragging the mouse. This is especially useful when using multiline
///textboxes. Instead of having to allocate a large number of preset visible lines,
///the textboxes can be added to an accordion and the user can increase the height
///of any textbox as needed by clicking and dragging in the grab area.</para>
///</summary>
public class Accordion : UserControl, IMessageFilter {

	///<summary>The glyph to prefix to a closed checkbox's text if the DownArrow member is null.
	///The default value is the unicode down arrow followed by two spaces.</summary>
	public static string GlobalDownArrow = "\u25bc  ";

	///<summary>The glyph to prefix to an open checkbox's text if the UpArrow member is null.
	///The default value is the unicode up arrow followed by two spaces.</summary>
	public static string GlobalUpArrow = "\u25b2  ";

	///<summary>The factory used to create a header CheckBox when a control is added to an Accordion.
	///A default factory is provided.</summary>
	public static ICheckBoxFactory GlobalCheckBoxFactory = new DefaultCheckBoxFactory();

	
	///---
	private static readonly int dummyHeight = 0; // testing only
	private FLP host = new FLP(); // Note: tried using TableLayoutPanel before, but it incorrectly displays its scrollbars
	private Control2 lastChecked; // used to track the last opened control so it can be closed if OpenOneOnly is true
	private ToolTip tips = new ToolTip();
	private ToolBox toolBox = new ToolBox();
	// used when OpenOneOnly is true. The previously open one is automatically closed which fires a checkedchanged event.
	// isAdjusting is used to avoid responding to that event.
	private bool isAdjusting;
	//---

	///<summary>Specifies the CheckBoxFactory for this specific instance. The default value is null, which means the
	///GlobalCheckBoxFactory will be used.</summary>
	public ICheckBoxFactory CheckBoxFactory { get; set; }

	///<summary>Allows at most one control to be expanded at a time. The previously opened control is automatically closed.
	///The default value is false.</summary>
	public bool OpenOneOnly { get; set; }

	///<summary>If a user expands a control by dragging it, then when the checkbox is closed the extra height will reset to zero.
	///FillResetOnCollapse is usually true when FillLastOpened is true. The default value is false.</summary>
	public bool FillResetOnCollapse { get; set; }

	///<summary>Specifies that extra height is strictly increasing and is not decreased (except by the user). Extra height is
	///accumulated by controls that have a fill weight > 0 and when the parent control increases in height.
	///The default value is false.</summary>
	public bool FillModeGrowOnly { get; set; }

	///<summary>The last eligible control that was opened receives all the extra height. The extra height that was previously
	///allocated to the previous control is removed. If FillLastOpened is true, then usually FillResetOnCollapse is also true.
	///The default value is false.</summary>
	public bool FillLastOpened { get; set; }

	///<summary>The default amount of space to set around a control (when it is the open state).
	///The total amount of space around a control is going to be the sum of ContentPadding + ContentMargin. The difference is that
	///if ContentBackColor has a value, then the ContentPadding area will fill with that color, where as the ContentMargin will appear
	///empty. The default is 5 pixels on each side.</summary>
	///<seealso cref="ContentBackColor"/>
	public Padding? ContentPadding { get; set; }

	///<summary>Similar to ContentPadding, it specifies a default amount of space to add around a control. The default value is null.
	///See Content Padding for more information.</summary>
	public Padding? ContentMargin { get; set; }

	///<summary>The default amount of empty space to add around a checkbox. The default value is null, which means no extra space
	///is added.</summary>
	public Padding? CheckBoxMargin { get; set; }

	///<summary>An option to automatically set the BackColor property of an added control. The default value is null.</summary>
	public Color? ControlBackColor { get; set; }

	///<summary>An option to automatically set the BackColor property of the content that is hosting an added control.
	///The ContentBackColor, together with ContentPadding, can be used to create a colored border around the control.
	///The default value is null.</summary>
	public Color? ContentBackColor { get; set; }

	///<summary>Specifies that the Accordion should expand to fill the width of its parent container if extra space is available.
	///The default value is true.</summary>
	public bool FillWidth { get; set; }

	///<summary>Specifies that the Accordion should expand to fill the height of its parent container if extra space is available.
	///The default value is true.</summary>
	public bool FillHeight { get; set; }

	///<summary>GrowAndShink is used only when FillWidth is false. When a control is opened, it might required more width and
	///a horizontal scrollbar might be displayed. If GrowAndShrink is true, then when the control is closed, the display area
	///will return to the previous size and the scrollbars will vanish. Otherwise, the display area will remain the same, and
	///the horizontal scrollbar will stay. The default value is true.</summary>
	public bool GrowAndShrink { get; set; }

	///<summary>Gets or sets the text to prefix to the checkbox text when the content is hidden. The default value is null,
	///which means the GlobalDownArrow value is used.</summary>
	public string DownArrow { get; set; }

	///<summary>Gets or sets the text to prefix to the checkbox text when the content is visible. The default value is null,
	///which means the GlobalUpArrow value is used.</summary>
	public string UpArrow { get; set; }

	///<summary>Option to display a tool box when either the mouse is hovered over the rightmost side of an open checkbox header
	///or a right click on the header is made. The tool box displayed gives the user the ability to control the height of the
	///control(s). The default value is true.</summary>
	public bool ShowToolMenu { get; set; }

	///<summary>Option to only display a tool box if a control has a fill weight greater than zero. The default value is false.</summary>
	public bool ShowToolMenuRequiresPositiveFillWeight { get; set; }

	///<summary>Option to display the tool box when the mouse is hovered over a closed checkbox header. The default value is false.</summary>
	public bool ShowToolMenuOnHoverWhenClosed { get; set; }

	///<summary>Option to display the tool box when the user right clicks on a checkbox header. The ShowToolMenu option must be true
	///for this to work. The default value is true.</summary>
	public bool ShowToolMenuOnRightClick { get; set; }

	///<summary>Specifies if the controls are in the open state when added. The default value is false.</summary>
	public bool OpenOnAdd { get; set; }

	///<summary>Insets put padding between the checkboxes and the edge of the accordion control. Normally this would be
	///considered the Padding. However, do not setting the Accordion's Padding because the scrollbars do not account
	///for the padding correctly. If you are using an ErrorProvider, and setting an error message on a header checkbox,
	///then the Insets should be 15 on the side where the error icon is shown. The default value is zero for all sides.</summary>
	public Padding Insets {	get { return host.Padding; } set { host.Padding = value; }}

	///<summary>Option to allow the user to resize the content height of an open header by dragging the bottom of the content,
	///similar to a SplitContainer. Specify the splitter width using the GrabWidth property.</summary>
	public bool AllowMouseResize { get; set; }

	///<summary>The width of the strip of pixels at the bottom of a control where a resize action can be activated.
	///The default value is 6 pixels.</summary>
	public int GrabWidth { get; set; }

	///<summary>The cursor to display when a control's height can be resized by the user. The default value is Cursors.SizeNS.</summary>
	public Cursor GrabCursor { get; set; }

	///<summary>Specifies that the cursor should not activate unless a control's fill weight is greater than zero.
	///The default value is true.</summary>
	public bool GrabRequiresPositiveFillWeight { get; set; }

	///<summary>
	///<para>Use the Add(Control, "Title") method to add controls to the accordion.</para>
	///<para>Use the Count property &amp; Control(i), CheckBox(i) methods to access the controls.</para>
	///<para>Use the CheckBoxForControl(...) method to find the header checkbox for a particular control.</para>
	///<para>Use the Open &amp; Close methods to expand or collapse controls programmatically.</para>
	///<para>Use the UpArrow &amp; DownArrow properties to change the glyphs displayed.</para>
	///<para>Use the Insets property to set amount of empty space around the Accordion.</para>
	///<para>Use the ContentPadding &amp; ContentMargin properties to set the amount of empty space around a specific control.</para>
	///</summary>
	public Accordion() {
		AutoSize = true;
		AutoSizeMode = AutoSizeMode.GrowAndShrink;
		AutoScroll = true;
		Dock = DockStyle.Fill;
		//this.DoubleBuffered = true;
		FillHeight = true;
		FillWidth = true;
		GrowAndShrink = true;
		ShowToolMenu = true;
		ContentPadding = new Padding(5); // whitespace between edges
		AllowMouseResize = true;
		ShowToolMenuOnHoverWhenClosed = false;
		ShowToolMenuOnRightClick = true;
		GrabWidth = 6;
		GrabCursor = Cursors.SizeNS;
		GrabRequiresPositiveFillWeight = true;
		Controls.Add(host);

		Application.AddMessageFilter(this);
	}

	// variables used for resizing:
	const int WM_MOUSEMOVE = 0x0200;
	const int WM_LBUTTONDOWN = 0x0201;
	const int WM_LBUTTONUP = 0x0202;
	private Control2 grabControl;
	private Point grabPoint = Point.Empty;
	private Point oldPoint = Point.Empty;
	private int originalDH;
	private bool isDragging;
	private bool origLocked;
	private bool resetLocked = true;

	///<summary>Handles the mouse resize events for increasing or decreasing the height of controls.</summary>
	public bool PreFilterMessage(ref Message m) {
		if (!AllowMouseResize)
			return false;

		if (m.Msg == WM_MOUSEMOVE) {
			// LParam is relative to the control the mouse is above
			//Point pt = new Point(m.LParam.ToInt32());
			// instead we need the absolute location:
			var pt = Cursor.Position;

			if (isDragging) {
				if (oldPoint == pt) {
					// same points are fired when dragging, so just return
					return false;
				}
				var newdh = Math.Max(0, originalDH + (pt.Y - grabPoint.Y));
				grabControl.dh = newdh;
				resetLocked = false;
				host.UpdateDeltaHeights();
				host.PerformLayout();
				oldPoint = pt;
				return false;
			}

			// determine if the cursor is in the bounds of the Accoridion
			var bounds = new Rectangle(PointToScreen(Point.Empty), Size);

			if (!bounds.Contains(pt))
				return false;

			var c = FindControl2(pt);

			if (c != null) {
				Cursor = GrabCursor;
			}
			else {
				// only reset cursor if it needs it, otherwise if the
				// mouse is over a textbox (for example), the cursor
				// will be the pointer and not the IBeam.
				if (Cursor == GrabCursor)
					Cursor = DefaultCursor;
			}
		}
		else if (m.Msg == WM_LBUTTONDOWN) {
			var pt = Cursor.Position;
			var c = FindControl2(pt);

			if (c != null) {
				var c3 = ControlAtPoint(pt);
				if (c3 is ScrollBar) {
					// if a grabbable area overlaps a scroll bar, then a
					// mouse down on a ScrollBar makes it consume the mouse events
					// so we need to release it. Are there other controls that also
					// need to be released?
					c3.BeginInvoke((Action) delegate {
						ReleaseCapture();
					});
				}

				grabControl = c; 
				origLocked = c.isLocked;
				c.isLocked = true;
				isDragging = true;
				resetLocked = true;
				originalDH = c.dh;
				grabPoint = pt;
				// don't return true here, otherwise if the mouse
				// is dragged off the window then no more mouse move
				// events are received.
				//return true;
			}
			else {
				isDragging = false;
			}
		}
		else if (m.Msg == WM_LBUTTONUP) {
			if (grabControl != null && resetLocked) {
				grabControl.isLocked = origLocked;
			}
			grabControl = null;
			isDragging = false;
		}
		return false;
	}

	// get the control directly under the point
	// traverse up and check if any parent control is this control
	private bool IsMouseOverThisControl(Point pt) {
		var c = ControlAtPoint(pt);
		while (c != null) {
			if (c == this)
				return true;
			c = c.Parent;
		}
		return false;
	}

	[DllImport("user32.dll")]
	public static extern bool ReleaseCapture();

	[DllImport("user32.dll")]
	extern static IntPtr GetForegroundWindow();

	[DllImport("user32.dll")]
	public static extern IntPtr GetParent(IntPtr hWnd);

	[DllImport("user32.dll")]
	private static extern IntPtr WindowFromPoint(Point pt);

	private static IntPtr GetTopMostHandle(IntPtr hWnd) {
		while (true) {
			var parent = GetParent(hWnd);
			if (parent == IntPtr.Zero)
				break;
			hWnd = parent;
		}
		return hWnd;
	}

	private static Control ControlAtPoint(Point pt) {
		var hWnd = WindowFromPoint(pt);
		if (hWnd != IntPtr.Zero)
			return FromHandle(hWnd);
		return null;
	}

	// pt needs to be in screen coordinates
	private Control2 FindControl2(Point pt) {
		// the mouse should only turn into a grab cursor if the accordion is in the window that
		// is currently the foreground window, AND the mouse is directly over the accordion (or
		// one of the accordion's child controls)
		var th = GetTopMostHandle(Handle); // works for both WPF (e.g. WindowsFormsHost) and Winforms
		var fg = GetForegroundWindow();
		if (fg != th || !IsMouseOverThisControl(pt))
			return null;

		// determine if the cursor is in the bounds of the Accoridion
		var bounds = new Rectangle(PointToScreen(Point.Empty), Size);
		var gw = GrabWidth;
		Control2 c = null;

		foreach (var c2 in host.Control2s) {
			var b2 = new Rectangle(c2.PointToScreen(Point.Empty), c2.Size);

			//c2.BackColor = Color.Transparent; // testing only
			//if (b2.Contains(pt))
			//	c2.BackColor = Color.Yellow;

			var y = b2.Y + b2.Height;
			var dy = y - pt.Y;
			// only grab when in the range and the control is open
			var canGrab = !GrabRequiresPositiveFillWeight || GrabRequiresPositiveFillWeight && c2.fillWt > 0;

			if (dy <= gw && dy > 0 && c2.cb.Checked && canGrab) {
				c = c2;
				break;
			}
		}
		return c;
	}

	public override Size GetPreferredSize(Size proposedSize) {
		var s = host.GetPreferredSize(proposedSize);
		return s;
	}

	protected override void OnLayout(LayoutEventArgs e) {
		var f = FindForm();
		if (Controls.Count > 0) {
			var flp = (FLP) Controls[0];
			flp.UpdateDeltaHeights();
		}

		base.OnLayout(e);
	}

	protected override void OnSizeChanged(EventArgs e) {
		var f = FindForm();
		// prevent doing a layout if the form is minimized. This prevents a layout problem when
		// the form is restored. This also prevents the OnLayout from being fired. Added 2015-01-15
		if (f != null && f.IsHandleCreated && !f.IsDisposed && f.WindowState == FormWindowState.Minimized)
			return;

		var flp = (FLP) Controls[0];
		flp.PerformLayout(); // required to remove scrollbar flicker
		base.OnSizeChanged(e);
	}

	protected override void OnFontChanged(EventArgs e) {
		base.OnFontChanged(e);
		toolBox.Font = Font;
	}

	///<summary>Adds a control to this accordion. A header CheckBox is displayed above the control with the specified text.</summary>
	///<param name="c">Required. The control to add. Note: If you want the control to expand then make sure c.DockStyle = DockStyle.Fill.</param>
	///<param name="text">Required. The text to display in the header checkbox.</param>
	///<param name="toolTip">Optional. The tooltip that is displayed when the mouse hovers over the checkbox header.</param>
	///<param name="fillWt">Optional. Specify a value greater than zero to have the control expand if there is extra height available.<br/>
	///There are two different modes:<br/>
	///<para>1) Extra height is allocated to a single control. The fillWt serves as a binary indicator (zero or positive).
	///All the extra height will be allocated to the last opened control that has a positive fill weight. All other
	///controls will appear at their preferred height. This mode is used when 'FillLastOpened' is true.</para>
	///<para>2) Extra height is distributed between open controls that have fillWt > 0 and are not locked by the user. The
	///extra height is allocated to these controls in proportion to their fillWt versus the sum of the fill weights.
	///This mode is used when 'FillLastOpened' is false.</para></param>
	///<param name="open">Optional. It overrides the 'OpenOnAdd' value for this particular control only.</param>
	///<param name="contentPadding">Optional. It overrides the 'ContentPadding' value for this particular control only.</param>
	///<param name="contentMargin">Optional. It overrides the 'ContentMargin' value for this particular control only.</param>
	///<param name="contentBackColor">Optional. It overrides the 'ContentBackColor' value for this particular control only.</param>
	///<param name="checkboxMargin">Optional. It overrides the 'CheckBoxMargin' value for this particular control only.</param>
	///<returns>Returns the CheckBox header created by the CheckBoxFactory.</returns>
	public CheckBox Add(Control c, string text, string toolTip = null, double fillWt = 0, bool? open = null, Padding? contentPadding = null, Padding? contentMargin = null, Color? contentBackColor = null, Padding? checkboxMargin = null) {
		var cbf = CheckBoxFactory ?? GlobalCheckBoxFactory;
		var check = open.HasValue ? open.Value : OpenOnAdd;
		var cb = cbf.CreateCheckBox(text, check, Get(checkboxMargin, CheckBoxMargin));
		// the unicode arrows cause a brief flicker of the scrollbar
		cb.Text = (cb.Checked ? GetUpArrow() : GetDownArrow()) + cb.Text;

		var c2 = new Control2(cb, c, fillWt); // { Dock = DockStyle.Fill };
		c2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
		c2.Padding = Get(contentPadding, ContentPadding);

		c2.marginCached = Get(contentMargin, ContentMargin);
		if (check)
			c2.Margin = c2.marginCached;

		if (contentBackColor.HasValue)
			c2.BackColor = contentBackColor.Value;
		else if (ContentBackColor.HasValue)
			c2.BackColor = ContentBackColor.Value;

		if (ControlBackColor.HasValue)
			c.BackColor = ControlBackColor.Value;

		if (!string.IsNullOrEmpty(toolTip)) {
			tips.SetToolTip(cb, toolTip);
			//tips.SetToolTip(cb, toolTip);
		}

		// adding controls fires a Layout event on the host which causes
		// the host height to increase because new controls are added.
		// If other controls are currently filling the space, then the
		// scrollbars would briefly appear before the controls are resized
		host.isAdjusting = true;
		host.Controls.Add(cb);
		host.Controls.Add(c2);
		host.isAdjusting = false;

		cb.MouseHover += delegate {
			var stm = ShowToolMenu && (c2.fillWt > 0 || !ShowToolMenuRequiresPositiveFillWeight);
			if (stm && !toolBox.Visible && (cb.Checked || ShowToolMenuOnHoverWhenClosed)) {
				var p1 = cb.PointToClient(MousePosition);
				// must use pref-w because width is not accurate until first show
				var w = toolBox.GetPreferredSize(Size.Empty).Width + 1;
				if (p1.X >= cb.Width - w) {
					var p = new Point { X = cb.Width - w, Y = cb.Height };
					toolBox.Current = c2;
					toolBox.Show(cb, p);
				}
			}
		};

		cb.MouseUp += (o, e) => {
			var stm = ShowToolMenu && (c2.fillWt > 0 || !ShowToolMenuRequiresPositiveFillWeight);
			if (stm && e.Button == MouseButtons.Right && ShowToolMenuOnRightClick) {
				var p1 = cb.PointToClient(MousePosition);
				var w = toolBox.Width;
				p1.X -= w/2;
				p1.Y -= w/2;
				toolBox.Current = c2;
				toolBox.Show(cb, p1);
			}
		};

		cb.MouseLeave += delegate {
			if (toolBox.Visible) {
				// since toolBox has no parent, its bounds are in screen coordinates
				if (!toolBox.Bounds.Contains(MousePosition))
					toolBox.Hide();
			}
		};

		Action<bool> layout = ControlAdded => {
			if (cb.Checked)
				c2.lastClicked = DateTime.Now;

			// for some reason, when the scrollbar first appears, it scrolls an
			// amount equal to the ContentPadding.Left to the right. Also, the scroll
			// value resets to 0 right before the checkbox is changed, so it's not
			// easy to know what the previous value is.
			//int hsorig = 0;
			//if (this.HorizontalScroll.Visible)
			//	hsorig = this.HorizontalScroll.Value;

			if (OpenOneOnly) {
				if (lastChecked != null && cb.Checked && c2 != lastChecked) {
					isAdjusting = true;
					lastChecked.cb.Checked = false;
					isAdjusting = false;
				}
			}

			lastChecked = c2;
			host.UpdateDeltaHeights();

			// have to call it at least twice to force scrollbars to refresh
			// I think it's a timing issue, and the queuing up a bunch of calls
			// means the actual execution is delayed. Since it waits longer, the
			// end result is more accurate.
			host.PerformLayout(); // always need at least one
			host.PerformLayout();
			if (!ControlAdded) {
				host.PerformLayout();
			}

			if (HorizontalScroll.Visible && IsHandleCreated) {
				// something causes the scrollbar to scroll to the right by an amount equal
				// to the ContentPadding.Left value.
				BeginInvoke((Action) delegate {
					AutoScrollPosition = Point.Empty;
				});
			}

			if (cb.Checked) // only focus if the control is opened
				GetFirstFocus(c).Focus();

			if (VerticalScroll.Visible) {
				ScrollControlIntoView(c2);
				ScrollControlIntoView(cb);
			}
		};

		cb.CheckedChanged += delegate {
			var a = cb.Checked ? GetUpArrow() : GetDownArrow();
			host.isAdjusting = true;
			cb.Text = a + text; // must set text regardless of isAdjusting
			host.isAdjusting = false;
			// FlowLayoutPanel always accounts for margin, so when the
			// control is collapsed, the margin has to be set to zero.
			c2.Margin = cb.Checked ? c2.marginCached : Padding.Empty;

			if (isAdjusting)
				return;

			layout(false);
		};

		layout(true);
		return cb;
	}

	///<summary>Returns the number of controls that are added to this Accordion.</summary>
	public int Count {
		get {
			return host.Controls.Count / 2;
		}
	}

	///<summary>Returns the n'th CheckBox header.</summary>
	public CheckBox CheckBox(int i) {
		return (CheckBox) host.Controls[2 * i + 1];
	}

	///<summary>Returns the n'th Control.</summary>
	public Control Content(int i) {
		return ((Control2) host.Controls[2 * i + 2]).c;
	}

	///<summary>Finds the header CheckBox for the specified control. This can be useful
	///when using an ErrorProvider and you want to set an error for both the control and
	///on its header checkbox (incase the checkbox is closed).</summary>
	///<param name="c">A control that was added to the accordion.</param>
	///<returns>Returns the checkbox if the control is found, otherwise null is returned.</returns>
	public CheckBox CheckBoxForControl(Control c) {
		var acc = this;
		for (var i = 0; i < acc.Count; i++) {
			var cb = acc.CheckBox(i);
			var parent = acc.Content(i);
			if (Contains(parent, c))
				return cb;
		}
		return null;
	}

	private static bool Contains(Control parent, Control c) {
		if (parent == c)
			return true;

		foreach (Control c2 in parent.Controls) {
			var b = Contains(c2, c);
			if (b)
				return b;
		}

		return false;
	}

	private static Padding Get(params Padding?[] arr) {
		foreach (var p in arr)
			if (p.HasValue)
				return p.Value;
		return Padding.Empty;
	}

	// returns the first control that has no children and is Enabled
	private static Control GetFirstFocus(Control parent) {
		foreach (Control child in parent.Controls) {
			var c = GetFirstFocus(child);
			if (c.Enabled && c.TabStop)
				return c;
		}

		return parent;
	}

	private string GetDownArrow() {
		return DownArrow == null ? GlobalDownArrow : DownArrow;
	}

	private string GetUpArrow() {
		return UpArrow == null ? GlobalUpArrow : UpArrow;
	}

	///<summary>Closes the specified controls. If a control cannot be found then it is skipped.</summary>
	public void Close(params Control[] controls) {
		Open(controls, false);
	}

	///<summary>Opens the specified controls. If a control cannot be found then it is skipped.</summary>
	public void Open(params Control[] controls) {
		Open(controls, true);
	}

	private void Open(Control[] controls, bool open) {
		isAdjusting = true;
		var changed = false;
		Control2 last = null;
		foreach (var c2 in host.Control2s) {
			Control2 c = null;
			if (controls == null)
				c = c2;
			else {
				foreach (var cc in controls) {
					if (cc == c2.c) {
						c = c2;
						break;
					}
				}
			}
			if (c != null) {
				last = c;
				if (c.cb.Checked != open) {
					changed = true;
					c.cb.Checked = open;
				}
			}
		}
		isAdjusting = false;

		if (changed) {
			host.UpdateDeltaHeights();
			host.PerformLayout(); // multiple calls required
			host.PerformLayout(); // to get scrollbars to either
			host.PerformLayout(); // show or hide
		}
	}

	// It's possible to listen for the Resize event, but then the scrollbars will flicker.
	// Subclassing provides much nicer behavior.
	private class FLP : FlowLayoutPanel {
		// For FlowLayoutPanel, the dummy control is used to set to be the width of the host so that the
		// checkboxes that use Anchor = AnchorStyles.Left | AnchorStyles.Right will fill to the edges.
		// disabled tabstop so it doesn't receive the focus when pressing tab
		DummyControl dummy = new DummyControl();
		internal bool isAdjusting;

		public FLP() {
			FlowDirection = FlowDirection.TopDown;
			WrapContents = false;
			AutoSize = true; // must be set true so that GetPreferredSize is called
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			//AutoScroll = true; // do not set true, the parent will scroll
			//DoubleBuffered = true;
			TabStop = false;
			Margin = new Padding(0);
			Padding = new Padding(0);
			//BackColor = Color.Purple;
			//Dock = DockStyle.Fill; // scrollbars won't show up
			//dummy.BackColor = Color.BlanchedAlmond;
			Controls.Add(dummy);
		}

		public override Size GetPreferredSize(Size proposedSize) {
			return GetPreferredSize(true);
		}

		public Size GetPreferredSize(bool addDH, bool returnEmptyIfTrue = true, bool fillSize = true) {
			var a = (Accordion) Parent;
			var s = Size.Empty;
			var start = a.GrowAndShrink ? 1 : 0;
			for (var i = start; i < Controls.Count; i++) {
				var c = Controls[i];
				var s2 = Size.Empty;
				if (c is Control2) {
					var c2 = (Control2) c;
					s2 = c2.GetPreferredSize(Size.Empty, addDH, returnEmptyIfTrue);
				}
				else {
					s2 = c.GetPreferredSize(Size.Empty);
				}

				var m = c.Margin;
				s2.Width += m.Horizontal;
				s2.Height += m.Vertical;

				s.Height += s2.Height;
				if (s2.Width > s.Width)
					s.Width = s2.Width;
			}

			var p = Padding;
			s.Width += p.Horizontal;
			s.Height += p.Vertical;

			if (fillSize && (a.FillWidth || a.FillHeight)) {
				var s2 = a.ClientSize;
				if (s2.Width > s.Width && a.FillWidth)
					s.Width = s2.Width;
				if (s2.Height > s.Height && a.FillHeight)
					s.Height = s2.Height;
			}

			return s;
		}

		protected override void OnFontChanged(EventArgs e) {
			UpdateDeltaHeights(); // call before base method to prevent the
			PerformLayout();	  // the vertical scrollbar from briefly showing
			base.OnFontChanged(e);
		}

		internal IEnumerable<Control2> Control2s {
			get {
				foreach (Control c in Controls) {
					if (c is Control2)
						yield return (Control2) c;
				}
			}
		}

		public void UpdateDeltaHeights() {
			var acc = (Accordion) Parent;
			UpdateDeltaHeights(acc.FillLastOpened, acc.FillModeGrowOnly, acc.FillResetOnCollapse);
		}

		public void UpdateDeltaHeights(bool fillLastOpened, bool fillModeGrowOnly, bool fillResetOnCollapse) {
			var r = GetPreferredSize(false, true, false);
			var a = (Accordion) Parent;
			var cs = a.ClientSize;

			double totalWt = 0;
			var mh = 0; // minus height
			foreach (var c2 in Control2s) {
				if (c2.cb.Checked) {
					if (c2.isLocked)
						mh += c2.dh;
					else
						totalWt += c2.fillWt;
				}
			}

			var eh = Math.Max(0, cs.Height - r.Height - mh);

			if (fillLastOpened) {
				var fc = getFillControl();

				foreach (var c2 in Control2s) {
					if (c2.isLocked) // height is locked, do nothing
						continue;

					if (c2 == fc) {
						if (fillModeGrowOnly) {
							if (eh > c2.dh)
								c2.dh = eh;
						}
						else {
							c2.dh = eh;
						}
					}
					else {
						if (fillResetOnCollapse)
							c2.dh = 0;
					}
				}
			}
			else {
				double pixels = 0; // pixel perfect
				foreach (var c2 in Control2s) {
					if (c2.isLocked) // height is locked, do nothing
						continue;

					// if totalWt is zero, that means no controls with a fillWt are currently open
					// thus if fillResetOnCollapse is true, then their dh values should be reset to zero
					if (c2.cb.Checked && totalWt > 0) {
						var ddh = c2.fillWt * eh / totalWt;
						var dh = (int) ddh;

						pixels += ddh % 1;
						if (pixels >= 0.5) {
							dh++;
							pixels--;
						}
						if (fillModeGrowOnly) {
							if (dh > c2.dh)
								c2.dh = dh;
						}
						else
							c2.dh = dh;
					}
					else {
						if (fillResetOnCollapse)
							c2.dh = 0;
					}
				}
			}
		}

		private Control2 getFillControl() {
			Control2 cc = null;
			foreach (var c2 in Control2s) {
				if (c2.isLocked) // don't return a locked control to fill
					continue;

				if (c2.fillWt > 0 && c2.cb.Checked) {
					if (cc == null)
						cc = c2;
					else if (c2.lastClicked > cc.lastClicked)
						cc = c2;
				}
			}
			return cc;
		}

		protected override void OnLayout(LayoutEventArgs levent) {
			// changing the checkbox text from up arrow to down arrow causes a OnLayout
			// to happen, which causes a brief flicker of the scrollbars. Thus, if the
			// the text is adjusting, then just ignore the event.
			if (isAdjusting)
				return;
			base.OnLayout(levent);
		}

		// The anchor styles in FlowLayoutPanel work in reference to the widest control
		// or tallest control. Thus, a dummy control is required. The checkboxes will
		// expand to width of the dummyControl. If the available area is made smaller,
		// then the checkboxes will be as wide as the checkbox with the longest text and
		// scrollbars will appear.
		private class DummyControl : UserControl {
			public DummyControl() {
				AutoSize = true;
				AutoSizeMode = AutoSizeMode.GrowAndShrink;
				TabStop = false;
				Margin = Padding.Empty;
				BackColor = Color.BlanchedAlmond;
			}

			public override Size GetPreferredSize(Size proposedSize) {
				var host = (FLP) Parent;
				var a = (Accordion) host.Parent;
				if (a == null)
					return Size.Empty;

				if (!a.FillWidth && !a.GrowAndShrink)
					return new Size(host.DisplayRectangle.Width, dummyHeight);

				var w = 0;
				for (var i = 1; i < host.Controls.Count; i++) {
					var s = host.Controls[i].GetPreferredSize(proposedSize);
					if (s.Width > w)
						w = s.Width;
				}

				var w2 = a.ClientSize.Width - host.Padding.Horizontal;
				if (w2 > w && a.FillWidth)
					w = w2;

				return new Size(w, dummyHeight);
			}
		}
	}

	// cannot subclass Control directly because then the Padding isn't used
	private class Control2 : UserControl {
		internal CheckBox cb;
		internal Control c;
		internal int dh;
		internal double fillWt;
		internal DateTime lastClicked = DateTime.MinValue;
		internal bool isLocked;
		internal Padding marginCached { get; set; } // use property to avoid marshal warning

		public Control2(CheckBox cb, Control c, double fillWt) {
			this.cb = cb;
			this.c = c;
			this.fillWt = fillWt;
			AutoSize = true;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			Controls.Add(c);
			Margin = new Padding(0);
		}

		public override Size GetPreferredSize(Size proposedSize) {
			var s = GetPreferredSize(proposedSize, true, true);
			return s;
		}

		internal Size GetPackSize() {
			return GetPreferredSize(Size.Empty, false, false);
		}

		internal Size GetPreferredSize(Size proposedSize, bool addDH, bool returnEmptyIfClosed) {
			if (returnEmptyIfClosed && !cb.Checked)
				return Size.Empty;

			// cannot use base.GetPreferredSize(...)
			// UserControl has no defined layout, so it makes sense
			// that the returned size doesn't look at the children
			var s2 = c.GetPreferredSize(proposedSize);
			var p = Padding;
			s2.Width += p.Horizontal;
			s2.Height += p.Vertical;

			if (addDH)
				s2.Height += dh;

			return s2;
		}
	}

	private class ToolBox : ToolStripDropDown {
		ToolStripSplitButton miPack = new ToolStripSplitButton("\u2191") { ToolTipText = "Pack", Anchor = AnchorStyles.Left | AnchorStyles.Right }; // up arrow
		ToolStripButton miPackAll = new ToolStripButton("\u21c8") { ToolTipText = "Pack All" }; // double up arrow
		ToolStripButton miCloseAll = new ToolStripButton("\u23EB") { ToolTipText = "Close All" }; // double up triangles

		ToolStripSplitButton miFill = new ToolStripSplitButton("\u2193") { ToolTipText = "Fill", Anchor = AnchorStyles.Left | AnchorStyles.Right }; // down arrow
		ToolStripButton miFillAll = new ToolStripButton("\u21ca") { ToolTipText = "Fill All" }; // double down arrow
		ToolStripButton miOpenAll = new ToolStripButton("\u23EC") { ToolTipText = "Open All" }; // double down triangles

		ToolStripSplitButton miLock = new ToolStripSplitButton("\uD83D\uDD12") { ToolTipText = "", Anchor = AnchorStyles.Left | AnchorStyles.Right }; // lock-open / lock-close (toggle)
		ToolStripButton miLockAll = new ToolStripButton("\uD83D\uDD10") { ToolTipText = "Lock All" }; // lock with key
		ToolStripButton miUnlockAll = new ToolStripButton("\uD83D\uDD11") { ToolTipText = "Unlock All" }; // key

		Control2 _c2;

		public ToolBox() {

			var menu = this;
			menu.Padding = Padding = new Padding(3, 2, 3, 1);
			//menu.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			menu.DropShadowEnabled = false;
			menu.Items.Add(miPack);
			menu.Items.Add(miFill);
			menu.Items.Add(miLock);
			menu.BackColor = Color.Transparent;
			//menu.DefaultDropDownDirection = ToolStripDropDownDirection.BelowRight;

			miPack.DropDown = new ToolStripDropDown();
			miPack.DropDown.Padding = new Padding(3, 2, 1, 1);
			//miPack.Margin = new System.Windows.Forms.Padding(10);
			//miPack.DropDown.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
			miPack.DropDown.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			miPack.DropDown.DropShadowEnabled = false;
			miPack.DropDown.Items.Add(miPackAll);
			miPack.DropDown.Items.Add(miCloseAll);

			miFill.DropDown = new ToolStripDropDown();
			miFill.DropDown.Padding = new Padding(3, 2, 1, 1);
			miFill.DropDown.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			miFill.DropDown.DropShadowEnabled = false;
			miFill.DropDown.Items.Add(miFillAll);
			miFill.DropDown.Items.Add(miOpenAll);

			miLock.DropDown = new ToolStripDropDown();
			miLock.DropDown.Padding = new Padding(3, 2, 1, 1);
			miLock.DropDown.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			miLock.DropDown.DropShadowEnabled = false;
			miLock.DropDown.Items.Add(miLockAll);
			miLock.DropDown.Items.Add(miUnlockAll);

			Action<object> fillAction = src => {
				Current.cb.Focus();
				var host = (FLP) Current.Parent;
				var a = (Accordion) host.Parent;
				var r = host.GetPreferredSize(true, true, false);
				var cs = a.ClientSize;
				var eh = cs.Height - r.Height;
				Current.lastClicked = DateTime.Now;

				if (src == miFill) {
					if (eh > 0) {
						var oh = 0;
						if (!Current.cb.Checked) {
							Current.dh = 0;
							var ps = Current.GetPackSize();
							oh = ps.Height;
							if (ps.Width > cs.Width)
								oh += SystemInformation.HorizontalScrollBarHeight;
						}
						Current.dh += Math.Max(eh - oh, 0);
						// since extra space exists, only one perform layout is required
						// because scrollbars won't change
						host.PerformLayout();
					}
					else {
						if (!Current.cb.Checked) {
							Current.dh = 0;
						}
						var s = Current.GetPackSize();
						s.Height += Current.marginCached.Vertical;
						var dhNew = Math.Max(Current.dh, cs.Height - s.Height - Current.cb.Height);
						Current.dh = dhNew;
						var lockOrig = Current.isLocked;
						Current.isLocked = true;
						host.PerformLayout();
						host.PerformLayout();
						if (a.VerticalScroll.Visible) {
							try {
								a.VerticalScroll.Value = Current.cb.Location.Y;
							} catch {} // just in case
						}
						host.PerformLayout();
						Current.isLocked = lockOrig;
					}
				}
				else if (src == miFillAll) {
					if (eh > 0) {
						var n = 0;
						foreach (var c2 in host.Control2s) {
							if (c2.cb.Checked && c2.fillWt > 0)
								n++;
						}
						if (n == 0) return; // no controls are open
						double pixel = 0;
						foreach (var c2 in host.Control2s) {
							if (c2.cb.Checked && c2.fillWt > 0) {
								var hh = 1.0 * eh / n;
								var ihh = (int) hh;
								pixel += hh % 1;
								if (pixel >= 0.5) {
									ihh++;
									pixel--;
								}
								c2.dh += ihh;
							}
						}
						host.PerformLayout();
					}
				}
				else if (src == miOpenAll) {
					Current.lastClicked = DateTime.Now;
					a.Open(null);
				}
			};

			miFill.ButtonClick += delegate {
				fillAction(miFill);
			};

			miFill.DropDown.ItemClicked += (o, e) => {
				fillAction(e.ClickedItem);
			};

			Action<object> packAction = src => {
				Current.cb.Focus();
				var host = (FLP) Current.Parent;
				if (src == miPack || src == miPackAll) {
					foreach (var c2 in host.Control2s) {
						c2.Tag = c2.isLocked;
						c2.isLocked = true;
						if (c2 == Current || src == miPackAll) {
							c2.dh = 0;
							//c2.lastClicked = DateTime.MinValue;
						}
					}

					host.UpdateDeltaHeights();
					host.PerformLayout();
					host.PerformLayout();
					host.PerformLayout();

					foreach (var c2 in host.Control2s)
						c2.isLocked = (bool) c2.Tag;

					Current.lastClicked = DateTime.Now;
				}
				else if (src == miCloseAll) {
					var acc = (Accordion) host.Parent;
					acc.Close(null);
				}
			};

			miPack.ButtonClick += delegate {
				packAction(miPack);
			};

			miPack.DropDownItemClicked += (o,e) => {
				packAction(e.ClickedItem);
			};

			Action<object> lockAction = src => {
				Current.cb.Focus();
				Current.lastClicked = DateTime.Now;
				var host = (FLP) Current.Parent;
				if (src == miLockAll || src == miUnlockAll) {
					foreach (var c2 in host.Control2s)
						c2.isLocked = src == miLockAll;
				}
				else {
					Current.isLocked = !Current.isLocked;
					//if (!Current.isLocked)
				}

				host.UpdateDeltaHeights();
				host.PerformLayout();
				host.PerformLayout();
				host.PerformLayout();
			};

			miLock.ButtonClick += delegate {
				lockAction(miLock);
			};
			miLock.DropDown.ItemClicked += (o,e) => {
				lockAction(e.ClickedItem);
			};
		}

		DateTime leaveTime;
		protected override void OnMouseLeave(EventArgs e) {
 			base.OnMouseLeave(e);
			var toolBox = this;

			// the tooltip of a menu item causes a mouse leave, so
			// confirm the mouse is outside of the bounds before hiding
			leaveTime = DateTime.Now;
			new Thread(o => {
				// allow the mouse to leave for up to 1 second before closing
				Thread.Sleep(1000);
				if ((DateTime) o != leaveTime)
					return;

				// it's possible that the thread sleeps and when it wakes up
				// the form was closed and the toolBox is disposed.
				if (!toolBox.IsDisposed) {
					toolBox.BeginInvoke((Action) delegate {
						if (!IsMouseHit(MousePosition)) {
							toolBox.Hide();
						}
					});
				}
			}).Start(leaveTime);
		}

		private bool IsMouseHit(Point pt) {
			if (Bounds.Contains(pt))
				return true;

			if (miPack.DropDown.Visible && miPack.DropDown.Bounds.Contains(pt))
				return true;

			if (miFill.DropDown.Visible && miFill.DropDown.Bounds.Contains(pt))
				return true;

			if (miLock.DropDown.Visible && miLock.DropDown.Bounds.Contains(pt))
				return true;

			return false;
		}


		public Control2 Current {
			get {
				return _c2;
			}
			set {
				_c2 = value;
				if (_c2 == null)
					return;

				var host = (FLP) _c2.Parent;
				var a = (Accordion) host.Parent;
				var r = host.GetPreferredSize(true, true, false);
				var cs = a.ClientSize;
				var eh = cs.Height - r.Height;

				var hasLocked = false;
				var hasUnlocked = false;
				var hasOpen = false;
				var hasClosed = false;
				var hasOpenAndFill = false;
				foreach (var c2 in host.Control2s) {
					if (c2.isLocked)
						hasLocked = true;
					else
						hasUnlocked = true;

					if (c2.cb.Checked) {
						hasOpen = true;
						if (c2.fillWt > 0)
							hasOpenAndFill = true;
					}
					else
						hasClosed = true;
				}

				miUnlockAll.Enabled = hasLocked;
				miLockAll.Enabled = hasUnlocked;
				miFillAll.Enabled = hasOpenAndFill && eh > 0;
				miOpenAll.Enabled = hasClosed;
				miCloseAll.Enabled = hasOpen;

				if (_c2.isLocked) {
					miLock.ToolTipText = "Unlock height.";
					miLock.Text = "\uD83D\uDD13";
				}
				else {
					miLock.ToolTipText = "Lock height.";
					miLock.Text = "\uD83D\uDD12";
				}
			}
		}

		protected override void OnFontChanged(EventArgs e) {
			base.OnFontChanged(e);
			// the sub dropdown menus don't inherit
			foreach (ToolStripDropDownItem item in Items) {
				if (item.DropDown != null)
					item.DropDown.Font = Font;
			}
		}
	}

	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);
		if (disposing) {
			if (tips != null)
				tips.Dispose();

			if (toolBox != null)
				toolBox.Dispose();

			Application.RemoveMessageFilter(this);

			tips = null;
			toolBox = null;
		}
	}

	public interface ICheckBoxFactory {
		CheckBox CreateCheckBox(string text, bool check, Padding margin);
	}

	///<summary>The default checkbox factory uses the 'Appearance.Button' and anchors
	///the checkbox such that it will span the width of the accordion.</summary>
	public class DefaultCheckBoxFactory : ICheckBoxFactory {
		public virtual CheckBox CreateCheckBox(string text, bool check, Padding margin) {
			var cb = new CheckBox();
			cb.Appearance = Appearance.Button;
			cb.AutoSize = true; // AutoSize must be true or the text will wrap under and be hidden
			cb.Checked = check;
			cb.Text = text;
			cb.Anchor = AnchorStyles.Left | AnchorStyles.Right; // cb.Dock = DockStyle.Fill also works.
			cb.Margin = margin; // typically 0 so that no are gaps between the buttons
			return cb;
		}
	}
}

#if DEBUG

// can be used to reproduce the layout problem if the Form WindowState == Minimized is not ignored.
public class AccordionLayoutProblemForm : Form {
	public AccordionLayoutProblemForm() {
		var acc = new Accordion();
		var dgv = new DataGridView { AutoSize = true, Dock = DockStyle.Fill };
		dgv.Columns.Add("Column1", "Column1");
		acc.Add(dgv, "DGV", fillWt:1, open:true);
		Controls.Add(acc);
	}
}

public class AccordionTestForm : Form {

	Accordion acc = new Accordion();

	RadioButton rbOption1 = new RadioButton { Text = "Very Very Very Long Option Text 1", AutoSize = true };
	RadioButton rbOption2 = new RadioButton { Text = "Option 2", AutoSize = true };

	SplitContainer splitPane = new SplitContainer { Orientation = Orientation.Vertical, Dock = DockStyle.Fill, BorderStyle = BorderStyle.Fixed3D, SplitterWidth = 6 };

	public AccordionTestForm() {
		Padding = new Padding(10);
		Controls.Add(splitPane);
		Size = new Size(700, 700);
		//ScrollPane scrollPane = new ScrollPane(acc);
		//ScrollPane scrollPane = new ScrollPane(new TextBox2("", "asdfasdfasdfasdfasdfasdfasdf"));
		
		splitPane.Panel1.Controls.Add(acc);
		splitPane.FixedPanel = FixedPanel.Panel1;
		acc.Insets = new Padding(10);
		acc.ContentPadding = new Padding(10);
		acc.ContentMargin = new Padding(0);
		acc.CheckBoxMargin = new Padding(0);

		//FlowLayoutPanel p = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Dock = DockStyle.Fill };
		var p = new TableLayoutPanel { Dock = DockStyle.Fill };
		p.Margin = new Padding(0);
		p.BackColor = Color.LightBlue;
		p.Controls.Add(rbOption1);
		p.Controls.Add(rbOption2);
		p.Controls.Add(new TextBox { Dock = DockStyle.Fill }); //, Anchor = (AnchorStyles) (AnchorStyles.Left | AnchorStyles.Right) });

		var s = p.GetPreferredSize(Size.Empty);

		//p.SetFlowBreak(rbOption2, true);
		//p.Padding = new System.Windows.Forms.Padding(10);
		//p.Margin = new System.Windows.Forms.Padding(10);

		//FlowLayoutPanel panel = new FlowLayoutPanel();// { Dock = DockStyle.Fill };
		//panel.BackColor = Color.Blue;
		//panel.Controls.Add(acc);
		//Controls.Add(panel);
		Control c = new TextBox { Text = "Some text information.", Dock = DockStyle.Fill, Multiline = true };
		acc.Add(c, "Some control", "Tooltip 1", 1, true);
		acc.Add(p, "Radio Options", "ToolTip 2");

		var panel = new UserControl {  Margin = Padding.Empty, Padding = Padding.Empty, Dock = DockStyle.Fill };
		var cb = new CheckBox { Text = "Check Box 1", Margin = Padding.Empty };
		//cb.BackColor = Color.Red;
		panel.Controls.Add(cb);
		Color? blue = null;
		acc.Add(panel, "Check Box", contentBackColor:blue, open:true);

		var options = new FlowLayoutPanel { Dock = DockStyle.Fill };
		var btn = new Button {Text = "Add" };
		var cb1 = new CheckBox{Text = "Only One Open", AutoSize = true };
		var cb2 = new CheckBox { Text = "Fill Last", AutoSize = true };
		var cb3 = new CheckBox { Text = "Grow Only", AutoSize = true };
		var cb4 = new CheckBox { Text = "Fill Reset", AutoSize = true};
		var cb5 = new CheckBox { Text = "Expand", AutoSize = true };
		var cb6 = new CheckBox { Text = "Fill Wt", AutoSize = true };
		options.Controls.Add(btn);
		options.Controls.Add(cb1);
		options.Controls.Add(cb2);
		options.Controls.Add(cb3);
		options.Controls.Add(cb4);
		options.Controls.Add(cb5);
		options.Controls.Add(cb6);
		cb1.CheckedChanged += delegate {
			acc.OpenOneOnly = cb1.Checked;
		};
		cb2.CheckedChanged += delegate {
			acc.FillLastOpened = cb2.Checked;
		};
		cb3.CheckedChanged += delegate {
			acc.FillModeGrowOnly = cb3.Checked;
		};
		cb4.CheckedChanged += delegate {
			acc.FillResetOnCollapse = cb4.Checked;
		};
		splitPane.Panel2.Controls.Add(options);
		var count = 10;
		btn.Click += delegate {
			Control cc = new TextBox { Text = "" + count, Dock = DockStyle.Fill, Multiline = true };
			acc.Add(cc, "Some control " + count, "", cb6.Checked ? 1 : 0, cb5.Checked);
			count++;
		};
	}
}
#endif

}