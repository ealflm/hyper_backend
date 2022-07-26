using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.BasePriceOfBusService;
using TourismSmartTransportation.Business.ViewModel.Admin.BasePriceOfBusService;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class BasePriceOfBusServiceManagement : BaseService, IBasePriceOfBusService
    {
        private readonly double[] rateArray = { 0.25, 0.5, 0.75, 1 };

        public BasePriceOfBusServiceManagement(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<Response> GeneratePriceOfBusService(AddBasePriceOfBusService model)
        {
            var maxDistance = await _unitOfWork.BasePriceOfBusServiceRepository
                                    .Query()
                                    .OrderBy(x => x.MaxDistance)
                                    .Select(x => x.MaxDistance)
                                    .LastOrDefaultAsync();
            if (model.MinDistance < maxDistance)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Giá đã tồn tại!"
                };
            }

            var basePrice = new BasePriceOfBusService()
            {
                BasePriceOfBusServiceId = Guid.NewGuid(),
                MinDistance = model.MinDistance,
                MaxDistance = model.MaxDistance,
                Price = model.Price,
                Status = 1
            };

            await _unitOfWork.BasePriceOfBusServiceRepository.Add(basePrice);

            // create price base on distance and take money follow by min distance and max distance
            var price = new PriceOfBusService()
            {
                PriceOfBusServiceId = Guid.NewGuid(),
                BasePriceId = basePrice.BasePriceOfBusServiceId,
                MinDistance = model.MinDistance,
                MaxDistance = (model.MaxDistance - model.MinDistance) * (decimal)rateArray[0] + model.MinDistance,
                Price = basePrice.Price * (decimal)rateArray[0] - 1000,
                MinStation = 0,
                MaxStation = 0,
                Mode = "distance",
                Status = 1,
            };

            await _unitOfWork.PriceOfBusServiceRepository.Add(price);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 200,
                Message = "Tạo giá thành công!"
            };
        }

        public async Task<BasePriceOfBusServiceViewModel> GetBasePricesById(Guid id)
        {
            var basePrice = await _unitOfWork.BasePriceOfBusServiceRepository.GetById(id);
            if (basePrice == null)
            {
                return null;
            }

            return basePrice.AsBasePriceOfBusServiceViewModel();
        }

        public async Task<List<BasePriceOfBusServiceViewModel>> GetBasePricesList(BasePriceOfBusServiceSearchModel model)
        {
            var basePricesList = await _unitOfWork.BasePriceOfBusServiceRepository
                            .Query()
                            .Where(x => model.Price == null || x.Price.ToString().Contains(model.Price.Value.ToString()))
                            .Where(x => model.status == null || x.Status == model.status.Value)
                            .Select(x => x.AsBasePriceOfBusServiceViewModel())
                            .ToListAsync();

            return basePricesList;
        }

        public async Task<Response> UpdateBasePrice(Guid id, UpdateBasePriceOfBusService model)
        {
            var basePrice = await _unitOfWork.BasePriceOfBusServiceRepository.GetById(id);
            if (basePrice == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy giá!"
                };
            }

            decimal oldMinDistance = basePrice.MinDistance;
            decimal oldMaxDistance = basePrice.MaxDistance;

            var basePricesList = await _unitOfWork.BasePriceOfBusServiceRepository
                                .Query()
                                .OrderBy(x => x.MaxDistance)
                                .Select(x => x.AsBasePriceOfBusServiceViewModel())
                                .ToListAsync();

            for (int i = 0; i < basePricesList.Count; i++)
            {
                // Chỉ kiểm tra record cần update.
                if (basePricesList[i].BasePriceOfBusServiceId != basePrice.BasePriceOfBusServiceId)
                {
                    continue;
                }

                if (model.MaxDistance != null && i < basePricesList.Count - 1)
                {
                    if (model.MaxDistance.Value > basePricesList[i + 1].MinDistance)
                    {
                        return new()
                        {
                            StatusCode = 400,
                            Message = $@"Độ dài khoảng cách không hợp lệ. 'MaxDistance' không lớn hơn {basePricesList[i + 1].MinDistance}!"
                        };
                    }
                }

                if (model.MinDistance != null && i != 0)
                {
                    if (model.MinDistance.Value < basePricesList[i - 1].MaxDistance)
                    {
                        return new()
                        {
                            StatusCode = 400,
                            Message = $"Độ dài khoảng cách không hợp lệ. 'MinDistance' không nhỏ hơn {basePricesList[i - 1].MinDistance}!"
                        };
                    }
                }
            }

            basePrice.MaxDistance = UpdateTypeOfNotNullAbleObject<decimal>(basePrice.MaxDistance, model.MaxDistance);
            basePrice.MinDistance = UpdateTypeOfNotNullAbleObject<decimal>(basePrice.MinDistance, model.MinDistance);
            basePrice.Price = UpdateTypeOfNotNullAbleObject<decimal>(basePrice.Price, model.Price);
            basePrice.Status = UpdateTypeOfNotNullAbleObject<int>(basePrice.Status, model.Status);
            _unitOfWork.BasePriceOfBusServiceRepository.Update(basePrice);

            // sắp xếp tăng dần theo min distance nên record đầu tiên luôn có mode là distance
            var priceOfBusServicesList = await _unitOfWork.PriceOfBusServiceRepository
                                    .Query()
                                    .Where(x => x.BasePriceId == basePrice.BasePriceOfBusServiceId)
                                    .OrderBy(x => x.MaxDistance)
                                    .Select(x => new PriceOfBusService()
                                    {
                                        PriceOfBusServiceId = x.PriceOfBusServiceId,
                                        BasePriceId = x.BasePriceId,
                                        MinDistance = x.MinDistance,
                                        MaxDistance = x.MaxDistance,
                                        Price = x.Price,
                                        MinStation = x.MinStation,
                                        MaxStation = x.MaxStation,
                                        Mode = x.Mode,
                                        Status = x.Status
                                    })
                                    .ToListAsync();

            var pairs = new List<Tuple<decimal, decimal, decimal>>();
            for (int i = 0; i < rateArray.Count(); i++)
            {
                switch (i)
                {
                    case 0:
                        {
                            var min = (oldMaxDistance - oldMinDistance) * (decimal)rateArray[i] + oldMinDistance;
                            var max = (oldMaxDistance - oldMinDistance) * (decimal)rateArray[i + 1] + oldMinDistance;
                            pairs.Add(Tuple.Create(min, max, (decimal)rateArray[i]));
                            break;
                        }
                    default:
                        {
                            int index = i == rateArray.Count() - 1 ? i : i + 1;
                            var min = (oldMaxDistance - oldMinDistance) * (decimal)rateArray[i] + oldMinDistance;
                            var max = (oldMaxDistance - oldMinDistance) * (decimal)rateArray[index] + oldMinDistance;
                            pairs.Add(Tuple.Create(min, max, (decimal)rateArray[i]));
                            break;
                        }
                }

            }

            for (int i = 0; i < priceOfBusServicesList.Count; i++)
            {
                if (priceOfBusServicesList[i].Mode == "distance")
                {
                    priceOfBusServicesList[i].PriceOfBusServiceId = priceOfBusServicesList[i].PriceOfBusServiceId;
                    priceOfBusServicesList[i].BasePriceId = basePrice.BasePriceOfBusServiceId;
                    priceOfBusServicesList[i].MinDistance = basePrice.MinDistance;
                    priceOfBusServicesList[i].MaxDistance = (basePrice.MaxDistance - basePrice.MinDistance) * (decimal)rateArray[0] + basePrice.MinDistance;
                    priceOfBusServicesList[i].Price = basePrice.Price * (decimal)rateArray[0] - 1000;
                    priceOfBusServicesList[i].MinStation = 0;
                    priceOfBusServicesList[i].MaxStation = 0;
                    priceOfBusServicesList[i].Mode = "distance";
                    priceOfBusServicesList[i].Status = 1;
                }
                else // mode station
                {
                    for (int j = 0; j < pairs.Count; j++)
                    {
                        if (priceOfBusServicesList[i].MinDistance == pairs[j].Item1 && priceOfBusServicesList[i].MaxDistance == pairs[j].Item2)
                        {
                            decimal tempRate = 1;
                            switch (pairs[j].Item3)
                            {
                                case 0.25M:
                                    {
                                        tempRate = 0.5M;
                                        break;
                                    }
                                case 0.5M:
                                    {
                                        tempRate = 0.75M;
                                        break;
                                    }
                                case 0.75M:
                                    {
                                        tempRate = 1;
                                        break;
                                    }
                                default: break;
                            }

                            priceOfBusServicesList[i].PriceOfBusServiceId = priceOfBusServicesList[i].PriceOfBusServiceId;
                            priceOfBusServicesList[i].BasePriceId = basePrice.BasePriceOfBusServiceId;
                            priceOfBusServicesList[i].MinDistance = basePrice.MaxDistance * pairs[j].Item3;
                            priceOfBusServicesList[i].MaxDistance = basePrice.MaxDistance * tempRate;
                            priceOfBusServicesList[i].Price = basePrice.Price * pairs[j].Item3;
                            priceOfBusServicesList[i].MinStation = priceOfBusServicesList[i].MinStation;
                            priceOfBusServicesList[i].MaxStation = priceOfBusServicesList[i].MaxStation;
                            priceOfBusServicesList[i].Mode = "station";
                            priceOfBusServicesList[i].Status = 1;

                            break;
                        }
                    }

                }

                _unitOfWork.PriceOfBusServiceRepository.Update(priceOfBusServicesList[i]);
            }

            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật giá thành công!"
            };
        }
    }
}