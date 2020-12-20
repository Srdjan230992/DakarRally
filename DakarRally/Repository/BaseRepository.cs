using DakarRally.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DakarRally.Repository
{
    /// <summary>
    /// BaseRepository class.
    /// </summary>
    /// <typeparam name="TEntity">Generic entity type.</typeparam>
    class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        #region Fields

        /// <summary>
        /// Database context.
        /// </summary>
        internal VehicleDbContext context;

        /// <summary>
        /// Generic database set.
        /// </summary>
        internal Microsoft.EntityFrameworkCore.DbSet<TEntity> dbSet;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository"/> class.
        /// </summary>
        public BaseRepository(VehicleDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.ToList();
        }

        /// <inheritdoc/>
        public virtual bool All(
           Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = dbSet;
            bool isFound = false;

            if (filter != null)
            {
                isFound = query.All(filter);
            }

            return isFound;
        }

        /// <inheritdoc/>
        public virtual bool Any(
          Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = dbSet;
            bool isFound = false;

            if (filter != null)
            {
                isFound = query.Any(filter);
            }

            return isFound;
        }

        /// <inheritdoc/>
        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        /// <inheritdoc/>
        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        /// <inheritdoc/>
        public virtual void Delete(TEntity entityToDelete)
        {
            dbSet.Remove(entityToDelete);
        }

        /// <inheritdoc/>
        /// <summary>
        /// Update existing entity.
        /// </summary>
        /// <param name="entityToUpdate">Entity to update.</param>
        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }

        #endregion
    }
}