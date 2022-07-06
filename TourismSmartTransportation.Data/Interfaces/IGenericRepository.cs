using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Data.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        DbSet<TEntity> Query();

        IQueryable<TEntity> FindAsNoTracking();

        Task<TEntity> GetById(Guid id);

        Task Add(TEntity entity);

        void Update(TEntity entity);

        void UpdateWithMultipleKey(TEntity entity);

        Task Remove(Guid id);

        void Remove(TEntity entity);
    }
}