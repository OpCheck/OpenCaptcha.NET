using System;

namespace OpenCaptcha
{
	public class CaptchaRandomNumberGenerator
	{
		public CaptchaRandomNumberGenerator ()
		{
			_Random = new Random();
		}
		
		
		public int Next (int Max)
		{
			return _Random.Next(0, Max);
		}
		
		
		private Random _Random;
	}
}
