using System;
using System.Text;

namespace OpenCaptcha
{
	public class CaptchaRandomTextGenerator
	{
		static CaptchaRandomTextGenerator()
		{
			//
			// DEFINE THE SET OF ALL POSSIBLE CHARACTERS FOR THE CAPTCHA.
			//
			_DefaultAlphabet = "123456789";
		}
		
		
		public CaptchaRandomTextGenerator ()
		{
			_CaptchaRandomNumberGenerator = new CaptchaRandomNumberGenerator();
			_Alphabet = _DefaultAlphabet;
			_Length = 5;
		}
		
		
		public string GenerateText ()
		{
			return GenerateTextWithLength(_Length);
		}


		public string GenerateTextWithLength (int Length)
		{
			StringBuilder CaptchaTextBuilder = new StringBuilder();
			
			for (int i = 0; i < Length; i++)
				CaptchaTextBuilder.Append(_Alphabet[_CaptchaRandomNumberGenerator.Next(_Alphabet.Length)]);
				
			return CaptchaTextBuilder.ToString();
		}


		public CaptchaRandomNumberGenerator CaptchaRandomNumberGenerator
		{
			set
			{
				_CaptchaRandomNumberGenerator = value;
			}
		}


		public int Length
		{
			set
			{
				_Length = value;
			}
		}


		public string Alphabet
		{
			set
			{
				_Alphabet = value;
			}
		}
		
		
		//
		// INPUT FIELDS.
		//
		private CaptchaRandomNumberGenerator _CaptchaRandomNumberGenerator;
		private int _Length;
		private string _Alphabet;
		
		
		/// <summary>
		/// The set of all possible characters that will appear in the image.
		/// </summary>
		private static string _DefaultAlphabet;
	}
}
