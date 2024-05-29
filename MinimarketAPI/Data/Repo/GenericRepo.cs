using System.Linq.Expressions;
using Minimarket.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Minimarket.Data.Repo;

public class GenericRepo<T> : IGenericRepo<T> where T : class
{
    private readonly MinimarketContext _context;

    public GenericRepo(MinimarketContext context)
    {
        _context = context;
    }

    public async Task<T> Create(T model)
    {
        _context.Set<T>().Add(model);
        if (await _context.SaveChangesAsync() > 0)
            return model;
        else
            throw new Exception("Create model fail");
    }

    public async Task<bool> Delete(T model)
    {
        _context.Set<T>().Remove(model);
        int success = await _context.SaveChangesAsync();
        return success > 0;
    }

    public async Task<bool> Edit(T model)
    {
        _context.Set<T>().Update(model);
        int success = await _context.SaveChangesAsync();
        return success > 0;
    }

    public IQueryable<T> Query(Expression<Func<T, bool>>? filter = null)
    {
        return (filter is not null) ? _context.Set<T>().Where(filter) : _context.Set<T>();
    }
}