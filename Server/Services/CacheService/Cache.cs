
namespace HistoCoin.Server.Services.CacheService
{
    using System;

    public class Cache<T> : IDisposable
    {
        private T _cache;
        private bool _disposed = false;
        
        public Cache(T store)
        {
            this._cache = store;
            this.Id = this._cache?.GetHashCode() ?? -1;
        }

        public int Id { get; }

        public T Get() => this._cache;

        public void Set(T state) => this._cache = state;

        public void Clear() => this._cache = default;

        public override string ToString()
        {
            return this._cache.ToString();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                this._cache = default;
                this._disposed = true;
            }
        }
        
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }
    }
}
