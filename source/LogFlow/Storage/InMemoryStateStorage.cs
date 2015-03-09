using System.Collections.Concurrent;

namespace LogFlow.Storage
{
    public class InMemoryStateStorage : IStateStorage
    {
        private readonly ConcurrentDictionary<string, object> _storage;

        public InMemoryStateStorage()
        {
            _storage = new ConcurrentDictionary<string, object>();
        }

        public void Insert<T>(string key, T objectToInsert)
        {
            _storage[key] = objectToInsert;
        }

        public T Get<T>(string key)
        {
            object value;
            if (_storage.TryGetValue(key, out value))
            {
                return (T) value;
            }

            return default (T);
        }
    }
}