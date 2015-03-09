using System;
using System.IO;
using BinaryRage;

namespace LogFlow.Storage
{
	public class BinaryRangeStateStorage : IStateStorage
	{
		private readonly string _storageName;

		public BinaryRangeStateStorage(string storageName)
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
