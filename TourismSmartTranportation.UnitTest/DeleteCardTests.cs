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
    public class DeleteCarđTest
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
            var result = _service.Delete(new Guid("f1d74956-cd18-4244-b906-a6788f9ecb17")).Result;
            Assert.IsTrue(result.StatusCode==201 && result.Message.Equals("Cập nhật trạng thái thành công!"));
        }

        [Test]
        [Order(2)]
        public  void UTCID02()
        {
            var result =  _service.Delete(new Guid("f1d74956-cd18-4244-b906-a6788f9ecb18")).Result;
            Assert.IsTrue(result.StatusCode == 404 && result.Message.Equals("Không tìm thấy!"));
        }


        [Test]
        [Order(3)]
        public void UTCID03()
        {
            Assert.Throws<FormatException>(() => { var result = _service.Delete(new Guid("")).Result; });

        }
    }
}