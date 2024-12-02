using System.Linq.Expressions;
using CatChatServer.Domain.Common.BaseEntity;
using CatChatServer.Domain.Interfaces;
using MongoDB.Driver;

namespace CatChatServer.Infrastructure.Common;

public class BaseRepository<TEntity> : IRepository<TEntity>
    where TEntity : IMongoEntity
{
    protected readonly IMongoCollection<TEntity> _collection;

    protected BaseRepository(IMongoCollection<TEntity> collection)
    {
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));
    }

    public virtual async Task<TEntity> GetByIdAsync(string id) =>
        await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public virtual async Task<IEnumerable<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate) =>
        await _collection.Find(predicate).ToListAsync();

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        entity.LastModifiedDate = DateTime.UtcNow;
        await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
    }

    public virtual async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(e => e.Id == id);
}
