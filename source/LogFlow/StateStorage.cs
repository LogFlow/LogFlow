using System;
using System.IO;
using BinaryRage;

namespace LogFlow
{
	public class StateStorage : IStateStorage
	{
		private readonly string _storageName;

		public StateStorage(string storageName)
		{
			if (string.IsNullOrWhiteSpace(storageName))
			{
				throw new ArgumentNullException("storageName");
			}

			_storageName = storageName;
		}

		private string GetDbPath()
		{
			return  Path.Combine(Config.StoragePath, _storageName);
		}

		public void Insert<T>(string key, T objectToInsert)
		{
			DB<T>.Insert(GenerateUniqueKey(key), objectToInsert, GetDbPath());
			while(DB<T>.Get(key, GetDbPath()) == default(T))
			{
				
			}
		}

		public T Get<T>(string key)
		{
			try
			{
				return DB<T>.Get(GenerateUniqueKey(key), GetDbPath());
			}
			catch (DirectoryNotFoundException)
			{
				return default(T);
			}
		}

		private static string GenerateUniqueKey(string key)
		{
			return Key.GenerateMD5Hash(key) + "_" + Key.CalculateChecksum(key);
		}
	}
}
