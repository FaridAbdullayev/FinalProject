using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext _context;
        public Repository(AppDbContext context)
        {
            _context = context;
        }
        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            foreach (var item in includes)
                query = query.Include(item);

            return query.Where(predicate);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            foreach (var item in includes)
                query = query.Include(item);


            return query.FirstOrDefault(predicate);
        }
        public int Save()
        {
            return _context.SaveChanges();
        }
        public bool Exists(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            foreach (var item in includes)
                query = query.Include(item);

            return query.Any(predicate);
        }

    }
}
