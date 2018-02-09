﻿namespace HistoCoin.Server.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    
    public class Cache
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int ReportTo { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }

    public class CacheService : ICacheService
    {
        private List<Cache> _cache;
        private int _newId;

        public CacheService()
        {
            this._cache = JsonConvert.DeserializeObject<List<Cache>>(this.GetEmbeddedResource("employees.json"));
            this._newId = _cache.Count;
        }

        public IList<Cache> GetAll() => _cache;

        public Cache GetById(int id) => _cache.FirstOrDefault(i => i.Id == id);

        public int Add(Cache record)
        {
            record.Id = ++_newId;
            _cache.Add(record);
            return record.Id;
        }

        public void Update(Cache record)
        {
            var idx = _cache.FindIndex(i => i.Id == record.Id);
            if (idx >= 0)
                _cache[idx] = record;
        }

        public void Delete(int id) => _cache.Remove(_cache.FirstOrDefault(i => i.Id == id));

        private string GetEmbeddedResource(string resourceName)
        {
            var assembly = GetType().Assembly;
            var name = assembly.GetManifestResourceNames()
                .Where(i => i.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (string.IsNullOrEmpty(name))
            {
                throw new FileNotFoundException();
            }

            using (var reader = new StreamReader(assembly.GetManifestResourceStream(name), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}