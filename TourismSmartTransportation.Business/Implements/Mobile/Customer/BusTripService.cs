using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using GeoCoordinatePortable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Implements.Admin;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.Order;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.OrderDetail;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.SearchModel.Shared.NotificationCollection;
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
        private IOrderHelpersService orderheplper;
        private IFirebaseCloudMsgService _firebaseCloud;
        private INotificationCollectionService _notificationCollection;
        private IPackageService _packageService;
        private static IConfiguration _configuration;
        public BusTripService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, IOrderHelpersService orderHelpers, IFirebaseCloudMsgService firebaseCloud, INotificationCollectionService notificationCollection, IPackageService packageService, IConfiguration configuration) : base(unitOfWork, blobServiceClient)
        {
            orderheplper = orderHelpers;
            _firebaseCloud = firebaseCloud;
            _notificationCollection = notificationCollection;
            _packageService = packageService;
            _configuration = configuration;
        }

        public async Task<List<List<RouteViewModel>>> FindBusTrip(BusTripSearchModel model)
        {

            string mapboxUri = _configuration["MapBox:uri"];
            string endMapboxUri = _configuration["MapBox:endUri"];
            var result = new List<List<RouteViewModel>>();
            var stationList = await _unitOfWork.StationRepository.Query().ToArrayAsync();
            List<Station> startList = new List<Station>();
            List<Station> endList = new List<Station>();
            Station startStation = null;
            Station endStation = null;
            double minDisStart = double.MaxValue;
            double minDisEnd = double.MaxValue;
            double[] distances = new double[stationList.Length];
            IDictionary<Guid, double> disStart = new Dictionary<Guid, double>();
            IDictionary<Guid, double> disEnd = new Dictionary<Guid, double>();
            HttpClient client = new HttpClient();

            Debug.WriteLine("Start " + DateTime.Now);
            //đo dduong chim bay
            for (int i=0; i<stationList.Length;i++)
            {
                var fromLocation = new GeoCoordinate((double)model.StartLatitude, (double)model.StartLongitude);
                var toLocation = new GeoCoordinate((double)stationList[i].Latitude,(double) stationList[i].Longitude);
                disStart.Add(stationList[i].StationId ,fromLocation.GetDistanceTo(toLocation));
            }
            disStart.Values.CopyTo(distances, 0);
            Array.Sort(distances, stationList);
            string startUri = mapboxUri;
            for (int i = 0; i < 5; i++)
            {
                startUri += model.StartLongitude + "," + model.StartLatitude+";"+ stationList[i].Longitude + "," + stationList[i].Latitude+";";
            }
            startUri = startUri.TrimEnd(';') + endMapboxUri;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(startUri)
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var jmessage = JObject.Parse(body);
                for (int i = 0; i < 5; i++)
                {
                    var distance = double.Parse(jmessage["routes"][0]["legs"][i * 2]["distance"].ToString());
                    if (distance < minDisStart)
                    {
                        minDisStart = distance;
                        startStation = stationList[i];
                    }
                }
            }


            //đo dduong chim bay
            for (int i = 0; i < stationList.Length; i++)
            {
                var fromLocation = new GeoCoordinate((double)model.EndLatitude, (double)model.EndLongitude);
                var toLocation = new GeoCoordinate((double)stationList[i].Latitude, (double)stationList[i].Longitude);
                disEnd.Add(stationList[i].StationId, fromLocation.GetDistanceTo(toLocation));
            }
            disEnd.Values.CopyTo(distances, 0);
            Array.Sort(distances, stationList);
            string endUri = mapboxUri;
            for (int i = 0; i < 5; i++)
            {
                endUri += model.EndLongitude + "," + model.EndLatitude + ";" + stationList[i].Longitude + "," + stationList[i].Latitude+";";
            }
            endUri = endUri.TrimEnd(';') + endMapboxUri;
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(endUri)
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var jmessage = JObject.Parse(body);
                for (int i = 0; i < 5; i++)
                {
                    var distance = double.Parse(jmessage["routes"][0]["legs"][i * 2]["distance"].ToString());
                    if (distance < minDisEnd)
                    {
                        minDisEnd = distance;
                        endStation = stationList[i];
                    }
                }
            }



            
            startList.Add(startStation);
            int linkStationCount = 0;
            var linkStartStationList = await _unitOfWork.LinkStationRepository.Query().Where(x => x.FirstStationId.Equals(startStation.StationId)).ToListAsync();
            foreach(var x in linkStartStationList)
            {
                var stationRoutesTmp = await _unitOfWork.StationRouteRepository.Query().Where(y => y.StationId.Equals(x.SecondStationId)).ToListAsync();
                foreach(var y in stationRoutesTmp)
                {
                    var nextStation = await _unitOfWork.StationRouteRepository.Query().Where(z => z.RouteId.Equals(y.RouteId) && z.OrderNumber == y.OrderNumber + 1).FirstOrDefaultAsync();
                    if(nextStation != null)
                    {
                        linkStationCount++;
                        startList.Add(await _unitOfWork.StationRepository.GetById(y.StationId));
                        break;
                        
                    }
                }
            }
            linkStartStationList = await _unitOfWork.LinkStationRepository.Query().Where(x => x.SecondStationId.Equals(startStation.StationId)).ToListAsync();
                foreach (var x in linkStartStationList)
                {
                    var stationRoutesTmp = await _unitOfWork.StationRouteRepository.Query().Where(y => y.StationId.Equals(x.FirstStationId)).ToListAsync();
                    foreach (var y in stationRoutesTmp)
                    {
                        var nextStation = await _unitOfWork.StationRouteRepository.Query().Where(z => z.RouteId.Equals(y.RouteId) && z.OrderNumber == y.OrderNumber + 1).FirstOrDefaultAsync();
                        if (nextStation != null)
                        {

                            linkStationCount++;
                            startList.Add(await _unitOfWork.StationRepository.GetById(y.StationId));
                            break;
                            
                        }
                    }
                }
            if (linkStationCount == 0)
            {
                for (int i = 0; i < stationList.Length; i++)
                {
                    var fromLocation = new GeoCoordinate((double)startStation.Latitude, (double)startStation.Longitude);
                    var toLocation = new GeoCoordinate((double)stationList[i].Latitude, (double)stationList[i].Longitude);
                    if (fromLocation.GetDistanceTo(toLocation) <= 200)
                    {
                        var stationRoutesTmp = await _unitOfWork.StationRouteRepository.Query().Where(y => y.StationId.Equals(stationList[i])).ToListAsync();
                        foreach (var y in stationRoutesTmp)
                        {
                            var nextStation = await _unitOfWork.StationRouteRepository.Query().Where(z => z.RouteId.Equals(y.RouteId) && z.OrderNumber == y.OrderNumber + 1).FirstOrDefaultAsync();
                            if (nextStation != null)
                            {
                                startList.Add(await _unitOfWork.StationRepository.GetById(y.StationId));
                                break;
                            }
                        }
                    }
                }
            }
            endList.Add(endStation);
            linkStationCount = 0;
            var linkEndStationList = await _unitOfWork.LinkStationRepository.Query().Where(x => x.FirstStationId.Equals(endStation.StationId)).ToListAsync();
            foreach (var x in linkEndStationList)
            {
                var stationRoutesTmp = await _unitOfWork.StationRouteRepository.Query().Where(y => y.StationId.Equals(x.SecondStationId)).ToListAsync();
                foreach (var y in stationRoutesTmp)
                {
                    var nextStation = await _unitOfWork.StationRouteRepository.Query().Where(z => z.RouteId.Equals(y.RouteId) && z.OrderNumber == y.OrderNumber + 1).FirstOrDefaultAsync();
                    if (nextStation != null)
                    {
                        linkStationCount++;
                        endList.Add(await _unitOfWork.StationRepository.GetById(y.StationId));
                        break;
                    }
                }
            }

            linkEndStationList = await _unitOfWork.LinkStationRepository.Query().Where(x => x.SecondStationId.Equals(endStation.StationId)).ToListAsync();
            foreach (var x in linkEndStationList)
            {
                var stationRoutesTmp = await _unitOfWork.StationRouteRepository.Query().Where(y => y.StationId.Equals(x.FirstStationId)).ToListAsync();
                foreach (var y in stationRoutesTmp)
                {
                    var nextStation = await _unitOfWork.StationRouteRepository.Query().Where(z => z.RouteId.Equals(y.RouteId) && z.OrderNumber == y.OrderNumber + 1).FirstOrDefaultAsync();
                    if (nextStation != null)
                    {
                        if ((double)disStart[nextStation.StationId] < (double)disStart[y.StationId])
                        {
                            linkStationCount++;
                            endList.Add(await _unitOfWork.StationRepository.GetById(y.StationId));
                            break;
                        }
                    }
                }
            }

            if (linkStationCount == 0)
            {
                for (int i = 0; i < stationList.Length; i++)
                {
                    var fromLocation = new GeoCoordinate((double)endStation.Latitude, (double)endStation.Longitude);
                    var toLocation = new GeoCoordinate((double)stationList[i].Latitude, (double)stationList[i].Longitude);
                    if (fromLocation.GetDistanceTo(toLocation) <= 200)
                    {
                        var stationRoutesTmp = await _unitOfWork.StationRouteRepository.Query().Where(y => y.StationId.Equals(stationList[i])).ToListAsync();
                        foreach (var y in stationRoutesTmp)
                        {
                            var nextStation = await _unitOfWork.StationRouteRepository.Query().Where(z => z.RouteId.Equals(y.RouteId) && z.OrderNumber == y.OrderNumber + 1).FirstOrDefaultAsync();
                            if (nextStation != null)
                            {
                                startList.Add(await _unitOfWork.StationRepository.GetById(y.StationId));
                                break;
                            }
                        }
                    }
                }
            }

            Debug.WriteLine("Perpart " + DateTime.Now);
            int LinkRouteCount = await _unitOfWork.LinkRouteRepository.Query().CountAsync();
            int routeCount = await _unitOfWork.RouteRepository.Query().CountAsync();

            HashSet<string> checkAlreadyRoute = new HashSet<string>();
            foreach (Station start in startList)
            {
                foreach (Station end in endList)
                {                   

                    Hashtable countAppearRouteList = new Hashtable();
                    var startRouteList = await _unitOfWork.StationRouteRepository.Query().Where(x => x.StationId.Equals(start.StationId)).ToListAsync();
                    var endRouteList = await _unitOfWork.StationRouteRepository.Query().Where(x => x.StationId.Equals(end.StationId)).ToListAsync();

                    Queue<Node> queue = new Queue<Node>();
                    var resultPathList = new List<Node>();
                    foreach (StationRoute startRoute in startRouteList)
                    {
                        foreach (StationRoute endRoute in endRouteList)
                        {
                            if (!checkAlreadyRoute.Add(startRoute.RouteId.ToString() + endRoute.RouteId.ToString()))
                            {
                                break;
                            }
                            queue.Clear();
                            queue.Enqueue(new Node(startRoute.RouteId, null, Guid.Empty));
                            countAppearRouteList.Clear();
                            countAppearRouteList.Add(startRoute.RouteId, LinkRouteCount);
                            Guid stationLink = Guid.Empty;
                            if (!endRoute.RouteId.Equals(startRoute.RouteId))
                            {

                                while (queue.Count != 0)
                                {
                                    var curentRoute = queue.Dequeue();
                                    if(curentRoute.Value.Equals(new Guid("d36c0d82-d788-407c-94c4-68bda44b5ea1")))
                                    {
                                        Debug.WriteLine("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                                    }
                                    if (!curentRoute.Value.Equals(endRoute.RouteId))
                                    {
                                        var linkRouteList = await _unitOfWork.LinkRouteRepository.Query().Where(x => x.FirstRouteId.Equals(curentRoute.Value) || x.SecondRouteId.Equals(curentRoute.Value)).ToListAsync();
                                        foreach (LinkRoute route in linkRouteList)
                                        {
                                            bool checkLink = true;
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

                                                if (route.StationId != null)
                                                {
                                                    if (route.StationId.Equals(curentRoute.StationId))
                                                    {
                                                        checkLink = false;
                                                    }
                                                    else
                                                    {
                                                        stationLink = route.StationId.Value;
                                                    }
                                                }
                                                else
                                                {
                                                    var linkStation = await _unitOfWork.LinkStationRepository.GetById(route.LinkStationId.Value);
                                                    if (linkStation.FirstStationId.Equals(curentRoute.StationId) || linkStation.SecondStationId.Equals(curentRoute.StationId))
                                                    {
                                                        checkLink = false;
                                                    }
                                                    else
                                                    {
                                                        stationLink = linkStation.FirstStationId;
                                                        var stationRoute = await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(routeId) && x.StationId.Equals(linkStation.FirstStationId)).FirstOrDefaultAsync();
                                                        if (stationRoute == null)
                                                        {
                                                            stationLink = linkStation.SecondStationId;
                                                        }

                                                    }
                                                }
                                                if (check && checkLink &&curentRoute.Count < routeCount && !routeId.Equals(startRoute.RouteId))
                                                {
                                                    Debug.WriteLine(routeId);
                                                    queue.Enqueue(new Node(routeId, curentRoute, stationLink));
                                                    countAppearRouteList[routeId] = ((int)countAppearRouteList[routeId]) - 1;
                                                }
                                            }


                                        }
                                    }
                                    else
                                    {
                                        resultPathList.Add(curentRoute);
                                    }
                                    if (countAppearRouteList[endRoute.RouteId] != null && ((int)countAppearRouteList[endRoute.RouteId]) == -1)
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                resultPathList.Add(new Node(startRoute.RouteId, null, Guid.Empty));
                            }
                        }
                    }

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
                    Debug.WriteLine("Done " + DateTime.Now);
                    foreach (Node node in resultPathList)
                    {
                        var checkSet = new HashSet<Guid>();
                        var tmpNode = node;
                        var resultPath = new List<RouteViewModel>();
                        var firstRoute = new RouteViewModel()
                        {
                            StationList = new List<StationViewModel>(),
                            Name = "Đi bộ",
                            Distance = (decimal)minDisEnd
                        };
                        firstRoute.StationList.Add(customerEndPoint.AsStationViewModel());
                        firstRoute.StationList.Add(end.AsStationViewModel());
                        resultPath.Add(firstRoute);
                        var currentStation = end;
                        bool check = true;
                        while (tmpNode != null)
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
                                    if (stationRouteOld.OrderNumber > stationRouteNew.OrderNumber)
                                    {
                                        int j = 0;
                                        for (int i = stationRouteOld.OrderNumber; i != stationRouteNew.OrderNumber - 1; i--)
                                        {
                                            var station = await _unitOfWork.StationRepository.GetById(stationListOfRoute[i].StationId);
                                            route.StationList.Add(station.AsStationViewModel());
                                            j++;
                                        }
                                        if (j == 1)
                                        {
                                            check = false;
                                        }
                                        route.Distance = Math.Abs(stationRouteOld.Distance - stationRouteNew.Distance);
                                        resultPath.Add(route);
                                        currentStation = await _unitOfWork.StationRepository.GetById(path.StationId.Value);
                                    }
                                    else
                                    {
                                        check = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    var linkStation = await _unitOfWork.LinkStationRepository.GetById(path.LinkStationId.Value);
                                    var firstStation = await _unitOfWork.StationRepository.GetById(linkStation.FirstStationId);
                                    var secondStation = await _unitOfWork.StationRepository.GetById(linkStation.SecondStationId);
                                    var checkFirstStation= await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(tmpNode.Value) && x.StationId.Equals(firstStation.StationId)).FirstOrDefaultAsync();
                                    if (checkFirstStation!=null)
                                    {
                                        var stationRouteNew = await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(tmpNode.Value) && x.StationId.Equals(firstStation.StationId)).FirstOrDefaultAsync();
                                        var stationRouteOld = await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(tmpNode.Value) && x.StationId.Equals(currentStation.StationId)).FirstOrDefaultAsync();
                                        if (stationRouteOld.OrderNumber > stationRouteNew.OrderNumber)
                                        {
                                            for (int i = stationRouteOld.OrderNumber ; i != stationRouteNew.OrderNumber - 1; i--)
                                            {
                                                var station = await _unitOfWork.StationRepository.GetById(stationListOfRoute[i].StationId);
                                                route.StationList.Add(station.AsStationViewModel());

                                            }
                                            //route.StationList.Add(secondStation.AsStationViewModel());
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
                                            check = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        var stationRouteNew = await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(tmpNode.Value) && x.StationId.Equals(secondStation.StationId)).FirstOrDefaultAsync();
                                        var stationRouteOld = await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(tmpNode.Value) && x.StationId.Equals(currentStation.StationId)).FirstOrDefaultAsync();
                                      //  int step = stationRouteOld.OrderNumber < stationRouteNew.OrderNumber ? 1 : -1;
                                        if (stationRouteOld.OrderNumber > stationRouteNew.OrderNumber)
                                        {
                                            for (int i = stationRouteOld.OrderNumber ; i != stationRouteNew.OrderNumber -1; i --)
                                            {
                                                var station = await _unitOfWork.StationRepository.GetById(stationListOfRoute[i].StationId);
                                                route.StationList.Add(station.AsStationViewModel());
                                            }
                                            //route.StationList.Add(firstStation.AsStationViewModel());
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
                                        else
                                        {
                                            check = false;
                                            break;
                                        }
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
                           /* if (result.Count >= 2)
                            {
                                break;
                            }*/
                        }
                    }
                    /*if (result.Count >= 2)
                    {
                        break;
                    }
                    Debug.WriteLine("truy vet " + DateTime.Now);*/
                }
               /* if (result.Count >= 2)
                {
                    break;
                }*/
            }

            return result;
        }

        public async Task<Response> PayWithIOT(BusPaySearchModel model)
        {

            var customerId = (await _unitOfWork.CardRepository.Query().Where(x => x.Uid.Equals(model.Uid)).FirstOrDefaultAsync()).CustomerId;
            var customer = await _unitOfWork.CustomerRepository.GetById(customerId.Value);
            var vehicle = await _unitOfWork.VehicleRepository.GetById(model.VehicleId);
            var today = DateTime.UtcNow.AddHours(7);
            var oldCustomerTrip = await _unitOfWork.CustomerTripRepository.Query().Where(x => x.CustomerId.Equals(customerId.Value) && x.Status == 1 && x.VehicleId.Equals(vehicle.VehicleId)).OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync();
            var serviceType = await _unitOfWork.ServiceTypeRepository.Query().Where(x => x.Name.Contains(ServiceTypeDefaultData.BUS_SERVICE_NAME)).FirstOrDefaultAsync();
            if (oldCustomerTrip != null && DateTime.UtcNow.TimeOfDay.TotalMinutes - oldCustomerTrip.CreatedDate.TimeOfDay.TotalMinutes < double.Parse(_configuration["TripTimeOut"]))
            {
                var location = oldCustomerTrip.Coordinates.Split(';');
                decimal startLongitude = decimal.Parse(location[0]);
                decimal startLatitude = decimal.Parse(location[1]);
                var tripEntity = await _unitOfWork.TripRepository.GetById(oldCustomerTrip.TripId.Value);
                var stationRouteList = await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(tripEntity.RouteId)).ToListAsync();
                decimal minDisStart = decimal.MaxValue;
                decimal minDisEnd = decimal.MaxValue;
                StationRoute startStation = null;
                StationRoute endStation = null;
                foreach (StationRoute x in stationRouteList)
                {
                    var station = await _unitOfWork.StationRepository.GetById(x.StationId);
                    decimal disStart = (decimal)Math.Sqrt((double)(Math.Abs(station.Longitude - startLongitude) * Math.Abs(station.Longitude - startLongitude) + Math.Abs(station.Latitude - startLatitude) * Math.Abs(station.Latitude - startLatitude)));
                    decimal disEnd = (decimal)Math.Sqrt((double)(Math.Abs(station.Longitude - model.Longitude) * Math.Abs(station.Longitude - model.Longitude) + Math.Abs(station.Latitude - model.Latitude) * Math.Abs(station.Latitude - model.Latitude)));
                    if (disStart < minDisStart)
                    {
                        minDisStart = disStart;
                        startStation = x;
                    }
                    if (disEnd < minDisEnd)
                    {
                        minDisEnd = disEnd;
                        endStation = x;
                    }
                }
                int totalStation = Math.Abs(endStation.OrderNumber - startStation.OrderNumber) + 1;
                decimal distance = Math.Abs(endStation.Distance - startStation.Distance);
                var routePriceList = await _unitOfWork.RoutePriceBusingRepository.Query().Where(x => x.RouteId.Equals(tripEntity.RouteId)).ToListAsync();
                decimal refundPrice = 0;
                foreach (RoutePriceBusing x in routePriceList)
                {
                    var price = await _unitOfWork.PriceOfBusServiceRepository.GetById(x.PriceBusingId);
                    if (price.Mode.Equals("distance"))
                    {
                        if (price.MinDistance <= distance && price.MaxDistance >= distance)
                        {
                            refundPrice = price.Price;
                            break;
                        }
                    }
                    else
                    {
                        if (price.MinStation <= totalStation && price.MaxStation >= totalStation)
                        {
                            refundPrice = price.Price;
                        }
                    }
                }
                var order = await _unitOfWork.OrderRepository.Query().Where(x => x.CustomerId.Equals(customerId) && x.ServiceTypeId.Equals(serviceType.ServiceTypeId)).OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync();
                refundPrice = order.TotalPrice - refundPrice;
                if (refundPrice > 0)
                {

                    order.TotalPrice = order.TotalPrice - refundPrice;
                    order.Status = (int)OrderStatus.Done;
                    _unitOfWork.OrderRepository.Update(order);

                    oldCustomerTrip.Coordinates = oldCustomerTrip.Coordinates + "&" + model.Longitude + ";" + model.Latitude;
                    oldCustomerTrip.Status = (int)CustomerTripStatus.Done;
                    _unitOfWork.CustomerTripRepository.Update(oldCustomerTrip);

                    var wallet = await _unitOfWork.WalletRepository.Query().Where(x => x.CustomerId.Equals(customerId)).FirstOrDefaultAsync();
                    var transaction = new Transaction()
                    {
                        WalletId = wallet.WalletId,
                        Amount = refundPrice,
                        Content = "Hoàn trả tiền dư từ dịch vụ xe buýt",
                        CreatedDate = DateTime.Now,
                        OrderId = order.OrderId,
                        Status = 1,
                        TransactionId = Guid.NewGuid()
                    };
                    wallet.AccountBalance += refundPrice;
                    _unitOfWork.WalletRepository.Update(wallet);
                    await _unitOfWork.TransactionRepository.Add(transaction);

                    // Add amout to partner wallet
                    var partnerWallet = await _unitOfWork.WalletRepository
                                        .Query()
                                        .Where(x => x.PartnerId == order.PartnerId)
                                        .FirstOrDefaultAsync();

                    var partnerTransaction = new Transaction()
                    {
                        TransactionId = Guid.NewGuid(),
                        Content = $"Đối tác gửi lại "+(decimal.Parse(_configuration["Partner"])*100)+"% tiền thừa",
                        OrderId = order.OrderId,
                        CreatedDate = DateTime.Now,
                        Amount = -(refundPrice * decimal.Parse(_configuration["Partner"])),
                        Status = 1,
                        WalletId = partnerWallet.WalletId
                    };
                    partnerWallet.AccountBalance -= partnerTransaction.Amount;
                    await _unitOfWork.TransactionRepository.Add(partnerTransaction);
                    _unitOfWork.WalletRepository.Update(partnerWallet);

                    // Add amout to admin wallet
                    var adminWallet = await _unitOfWork.WalletRepository
                                        .Query()
                                        .Where(x => x.PartnerId == null && x.CustomerId == null)
                                        .FirstOrDefaultAsync();

                    var adminTransaction = new Transaction()
                    {
                        TransactionId = Guid.NewGuid(),
                        Content = $"Hệ Thống Gửi lại " + (decimal.Parse(_configuration["Admin"]) * 100) + "% tiền thừa",
                        OrderId = order.OrderId,
                        CreatedDate = DateTime.Now,
                        Amount = -(refundPrice * decimal.Parse(_configuration["Admin"])),
                        Status = 1,
                        WalletId = adminWallet.WalletId
                    };
                    adminWallet.AccountBalance -= adminTransaction.Amount;
                    await _unitOfWork.TransactionRepository.Add(adminTransaction);
                    _unitOfWork.WalletRepository.Update(adminWallet);

                    oldCustomerTrip.Status = (int)CustomerTripStatus.Done;
                    oldCustomerTrip.Distance = distance;
                    _unitOfWork.CustomerTripRepository.Update(oldCustomerTrip);
                    await _unitOfWork.SaveChangesAsync();
                    CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
                    var mesReturnPrice = string.Format(elGR,"Quý khách vừa được hoàn {0:N0} VNĐ từ hệ thống cho dịch vụ xe buýt", refundPrice);
                    await _firebaseCloud.SendNotificationForRentingService(customer.RegistrationToken, "Hoàn phí dịch vụ xe buýt", mesReturnPrice);
                    SaveNotificationModel noti = new SaveNotificationModel()
                    {
                        CustomerId = customer.CustomerId.ToString(),
                        CustomerFirstName = customer.FirstName,
                        CustomerLastName = customer.LastName,
                        Title = "Hoàn phí dịch vụ xe buýt",
                        Message = mesReturnPrice,
                        Type = "Refund",
                        Status = (int)NotificationStatus.Active
                    };
                    await _notificationCollection.SaveNotification(noti);

                }
                // trường hợp order có total price = 0 là xài gói dịch vụ, 
                // tránh trường hợp order có total price != 0 và refundPrice < 0
                else if (order.TotalPrice == 0)
                {

                    // cập nhật lại trạng thái của order -> hoàn thành
                    order.Status = (int)OrderStatus.Done;
                    _unitOfWork.OrderRepository.Update(order);
                    

                    // Lấy full giá của trip
                    var route = await _unitOfWork.RouteRepository.GetById(tripEntity.RouteId);
                    var routePriceBusing = await _unitOfWork.RoutePriceBusingRepository.Query().Where(x => x.RouteId.Equals(route.RouteId)).FirstOrDefaultAsync();
                    var priceBusing = await _unitOfWork.PriceOfBusServiceRepository.GetById(routePriceBusing.PriceBusingId);
                    var basePrice = await _unitOfWork.BasePriceOfBusServiceRepository.GetById(priceBusing.BasePriceId);
                    var priceAfterRefunding = basePrice.Price - (-refundPrice); // tính tiền dựa trên khoảng cách mà khách hàng đi được

                    // Cập nhật ví lại cho partner
                    var partnerWallet = await _unitOfWork.WalletRepository
                                        .Query()
                                        .Where(x => x.PartnerId == order.PartnerId)
                                        .FirstOrDefaultAsync();

                    var partnerTransaction = new Transaction()
                    {
                        TransactionId = Guid.NewGuid(),
                        Content = $"Đối tác gửi tiền hoa hồng lại cho hệ thống",
                        OrderId = order.OrderId,
                        CreatedDate = DateTime.Now,
                        Amount = -priceAfterRefunding * decimal.Parse(_configuration["Admin"]), // xét dấu âm cho giao dịch trừ tiền
                        Status = 1,
                        WalletId = partnerWallet.WalletId
                    };
                    partnerWallet.AccountBalance = partnerWallet.AccountBalance - (-partnerTransaction.Amount); // trừ tiền từ transaction
                    await _unitOfWork.TransactionRepository.Add(partnerTransaction);
                    _unitOfWork.WalletRepository.Update(partnerWallet);

                    // Cập nhật ví lại cho Admin
                    var adminWallet = await _unitOfWork.WalletRepository
                                        .Query()
                                        .Where(x => x.PartnerId == null && x.CustomerId == null)
                                        .FirstOrDefaultAsync();

                    var adminTransaction = new Transaction()
                    {
                        TransactionId = Guid.NewGuid(),
                        Content = $"Hệ thống nhận tiền hoa hồng từ đối tác",
                        OrderId = order.OrderId,
                        CreatedDate = DateTime.Now,
                        Amount = priceAfterRefunding * decimal.Parse(_configuration["Admin"]),
                        Status = 1,
                        WalletId = adminWallet.WalletId
                    };
                    adminWallet.AccountBalance += adminTransaction.Amount;
                    await _unitOfWork.TransactionRepository.Add(adminTransaction);
                    _unitOfWork.WalletRepository.Update(adminWallet);

                    // cập nhật thêm vị trí xuống trạm và trạng thái -> hoàn thành của customer trip
                    oldCustomerTrip.Coordinates = oldCustomerTrip.Coordinates + "&" + model.Longitude + ";" + model.Latitude;
                    oldCustomerTrip.Status = (int)CustomerTripStatus.Done;
                    oldCustomerTrip.Distance = distance;
                    _unitOfWork.CustomerTripRepository.Update(oldCustomerTrip);

                    await _unitOfWork.SaveChangesAsync();
                    CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
                    var mesReturnPrice = string.Format(elGR,"Quý khách vừa sử dụng dịch vụ xe buýt hết {0:N0} Km", distance/1000);
                    await _firebaseCloud.SendNotificationForRentingService(customer.RegistrationToken, "Thông báo gói dịch vụ", mesReturnPrice);
                    SaveNotificationModel noti = new SaveNotificationModel()
                    {
                        CustomerId = customer.CustomerId.ToString(),
                        CustomerFirstName = customer.FirstName,
                        CustomerLastName = customer.LastName,
                        Title = "Thông báo gói dịch vụ",
                        Message = mesReturnPrice,
                        Type = "Package",
                        Status = (int)NotificationStatus.Active
                    };
                    await _notificationCollection.SaveNotification(noti);


                }


                return new()
                {
                    StatusCode = 204,
                    Message = "Đã hoàn trả tiền dư"
                };
            }
            else
            {


                var trip = await _unitOfWork.TripRepository.Query().Where(x => x.VehicleId.Equals(model.VehicleId) && ((int)today.DayOfWeek % 7) == (x.DayOfWeek - 1) % 7 && today.ToString("HH:mm").CompareTo(x.TimeStart) >= 0 && today.ToString("HH:mm").CompareTo(x.TimeEnd) <= 0).FirstOrDefaultAsync();


                var route = await _unitOfWork.RouteRepository.GetById(trip.RouteId);
                var routePriceBusing = await _unitOfWork.RoutePriceBusingRepository.Query().Where(x => x.RouteId.Equals(route.RouteId)).FirstOrDefaultAsync();
                var priceBusing = await _unitOfWork.PriceOfBusServiceRepository.GetById(routePriceBusing.PriceBusingId);
                var basePrice = await _unitOfWork.BasePriceOfBusServiceRepository.GetById(priceBusing.BasePriceId);
                priceBusing = await _unitOfWork.PriceOfBusServiceRepository.Query().Where(x => x.BasePriceId.Equals(basePrice.BasePriceOfBusServiceId)).OrderByDescending(x => x.MaxStation).FirstOrDefaultAsync();


                OrderDetailsInfo orderDetails = new OrderDetailsInfo()
                {
                    Content = ServiceTypeDefaultData.BUS_SERVICE_CONTENT,
                    Price = basePrice.Price,
                    Quantity = 1,
                    PriceOfBusServiceId = priceBusing.PriceOfBusServiceId
                };
                var orderDetailList = new List<OrderDetailsInfo>();
                orderDetailList.Add(orderDetails);
                CreateOrderModel createOrder = new CreateOrderModel()
                {
                    CustomerId = customerId.Value,
                    PartnerId = vehicle.PartnerId,
                    ServiceTypeId = serviceType.ServiceTypeId,
                    TotalPrice = basePrice.Price,
                    OrderDetailsInfos = orderDetailList,
                    Distance = route.Distance
                };
                var respone = await orderheplper.CreateOrder(createOrder);
                if (respone.StatusCode != 201)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Thanh toán thất bại"
                    };
                }
                var customerTrip = new CustomerTrip()
                {
                    CustomerTripId = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    CustomerId = customerId.Value,
                    TripId = trip.TripId,
                    VehicleId = model.VehicleId,
                    Distance = route.Distance,
                    Coordinates = model.Longitude + ";" + model.Latitude,
                    Status = 1
                };
                await _unitOfWork.CustomerTripRepository.Add(customerTrip);
                await _unitOfWork.SaveChangesAsync();
                var driver = await _unitOfWork.DriverRepository.GetById(trip.DriverId);
                var package= _packageService.GetCurrentPackageIsUsed(customer.CustomerId).Result;
                int quantityPeople = 1;
                if(package!= null)
                {
                    var packageTmp = await _unitOfWork.PackageRepository.GetById(package.PackageId);
                    quantityPeople = packageTmp.PeopleQuantity;
                }
                var mes = string.Format("Khách hàng {0} vừa lên xe. Số người lên xe {1} người", customer.FirstName+" "+customer.LastName , quantityPeople);
                await _firebaseCloud.SendNotificationForRentingService(driver.RegistrationToken, "Thông báo lên xe", mes);
                SaveNotificationModel noti = new SaveNotificationModel()
                {
                    CustomerId = driver.DriverId.ToString(),
                    CustomerFirstName = driver.FirstName,
                    CustomerLastName = driver.LastName,
                    Title = "Thông báo lên xe",
                    Message = mes,
                    Type = "Busing",
                    Status = (int)NotificationStatus.Active
                };
                await _notificationCollection.SaveNotification(noti);

                return new()
                {
                    StatusCode = 201,
                    Message = "Thanh toán thành công"
                };
            }
        }

        public async Task<Response> PayWithMobileApp(BusPaySearchModel model)
        {
            var vehicleId = new Guid(DecryptString(model.Uid));
            var vehicle = await _unitOfWork.VehicleRepository.GetById(vehicleId);
            var today = DateTime.UtcNow.AddHours(7);
            var customer = await _unitOfWork.CustomerRepository.GetById(model.CustomerId);

            var oldCustomerTrip = await _unitOfWork.CustomerTripRepository.Query().Where(x => x.CustomerId.Equals(model.CustomerId) && x.Status == 1 && x.VehicleId.Equals(vehicle.VehicleId)).OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync();
            var serviceType = await _unitOfWork.ServiceTypeRepository.Query().Where(x => x.Name.Contains(ServiceTypeDefaultData.BUS_SERVICE_NAME)).FirstOrDefaultAsync();
            if (oldCustomerTrip != null && DateTime.Now.TimeOfDay.TotalMinutes - oldCustomerTrip.CreatedDate.TimeOfDay.TotalMinutes < double.Parse(_configuration["TripTimeOut"]))
            {
                var location = oldCustomerTrip.Coordinates.Split(';');
                decimal startLongitude = decimal.Parse(location[0]);
                decimal startLatitude = decimal.Parse(location[1]);
                var tripEntity = await _unitOfWork.TripRepository.Query().Where(x => x.TripId == oldCustomerTrip.TripId).FirstOrDefaultAsync();
                var stationRouteList = await _unitOfWork.StationRouteRepository.Query().Where(x => x.RouteId.Equals(tripEntity.RouteId)).ToListAsync();
                decimal minDisStart = decimal.MaxValue;
                decimal minDisEnd = decimal.MaxValue;
                StationRoute startStation = null;
                StationRoute endStation = null;
                foreach (StationRoute x in stationRouteList)
                {
                    var station = await _unitOfWork.StationRepository.GetById(x.StationId);
                    decimal disStart = (decimal)Math.Sqrt((double)(Math.Abs(station.Longitude - startLongitude) * Math.Abs(station.Longitude - startLongitude) + Math.Abs(station.Latitude - startLatitude) * Math.Abs(station.Latitude - startLatitude)));
                    decimal disEnd = (decimal)Math.Sqrt((double)(Math.Abs(station.Longitude - model.Longitude) * Math.Abs(station.Longitude - model.Longitude) + Math.Abs(station.Latitude - model.Latitude) * Math.Abs(station.Latitude - model.Latitude)));
                    if (disStart < minDisStart)
                    {
                        minDisStart = disStart;
                        startStation = x;
                    }
                    if (disEnd < minDisEnd)
                    {
                        minDisEnd = disEnd;
                        endStation = x;
                    }
                }
                int totalStation = Math.Abs(endStation.OrderNumber - startStation.OrderNumber) + 1;
                decimal distance = Math.Abs(endStation.Distance - startStation.Distance);
                var routePriceList = await _unitOfWork.RoutePriceBusingRepository.Query().Where(x => x.RouteId.Equals(tripEntity.RouteId)).ToListAsync();
                decimal refundPrice = 0;
                foreach (RoutePriceBusing x in routePriceList)
                {
                    var price = await _unitOfWork.PriceOfBusServiceRepository.GetById(x.PriceBusingId);
                    if (price.Mode.Equals("distance"))
                    {
                        if (price.MinDistance <= distance && price.MaxDistance >= distance)
                        {
                            refundPrice = price.Price;
                            break;
                        }
                    }
                    else
                    {
                        if (price.MinStation <= totalStation && price.MaxStation >= totalStation)
                        {
                            refundPrice = price.Price;
                        }
                    }
                }
                var order = await _unitOfWork.OrderRepository.Query().Where(x => x.CustomerId.Equals(model.CustomerId) && x.ServiceTypeId.Equals(serviceType.ServiceTypeId)).OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync();
                refundPrice = order.TotalPrice - refundPrice;
                if (refundPrice > 0)
                {

                    order.TotalPrice = order.TotalPrice - refundPrice;
                    order.Status = (int)OrderStatus.Done;
                    _unitOfWork.OrderRepository.Update(order);

                    oldCustomerTrip.Coordinates = oldCustomerTrip.Coordinates + "&" + model.Longitude + ";" + model.Latitude;
                    oldCustomerTrip.Status = (int)CustomerTripStatus.Done;
                    _unitOfWork.CustomerTripRepository.Update(oldCustomerTrip);

                    var wallet = await _unitOfWork.WalletRepository.Query().Where(x => x.CustomerId.Equals(model.CustomerId)).FirstOrDefaultAsync();
                    var transaction = new Transaction()
                    {
                        WalletId = wallet.WalletId,
                        Amount = refundPrice,
                        Content = "Hoàn trả tiền dư",
                        CreatedDate = DateTime.Now,
                        OrderId = order.OrderId,
                        Status = 1,
                        TransactionId = Guid.NewGuid()
                    };
                    wallet.AccountBalance += refundPrice;
                    _unitOfWork.WalletRepository.Update(wallet);
                    await _unitOfWork.TransactionRepository.Add(transaction);

                    // Add amout to partner wallet
                    var partnerWallet = await _unitOfWork.WalletRepository
                                        .Query()
                                        .Where(x => x.PartnerId == order.PartnerId)
                                        .FirstOrDefaultAsync();

                    var partnerTransaction = new Transaction()
                    {
                        TransactionId = Guid.NewGuid(),
                        Content = $"Đối tác gửi lại " + (decimal.Parse(_configuration["Partner"]) * 100) + "% tiền thừa",
                        OrderId = order.OrderId,
                        CreatedDate = DateTime.Now,
                        Amount = -(refundPrice * decimal.Parse(_configuration["Partner"])), // xét dấu âm cho giao dịch trừ tiền
                        Status = 1,
                        WalletId = partnerWallet.WalletId
                    };
                    partnerWallet.AccountBalance = partnerWallet.AccountBalance - (-partnerTransaction.Amount); // trừ tiền từ transaction
                    await _unitOfWork.TransactionRepository.Add(partnerTransaction);
                    _unitOfWork.WalletRepository.Update(partnerWallet);

                    // Add amout to admin wallet
                    var adminWallet = await _unitOfWork.WalletRepository
                                        .Query()
                                        .Where(x => x.PartnerId == null && x.CustomerId == null)
                                        .FirstOrDefaultAsync();

                    var adminTransaction = new Transaction()
                    {
                        TransactionId = Guid.NewGuid(),
                        Content = $"Hệ Thống Gửi lại " + (decimal.Parse(_configuration["Admin"]) * 100) + "% tiền thừa",
                        OrderId = order.OrderId,
                        CreatedDate = DateTime.Now,
                        Amount = -(refundPrice * decimal.Parse(_configuration["Admin"])), // xét dấu âm cho giao dịch trừ tiền
                        Status = 1,
                        WalletId = adminWallet.WalletId
                    };
                    adminWallet.AccountBalance = adminWallet.AccountBalance - (-adminTransaction.Amount); // trừ tiền từ transaction
                    await _unitOfWork.TransactionRepository.Add(adminTransaction);
                    _unitOfWork.WalletRepository.Update(adminWallet);

                    oldCustomerTrip.Status = (int)CustomerTripStatus.Done;
                    oldCustomerTrip.Distance = distance;
                    _unitOfWork.CustomerTripRepository.Update(oldCustomerTrip);
                    await _unitOfWork.SaveChangesAsync();
                    CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
                    var mesReturnPrice = string.Format(elGR,"Quý khách vừa được hoàn {0:N0} VNĐ từ hệ thống cho dịch vụ xe buýt", refundPrice);
                    await _firebaseCloud.SendNotificationForRentingService(customer.RegistrationToken, "Hoàn phí dịch vụ xe buýt", mesReturnPrice);
                    SaveNotificationModel noti = new SaveNotificationModel()
                    {
                        CustomerId = customer.CustomerId.ToString(),
                        CustomerFirstName = customer.FirstName,
                        CustomerLastName = customer.LastName,
                        Title = "Hoàn phí dịch vụ xe buýt",
                        Message = mesReturnPrice,
                        Type = "Refund",
                        Status = (int)NotificationStatus.Active
                    };
                    await _notificationCollection.SaveNotification(noti);

                }
                // trường hợp order có total price = 0 là xài gói dịch vụ, 
                // tránh trường hợp order có total price != 0 và refundPrice < 0
                else if (order.TotalPrice == 0)
                {
                    
                    // cập nhật lại trạng thái của order -> hoàn thành
                    order.Status = (int)OrderStatus.Done;
                    _unitOfWork.OrderRepository.Update(order);

                    

                    // Lấy full giá của trip
                    var route = await _unitOfWork.RouteRepository.GetById(tripEntity.RouteId);
                    var routePriceBusing = await _unitOfWork.RoutePriceBusingRepository.Query().Where(x => x.RouteId.Equals(route.RouteId)).FirstOrDefaultAsync();
                    var priceBusing = await _unitOfWork.PriceOfBusServiceRepository.GetById(routePriceBusing.PriceBusingId);
                    var basePrice = await _unitOfWork.BasePriceOfBusServiceRepository.GetById(priceBusing.BasePriceId);
                    var priceAfterRefunding = basePrice.Price - (-refundPrice); // tính tiền dựa trên khoảng cách mà khách hàng đi được

                    // Cập nhật ví lại cho partner
                    var partnerWallet = await _unitOfWork.WalletRepository
                                        .Query()
                                        .Where(x => x.PartnerId == order.PartnerId)
                                        .FirstOrDefaultAsync();

                    var partnerTransaction = new Transaction()
                    {
                        TransactionId = Guid.NewGuid(),
                        Content = $"Đối tác gửi tiền hoa hồng lại cho hệ thống",
                        OrderId = order.OrderId,
                        CreatedDate = DateTime.Now,
                        Amount = -priceAfterRefunding * decimal.Parse(_configuration["Admin"]), // xét dấu âm cho giao dịch trừ tiền
                        Status = 1,
                        WalletId = partnerWallet.WalletId
                    };
                    partnerWallet.AccountBalance = partnerWallet.AccountBalance - (-partnerTransaction.Amount); // trừ tiền từ transaction
                    await _unitOfWork.TransactionRepository.Add(partnerTransaction);
                    _unitOfWork.WalletRepository.Update(partnerWallet);

                    // Cập nhật ví lại cho Admin
                    var adminWallet = await _unitOfWork.WalletRepository
                                        .Query()
                                        .Where(x => x.PartnerId == null && x.CustomerId == null)
                                        .FirstOrDefaultAsync();

                    var adminTransaction = new Transaction()
                    {
                        TransactionId = Guid.NewGuid(),
                        Content = $"Hệ thống nhận tiền hoa hồng từ đối tác",
                        OrderId = order.OrderId,
                        CreatedDate = DateTime.Now,
                        Amount = priceAfterRefunding * decimal.Parse(_configuration["Admin"]),
                        Status = 1,
                        WalletId = adminWallet.WalletId
                    };
                    adminWallet.AccountBalance += adminTransaction.Amount;
                    await _unitOfWork.TransactionRepository.Add(adminTransaction);
                    _unitOfWork.WalletRepository.Update(adminWallet);

                    // cập nhật thêm vị trí xuống trạm và trạng thái -> hoàn thành của customer trip
                    oldCustomerTrip.Coordinates = oldCustomerTrip.Coordinates + "&" + model.Longitude + ";" + model.Latitude;
                    oldCustomerTrip.Status = (int)CustomerTripStatus.Done;
                    oldCustomerTrip.Distance = distance;
                    _unitOfWork.CustomerTripRepository.Update(oldCustomerTrip);

                    await _unitOfWork.SaveChangesAsync();
                    CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
                    var mesReturnPrice = string.Format(elGR,"Quý khách vừa sử dụng dịch vụ xe buýt hết {0:N0} Km", distance/1000);
                    await _firebaseCloud.SendNotificationForRentingService(customer.RegistrationToken, "Thông báo gói dịch vụ", mesReturnPrice);
                    SaveNotificationModel noti = new SaveNotificationModel()
                    {
                        CustomerId = customer.CustomerId.ToString(),
                        CustomerFirstName = customer.FirstName,
                        CustomerLastName = customer.LastName,
                        Title = "Thông báo gói dịch vụ",
                        Message = mesReturnPrice,
                        Type = "Package",
                        Status = (int)NotificationStatus.Active
                    };
                    await _notificationCollection.SaveNotification(noti);


                }

                return new()
                {
                    StatusCode = 204,
                    Message = "Đã hoàn trả tiền dư"
                };
            }
            else
            {




                var trip = await _unitOfWork.TripRepository.Query().Where(x => x.VehicleId.Equals(vehicleId) && ((int)today.DayOfWeek % 7) == (x.DayOfWeek - 1) % 7 && today.ToString("HH:mm").CompareTo(x.TimeStart) >= 0 && today.ToString("HH:mm").CompareTo(x.TimeEnd) <= 0).FirstOrDefaultAsync();


                var route = await _unitOfWork.RouteRepository.GetById(trip.RouteId);
                var routePriceBusing = await _unitOfWork.RoutePriceBusingRepository.Query().Where(x => x.RouteId.Equals(route.RouteId)).FirstOrDefaultAsync();
                var priceBusing = await _unitOfWork.PriceOfBusServiceRepository.GetById(routePriceBusing.PriceBusingId);
                var basePrice = await _unitOfWork.BasePriceOfBusServiceRepository.GetById(priceBusing.BasePriceId);
                priceBusing = await _unitOfWork.PriceOfBusServiceRepository.Query().Where(x => x.BasePriceId.Equals(basePrice.BasePriceOfBusServiceId)).OrderByDescending(x => x.MaxStation).FirstOrDefaultAsync();


                OrderDetailsInfo orderDetails = new OrderDetailsInfo()
                {
                    Content = ServiceTypeDefaultData.BUS_SERVICE_CONTENT,
                    Price = basePrice.Price,
                    Quantity = 1,
                    PriceOfBusServiceId = priceBusing.PriceOfBusServiceId
                };
                var orderDetailList = new List<OrderDetailsInfo>();
                orderDetailList.Add(orderDetails);
                CreateOrderModel createOrder = new CreateOrderModel()
                {
                    CustomerId = model.CustomerId,
                    PartnerId = vehicle.PartnerId,
                    ServiceTypeId = serviceType.ServiceTypeId,
                    TotalPrice = basePrice.Price,
                    OrderDetailsInfos = orderDetailList,
                    Distance = route.Distance
                };
                var respone = await orderheplper.CreateOrder(createOrder);
                if (respone.StatusCode != 201)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Thanh toán thất bại"
                    };
                }
                var customerTrip = new CustomerTrip()
                {
                    CustomerTripId = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    CustomerId = model.CustomerId,
                    TripId = trip.TripId,
                    VehicleId = vehicleId,
                    Distance = route.Distance,
                    Coordinates = model.Longitude + ";" + model.Latitude,
                    Status = 1
                };
                await _unitOfWork.CustomerTripRepository.Add(customerTrip);
                await _unitOfWork.SaveChangesAsync();

                var driver = await _unitOfWork.DriverRepository.GetById(trip.DriverId);
                var package = _packageService.GetCurrentPackageIsUsed(customer.CustomerId).Result;
                int quantityPeople = 1;
                if (package != null)
                {
                    var packageTmp = await _unitOfWork.PackageRepository.GetById(package.PackageId);
                    quantityPeople = packageTmp.PeopleQuantity;
                }
                var mes = string.Format("Khách hàng {0} vừa lên xe. Số người lên xe {1} người", customer.FirstName + " " + customer.LastName, quantityPeople);
                await _firebaseCloud.SendNotificationForRentingService(driver.RegistrationToken, "Thông báo lên xe", mes);
                SaveNotificationModel noti = new SaveNotificationModel()
                {
                    CustomerId = driver.DriverId.ToString(),
                    CustomerFirstName = driver.FirstName,
                    CustomerLastName = driver.LastName,
                    Title = "Thông báo lên xe",
                    Message = mes,
                    Type = "Busing",
                    Status = (int)NotificationStatus.Active
                };
                await _notificationCollection.SaveNotification(noti);
                return new()
                {
                    StatusCode = 201,
                    Message = "Thanh toán thành công"
                };
            }
        }

        public static string DecryptString(string cipherText)
        {
            string key = _configuration["QRKey"];
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public async Task<BusPriceViewModel> GetPrice(string uid, Guid customerId)
        {
            var vehicleId = new Guid(DecryptString(uid));
            var vehicle = await _unitOfWork.VehicleRepository.GetById(vehicleId);
            var today = DateTime.UtcNow.AddHours(7);
            var trip = await _unitOfWork.TripRepository.Query().Where(x => x.VehicleId.Equals(vehicleId) && ((int)today.DayOfWeek % 7) == (x.DayOfWeek - 1) % 7 && today.ToString("HH:mm").CompareTo(x.TimeStart) >= 0 && today.ToString("HH:mm").CompareTo(x.TimeEnd) <= 0).FirstOrDefaultAsync();


            var route = await _unitOfWork.RouteRepository.GetById(trip.RouteId);
            var oldCustomerTrip = await _unitOfWork.CustomerTripRepository.Query().Where(x => x.CustomerId.Equals(customerId) && x.Status == 1 && x.VehicleId.Equals(vehicle.VehicleId)).OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync();
            if (oldCustomerTrip != null)
            {
                return null;
            }
            var routePriceBusing = await _unitOfWork.RoutePriceBusingRepository.Query().Where(x => x.RouteId.Equals(route.RouteId)).FirstOrDefaultAsync();
            var priceBusing = await _unitOfWork.PriceOfBusServiceRepository.GetById(routePriceBusing.PriceBusingId);
            var basePrice = await _unitOfWork.BasePriceOfBusServiceRepository.GetById(priceBusing.BasePriceId);
            var currentPackageIsUsed = await _packageService.GetCurrentPackageIsUsed(customerId);
            var busPrice = new BusPriceViewModel()
            {
                Name = route.Name,
                Distance = route.Distance,
                TotalStation = route.TotalStation,
                Price = basePrice.Price,
                IsUsePackage = currentPackageIsUsed != null
            };
            return busPrice;
        }
    }
}