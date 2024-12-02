using System.Linq.Expressions;
using CatChatServer.Domain.Common.BaseEntity;

namespace CatChatServer.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity : IMongoEntity
{
    Task<TEntity> GetByIdAsync(string id);
    
    Task<IEnumerable<TEntity>> GetAllAsync();
    
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    
    Task<TEntity> AddAsync(TEntity entity);
    
    Task UpdateAsync(TEntity entity);
    
    Task DeleteAsync(string id);
}
