using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.SearchModel.Partner.Dashboard;
using TourismSmartTransportation.Business.ViewModel.Partner.Dashboard;
using TourismSmartTransportation.Data.Interfaces;

namespace TourismSmartTransportation.Business.Implements.Partner
{
    public class NormalizeList
    {
        public decimal Key { get; set; }
        public List<int> Value { get; set; }
        public List<PriceObject> Price { get; set; }
    }

    public class PriceObject
    {
        public decimal Month { get; set; }
        public decimal Price { get; set; }
    }

    public class DashboardService : BaseService, IDashboardService
    {
        public DashboardService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        private void DoProcess(Dictionary<decimal, object> dictionary, Hashtable hashtable, IEnumerable<NormalizeList> normalizeList,
                                bool isCalculateRevenue = false, decimal value = 1)
        {
            foreach (var item in normalizeList)
            {
                // clear oldest data
                hashtable.Clear();

                // Create hashtable list with "MONTH" is "key" and default "value" is "0"
                for (int i = 0; i < 12; i++)
                {
                    hashtable.Add(i + 1, 0);
                }

                // Check existed key, If not, added to dictionary
                if (!dictionary.ContainsKey(item.Key))
                {
                    dictionary.Add(item.Key, hashtable);
                }

                dynamic list = isCalculateRevenue ? item.Price : item.Value;

                // Count
                foreach (var month in list)
                {
                    var choice = isCalculateRevenue ? (int)month.Month : month;
                    switch (choice)
                    {
                        case (int)Months.Jan:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Jan] = Int32.Parse(hashtable[(int)Months.Jan].ToString()) + temp;
                                break;
                            }
                        case (int)Months.Feb:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Feb] = Int32.Parse(hashtable[(int)Months.Feb].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Mar:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Mar] = Int32.Parse(hashtable[(int)Months.Mar].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Apr:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Apr] = Int32.Parse(hashtable[(int)Months.Apr].ToString()) + temp;
                                break;
                            };
                        case (int)Months.May:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.May] = Int32.Parse(hashtable[(int)Months.May].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Jun:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Jun] = Int32.Parse(hashtable[(int)Months.Jun].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Jul:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Jul] = Int32.Parse(hashtable[(int)Months.Jul].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Aug:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Aug] = Int32.Parse(hashtable[(int)Months.Aug].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Sep:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Sep] = Int32.Parse(hashtable[(int)Months.Sep].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Oct:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Oct] = Int32.Parse(hashtable[(int)Months.Oct].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Nov:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Nov] = Int32.Parse(hashtable[(int)Months.Nov].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Dec:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Dec] = Int32.Parse(hashtable[(int)Months.Dec].ToString()) + temp;
                                break;
                            };
                        default: break;
                    }
                }
                dictionary[item.Key] = hashtable;
            }
        }

        public async Task<Dictionary<decimal, object>> GetDriverReportByMonth(ReportSearchModel model)
        {
            var driversList = await _unitOfWork.DriverRepository
                            .Query()
                            .Where(x => x.PartnerId == model.PartnerId)
                            .Select(x => new
                            {
                                DriverId = x.DriverId,
                                CreatedDate = x.CreatedDate
                            })
                            .ToListAsync();

            var normalizeList = driversList.GroupBy(
                x => x.CreatedDate.Year,
                x => x.CreatedDate,
                (key, value) => new NormalizeList()
                {
                    Key = key,
                    Value = value.Select(x => x.Month).ToList()
                }
            );

            Hashtable hashtable = new Hashtable();
            Dictionary<decimal, object> dictionary = new Dictionary<decimal, object>();
            DoProcess(dictionary, hashtable, normalizeList);

            return dictionary;
        }

        public Task<Dictionary<decimal, object>> GetVehicleReportByMonth(ReportSearchModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<Dictionary<decimal, object>> GetTripReportByMonth(ReportSearchModel model)
        {
            var routesList = await _unitOfWork.RouteRepository
                            .Query()
                            .Where(x => x.PartnerId == model.PartnerId)
                            .Join(_unitOfWork.TripRepository.Query(),
                                r => r.RouteId,
                                t => t.RouteId,
                                (r, t) => new
                                {
                                    PartnerId = r.PartnerId,
                                    TripId = t.TripId,
                                    RouteId = r.RouteId,
                                    CreatedDate = r.CreatedDate,
                                }
                            )
                            .ToListAsync();

            var normalizeList = routesList.GroupBy(
                            x => x.CreatedDate.Year,
                            x => x.CreatedDate,
                            (key, value) => new NormalizeList()
                            {
                                Key = key,
                                Value = value.Select(x => x.Month).ToList()
                            }
                        );

            Hashtable hashtable = new Hashtable();
            Dictionary<decimal, object> dictionary = new Dictionary<decimal, object>();
            DoProcess(dictionary, hashtable, normalizeList);

            return dictionary;
        }

        public async Task<Dictionary<decimal, object>> GetRevenueReportByMonth(ReportSearchModel model)
        {
            var orderList = await _unitOfWork.OrderRepository
                        .Query()
                        .Where(x => x.PartnerId != null ? (x.PartnerId.Value == model.PartnerId) : false)
                        .ToListAsync();

            var normalizeList = orderList.GroupBy(
                x => x.CreatedDate.Year,
                x => x,
                (key, value) => new NormalizeList()
                {
                    Key = key,
                    Value = value.Select(x => x.CreatedDate.Month).ToList(),
                    Price = value.Select(x => new PriceObject() { Month = x.CreatedDate.Month, Price = x.TotalPrice }).ToList()
                }
            );

            Hashtable hashtable = new Hashtable();
            Dictionary<decimal, object> dictionary = new Dictionary<decimal, object>();
            DoProcess(dictionary, hashtable, normalizeList, true);

            return dictionary;
        }

        public async Task<StatisticalReportViewModel> GetStatisticalReport(ReportSearchModel model)
        {
            var driver = (await _unitOfWork.DriverRepository
                        .Query()
                        .Where(x => x.PartnerId == model.PartnerId)
                        .ToListAsync()).Count;

            var vehicle = (await _unitOfWork.VehicleRepository
                        .Query()
                        .Where(x => x.PartnerId == model.PartnerId)
                        .ToListAsync()).Count;

            var trip = (await _unitOfWork.RouteRepository
                            .Query()
                            .Where(x => x.PartnerId == model.PartnerId)
                            .Join(_unitOfWork.TripRepository.Query(),
                                r => r.RouteId,
                                t => t.RouteId,
                                (r, t) => new { }
                            )
                            .ToListAsync()).Count;

            var orderList = await _unitOfWork.OrderRepository
                        .Query()
                        .Where(x => x.PartnerId != null ? (x.PartnerId.Value == model.PartnerId) : false)
                        .ToListAsync();

            decimal totalPrice = 0;
            foreach (var o in orderList)
            {
                totalPrice += o.TotalPrice;
            }

            return new()
            {
                Driver = driver,
                Vehicle = vehicle,
                Trip = trip,
                Order = totalPrice
            };
        }
    }
}