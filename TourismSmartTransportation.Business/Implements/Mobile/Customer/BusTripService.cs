using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Admin.StationManagement;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer.BusTrip;
using TourismSmartTransportation.Business.ViewModel.Partner.RouteManagement;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Mobile.Customer
{
    public class BusTripService : BaseService, IBusTripService
    {
        public BusTripService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<List<List<RouteViewModel>>> FindBusTrip(BusTripSearchModel model)
        {
            var stationList = await _unitOfWork.StationRepository.Query().ToListAsync();
            Station start= new Station();
            Station end= new Station();
            double minDisStart = double.MaxValue;
            double minDisEnd = double.MaxValue;
            HttpClient client = new HttpClient();
            foreach(Station x in stationList)
            {
                
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://api.mapbox.com/directions/v5/mapbox/walking/" + x.Longitude + "," + x.Latitude + ";" + model.StartLongitude + "," + model.StartLatitude + "?overview=simplified&geometries=geojson&access_token=pk.eyJ1Ijoic2FuZ2RlcHRyYWkiLCJhIjoiY2w0bXFvaDRwMW9uZjNpbWtpMjZ3eGxnbCJ9.2gQ3NUL1eBYTwP1Q_qS34A")
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    var jmessage = JObject.Parse(body);
                    var distance = double.Parse(jmessage["routes"][0]["distance"].ToString());
                    if (distance < minDisStart)
                    {
                        minDisStart = distance;
                        start = x;
                    }
                    
                }
            }
            
            foreach (Station x in stationList)
            {

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://api.mapbox.com/directions/v5/mapbox/walking/" + x.Longitude + "," + x.Latitude + ";" + model.EndLongitude + "," + model.EndLatitude + "?overview=simplified&geometries=geojson&access_token=pk.eyJ1Ijoic2FuZ2RlcHRyYWkiLCJhIjoiY2w0bXFvaDRwMW9uZjNpbWtpMjZ3eGxnbCJ9.2gQ3NUL1eBYTwP1Q_qS34A")
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    var jmessage = JObject.Parse(body);
                    var distance = double.Parse(jmessage["routes"][0]["distance"].ToString());
                    if (distance < minDisEnd)
                    {
                        minDisEnd = distance;
                        end = x;
                    }

                }
            }

            int LinkRouteCount = await _unitOfWork.LinkRouteRepository.Query().CountAsync();
            int routeCount = await _unitOfWork.RouteRepository.Query().CountAsync();
            Hashtable countAppearRouteList = new Hashtable();
            var startRouteList = await _unitOfWork.StationRouteRepository.Query().Where(x => x.StationId.Equals(start.StationId)).ToListAsync();
            var endRouteList = await _unitOfWork.StationRouteRepository.Query().Where(x => x.StationId.Equals(end.StationId)).ToListAsync();
            Queue<Node> queue = new Queue<Node>();
            var resultPathList = new List<Node>();
            foreach(StationRoute startRoute in startRouteList)
            {
                queue.Clear();
                queue.Enqueue(new Node(startRoute.RouteId, null));
                countAppearRouteList.Clear();
                countAppearRouteList.Add(startRoute.RouteId, LinkRouteCount);
                foreach (StationRoute endRoute in endRouteList)
                {
                    while (queue.Count != 0)
                    {
                        var curentRoute = queue.Dequeue();
                        if (!curentRoute.Value.Equals(endRoute.RouteId))
                        {
                            var linkRouteList = await _unitOfWork.LinkRouteRepository.Query().Where(x => x.FirstRouteId.Equals(curentRoute.Value) || x.SecondRouteId.Equals(curentRoute.Value)).ToListAsync();
                            foreach (LinkRoute route in linkRouteList)
                            {
                                var routeId = route.FirstRouteId;
                                if (routeId.Equals(curentRoute.Value))
                                {
                                    routeId = route.SecondRouteId;
                                }
                                bool check = false;
                                try
                                {
                                    countAppearRouteList.Add(routeId, LinkRouteCount);
                                    check = true;
                                }
                                catch
                                {
                                    check = ((int)countAppearRouteList[routeId]) > 0;
                                }
                                finally
                                {
                                    if (check && curentRoute.Count < routeCount && !routeId.Equals(startRoute.RouteId))
                                    {
                                        queue.Enqueue(new Node(routeId, curentRoute));
                                        countAppearRouteList[routeId] = ((int)countAppearRouteList[routeId])-1;
                                    }
                                }


                            }
                        }
                        else
                        {
                            resultPathList.Add(curentRoute);
                        }
                        if (countAppearRouteList[endRoute.RouteId]!=null && ((int)countAppearRouteList[endRoute.RouteId]) == 0)
                        {
                            break;
                        }
                    }
                }
            }
            var result =new List<List<RouteViewModel>>();
            var customerStartPoint = new Station()
            {
                Longitude = model.StartLongitude,
                Latitude = model.StartLatitude
            };
            var customerEndPoint = new Station()
            {
                Longitude = model.EndLongitude,
                Latitude = model.EndLatitude
            };

            foreach (Node node in resultPathList)
            {
                var checkSet =new HashSet<Guid>();
                var tmpNode = node;
                var resultPath = new List<RouteViewModel>();
                var firstRoute = new RouteViewModel()
                {
                    StationList = new List<StationViewModel>(),
                    Name = "Đi bộ",
                    Distance = (decimal) minDisEnd
                };
                firstRoute.StationList.Add(customerEndPoint.AsStationViewModel());
                firstRoute.StationList.Add(end.AsStationViewModel());
                resultPath.Add(firstRoute);
                var currentStation = end;
                bool check = true;
                while(tmpNode!=null)
                {
                    if (!checkSet.Add(tmpNode.Value))
                    {
                        check = false;
                        break;
                    }
                    else
                    {
                        LinkRoute path = null;
                        try
                        {
                             path = await _unitOfWork.LinkRouteRepository.Query().Where(x => (x.FirstRouteId.Equals(tmpNode.Value) && x.SecondRouteId.Equals(tmpNode.Parent.Value)) || (x.SecondRouteId.Equals(tmpNode.Value) && x.FirstRouteId.Equals(tmpNode.Parent.Value))).FirstOrDefaultAsync();
                        }
                        catch
                        {
                            path = new LinkRoute()
                            {
                                StationId = start.StationId
                            };
                        }
                        var stationListOfRoute = await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(tmpNode.Value)).OrderBy(x => x.OrderNumber).ToListAsync();
                        var route = (await _unitOfWork.RouteRepository.GetById(tmpNode.Value)).AsRouteViewModel();
                        route.StationList = new List<StationViewModel>();
                        if (path.LinkStationId == null)
                        {
                            var stationRouteNew = await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(tmpNode.Value) && x.StationId.Equals(path.StationId)).FirstOrDefaultAsync();
                            var stationRouteOld = await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(tmpNode.Value) && x.StationId.Equals(currentStation.StationId)).FirstOrDefaultAsync();
                            int step= stationRouteOld.OrderNumber<stationRouteNew.OrderNumber?1:-1;
                            for(int i=stationRouteOld.OrderNumber; i!=stationRouteNew.OrderNumber+step; i +=step)
                            {
                                var station = await _unitOfWork.StationRepository.GetById(stationListOfRoute[i].StationId);
                                route.StationList.Add(station.AsStationViewModel());
                            }
                            route.Distance = Math.Abs(stationRouteOld.Distance - stationRouteNew.Distance);
                            resultPath.Add(route);
                            currentStation = await _unitOfWork.StationRepository.GetById(path.StationId.Value);
                        }
                        else
                        {
                            var linkStation = await _unitOfWork.LinkStationRepository.GetById(path.LinkStationId.Value);
                            var firstStation = await _unitOfWork.StationRepository.GetById(linkStation.FirstStationId);
                            var secondStation = await _unitOfWork.StationRepository.GetById(linkStation.SecondStationId);
                            if (tmpNode.Value.Equals(linkStation.FirstStationId))
                            {
                                var stationRouteNew = await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(tmpNode.Value) && x.StationId.Equals(firstStation.StationId)).FirstOrDefaultAsync();
                                var stationRouteOld = await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(tmpNode.Value) && x.StationId.Equals(currentStation.StationId)).FirstOrDefaultAsync();
                                int step = stationRouteOld.OrderNumber < stationRouteNew.OrderNumber ? 1 : -1;
                                for (int i = stationRouteOld.OrderNumber+step; i != stationRouteNew.OrderNumber + step; i += step)
                                {
                                    var station = await _unitOfWork.StationRepository.GetById(stationListOfRoute[i].StationId);
                                    route.StationList.Add(station.AsStationViewModel());
                                }
                                route.StationList.Add(secondStation.AsStationViewModel());
                                route.Distance = Math.Abs(stationRouteOld.Distance - stationRouteNew.Distance);
                                resultPath.Add(route);
                                var midRoute = new RouteViewModel()
                                {
                                    StationList = new List<StationViewModel>(),
                                    Name = "Đi bộ",
                                    Distance = linkStation.Distance
                                };
                                midRoute.StationList.Add(firstStation.AsStationViewModel());
                                midRoute.StationList.Add(secondStation.AsStationViewModel());
                                resultPath.Add(midRoute);
                                currentStation = secondStation;
                            }
                            else
                            {
                                var stationRouteNew = await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(tmpNode.Value) && x.StationId.Equals(secondStation.StationId)).FirstOrDefaultAsync();
                                var stationRouteOld = await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(tmpNode.Value) && x.StationId.Equals(currentStation.StationId)).FirstOrDefaultAsync();
                                int step = stationRouteOld.OrderNumber < stationRouteNew.OrderNumber ? 1 : -1;
                                for (int i = stationRouteOld.OrderNumber+step; i!= stationRouteNew.OrderNumber + step; i += step)
                                {
                                    var station = await _unitOfWork.StationRepository.GetById(stationListOfRoute[i].StationId);
                                    route.StationList.Add(station.AsStationViewModel());
                                }
                                route.StationList.Add(firstStation.AsStationViewModel());
                                route.Distance = Math.Abs(stationRouteOld.Distance - stationRouteNew.Distance);
                                resultPath.Add(route);
                                var midRoute = new RouteViewModel()
                                {
                                    StationList = new List<StationViewModel>(),
                                    Name = "Đi bộ",
                                    Distance = linkStation.Distance
                                };
                                midRoute.StationList.Add(secondStation.AsStationViewModel());
                                midRoute.StationList.Add(firstStation.AsStationViewModel());
                                resultPath.Add(midRoute);
                                currentStation = firstStation;
                            }
                        }
                        tmpNode = tmpNode.Parent;
                    }
                }
                if (check)
                {
                    var lastRoute = new RouteViewModel()
                    {
                        StationList = new List<StationViewModel>(),
                        Name = "Đi bộ",
                        Distance = (decimal)minDisStart
                    };
                    lastRoute.StationList.Add(currentStation.AsStationViewModel());
                    lastRoute.StationList.Add(customerStartPoint.AsStationViewModel());
                    resultPath.Add(lastRoute);
                    result.Add(resultPath);
                }
            }

            return result;
        }
    }
}