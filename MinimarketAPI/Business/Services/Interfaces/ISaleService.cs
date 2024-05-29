using Minimarket.DTO;

namespace Minimarket.Business.Service;

public interface ISaleService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<SaleDTO> Register(SaleDTO model);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="seach"></param>
    /// <param name="saleNumber"></param>
    /// <param name="StartDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    Task<List<SaleDTO>> History(string seach, string saleNumber, string StartDate, string endDate);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    Task<List<ReportDTO>> Report(string startDate, string endDate);
}