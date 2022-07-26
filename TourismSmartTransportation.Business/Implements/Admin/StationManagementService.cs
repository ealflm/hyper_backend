using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.StationManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.StationManagement;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;
using Azure.Storage.Blobs;
using TourismSmartTransportation.Business.CommonModel;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class StationManagementService : BaseService, IStationManagementService
    {
        public StationManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<Response> AddStation(AddStationViewModel model)
        {

            var station = new Station()
            {
                StationId = Guid.NewGuid(),
                Address = model.Address,
                Title = model.Title,
                Description = model.Description,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Status = 1
            };
            var linkStations = await _unitOfWork.StationRepository.Query().ToListAsync();
            HttpClient client = new HttpClient();
            foreach(Station x in linkStations)
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://api.mapbox.com/directions/v5/mapbox/driving/"+x.Longitude+","+x.Latitude+";"+station.Longitude+","+station.Latitude+"?annotations=maxspeed&overview=full&geometries=geojson&access_token=pk.eyJ1Ijoic2FuZ2RlcHRyYWkiLCJhIjoiY2w0bXFvaDRwMW9uZjNpbWtpMjZ3eGxnbCJ9.2gQ3NUL1eBYTwP1Q_qS34A")
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    var jmessage = JObject.Parse(body);
                    var distance = double.Parse(jmessage["routes"][0]["distance"].ToString());
                    if ( distance<= 200)
                    {
                        var linkStation = new LinkStation()
                        {
                            LinkStationId = Guid.NewGuid(),
                            FirstStationId = station.StationId,
                            SecondStationId = x.StationId,
                            Content = "Đi bộ khoảng cách " + distance+"m"
                        };
                        await _unitOfWork.LinkStationRepository.Add(linkStation);
                    }   
                }
            }
            await _unitOfWork.StationRepository.Add(station);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Tạo trạm thành công!"
            };
        }

        public async Task<Response> DeleteStation(Guid id)
        {
            var station = await _unitOfWork.StationRepository.GetById(id);
            if (station == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            station.Status = 0;
            _unitOfWork.StationRepository.Update(station);
            var linkStations = await _unitOfWork.LinkStationRepository.Query().Where(x => x.FirstStationId.Equals(id) || x.SecondStationId.Equals(id)).ToListAsync();
            foreach (LinkStation x in linkStations)
            {
                await _unitOfWork.LinkStationRepository.Remove(x.LinkStationId);
            }
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };

        }

        public async Task<StationViewModel> GetStation(Guid id)
        {
            var station = await _unitOfWork.StationRepository.GetById(id);
            if (station == null)
            {
                return null;
            }
            StationViewModel model = station.AsStationViewModel();
            return model;
        }

        public async Task<SearchResultViewModel<StationViewModel>> SearchStation(StationSearchModel model)
        {
            var stations = await _unitOfWork.StationRepository.Query()
                .Where(x => model.Title == null || x.Title.Contains(model.Title))
                .Where(x => model.Address == null || x.Address.Contains(model.Address))
                .Where(x => model.Status == null || x.Status == model.Status.Value)
                .OrderBy(x => x.Title)
                .Select(x => x.AsStationViewModel())
                .ToListAsync();
            var listAfterSorting = GetListAfterSorting(stations, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            SearchResultViewModel<StationViewModel> result = null;
            result = new SearchResultViewModel<StationViewModel>()
            {
                Items = listItemsAfterPaging,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
                TotalItems = totalRecord
            };
            return result;
        }

        public async Task<Response> UpdateStation(Guid id, UpdateStationModel model)
        {

            var station = await _unitOfWork.StationRepository.GetById(id);
            if (station == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            station.Title = UpdateTypeOfNullAbleObject<string>(station.Title, model.Title);
            station.Address = UpdateTypeOfNullAbleObject<string>(station.Address, model.Address);
            station.Description = UpdateTypeOfNullAbleObject<string>(station.Description, model.Description);
            station.Latitude = UpdateTypeOfNotNullAbleObject<decimal>(station.Latitude, model.Latitude);
            station.Longitude = UpdateTypeOfNotNullAbleObject<decimal>(station.Longitude, model.Longitude);
            station.Status = UpdateTypeOfNotNullAbleObject<int>(station.Status, model.Status);
            _unitOfWork.StationRepository.Update(station);
            await _unitOfWork.SaveChangesAsync();
            var linkStations = await _unitOfWork.LinkStationRepository.Query().Where(x => x.FirstStationId.Equals(id) || x.SecondStationId.Equals(id)).ToListAsync();
            foreach(LinkStation x in linkStations)
            {
                await _unitOfWork.LinkStationRepository.Remove(x.LinkStationId);
            }
            var stations = await _unitOfWork.StationRepository.Query().ToListAsync();
            HttpClient client = new HttpClient();
            foreach (Station x in stations)
            {
                if (!x.StationId.Equals(id))
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri("https://api.mapbox.com/directions/v5/mapbox/driving/" + x.Longitude + "," + x.Latitude + ";" + station.Longitude + "," + station.Latitude + "?annotations=maxspeed&overview=simplified&geometries=geojson&access_token=pk.eyJ1Ijoic2FuZ2RlcHRyYWkiLCJhIjoiY2w0bXFvaDRwMW9uZjNpbWtpMjZ3eGxnbCJ9.2gQ3NUL1eBYTwP1Q_qS34A")
                    };
                    using (var response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        var body = await response.Content.ReadAsStringAsync();
                        var jmessage = JObject.Parse(body);
                        var distance = double.Parse(jmessage["routes"][0]["distance"].ToString());
                        if (distance <= 200)
                        {
                            var linkStation = new LinkStation()
                            {
                                LinkStationId = Guid.NewGuid(),
                                FirstStationId = station.StationId,
                                SecondStationId = x.StationId,
                                Distance= (decimal)distance
                            };
                            await _unitOfWork.LinkStationRepository.Add(linkStation);
                        }
                    }
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }
    }
}
