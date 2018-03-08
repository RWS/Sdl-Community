// ---------------------------------
// <copyright file="LanguagePairListBox.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-02</date>
// ---------------------------------
namespace Sdl.TranslationStudio.Plugins.Trados2007.UI
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Sdl.Core.Globalization;
    using Sdl.LanguagePlatform.Core;

    /// <summary>
    /// Custom ListBox implementation for displaying Language pairs
    /// </summary>
    public sealed class LanguagePairListBox : ListBox
    {
        /// <summary>
        /// Evristic width of country flag inside the listbox item
        /// </summary>
        private readonly int imageWidth = 25;

        /// <summary>
        /// Evristic width of iso language name string inside the listbox item
        /// </summary>
        private readonly int isoAbbreviationWidth = 40;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguagePairListBox"/> class.
        /// </summary>
        public LanguagePairListBox()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ListBox.DrawItem"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DrawItemEventArgs"/> that contains the event data.</param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            e.DrawBackground();
            e.DrawFocusRectangle();

            Rectangle bounds = e.Bounds;

            try
            {
                LanguagePair pair = this.Items[e.Index] as LanguagePair;

                // get languages (with flaags and names) to display
                Language sourceLang = new Language(pair.SourceCulture);
                Language targetLang = new Language(pair.TargetCulture);

                // draw source flag
                e.Graphics.DrawImage(sourceLang.Image, bounds.Left, bounds.Top);

                // draw source string
                e.Graphics.DrawString(
                    sourceLang.IsoAbbreviation,
                    e.Font,
                    new SolidBrush(e.ForeColor),
                    bounds.Left + this.imageWidth,
                    bounds.Top);

                // draw target flag
                e.Graphics.DrawImage(
                    targetLang.Image,
                    bounds.Left + this.imageWidth + this.isoAbbreviationWidth,
                    bounds.Top);

                // draw target string
                e.Graphics.DrawString(
                    targetLang.IsoAbbreviation,
                    e.Font,
                    new SolidBrush(e.ForeColor),
                    bounds.Left + this.imageWidth + this.isoAbbreviationWidth + this.imageWidth,
                    bounds.Top);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
