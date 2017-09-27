using System;
using System.Drawing;

namespace OpenCaptcha
{
	public class CaptchaBitmapArgs
	{
		public CaptchaBitmapArgs ()
		{
			Width = 200;
			Height = 70;
			MinWordLength = 5;
			MaxWordLength = 8;
			BackColor = Color.White;
			ForeColor = Color.Black;
			LineWidth = 0;
			Scale = 3;
			MaxRotation = 8;
			Yperiod = 12;
			Yamplitude = 14;
			Xperiod = 11;
			Xamplitude = 5;
		}
		
		public int Width;
		public int Height;
		public Int32 MinWordLength;
		public Int32 MaxWordLength;
		public Color ForeColor;
		public Color BackColor;
		public Color ShadowColor;
		public Boolean ShowShadow;
		public Int32 LineWidth;
		public Int32 Scale;
		public Int32 MaxRotation;
		public CaptchaFontConfig FontConfig;
		public Int32 Yperiod;
		public Int32 Yamplitude;
		public Int32 Xperiod;
		public Int32 Xamplitude;
	}
}
