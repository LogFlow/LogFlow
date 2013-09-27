using System;
using System.IO;
using BinaryRage;
using Newtonsoft.Json;

namespace LogFlow
{
	public class StateStorage
	{
		public static string GetDbPath()
		{
			return  Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StateData");
		}

		public static void Insert<T>(string key, T objectToInsert)
		{
			var jsonString = JsonConvert.SerializeObject(objectToInsert);
			DB<string>.Insert(key, jsonString, GetDbPath());
		}

		public static T Get<T>(string key)
		{
			try
			{
				Console.WriteLine("AppDomain" + AppDomain.CurrentDomain.BaseDirectory);
				Console.WriteLine("Get started for " + key + " with path " + GetDbPath());
				var jsonString = DB<string>.Get(key, GetDbPath());

				if(string.IsNullOrWhiteSpace(jsonString))
					return default(T);

				return JsonConvert.DeserializeObject<T>(jsonString);
			}
			catch(Exception)
			{
				return default(T);
			}
		}
		
	}
}
