using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Ejercicio2.Repositorio.Repositories.Json
{
    public abstract class JsonRepositoryBase<T, TKey>
        where T : class
    {
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        protected JsonRepositoryBase(string filePath)
        {
            FilePath = filePath;
            Items = Load(filePath);
        }

        protected string FilePath { get; }
        protected List<T> Items { get; }

        private static List<T> Load(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            return JsonSerializer.Deserialize<List<T>>(stream, SerializerOptions) ?? new List<T>();
        }

        protected void SaveChanges()
        {
            var contents = JsonSerializer.Serialize(Items, SerializerOptions);
            File.WriteAllText(FilePath, contents);
        }

        protected Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var contents = JsonSerializer.Serialize(Items, SerializerOptions);
            return File.WriteAllTextAsync(FilePath, contents, cancellationToken);
        }

        protected static List<TValue> LoadFromFile<TValue>(string path)
        {
            using var stream = File.OpenRead(path);
            return JsonSerializer.Deserialize<List<TValue>>(stream, SerializerOptions) ?? new List<TValue>();
        }

        public virtual Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<T>>(Items);

        public abstract Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
        public abstract void Insert(T entity);
        public abstract Task InsertAsync(T entity, CancellationToken cancellationToken = default);
        public abstract void Update(T entity);
        public abstract Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        public abstract void Delete(TKey id);
        public abstract Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);
        public abstract bool Exists(TKey id);
        public abstract Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);
    }
}
