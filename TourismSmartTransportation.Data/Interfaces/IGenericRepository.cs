using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Data.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        DbSet<TEntity> Query();

        Task<TEntity> GetById(Guid id);

        Task Add(TEntity entity);

        void Update(TEntity entity);

        Task Remove(Guid id);
    }
}