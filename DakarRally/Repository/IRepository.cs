using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DakarRally.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Find desired data.
        /// </summary>
        /// <param name="filter">Filter conditions.</param>
        /// <returns>Filtered data.</returns>
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null);

        /// <summary>
        /// Find desired data.
        /// </summary>
        /// <param name="filter">Filter conditions.</param>
        /// <returns>Filtered data.</returns>
        bool All(
           Expression<Func<TEntity, bool>> filter = null);

        /// <summary>
        /// Find desired data.
        /// </summary>
        /// <param name="filter">Filter conditions.</param>
        /// <returns>Filtered data.</returns>
        bool Any(
          Expression<Func<TEntity, bool>> filter = null);

        /// <summary>
        /// Get entity by id.
        /// </summary>
        /// <param name="id">Entity id.</param>
        /// <returns>Desired entity.</returns>
        TEntity GetByID(object id);

        /// <summary>
        /// Insert entity into database.
        /// </summary>
        /// <param name="entity">Entity.</param>
        void Insert(TEntity entity);

        /// <summary>
        /// Delete entity from database.
        /// </summary>
        /// <param name="entityToDelete">Entitu to delete.</param>
        void Update(TEntity entityToUpdate);

        /// <summary>
        /// Update existing entity.
        /// </summary>
        /// <param name="entityToUpdate">Entity to update.</param>
        void Delete(TEntity entityToUpdate);
    }
}