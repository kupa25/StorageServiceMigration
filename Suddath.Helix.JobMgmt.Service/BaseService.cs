using AutoMapper;
using Helix.API.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Suddath.Helix.JobMgmt.Infrastructure;
using Suddath.Helix.JobMgmt.Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Services
{
    public abstract class BaseService
    {
        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseService()
        {
        }

        public BaseService(DbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUserEmail()
        {
            return _httpContextAccessor?.HttpContext?.User?.Claims.Where(x => x.Type == "email").FirstOrDefault()?.Value;
        }

        public string GetCurrentUserName()
        {
            return _httpContextAccessor?.HttpContext?.User?.Claims.Where(x => x.Type == "name").FirstOrDefault()?.Value;
        }

        public string GetJobsMoveConsultantEmail(int jobId)
        {
            var moveConsultant = ((JobDbContext)_dbContext).JobContact.
                                    SingleOrDefault(jc => jc.ContactType.ToUpper() == ConsultantType.MoveConsultant && jc.JobId == jobId);

            return moveConsultant == null ? GetCurrentUserEmail() : moveConsultant.Email;
        }

        /// <summary>
        /// Gets Pagination object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="recordCount"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PagedResults<T> GetPagedList<T>(int recordCount, int pageNumber, int pageSize)
        {
            return new PagedResults<T>
            {
                Pagination = new Pagination()
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalRecordsFound = recordCount
                }
            };
        }

        /// <summary>
        /// Uses reflection to get property value (nested or top level)
        /// </summary>
        /// <param name="srcObj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetPropertyValue(object srcObj, string propertyName)
        {
            if (srcObj == null) throw new ArgumentNullException("Value cannot be null.", nameof(srcObj));
            if (propertyName == null) throw new ArgumentNullException("Value cannot be null.", nameof(propertyName));

            if (propertyName.Contains(".")) //detect nested prop
            {
                var propNames = propertyName.Split(new char[] { '.' }, 2);
                return GetPropertyValue(GetPropertyValue(srcObj, propNames[0]), propNames[1]);
            }
            else
            {
                var prop = srcObj.GetType().GetProperty(propertyName);
                var value = prop?.GetValue(srcObj, null);

                if (value != null && (value is List<string> || value is ICollection<string>))
                {
                    value = string.Join(",", value);
                }

                return value;
            }
        }

        public async Task<T> Get<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task Save<T>(T entity) where T : class
        {
            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Add<T>(T entity) where T : class
        {
            _dbContext.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Remove<T>(T entity) where T : class
        {
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ICollection<T>> Find<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<TResponse> GetMappedResponse<TResponse, TEntity>(TEntity request)
        {
            return await Task.FromResult(_mapper.Map<TResponse>(request));
        }

        public static string EvalProperty(string property)
        {
            return property.Format() ?? "";
        }
    }
}