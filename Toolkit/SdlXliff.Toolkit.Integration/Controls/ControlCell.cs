using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SdlXliff.Toolkit.Integration.Data;

namespace SdlXliff.Toolkit.Integration.Controls
{
    public class ControlCell : DataGridViewCell
    {
        private const int _cellMargin = 5;
        private const int _rtbCellMargin = 3;
        private const int _imageSize = 16;

        private const int _minCutSegment = 20;
        private const int _textBoxHeight = 22;

        private int _textLeftMargin = _cellMargin * 2 + _imageSize;
        private Color _lineColor = Color.DarkGray;
        private Color _selectedLineColor = Color.LightGray;

        private bool _fileFilter;
        private bool _isSearch;
        private bool _searchInTags;
        private bool _searchInSource;
        private bool _searchInTarget;

        public ControlCell() : base()
        {

        }

        protected override void Paint(System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds,
            System.Drawing.Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates cellState, object value,
            object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            // general settings
            _fileFilter = ((ControlColumn)this.OwningColumn).FileFilter;
            _isSearch = ((ControlColumn)this.OwningColumn).IsSearch;
            _searchInTags = ((ControlColumn)this.OwningColumn).SearchInTags;
            _searchInSource = ((ControlColumn)this.OwningColumn).SearchInSource;
            _searchInTarget = ((ControlColumn)this.OwningColumn).SearchInTarget;

            bool isCellSelected = (cellState & DataGridViewElementStates.Selected) != 0;

            // paint border
            PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);

            // Discard the space taken up by the borders.
            Rectangle borderWidths = BorderWidths(advancedBorderStyle);
            Rectangle valBounds = cellBounds;
            valBounds.Offset(borderWidths.X, borderWidths.Y);
            valBounds.Width -= borderWidths.Right;
            valBounds.Height -= borderWidths.Bottom;

            // paint background
            using (SolidBrush backgroundBrush = new SolidBrush(isCellSelected ? cellStyle.SelectionBackColor : cellStyle.BackColor))
            {
                Rectangle backgroundRect = valBounds;
                backgroundRect.Intersect(clipBounds);
                graphics.FillRectangle(backgroundBrush, backgroundRect);
            }

            // Discard the space taken up by the padding area.
            if (cellStyle.Padding != Padding.Empty)
            {
                valBounds.Offset(cellStyle.Padding.Left, cellStyle.Padding.Top);
                valBounds.Width -= cellStyle.Padding.Horizontal;
                valBounds.Height -= cellStyle.Padding.Vertical;
            }

            // DRAW CONTENT
            if (formattedValue != null && formattedValue is DetailCellData)
            {
                DetailCellData cellData = (DetailCellData)formattedValue;

                int currTopPos = valBounds.Top + _cellMargin;
                if (formattedValue.ToString().Length > 0)
                {
                    // TODO - REMOVE
                    //// draw image
                    //if (valBounds.Width > 0 && valBounds.Height > 0)
                    //{
                    //    Image img = null;
                    //    if (cellData.Type == Properties.Resources.typeWarning)
                    //        img = Properties.Resources.warningIcon;
                    //    else if (cellData.Type == Properties.Resources.typeReplaced)
                    //        img = Properties.Resources.acceptIcon;
                    //    else if (cellData.Type == Properties.Resources.typeTarget)
                    //        img = Properties.Resources.targetIcon;
                    //    else img = Properties.Resources.sourceIcon;

                    //    graphics.DrawImage(img, valBounds.Left + _cellMargin, currTopPos);
                    //}

                    int paddingTop = cellStyle.Font.Height + _cellMargin;

                    // if file in File Filter was not selected
                    // TODO - REMOVE
                    //if (!_fileFilter)
                    //{
                    //    // draw label -> FILE NAME
                    //    if (valBounds.Width > 0 && valBounds.Height > 0)
                    //    {
                    //        Font cellFont = new Font(cellStyle.Font, FontStyle.Bold);
                    //        Point glyphLocation = new Point(valBounds.Left + _cellMargin, currTopPos);
                    //        Size glyphSz = new System.Drawing.Size(valBounds.Width - _cellMargin * 2, _textBoxHeight);
                    //        Rectangle glyphRect = new Rectangle(glyphLocation, glyphSz);
                    //        TextRenderer.DrawText(graphics, cellData.FileName, cellFont, glyphRect,
                    //                            (isCellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor),
                    //                            (isCellSelected ? cellStyle.SelectionBackColor : cellStyle.BackColor),
                    //                            TextFormatFlags.Left);
                    //    }

                    //    // Discard the space
                    //    currTopPos += paddingTop;
                    //    valBounds.Height -= paddingTop;
                    //}
                    valBounds.Height -= _cellMargin;

                    // SIDE-BY-SIDE
                    int panelWidth = valBounds.Width / 2;
                    int xMiddlePos = valBounds.Left + panelWidth;
                    if (valBounds.Width > 0 && valBounds.Height > 0)
                    {
                        // draw line-separator
                        using (Pen linePen = new Pen(isCellSelected ? _selectedLineColor : _lineColor))
                        {
                            Point topPoint = new Point(xMiddlePos, currTopPos);
                            Point bottomPoint = new Point(xMiddlePos, currTopPos + valBounds.Height - _cellMargin);
                            graphics.DrawLine(linePen, topPoint, bottomPoint);
                        }

                        // draw label -> MATCHES NUMBER
                        // 1 - SOURCE
                        // image
                        var img = Properties.Resources.sourceIcon;
                        graphics.DrawImage(img, valBounds.Left + _cellMargin, currTopPos - 1);

                        // label
                        string labelText = GetMatchesNumberText(cellData, true);

                        Font cellFont = new Font(cellStyle.Font, FontStyle.Bold);

                        Point glyphLocation = new Point(valBounds.Left + _textLeftMargin, currTopPos);
                        Size glyphSz = new System.Drawing.Size(panelWidth - _cellMargin - _textLeftMargin, _textBoxHeight);
                        Rectangle glyphRect = new Rectangle(glyphLocation, glyphSz);
                        TextRenderer.DrawText(graphics, labelText,
                                            cellFont, glyphRect,
                                            (isCellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor),
                                            (isCellSelected ? cellStyle.SelectionBackColor : cellStyle.BackColor),
                                            TextFormatFlags.Left);

                        // 2 - TARGET
                        // image
                        if (_isSearch)
                            img = Properties.Resources.targetIcon;
                        else if (cellData.Warnings != null && cellData.Warnings.Count > 0)
                            img = Properties.Resources.warningIcon;
                        else img = Properties.Resources.acceptIcon;

                        graphics.DrawImage(img, xMiddlePos + _cellMargin, currTopPos - 1);

                        // label
                        labelText = GetMatchesNumberText(cellData, false);

                        glyphLocation = new Point(xMiddlePos + _textLeftMargin, currTopPos);
                        glyphSz = new System.Drawing.Size(panelWidth - _cellMargin - _textLeftMargin, _textBoxHeight);
                        glyphRect = new Rectangle(glyphLocation, glyphSz);
                        TextRenderer.DrawText(graphics, labelText,
                                            cellFont, glyphRect,
                                            (isCellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor),
                                            (isCellSelected ? cellStyle.SelectionBackColor : cellStyle.BackColor),
                                            TextFormatFlags.Left);

                        // Discard the space
                        currTopPos += paddingTop;
                        valBounds.Height -= paddingTop;
                        // valBounds.Height -= _cellMargin; - > to leave more space for text
                    }

                    // draw richtextbox -> TEXT
                    if (valBounds.Width > 0 && valBounds.Height > 0)
                    {

                        // set richtextbox with text
                        RichTextBox rtbControl = new RichTextBox();
                        rtbControl.Size = new Size(panelWidth - _textLeftMargin - _cellMargin, valBounds.Height);

                        // paint image from richtextbox
                        Size imgSize = new Size(rtbControl.Width - 1, rtbControl.Height - 1);

                        // 1 - SOURCE
                        if (cellData.Source.Text.Length > 0)
                        {
                            // create image
                            var img = PrintCellText(ref rtbControl, imgSize, cellStyle, cellData.Source, isCellSelected);
                            // draw image in cell
                            graphics.DrawImage(img, valBounds.Left + _textLeftMargin + _rtbCellMargin, currTopPos);
                        }

                        rtbControl.Clear();

                        // 2 - TARGET
                        if (cellData.Target.Text.Length > 0)
                        {
                            // create image
                            var img = PrintCellText(ref rtbControl, imgSize, cellStyle, cellData.Target, isCellSelected);
                            // draw image in cell
                            graphics.DrawImage(img, xMiddlePos + _textLeftMargin + _rtbCellMargin, currTopPos);
                        }
                    }

                    // TODO - REMOVE
                    //// draw label -> MATCHES NUMBER
                    //if (valBounds.Width > 0 && valBounds.Height > 0)
                    //{
                    //    string labelText = "";
                    //    if (cellData.IsTagMatch)
                    //        labelText = string.Format(Properties.Resources.gvMatchesNumberTag,
                    //         cellData.TagMatchesCount);
                    //    else labelText = string.Format(Properties.Resources.gvMatchesNumber,
                    //         cellData.MatchStart.Count);
                    //    Font cellFont = new Font(cellStyle.Font, FontStyle.Bold);

                    //    Point glyphLocation = new Point(valBounds.Left + _textLeftMargin, currTopPos);
                    //    Size glyphSz = new System.Drawing.Size(valBounds.Width - _cellMargin - _textLeftMargin, _textBoxHeight);
                    //    Rectangle glyphRect = new Rectangle(glyphLocation, glyphSz);
                    //    TextRenderer.DrawText(graphics, labelText,
                    //                        cellFont, glyphRect,
                    //                        (isCellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor),
                    //                        (isCellSelected ? cellStyle.SelectionBackColor : cellStyle.BackColor),
                    //                        TextFormatFlags.Left);
                    //}

                    //// Discard the space
                    //currTopPos += paddingTop;
                    //valBounds.Height -= paddingTop;
                    //valBounds.Height -= _cellMargin;

                    //// draw label -> WARNING
                    //if (cellData.Type == Properties.Resources.typeWarning && cellData.WarningText.Length > 0)
                    //{
                    //    if (valBounds.Width > 0 && valBounds.Height > 0)
                    //    {
                    //        Point glyphLocation = new Point(valBounds.Left + _textLeftMargin, currTopPos);
                    //        Size glyphSz = new System.Drawing.Size(valBounds.Width - _cellMargin - _textLeftMargin, _textBoxHeight);
                    //        Rectangle glyphRect = new Rectangle(glyphLocation, glyphSz);
                    //        TextRenderer.DrawText(graphics, cellData.WarningText,
                    //                            cellStyle.Font, glyphRect,
                    //                            (isCellSelected ? cellStyle.SelectionForeColor : Color.Red),
                    //                            (isCellSelected ? cellStyle.SelectionBackColor : cellStyle.BackColor),
                    //                            TextFormatFlags.Left);
                    //    }

                    //    // Discard the space
                    //    currTopPos += paddingTop;
                    //    valBounds.Height -= paddingTop;
                    //    //valBounds.Height -= _cellMargin; < -- better without it
                    //}

                    //// draw richtextbox -> TEXT
                    //if (valBounds.Width > 0 && valBounds.Height > 0)
                    //{
                    //    Image img = null;

                    //    // set richtextbox with text
                    //    RichTextBox rtbControl = new RichTextBox();
                    //    rtbControl.Size = new Size(valBounds.Width - _textLeftMargin - _cellMargin, valBounds.Height);

                    //    // paint image from richtextbox
                    //    Size imgSize = new Size(rtbControl.Width - 1, rtbControl.Height - 1);
                    //    if (isCellSelected)
                    //    {
                    //        // Selected cell state
                    //        rtbControl.BackColor = cellStyle.SelectionBackColor;
                    //        rtbControl.ForeColor = cellStyle.SelectionForeColor;

                    //        SetRichTextBoxText(rtbControl, cellData.Source, isCellSelected, imgSize.Width, imgSize.Height);

                    //        // Print image
                    //        img = RTBControlPrinter.Print(rtbControl, imgSize.Width, imgSize.Height);

                    //        // Restore RichTextBox
                    //        rtbControl.BackColor = cellStyle.SelectionBackColor;
                    //        rtbControl.ForeColor = cellStyle.ForeColor;
                    //    }
                    //    else
                    //    {
                    //        SetRichTextBoxText(rtbControl, cellData.Source, isCellSelected, imgSize.Width, imgSize.Height);

                    //        img = RTBControlPrinter.Print(rtbControl, imgSize.Width, imgSize.Height);
                    //    }

                    //    // draw image in cell
                    //    graphics.DrawImage(img, valBounds.Left + _textLeftMargin + _rtbCellMargin, currTopPos);
                    //}
                }
            }
        }

        public override void InitializeEditingControl(int rowIndex, object
        initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            // Set the value of the editing control to the current cell value.
            base.InitializeEditingControl(rowIndex, initialFormattedValue,
                dataGridViewCellStyle);
            ControlPanel ctl =
                DataGridView.EditingControl as ControlPanel;
            // Use the default row value when Value property is null.
            if (this.Value == null)
            {
                ctl.Value = (DetailCellData)this.DefaultNewRowValue;
            }
            else
            {
                ctl.Value = (DateTime)this.Value;
            }
        }

        public override Type EditType
        {
            get
            {
                // Return the type of the editing control that CalendarCell uses.
                return typeof(ControlPanel);
            }
        }

        public override Type ValueType
        {
            get
            {
                // Return the type of the value that CalendarCell contains.

                return typeof(DetailCellData);
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                // Use the current date and time as the default value.
                List<int> defList = new List<int>();
                defList.Add(0);
                return new DetailCellData("def", 0, 0, "def");
            }
        }

        #region private voids
        private Image PrintCellText(ref RichTextBox richTB, Size imgSize, DataGridViewCellStyle cellStyle, SegmentCellData segData, bool isSelected)
        {
            Image img = null;
            if (isSelected)
            {
                // Selected cell state
                richTB.BackColor = cellStyle.SelectionBackColor;
                richTB.ForeColor = cellStyle.SelectionForeColor;

                SetRichTextBoxText(richTB, segData, isSelected, imgSize.Width, imgSize.Height);

                // Print image
                img = RTBControlPrinter.Print(richTB, imgSize.Width, imgSize.Height);

                // Restore RichTextBox
                richTB.BackColor = cellStyle.SelectionBackColor;
                richTB.ForeColor = cellStyle.ForeColor;
            }
            else
            {
                SetRichTextBoxText(richTB, segData, isSelected, imgSize.Width, imgSize.Height);

                img = RTBControlPrinter.Print(richTB, imgSize.Width, imgSize.Height);
            }

            return img;
        }

        private void SetRichTextBoxText(RichTextBox ctl, SegmentCellData sData, bool isSelected, int imgWidth, int imgHeight)
        {
            SegmentCellData newSData = sData.Clone();

            // set text
            int maxDisplayLength = GetSegmentLength(ctl, sData.Text, imgWidth, imgHeight);
            if (sData.MatchIndexes == null || sData.MatchIndexes.Count <= 0)
                CutSegmentText(ref newSData, maxDisplayLength);
            else
                CutSegmentTextMatches(ref newSData, maxDisplayLength);

            try
            {
                ctl.Rtf = newSData.Text;
            }
            catch (ArgumentException)
            {
                ctl.Text = newSData.Text;
            }

            // highlight matches
            if (sData.MatchIndexes != null && sData.MatchIndexes.Count > 0)
                RTBManager.SelectRTBSegmentText(ctl, newSData.MatchIndexes, isSelected);
        }

        private void CutSegmentTextMatches(ref SegmentCellData data, int maxLength)
        {
            string cutSymb = Properties.Resources.gvCutTextString;
            string cutSeparator = Properties.Resources.gvCutTextSeparator;
            int textLength = data.Text.Length;

            int minConcordance = data.MatchIndexes.Max(ind => ind.Length) + _minCutSegment;
            int dataMatchesCount = data.MatchIndexes.Count;

            bool isReversedType = (dataMatchesCount > 1 && data.MatchIndexes[0].IndexStart > data.MatchIndexes[1].IndexStart);

            if (textLength > maxLength)
            {
                // calc max length of each part with match
                // do not show all matches if maxPartLength is too small
                int maxPartLength = GetPartLength(ref dataMatchesCount, maxLength, minConcordance);

                // if text length is enougth for cutting -> START CUTTING
                if (maxPartLength >= minConcordance)
                {
                    StringBuilder newText = new StringBuilder();
                    int maxPartLeftLength;

                    int prevTextIndex = 0;
                    int prevCutTextIndex = 0;
                    int leftCutIndex = 0;
                    int leftIndex, rightIndex, leftShift;
                    int firstLeftIndex = 0;
                    int firstLeftShift = 0;

                    // check operation type, change order of indexes to process
                    bool isStart = true;
                    int i = (isReversedType ? dataMatchesCount - 1 : 0);

                    // for each match found
                    //for (int i = 0; i < dataMatchesCount; i++)
                    while ((isReversedType && i >= 0) || (!isReversedType && i < dataMatchesCount))
                    {
                        maxPartLeftLength = (maxPartLength - data.MatchIndexes[i].Length) / 2;

                        // left index
                        leftIndex = data.MatchIndexes[i].IndexStart - maxPartLeftLength;

                        // handle cases when indexes overlap
                        if ((leftIndex <= prevTextIndex && leftIndex > 0) || (leftIndex < 0 && prevTextIndex > 0))
                        {
                            leftIndex = prevTextIndex;
                            if (leftIndex != textLength) // only if not text end
                            {
                                newText = newText.Remove(newText.Length - cutSymb.Length, cutSymb.Length);
                                prevCutTextIndex -= cutSymb.Length;
                            }
                        }
                        if (leftIndex < 0)
                            leftIndex = 0;
                        leftCutIndex = data.MatchIndexes[i].IndexStart - leftIndex;

                        // right index
                        rightIndex = data.MatchIndexes[i].IndexStart + data.MatchIndexes[i].Length + maxPartLeftLength;
                        if (rightIndex > textLength)
                            rightIndex = textLength;

                        // create cut text
                        leftShift = 0;
                        if (isStart && leftIndex > 0)
                        { newText.Append(cutSymb); leftShift = cutSymb.Length; }
                        else if (!isStart && leftIndex > prevTextIndex)
                        { newText.Append(cutSeparator + cutSymb); leftShift = (cutSeparator + cutSymb).Length; }
                        newText.Append(data.Text.Substring(leftIndex, rightIndex - leftIndex));
                        newText.Append(rightIndex == textLength ? "" : cutSymb);

                        #region modify indexes for remaining matches
                        if (isStart)
                        {
                            if (isReversedType)
                            {
                                firstLeftIndex = leftIndex - leftShift;
                                firstLeftShift = leftShift;
                            }
                            isStart = false;
                        }
                        if (!isReversedType)
                        {
                            firstLeftIndex = data.MatchIndexes[i].IndexStart - (leftShift + prevCutTextIndex + leftCutIndex);
                            firstLeftShift = leftShift;
                        }
                        #endregion

                        // update start index
                        data.MatchIndexes[i].IndexStart = leftShift;
                        data.MatchIndexes[i].IndexStart += prevCutTextIndex;
                        data.MatchIndexes[i].IndexStart += leftCutIndex;

                        // update cut text indexes
                        prevTextIndex = rightIndex;
                        prevCutTextIndex = newText.Length;

                        i = (isReversedType ? i - 1 : i + 1);
                    }

                    // update DetailsData text
                    data.Text = newText.ToString();

                    // update MatchStart for all indexes not in cut range
                    if (dataMatchesCount < data.MatchIndexes.Count)
                        for (int j = dataMatchesCount; j < data.MatchIndexes.Count; j++)
                        {
                            data.MatchIndexes[j].IndexStart = data.MatchIndexes[j].IndexStart - firstLeftIndex;
                            if (data.MatchIndexes[j].IndexStart < 0)
                            {
                                data.MatchIndexes[j].Length = data.MatchIndexes[j].Length + data.MatchIndexes[j].IndexStart - firstLeftShift;
                                data.MatchIndexes[j].IndexStart = 0;
                                if (data.MatchIndexes[j].Length < 0) data.MatchIndexes[j].Length = 0;
                                else data.MatchIndexes[j].Length = data.MatchIndexes[j].Length + firstLeftShift;
                            }
                            if (data.MatchIndexes[j].IndexStart >= data.Text.Length)
                                data.MatchIndexes[j].IndexStart = data.Text.Length;
                            if (data.MatchIndexes[j].IndexStart + data.MatchIndexes[j].Length > data.Text.Length)
                                data.MatchIndexes[j].Length = data.Text.Length - data.MatchIndexes[j].IndexStart;
                        }
                }
            }
        }

        private void CutSegmentText(ref SegmentCellData data, int maxLength)
        {
            string cutSymb = Properties.Resources.gvCutTextString;
            string text = data.Text;

            if (text.Length > maxLength)
            {
                int maxPartLength = maxLength;
                maxPartLength -= cutSymb.Length;

                string cutText = string.Format("{0}{1}", text.Substring(0, maxPartLength), cutSymb);
                data.Text = cutText;
            }
        }

        private int GetPartLength(ref int matchesCount, int maxLength, int minPartLength)
        {
            int maxPartLength = 0;
            int symbLength = Properties.Resources.gvCutTextString.Length;
            int separatorLength = Properties.Resources.gvCutTextSeparator.Length;

            while (matchesCount > 0 && maxPartLength < minPartLength)
            {
                maxPartLength = maxLength / matchesCount;
                maxPartLength -= symbLength * matchesCount * 2;
                maxPartLength -= separatorLength * (matchesCount - 1);

                // decrease number of matches to show
                if (maxPartLength < minPartLength)
                    matchesCount--;
            }

            return maxPartLength;
        }

        private int GetSegmentLength(RichTextBox ctl, string txt, int imgWidth, int imgHeight)
        {
            int defNumber = 60;
            int charsNumber = defNumber;
            using (Graphics g = ctl.CreateGraphics())
            {
                SizeF charSize = g.MeasureString(txt, ctl.Font);

                // get number of chars in one line
                // float charWidth = (charSize.Width - ctl.Margin.Horizontal) / txt.Length; // < -- correct
                float charWidth = (charSize.Width + 8) / txt.Length; // < -- works better
                float charsInLine = imgWidth / charWidth;
                float linesNumber2 = imgHeight / charSize.Height;
                int linesNumber = (int)linesNumber2;
                if (linesNumber < linesNumber2 && (linesNumber + 1) < (linesNumber2 + 0.15))
                    linesNumber++;
                charsNumber = (int)Math.Round(charsInLine * linesNumber);
            }

            return (charsNumber < defNumber ? defNumber : charsNumber);
        }

        private string GetMatchesNumberText(DetailCellData cData, bool isSource)
        {
            string text = "";

            if (isSource)
            {
                if (_searchInSource)
                {
                    if (_searchInTags)
                        text = string.Format(Properties.Resources.gvMatchesNumberTag,
                         cData.Source.TagMatchesCount);
                    else text = string.Format(Properties.Resources.gvMatchesNumber,
                        (cData.Source.MatchIndexes == null ? 0 : cData.Source.MatchIndexes.Count));
                }
                else text = Properties.Resources.gvMatchesNumberNA;
            }
            else
            {
                if (_isSearch)
                {
                    if (_searchInTarget && _searchInTags)
                        text = string.Format(Properties.Resources.gvMatchesNumberTag,
                              cData.Target.TagMatchesCount);
                    else if (_searchInTarget)
                        text = string.Format(Properties.Resources.gvMatchesNumber,
                            (cData.Target.MatchIndexes == null ? 0 : cData.Target.MatchIndexes.Count));
                    else text = Properties.Resources.gvMatchesNumberNA;
                }
                else
                {
                    text = string.Format(Properties.Resources.gvReplacesNumber,
                              cData.Target.MatchIndexes.Count);
                    if (cData.Warnings != null && cData.Warnings.Count > 0)
                        text += string.Format(", " + Properties.Resources.gvWarningsNumber,
                                      cData.WarningsCount);
                }
            }

            return text;
        }

        // TODO - REMOVE
        //private void CutSegmentTextMatches(ref DetailData data, int maxLength)
        //{
        //    string cutSymb = Properties.Resources.gvCutTextString;
        //    string cutSeparator = Properties.Resources.gvCutTextSeparator;
        //    int minConcordance = data.MatchLength.Max() + _minCutSegment;
        //    int textLength = data.Text.Length;

        //    bool isReversedType = (data.Type == Properties.Resources.typeReplaced || data.Type == Properties.Resources.typeWarning);

        //    if (data.SegID == 118)
        //    { }

        //    if (textLength > maxLength)
        //    {
        //        // calc max length of each part with match
        //        // do not show all matches if maxPartLength is too small
        //        int dataMatchesCount = data.MatchStart.Count;
        //        int maxPartLength = 0;
        //        while (dataMatchesCount > 0 && maxPartLength < minConcordance)
        //        {
        //            maxPartLength = maxLength / dataMatchesCount;
        //            maxPartLength -= cutSymb.Length * dataMatchesCount * 2;
        //            maxPartLength -= cutSeparator.Length * (dataMatchesCount - 1);

        //            // decrease number of matches to show
        //            if (maxPartLength < minConcordance)
        //                dataMatchesCount--;
        //        }

        //        // if text length is enougth for cutting -> START CUTTING
        //        if (maxPartLength >= minConcordance)
        //        {
        //            StringBuilder newText = new StringBuilder();
        //            int maxPartLeftLength;

        //            int prevTextIndex = 0;
        //            int prevCutTextIndex = 0;
        //            int leftCutIndex = 0;
        //            int leftIndex, rightIndex, leftShift;
        //            int firstLeftIndex = 0;
        //            int firstLeftShift = 0;

        //            // check operation type, change order of indexes to process
        //            bool isStart = true;
        //            int i = (isReversedType ? dataMatchesCount - 1 : 0);

        //            // for each match found
        //            //for (int i = 0; i < dataMatchesCount; i++)
        //            while ((isReversedType && i >= 0) || (!isReversedType && i < dataMatchesCount))
        //            {
        //                maxPartLeftLength = (maxPartLength - data.MatchLength[i]) / 2;

        //                // left index
        //                leftIndex = data.MatchStart[i] - maxPartLeftLength;

        //                // handle cases when indexes overlap
        //                if ((leftIndex <= prevTextIndex && leftIndex > 0) || (leftIndex < 0 && prevTextIndex > 0))
        //                {
        //                    leftIndex = prevTextIndex;
        //                    if (leftIndex != textLength) // only if not text end
        //                    {
        //                        newText = newText.Remove(newText.Length - cutSymb.Length, cutSymb.Length);
        //                        prevCutTextIndex -= cutSymb.Length;
        //                    }
        //                }
        //                if (leftIndex < 0)
        //                    leftIndex = 0;
        //                leftCutIndex = data.MatchStart[i] - leftIndex;

        //                // right index
        //                rightIndex = data.MatchStart[i] + data.MatchLength[i] + maxPartLeftLength;
        //                if (rightIndex > textLength)
        //                    rightIndex = textLength;

        //                // create cut text
        //                leftShift = 0;
        //                if (isStart && leftIndex > 0)
        //                { newText.Append(cutSymb); leftShift = cutSymb.Length; }
        //                else if (!isStart && leftIndex > prevTextIndex)
        //                { newText.Append(cutSeparator + cutSymb); leftShift = (cutSeparator + cutSymb).Length; }
        //                newText.Append(data.Text.Substring(leftIndex, rightIndex - leftIndex));
        //                newText.Append(rightIndex == textLength ? "" : cutSymb);

        //                #region modify indexes for remaining matches
        //                if (isStart)
        //                {
        //                    if (isReversedType)
        //                    {
        //                        firstLeftIndex = leftIndex - leftShift;
        //                        firstLeftShift = leftShift;
        //                    }
        //                    isStart = false;
        //                }
        //                if (!isReversedType)
        //                {
        //                    firstLeftIndex = data.MatchStart[i] - (leftShift + prevCutTextIndex + leftCutIndex);
        //                    firstLeftShift = leftShift;
        //                }
        //                #endregion

        //                // update start index
        //                data.MatchStart[i] = leftShift;
        //                data.MatchStart[i] += prevCutTextIndex;
        //                data.MatchStart[i] += leftCutIndex;

        //                // update cut text indexes
        //                prevTextIndex = rightIndex;
        //                prevCutTextIndex = newText.Length;

        //                i = (isReversedType ? i - 1 : i + 1);
        //            }

        //            // update DetailsData text
        //            data.Text = newText.ToString();

        //            // update MatchStart for all indexes not in cut range
        //            if (dataMatchesCount < data.MatchStart.Count)
        //                for (int j = dataMatchesCount; j < data.MatchStart.Count; j++)
        //                {
        //                    data.MatchStart[j] = data.MatchStart[j] - firstLeftIndex;
        //                    if (data.MatchStart[j] < 0)
        //                    {
        //                        data.MatchLength[j] = data.MatchLength[j] + data.MatchStart[j] - firstLeftShift;
        //                        data.MatchStart[j] = 0;
        //                        if (data.MatchLength[j] < 0) data.MatchLength[j] = 0;
        //                        else data.MatchLength[j] = data.MatchLength[j] + firstLeftShift;
        //                    }
        //                    if (data.MatchStart[j] >= data.Text.Length)
        //                        data.MatchStart[j] = data.Text.Length;
        //                    if (data.MatchStart[j] + data.MatchLength[j] > data.Text.Length)
        //                        data.MatchLength[j] = data.Text.Length - data.MatchStart[j];
        //                }
        //        }
        //    }
        //}
        #endregion
    }
}
