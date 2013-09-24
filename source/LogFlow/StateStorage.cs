using System;
using BinaryRage;
using Newtonsoft.Json;

namespace LogFlow
{
	public class StateStorage
	{
		static string _dbPath; 
		public StateStorage()
		{
			_dbPath = AppDomain.CurrentDomain.BaseDirectory + "\appState.db";
		}

		public static void Insert<T>(string key, T objectToInsert)
		{
			var jsonString = JsonConvert.SerializeObject(objectToInsert);
			DB<string>.Insert(key, jsonString, _dbPath);
		}

		public static T Get<T>(string key)
		{
			var jsonString = DB<string>.Get(key, _dbPath);

			if(string.IsNullOrWhiteSpace(jsonString))
				return default(T);

			return JsonConvert.DeserializeObject<T>(jsonString);
		}
		
	}
}
