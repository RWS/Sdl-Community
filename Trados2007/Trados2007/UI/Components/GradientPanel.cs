// ---------------------------------
// <copyright file="GradientPanel.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-10-14</date>
// ---------------------------------
namespace Sdl.Community.Trados2007.UI
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    /// <summary>
    /// The gradient panel.
    /// </summary>
    public partial class GradientPanel : Panel
    {
        #region Constants and Fields

        /// <summary>
        /// The panel category.
        /// </summary>
        public const string PanelCategory = "GradientPanel";

        /// <summary>
        /// The _border color.
        /// </summary>
        private Color _borderColor = Color.Gray;

        /// <summary>
        /// The _border width.
        /// </summary>
        private int _borderWidth = 1;

        /// <summary>
        /// The _gradient end color.
        /// </summary>
        private Color _gradientEndColor = Color.Gray;

        /// <summary>
        /// The _gradient start color.
        /// </summary>
        private Color _gradientStartColor = Color.White;

        /// <summary>
        /// The _image.
        /// </summary>
        private Image _image;

        /// <summary>
        /// The _image location.
        /// </summary>
        private Point _imageLocation = new Point(4, 4);

        /// <summary>
        /// The _round corner radius.
        /// </summary>
        private int _roundCornerRadius = 4;

        /// <summary>
        /// The _shadow off set.
        /// </summary>
        private int _shadowOffSet = 5;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientPanel"/> class.
        /// </summary>
        public GradientPanel()
        {
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets BorderColor.
        /// </summary>
        [Browsable(true)]
        [Category(PanelCategory)]
        [DefaultValue("Color.Gray")]
        public Color BorderColor
        {
            get
            {
                return this._borderColor;
            }

            set
            {
                this._borderColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets BorderWidth.
        /// </summary>
        [Browsable(true)]
        [Category(PanelCategory)]
        [DefaultValue(1)]
        public int BorderWidth
        {
            get
            {
                return this._borderWidth;
            }

            set
            {
                this._borderWidth = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets GradientEndColor.
        /// </summary>
        [Browsable(true)]
        [Category(PanelCategory)]
        [DefaultValue("Color.Gray")]
        public Color GradientEndColor
        {
            get
            {
                return this._gradientEndColor;
            }

            set
            {
                this._gradientEndColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets GradientStartColor.
        /// </summary>
        [Browsable(true)]
        [Category(PanelCategory)]
        [DefaultValue("Color.White")]
        public Color GradientStartColor
        {
            get
            {
                return this._gradientStartColor;
            }

            set
            {
                this._gradientStartColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets Image.
        /// </summary>
        [Browsable(true)]
        [Category(PanelCategory)]
        public Image Image
        {
            get
            {
                return this._image;
            }

            set
            {
                this._image = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets ImageLocation.
        /// </summary>
        [Browsable(true)]
        [Category(PanelCategory)]
        [DefaultValue("4,4")]
        public Point ImageLocation
        {
            get
            {
                return this._imageLocation;
            }

            set
            {
                this._imageLocation = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets RoundCornerRadius.
        /// </summary>
        [Browsable(true)]
        [Category(PanelCategory)]
        [DefaultValue(4)]
        public int RoundCornerRadius
        {
            get
            {
                return this._roundCornerRadius;
            }

            set
            {
                this._roundCornerRadius = Math.Abs(value);
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets ShadowOffSet.
        /// </summary>
        [Browsable(true)]
        [Category(PanelCategory)]
        [DefaultValue(5)]
        public int ShadowOffSet
        {
            get
            {
                return this._shadowOffSet;
            }

            set
            {
                this._shadowOffSet = Math.Abs(value);
                this.Invalidate();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The get round path.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <param name="depth">
        /// The depth.
        /// </param>
        /// <returns>
        /// </returns>
        public static GraphicsPath GetRoundPath(Rectangle r, int depth)
        {
            var graphPath = new GraphicsPath();

            graphPath.AddArc(r.X, r.Y, depth, depth, 180, 90);
            graphPath.AddArc(r.X + r.Width - depth, r.Y, depth, depth, 270, 90);
            graphPath.AddArc(r.X + r.Width - depth, r.Y + r.Height - depth, depth, depth, 0, 90);
            graphPath.AddArc(r.X, r.Y + r.Height - depth, depth, depth, 90, 90);
            graphPath.AddLine(r.X, r.Y + r.Height - depth, r.X, r.Y + depth / 2);

            return graphPath;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on paint background.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            int tmpShadowOffSet = Math.Min(Math.Min(this._shadowOffSet, this.Width - 2), this.Height - 2);
            int tmpSoundCornerRadius = Math.Min(Math.Min(this._roundCornerRadius, this.Width - 2), this.Height - 2);
            if (this.Width > 1 && this.Height > 1)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                var rect = new Rectangle(0, 0, this.Width - tmpShadowOffSet - 1, this.Height - tmpShadowOffSet - 1);
                var rectShadow = new Rectangle(
                    tmpShadowOffSet, 
                    tmpShadowOffSet, 
                    this.Width - tmpShadowOffSet - 1, 
                    this.Height - tmpShadowOffSet - 1);

                GraphicsPath graphPathShadow = GetRoundPath(rectShadow, tmpSoundCornerRadius);
                GraphicsPath graphPath = GetRoundPath(rect, tmpSoundCornerRadius);

                if (tmpSoundCornerRadius > 0)
                {
                    using (var gBrush = new PathGradientBrush(graphPathShadow))
                    {
                        gBrush.WrapMode = WrapMode.Clamp;
                        var colorBlend = new ColorBlend(3);
                        colorBlend.Colors = new[]
                            {
                                Color.Transparent, Color.FromArgb(180, Color.DimGray), Color.FromArgb(180, Color.DimGray)
                            };

                        colorBlend.Positions = new[] { 0f, .1f, 1f };

                        gBrush.InterpolationColors = colorBlend;
                        e.Graphics.FillPath(gBrush, graphPathShadow);
                    }
                }

                // Draw backgroup
                using (
                    var brush = new LinearGradientBrush(
                        rect, this._gradientStartColor, this._gradientEndColor, LinearGradientMode.BackwardDiagonal))
                {
                    e.Graphics.FillPath(brush, graphPath);

                    // TODO: Can This Pen Object Be Made Static If It Never Changes (For Preventing leaks)
                    e.Graphics.DrawPath(new Pen(Color.FromArgb(180, this._borderColor), this._borderWidth), graphPath);

                    // Draw Image
                    if (this._image != null)
                    {
                        e.Graphics.DrawImageUnscaled(this._image, this._imageLocation);
                    }
                }
            }
        }

        #endregion
    }
}