using IARA.Application.DTOs.Report;

namespace IARA.Application.Interfaces;

public interface IReportService
{
    Task<IEnumerable<ExpiringPermitReportDto>> GetExpiringPermitsReportAsync(int daysFromNow = 30);
    Task<IEnumerable<TopRecreationalFisherDto>> GetTopRecreationalFishersReportAsync(int year, int topCount = 10);
    Task<ShipStatisticsDto> GetShipStatisticsAsync(int shipId, int year);
    Task<IEnumerable<ShipStatisticsDto>> GetAllShipsStatisticsAsync(int year);
    Task<IEnumerable<CarbonFootprintDto>> GetCarbonFootprintReportAsync();
}