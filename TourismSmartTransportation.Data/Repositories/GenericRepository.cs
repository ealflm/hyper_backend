using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TourismSmartTransportation.Data.Context;
using TourismSmartTransportation.Data.Interfaces;

namespace TourismSmartTransportation.Data.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet;
        private readonly tourismsmarttransportationContext _dbContext;

        public GenericRepository(tourismsmarttransportationContext dbContext)
        {
            _dbSet = dbContext.Set<TEntity>();
            _dbContext = dbContext;
        }

        public DbSet<TEntity> Query()
        {
            return _dbSet;
        }

        public IQueryable<TEntity> FindAsNoTracking()
        {
            return _dbContext.Set<TEntity>().AsNoTracking();
        }

        public async Task<TEntity> GetById(Guid id)
        {
            var data = await _dbSet.FindAsync(id);
            return data;
        }

        public async Task Add(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Attach(entity);

            foreach (PropertyInfo prop in entity.GetType().GetProperties())
            {
                if (prop.GetGetMethod().IsVirtual) continue;
                if (prop.Name == "Id") continue;
                if (prop.GetValue(entity, null) != null)
                {
                    _dbContext.Entry(entity).Property(prop.Name).IsModified = true;
                }
            }
        }

        public async Task Remove(Guid id)
        {
            var entity = await GetById(id);
            _dbSet.Remove(entity);
        }
    }
}