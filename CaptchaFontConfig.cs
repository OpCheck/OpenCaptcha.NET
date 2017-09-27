using System;
using System.Drawing;

namespace OpenCaptcha
{
    /// <summary>
    /// class font configuration.
    /// </summary>
    public class CaptchaFontConfig
    {
		/// <summary>
		/// The font family.
		/// </summary>
		public FontFamily FontFamily;


		/// <summary>
		/// The relative pixel space between characters.
		/// </summary>
		public float Spacing;


		/// <summary>
		/// The minimum size to use for this font.
		/// </summary>
		public int MinSize;


		/// <summary>
		/// The maximum font size to use for this font.
		/// </summary>
		public int MaxSize;
    }
}
