// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DataAnalysisPlugin.Domain;
using Microsoft.EntityFrameworkCore;

namespace DataAnalysisPlugin.Repositories.EFCore.Core
{
    /// <summary>
    /// 而一个业务中经常涉及到对多张表操作，我们期望连接一次数据库，完成对多张表数据的操作，提高性能
    /// 所以，可增加数据会话层，来统一 SaveChanges()
    /// 数据会话层：就是一个工厂类，负责完成所有数据操作类实例的创建，然后业务层通过数据会话层来获取要操作数据类的实例,所以数据会话层将业务层与数据层解耦。
    /// 在数据会话层中提供一个方法：完成所有数据的保存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseRepository<T> : IDependency, IRepository<T> 
        where T : BaseEntity, new()
    {
        #region Fields

        private readonly DbContext _context;
        private DbSet<T> _entities;

        #endregion

        #region Properties

        /// <summary>
        /// Entities
        /// </summary>
        protected virtual DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }

        #endregion

        #region Ctor

        public BaseRepository(DbContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all objects from database
        /// </summary>
        /// <returns></returns>
        public async Task<IQueryable<T>> AllAsync()
        {
            //return _context.Set<T>().AsQueryable();
            return await Task.Run(() => _context.Set<T>().AsQueryable());
        }

        /// <summary>
        /// Gets objects from database by filter.
        /// </summary>
        /// <param name="predicate">Specified a filter</param>
        /// <returns></returns>
        public virtual async Task<IQueryable<T>> FilterAsync(Expression<Func<T, bool>> predicate)
        {
            //return _context.Set<T>().Where<T>(predicate).AsQueryable<T>();
            return await Task.Run(() => _context.Set<T>().Where(predicate).AsQueryable());
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
            //var skipCount = (index - 1) * size;
            //var resultSet = _context.Set<T>().Where(filter).AsQueryable();
            //total = resultSet.Count();
            //resultSet = isAsc ? resultSet.OrderBy(order) : resultSet.OrderByDescending(order);
            //resultSet = skipCount == 0 ? resultSet.Take(size) : resultSet.Skip(skipCount).Take(size);
            //return resultSet.AsQueryable();

            var page = new PageModel<T>();
            var skipCount = (index - 1) * size;
            var resultSet = _context.Set<T>().Where(filter).AsQueryable();
            page.TotalCount = resultSet.Count();
            resultSet = isAsc ? resultSet.OrderBy(order) : resultSet.OrderByDescending(order);
            resultSet = skipCount == 0 ? resultSet.Take(size) : resultSet.Skip(skipCount).Take(size);
            page.Data = resultSet.AsQueryable();
            return await Task.Run(() => page);
        }

        /// <summary>
        /// Gets the object(s) is exists in database by specified filter.
        /// </summary>
        /// <param name="predicate">Specified the filter expression</param>
        /// <returns></returns>
        public async Task<bool> ContainsAsync(Expression<Func<T, bool>> predicate)
        {
            //return _context.Set<T>().Any(predicate);
            return await _context.Set<T>().AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            //return _context.Set<T>().Count(predicate);
            return await _context.Set<T>().CountAsync(predicate);
        }

        /// <summary>
        /// Find object by keys.
        /// </summary>
        /// <param name="keys">Specified the search keys.</param>
        /// <returns></returns>
        public virtual async Task<T> FindAsync(params object[] keys)
        {
            //return _context.Set<T>().Find(keys);
            return await _context.Set<T>().FindAsync(keys);
        }

        /// <summary>
        /// Find object by specified expression.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            //return _context.Set<T>().FirstOrDefault<T>(predicate);
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Create a new object to database.
        /// </summary>
        /// <param name="t">Specified a new object to create.</param>
        /// <returns></returns>
        public virtual async Task CreateAsync(T t)
        {
            //_context.Set<T>().Add(t);
            await _context.Set<T>().AddAsync(t);

            //_context.SaveChanges();
        }

        /// <summary>
        /// Delete the object from database.
        /// </summary>
        /// <param name="t">Specified a existing object to delete.</param>
        public virtual async Task DeleteAsync(T t)
        {
            //_context.Set<T>().Remove(t);
            await Task.Run(() => _context.Set<T>().Remove(t));

            //_context.SaveChanges();
        }

        /// <summary>
        /// Delete objects from database by specified filter expression.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            //var objects = Filter(predicate);
            //foreach (var obj in objects)
            //    _context.Set<T>().Remove(obj);
            //return _context.SaveChanges();

            var objects = await FilterAsync(predicate);
            foreach (var obj in objects)
                _context.Set<T>().Remove(obj);
        }

        /// <summary>
        /// Update object changes and save to database.
        /// </summary>
        /// <param name="t">Specified the object to save.</param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(T t)
        {
            try
            {
                //var entry = _context.Entry(t);
                //_context.Set<T>().Attach(t);
                //entry.State = EntityState.Modified;

                var entry = _context.Entry(t);
                _context.Set<T>().Attach(t);
                entry.State = EntityState.Modified;

                //_context.SaveChanges();
            }
            //catch (OptimisticConcurrencyException ex)
            //{
            //    throw ex;
            //}
            catch
            { }
        }

        /// <summary>
        /// Select Single Item by specified expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            //return All().FirstOrDefault(expression);
            return await _context.Set<T>().FirstOrDefaultAsync(expression);
        }

        public virtual async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (InvalidOperationException ex)
            {
                if (!ex.Message.Contains("The changes to the database were committed successfully"))
                {
                    throw;
                }
            }
        }

        #endregion
    }
}
