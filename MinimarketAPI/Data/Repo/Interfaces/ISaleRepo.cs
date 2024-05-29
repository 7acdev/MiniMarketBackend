using Minimarket.Model;

namespace Minimarket.Data.Repo;

public interface ISaleRepo : IGenericRepo<Sale>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<Sale> Register(Sale model);
}
