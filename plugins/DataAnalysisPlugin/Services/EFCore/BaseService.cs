// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataAnalysisPlugin.Domain;

namespace DataAnalysisPlugin.Services.EFCore
{
    //public abstract class BaseService<T> : IService<T>, IDependency where T : BaseEntity, new()
    public abstract class BaseService<T>
        where T : BaseEntity, new()
    {
        #region Fields
        private readonly IRepository<T> _repository;
        #endregion

        #region Ctor
        protected BaseService(IRepository<T> repository)
        {
            _repository = repository;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets all objects from database
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IQueryable<T>> AllAsync()
        {
            return await _repository.AllAsync();
        }

        /// <summary>
        /// Gets objects from database by filter.
        /// </summary>
        /// <param name="predicate">Specified a filter</param>
        /// <returns></returns>
        public virtual async Task<IQueryable<T>> FilterAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.FilterAsync(predicate);
        }

        /// <summary>
        /// Gets objects from database with filtering and paging.
        /// </summary>
        /// <param name="index">Specified the page index.</param>
        /// <param name="size">Specified the page size</param>
        /// <param name="total">Returns the total records count of the filter.</param>
        /// <param name="filter">Specified a filter</param>
        /// <param name="order">Specified a order</param>
        /// <param name="isAsc">Specified ascending or descending</param>
        /// <returns></returns>
        public virtual async Task<PageModel<T>> FilterAsync<TOrder>(int index, int size, Expression<Func<T, bool>> filter, Expression<Func<T, TOrder>> order, bool isAsc = true)
        {
            return await _repository.FilterAsync<TOrder>(index, size, filter, order, isAsc);
        }

        /// <summary>
        /// Gets the object(s) is exists in database by specified filter.
        /// </summary>
        /// <param name="predicate">Specified the filter expression</param>
        /// <returns></returns>
        public virtual async Task<bool> ContainsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.ContainsAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.CountAsync(predicate);
        }

        /// <summary>
        /// Find object by keys.
        /// </summary>
        /// <param name="keys">Specified the search keys.</param>
        /// <returns></returns>
        public virtual async Task<T> FindAsync(params object[] keys)
        {
            return await _repository.FindAsync(keys);
        }

        /// <summary>
        /// Find object by specified expression.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.FindAsync(predicate);
        }

        /// <summary>
        /// Create a new object to database.
        /// </summary>
        /// <param name="t">Specified a new object to create.</param>
        /// <returns></returns>
        public virtual async Task CreateAsync(T t)
        {
            await _repository.CreateAsync(t);

            await _repository.SaveChangesAsync();
        }

        /// <summary>
        /// Delete the object from database.
        /// </summary>
        /// <param name="t">Specified a existing object to delete.</param>
        public virtual async Task DeleteAsync(T t)
        {
            await _repository.DeleteAsync(t);

            await _repository.SaveChangesAsync();
        }

        /// <summary>
        /// Delete objects from database by specified filter expression.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            await _repository.DeleteAsync(predicate);

            await _repository.SaveChangesAsync();
        }

        /// <summary>
        /// Update object changes and save to database.
        /// </summary>
        /// <param name="t">Specified the object to save.</param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(T t)
        {
            await _repository.UpdateAsync(t);

            await _repository.SaveChangesAsync();
        }

        /// <summary>
        /// Select Single Item by specified expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            //return All().FirstOrDefault(expression);
            return await _repository.FirstOrDefaultAsync(expression);
        }
        #endregion

    }
}
