using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Partner.Dashboard;
using TourismSmartTransportation.Business.ViewModel.Partner.Dashboard;

namespace TourismSmartTransportation.Business.Interfaces.Partner
{
    public interface IDashboardService
    {
        Task<Dictionary<decimal, object>> GetDriverReportByMonth(ReportSearchModel model);
        Task<Dictionary<decimal, object>> GetVehicleReportByMonth(ReportSearchModel model);
        Task<Dictionary<decimal, object>> GetTripReportByMonth(ReportSearchModel model);
        Task<Dictionary<decimal, object>> GetRevenueReportByMonth(ReportSearchModel model);
        Task<StatisticalReportViewModel> GetStatisticalReport(ReportSearchModel model);
        Task<List<ItemCounterReportViewModel>> GetVehicleReportByServiceType(ReportSearchModel model);
    }
}