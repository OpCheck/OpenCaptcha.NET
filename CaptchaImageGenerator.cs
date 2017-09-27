using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenCaptcha
{
	public class CaptchaImageGenerator
	{
		static CaptchaImageGenerator()
		{
			//
			// CREATE THE LIST OF FONTS THAT WE WILL USE AND HOW THEY ARE RENDERED.
			//
			List<CaptchaFontConfig> FontConfigList = new List<CaptchaFontConfig>();
			
			{
				CaptchaFontConfig CurrentFontConfig = new CaptchaFontConfig();
				CurrentFontConfig.Spacing = 3f;
				CurrentFontConfig.MinSize = 24;
				CurrentFontConfig.MaxSize = 29;
				CurrentFontConfig.FontFamily = FontFamily.GenericMonospace;
				FontConfigList.Add(CurrentFontConfig);
			}

			{
				CaptchaFontConfig CurrentFontConfig = new CaptchaFontConfig();
				CurrentFontConfig.Spacing = 3f;
				CurrentFontConfig.MinSize = 24;
				CurrentFontConfig.MaxSize = 29;
				CurrentFontConfig.FontFamily = FontFamily.GenericSansSerif;
				FontConfigList.Add(CurrentFontConfig);
			}

			{
				CaptchaFontConfig CurrentFontConfig = new CaptchaFontConfig();
				CurrentFontConfig.Spacing = 3f;
				CurrentFontConfig.MinSize = 24;
				CurrentFontConfig.MaxSize = 29;
				CurrentFontConfig.FontFamily = FontFamily.GenericSerif;
				FontConfigList.Add(CurrentFontConfig);
			}

			_Fonts = FontConfigList.ToArray();
		}
		
		
		public CaptchaImageGenerator ()
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
			
			//
			//
			//
			_CaptchaRandomTextGenerator = new CaptchaRandomTextGenerator();
			_CaptchaBitmapArgs = new CaptchaBitmapArgs();
		}


		/// <summary>
		/// Creates the captcha bitmap.
		/// </summary>
		public Bitmap GenerateRandomBitmap ()
		{
			return CreateBitmapFromString(_CaptchaRandomTextGenerator.GenerateText());
		}


		/// <summary>
		/// Creates the captcha image.
		/// </summary>
		public Bitmap CreateBitmapFromString (string text)
		{
			var image = ImageAllocate();
			this.WriteText(image, text);
			this.WriteLine(image);
			this.WaveImage(image);
			return new Bitmap(image, Width, Height);
		}


		/// <summary>
		/// Allocate the image.
		/// </summary>
		protected virtual Bitmap ImageAllocate()
		{
			var imageWidth = this.Width * this.Scale;
			var imageHeight = this.Height * this.Scale;
			var image = new Bitmap(imageWidth, imageHeight);
			using (Graphics graphics = Graphics.FromImage(image))
			{
				using (SolidBrush sb = new SolidBrush(this.BackColor))
				{
					graphics.FillRectangle(sb, 0, 0, imageWidth, imageHeight);
				}
			}

			return image;
		}


		/// <summary>
		/// Writes the text.
		/// </summary>
		/// <param name="image">The image.</param>
		/// <param name="text">The text.</param>
		protected virtual void WriteText (Bitmap image, String text)
		{
			var fontConfig = this.FontConfig;
			if (fontConfig == null)
			{
				var fontIndex = _random.Next(0, _Fonts.Length - 1);
				fontConfig = _Fonts[fontIndex];
			}

			var length = text.Length;
			var lettersMissing = this.MaxWordLength - length;
			var fontSizefactor = 1 + (lettersMissing * 0.09F);
			using (var graphics = Graphics.FromImage(image))
			using (var shadowBrush = new SolidBrush(this.ShadowColor))
			using (var foreBrush = new SolidBrush(this.ForeColor))
			{
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				var fontSize = fontConfig.MaxSize * this.Scale * fontSizefactor;
				var font = new Font(fontConfig.FontFamily, fontSize);
				var textSize = graphics.MeasureString(text, font);
				var x = (image.Width - textSize.Width - fontConfig.Spacing * text.Length) / 2;
				for (int i = 0; i < length; i++)
				{
					fontSize = _random.Next(fontConfig.MinSize, fontConfig.MaxSize) * this.Scale * fontSizefactor;
					font = new Font(fontConfig.FontFamily, fontSize);
					var letter = text.Substring(i, 1);
					var letterSize = graphics.MeasureString(letter, font);
					var y = (image.Height - letterSize.Height) / 2;
					var matrix = new Matrix();
					var degree = _random.Next(-this.MaxRotation, this.MaxRotation);
					matrix.RotateAt(degree, new PointF(x + letterSize.Width / 2, image.Height / 2), MatrixOrder.Append);
					graphics.Transform = matrix;
					graphics.DrawString(letter, font, foreBrush, x + Scale, y + Scale);
					graphics.DrawString(letter, font, foreBrush, x, y);
					x += Convert.ToInt32(letterSize.Width + (fontConfig.Spacing - 12) * this.Scale);
					textFinalX = x;
				}
			}
		}


		/// <summary>
		/// Writes the line.
		/// </summary>
		/// <param name="image">The image.</param>
		protected virtual void WriteLine(Bitmap image)
		{
			if (this.LineWidth > 0)
			{
				var x1 = this.Width * this.Scale * 0.15F;
				var x2 = this.textFinalX;
				var y1 = _random.Next(this.Height * this.Scale * 40, this.Height * this.Scale * 65) / 100F;
				var y2 = _random.Next(this.Height * this.Scale * 40, this.Height * this.Scale * 65) / 100F;
				var width = this.LineWidth / 2 * this.Scale;
				using (var graphics = Graphics.FromImage(image))
				using (var linePen = new Pen(ForeColor, 1))
				{
					for (var i = width * -1; i <= width; i++)
					{
						graphics.DrawLine(linePen, x1, y1 + i, x2, y2 + i);
					}
				}
			}
		}


		/// <summary>
		/// Wave the image.
		/// </summary>
		/// <param name="image">The image.</param>
		protected virtual void WaveImage (Bitmap image)
		{
			var imageLength = image.Width * image.Height;
			var imageWidth = image.Width;
			var imageHeight = image.Height;
			var backColorArgb = this.BackColor.ToArgb();
			BitmapData imageData = image.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
			try
			{
				unsafe
				{
					var imageDataPtr = (Int32*)imageData.Scan0;
					{
						//wave x
						var xp = this.Scale * this.Xperiod * _random.Next(1, 3);
						var k = _random.Next(0, 100);
						for (var i = 1; i < imageWidth; i++)
						{
							var amplitude = unchecked(Convert.ToInt32(Math.Sin(k + i * 1.00 / xp) * this.Scale * this.Xamplitude));
							for (int j = 0; j < imageHeight; j++)
							{
								if (j + amplitude >= 0 && j + amplitude < imageHeight)
								{
									imageDataPtr[(j * imageWidth + i - 1)] = imageDataPtr[((j + amplitude) * imageWidth + i)];
								}
								else
								{
									imageDataPtr[(j * imageWidth + i - 1)] = backColorArgb;
								}
							}
						}
					}
					{
						//wave y
						var yp = this.Scale * this.Yperiod * _random.Next(1, 2);
						var k = _random.Next(0, 100);
						for (var i = 1; i < imageHeight; i++)
						{
							var amplitude = unchecked(Convert.ToInt32(Math.Sin(k + i * 1.00 / yp) * this.Scale * this.Yamplitude));
							for (int j = 0; j < imageWidth; j++)
							{
								if (j + amplitude >= 0 && j + amplitude < imageWidth)
								{
									imageDataPtr[((i - 1) * imageWidth + j)] = imageDataPtr[(i * imageWidth + j + amplitude)];
								}
								else
								{
									imageDataPtr[((i - 1) * imageWidth + j)] = backColorArgb;
								}
							}
						}
					}
				}
			}
			catch
			{
			}
			finally
			{
				image.UnlockBits(imageData);
			}
		}


		/// <summary>
		/// The list of font configuration objects used to generate the image.
		/// </summary>
		private static CaptchaFontConfig[] _Fonts;


		/// <summary>
		/// Gets or sets the width of the captcha image.
		/// <remarks>
		/// the default value is 200
		/// </remarks>
		/// </summary>
		public Int32 Width { get; set; }


		/// <summary>
		/// Gets or sets the height of the captcha image.
		/// <remarks>
		/// the default value is 70
		/// </remarks>
		/// </summary>
		public Int32 Height { get; set; }


		/// <summary>
		/// Gets or sets the minimum length of the word (for non-dictionary _random text generation).
		/// <remarks>
		/// the default value is 5
		/// </remarks>
		/// </summary>
		public Int32 MinWordLength { get; set; }


		/// <summary>
		/// Gets or sets the maximum length of the word (for non-dictionary _random text generation).
		/// </summary>
		/// <remarks>
		/// the default value is 5
		/// </remarks>
		public Int32 MaxWordLength { get; set; }


		/// <summary>
		/// Gets or sets the color of the background.
		/// </summary>
		/// <value>The color of the background.</value>
		public Color BackColor { get; set; }


		/// <summary>
		/// Gets or sets the color of the foreground.
		/// </summary>
		/// <value>The color of the foreground.</value>
		public Color ForeColor { get; set; }


		/// <summary>
		/// Gets or sets the color of the shadow.
		/// </summary>
		/// <value>The color of the shadow.</value>
		public Color ShadowColor { get; set; }


		/// <summary>
		/// Gets or sets a value indicating whether show captcha text shadow.
		/// </summary>
		/// <value><c>true</c> if show captcha text shadow; otherwise, <c>false</c>.</value>
		public Boolean ShowShadow { get; set; }


		/// <summary>
		/// Gets or sets the width of the horizontal lineNum through the text.
		/// </summary>
		/// <value>The width of the horizontal lineNum through the text.</value>
		public Int32 LineWidth { get; set; }


		/// <summary>
		/// Gets or sets the internal image size factor (for better image quality).
		/// <para>1: low, 2: medium, 3: high</para>
		/// <remarks>
		/// the default value is 3
		/// </remarks>
		/// </summary>
		/// <value>The scale.</value>
		public Int32 Scale { get; set; }


		/// <summary>
		/// Gets or sets the maximum letter rotation clockwise.
		/// <remarks>
		/// the default value is 8
		/// </remarks>
		/// </summary>
		/// <value>The maximum letter rotation clockwise.</value>
		public Int32 MaxRotation { get; set; }


		/// <summary>
		/// Gets or sets the font configuration.
		/// </summary>
		/// <value>The font configuration.</value>
		public CaptchaFontConfig FontConfig { get; set; }


		/// <summary>
		/// Gets or sets the wave period of y axis.
		/// <remarks>
		/// the default value is 12
		/// </remarks>
		/// </summary>
		/// <value>The wave period of y axis.</value>
		public Int32 Yperiod { get; set; }


		/// <summary>
		/// Gets or sets the wave amplitude of y axis.
		/// <remarks>
		/// the default value is 14
		/// </remarks>
		/// </summary>
		/// <value>The wave amplitude of y axis.</value>
		public Int32 Yamplitude { get; set; }


		/// <summary>
		/// Gets or sets the wave period of x axis.
		/// <remarks>
		/// the default value is 11
		/// </remarks>
		/// </summary>
		/// <value>The wave period of x axis.</value>
		public Int32 Xperiod { get; set; }


		/// <summary>
		/// Gets or sets the wave amplitude of x axis.
		/// <remarks>
		/// the default value is 5
		/// </remarks>
		/// </summary>
		/// <value>The wave amplitude of x axis.</value>
		public Int32 Xamplitude { get; set; }


		/// <summary>
		/// Sets the random text generator.
		/// </summary>
		public CaptchaRandomTextGenerator CaptchaRandomTextGenerator
		{
			set
			{
				_CaptchaRandomTextGenerator = value;
			}
		}


		public CaptchaBitmapArgs CaptchaBitmapArgs
		{
			set
			{
				_CaptchaBitmapArgs = value;
			}
		}


		//
		// INPUT FIELDS.
		//
		private CaptchaRandomTextGenerator _CaptchaRandomTextGenerator;
		private CaptchaBitmapArgs _CaptchaBitmapArgs;


		private Random _random = new Random();
		private float textFinalX = 0;
	}
}
