using System;
using System.IO;
using BinaryRage;

namespace LogFlow
{
	public class StateStorage
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

		private static string GetBaseStoragePath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StateStorage");
		}

		private string GetDbPath()
		{
			return  Path.Combine(GetBaseStoragePath(), _storageName);
		}

		public void Insert<T>(string key, T objectToInsert)
		{
			DB<T>.Insert(GenerateUniqueKey(key), objectToInsert, GetDbPath());
			DB<T>.WaitForCompletion();
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
