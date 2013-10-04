using System.Configuration;

namespace LogFlow
{
	internal static class Config
	{
		public static bool EnableNancyHealthModule
		{
			get { 
				return ConfigurationManager.AppSettings["EnableNancyHealthModule"].IsTrue();
			}
		}

		public static string NancyHostUrl
		{
			get { return ConfigurationManager.AppSettings["NancyHostUrl"] ?? "http://localhost:1337"; }
		}

		private static bool IsTrue(this string str)
		{
			if (!string.IsNullOrWhiteSpace(str))
			{
				return str.Equals("1") || str.ToLower().Equals("true");
			}

			return false;
		}
	}
}
