using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Implements.Admin;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Data.Context;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Repositories;

namespace TourismSmartTranportation.UnitTest
{
    [TestFixture]
    public class CreateCarđTest
    {
        private ICardManagementService _service;
        ServiceProvider serviceProvider;



        [SetUp]
        public void Setup()
        {
            ServiceCollection service = new ServiceCollection();
            service.AddDbContext<tourismsmarttransportationContext>(
               options => options.UseSqlServer("Server=se32.database.windows.net;Database=tourism-smart-transportation;TrustServerCertificate=true;User Id=se32;Password=Password@32"));
            service.AddScoped(_ => new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=se32;AccountKey=C7L0vKzxnNTUYw6IUAnB/lBXm+VQNTLSlMq8HW52j1ayqbdW6RZBEb03WChlKHfLdZ2BceIyEIusNWM5/wtNWA==;EndpointSuffix=core.windows.net"));
            service.AddTransient<IUnitOfWork, UnitOfWork>();
            service.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            service.AddScoped<ICardManagementService, CardManagementService>();
            serviceProvider = service.BuildServiceProvider();
            _service = serviceProvider.GetService<ICardManagementService>();
        }

        [Test]
        [Order(1)]
        public void UTCID01()
        {
            var result = _service.Create("6700798A").Result;
            Assert.IsTrue(result.StatusCode==201 && result.Message.Equals("Tạo mới thẻ thành công!"));
        }

        [Test]
        [Order(2)]
        public  void UTCID02()
        {
            var result =  _service.Create("6700798A").Result;
            Assert.IsTrue(result.StatusCode == 400 && result.Message.Equals("Thẻ đã tồn tại!"));
        }

        [Test]
        [Order(3)]
        public  void UTCID03()
        {
             Assert.Throws<AggregateException>( ()=> { var result = _service.Create("02236700798").Result;});

        }

        [Test]
        [Order(4)]
        public void UTCID04()
        {
            Assert.Throws<AggregateException>(() => { var result = _service.Create(null).Result; });
        }
    }
}