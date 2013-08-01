using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewUserAdds.Classes
{
    public class StatusOverlayProgressBar : ProgressBar
    {
        #region Overlay Variables
        /// <summary>
        /// [Optional] Message to overlay on the status bar. Defaults to "[Value]/[Maximum]"
        /// </summary>
        private string _message = null;
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        /// <summary>
        /// [Optional] Typeface for the Overlay - set with String or new FontFamily. Defaults to Arial Unicode MS
        /// </summary>
        private FontFamily _fontFamily = null;
        public FontFamily Font_Face
        {
            get
            {
                return _fontFamily ?? new FontFamily("Arial Unicode MS");
            }

            set
            {
                if (value.GetType() == typeof(FontFamily))
                    _fontFamily = value;
                else
                {
                    _fontFamily = FontFamily.Families.ToList<FontFamily>().Find(f => String.Compare(f.Name, value.ToString(), true) == 0);
                    if (_fontFamily == null) throw new System.ArgumentNullException("Named FontFamily doesn't exist");
                }
            }
        }

        /// <summary>
        /// [Optional] Size of the Overlay font. Defaults to 8
        /// </summary>
        private int? _fontSize = null;
        public int Font_Size
        {
            get 
            {
                return _fontSize ?? 8;
            }
            set { _fontSize = value; }
        }

        /// <summary>
        /// [Optional] Style of the Overlay font. Defaults to Regular
        /// </summary>
        private FontStyle? _fontStyle = null;
        public FontStyle Font_Style
        {
            get 
            {
                return _fontStyle ?? FontStyle.Regular;
            }

            set { _fontStyle = value; }
        }

        /// <summary>
        /// [Optional] Font to use for the Overlay. Defaults to values set in Font_Face, Font_Size, and Font_Style
        /// </summary>
        private Font _font = null;
        public Font Overlay_Font
        {
            get
            {
                return _font ?? new Font(Font_Face, Font_Size, Font_Style);
            }
            set { _font = value; }
        }

        /// <summary>
        /// [Optional] Color for the Overlay font. Defaults to black.
        /// </summary>
        private Brush _fontColor = null;
        public Brush Font_Color
        {
            get
            {
                return _fontColor ?? Brushes.Black;
            }

            set { _fontColor = value; }
        }
        #endregion

        public StatusOverlayProgressBar()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public StatusOverlayProgressBar(string message)
            : this()
        {
            _message = message;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = this.ClientRectangle;
            Graphics gfx = e.Graphics;

            // Fill in the progress
            ProgressBarRenderer.DrawHorizontalBar(gfx, rect);
            rect.Inflate(-2, -2);
            if (this.Value > 0)
            {
                // X & Y are the coords for the upper left corner. Height is the height.
                // Width of the status part is the percentage complete (value/max) applied to the available width.
                // i.e. if the statis is 20% done, fill 20% of the width of the bar.
                Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round(((float)this.Value / this.Maximum) * rect.Width), rect.Height);
                ProgressBarRenderer.DrawHorizontalChunks(gfx, clip);
            }


            /* ?? is a "Null-Coalescing" operator.
             * Ex:
             * int j = i ?? 8;
             * Sets j == to the value of i, if i is not null. Else set j to 8.
             */
            string msg = Message ?? this.Value.ToString() + '/' + this.Maximum.ToString();
            SizeF strLen = gfx.MeasureString(msg, Overlay_Font);


            //Location is the upper-left corner of the Message rectangle, as drawn with the defined font (graphics always start in the upper left corner)
            // Width => 1/2 the bar width - 1/2 the overlay width. That offsets the overlay so the centerlines of both the overlay and the bar align - centering the text.
            Point location = new Point((int)((rect.Width / 2) - (strLen.Width / 2)), (int)((rect.Height / 2) - (strLen.Height / 2)) + 3);
            gfx.DrawString(msg, Overlay_Font, Font_Color, location);
        }

    }
}
