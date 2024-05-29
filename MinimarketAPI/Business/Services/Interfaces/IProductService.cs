using Minimarket.DTO;

namespace Minimarket.Business.Service;

public interface IProductService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="seach"></param>
    /// <returns></returns>
    Task<List<ProductDTO>> ProductList(string seach);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ProductDTO> GetProduct(int id);
    
    /// <summary>
    /// /
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<ProductDTO> Create(ProductDTO model);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<bool> Edit(ProductDTO model);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> Delete(int id);
}