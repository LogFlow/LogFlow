namespace LogFlow.Storage
{
	public interface IStateStorage
	{
		void Insert<T>(string key, T objectToInsert);
		T Get<T>(string key);
	}
}