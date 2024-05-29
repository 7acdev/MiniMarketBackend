using System.Linq.Expressions;

namespace Minimarket.Data.Repo;

public interface IGenericRepo<T> where T : class
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    IQueryable<T> Query(Expression<Func<T, bool>>? filter = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<T> Create(T model);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<bool> Edit(T model);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<bool> Delete(T model);
}