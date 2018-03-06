
namespace HistoCoin.Server.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;

    public class History
    {
        private readonly Dictionary<string, double> _backingDict = new Dictionary<string, double>();

        public History()
        {
        }

        public History(IDictionary<string, double> seedDict)
        {
            foreach (var item in seedDict)
            {
                this._backingDict.Add(item.Key, item.Value);
            }
        }

        public string[] GetDates() => this._backingDict.Keys.ToArray();

        public double[] GetValues() => this._backingDict.Values.ToArray();

        public string[] GetDates(int count) => this._backingDict.Keys.TakeLast(count).ToArray();

        public double[] GetValues(int count) => this._backingDict.Values.TakeLast(count).ToArray();

        public double GetLastEntryValue() => this._backingDict.LastOrDefault().Value;

        public string GetLastEntryTime() => this._backingDict.LastOrDefault().Key;
        
        public History Add(string key, double value)
        {
            if (this._backingDict.ContainsKey(key))
            {
                return this;
            }

            try
            {
                this._backingDict.Add(key, value);
            }
            catch
            {
                // thread collisions for now
            }

            return this;
        }

        public History Remove(string key)
        {
            if (this._backingDict.ContainsKey(key))
            {
                this._backingDict.Remove(key);
            }

            return this;
        }

        public History Clear()
        {
            this._backingDict.Clear();

            return this;
        }
    }
}
