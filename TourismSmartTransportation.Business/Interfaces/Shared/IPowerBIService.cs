using System.Threading.Tasks;
using System.Collections.Generic;
using TourismSmartTransportation.Data.MongoCollections.Vehicle;
using TourismSmartTransportation.Business.CommonModel;
using System;
using Microsoft.PowerBI.Api.Models;

namespace TourismSmartTransportation.Business.Interfaces.Shared
{
    public interface IPowerBIService
    {
        public Task<EmbedToken> GetToken();
    }
}
