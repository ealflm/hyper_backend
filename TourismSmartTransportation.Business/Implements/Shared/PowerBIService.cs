using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Shared;
using System;
using TourismSmartTransportation.Data.Interfaces;
using Azure.Storage.Blobs;
using TourismSmartTransportation.Business.ViewModel.Shared;
using Microsoft.Identity.Client;
using System.Security;
using Microsoft.PowerBI.Api;
using Microsoft.Rest;
using Microsoft.PowerBI.Api.Models;
using System.Collections.Generic;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;
using System.Net.Http.Headers;

namespace TourismSmartTransportation.Business.Implements.Vehicle
{
    public class PowerBIService :BaseService, IPowerBIService
    {
        public PowerBIService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, AzureViewModel azureViewModel) : base(unitOfWork, blobServiceClient)
        {
            this.azureViewModel = azureViewModel;
        }

        private AzureViewModel azureViewModel;


        public async Task<EmbedToken> GetToken()
        {
            /*string[] scopes = new string[] { "user.read" };
            var securePassword = new SecureString();
            foreach (char c in azureViewModel.Password) 
                securePassword.AppendChar(c);
            IPublicClientApplication app;
            app = PublicClientApplicationBuilder.Create(azureViewModel.ClientId)
                  .WithAuthority("https://login.windows.net/common/oauth2/token")
                  .Build();
            var result = await app.AcquireTokenByUsernamePassword(scopes, azureViewModel.Username, securePassword)
                      .ExecuteAsync();
            while(result.AccessToken == null)
            {

            }*/
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var requestBody= new Dictionary<string, string>();
            var url = "https://login.windows.net/common/oauth2/token";
            requestBody.Add("grant_type", azureViewModel.GrantType);
            requestBody.Add("resource", azureViewModel.Resource);
            requestBody.Add("username", azureViewModel.Username);
            requestBody.Add("password", azureViewModel.Password);
            requestBody.Add("client_id", azureViewModel.ClientId);
            var response= await httpClient.PostAsync(url, new FormUrlEncodedContent(requestBody));
            var responseBody= response.Content.ReadAsStringAsync().Result;
            int start= responseBody.IndexOf("\"access_token\":\"")+16;
            int end = responseBody.IndexOf("\"", start+1);
            var token = responseBody.Substring(start, end - start);
            var tokenCredentials = new TokenCredentials(token, "Bearer");
            PowerBIClient client = new PowerBIClient(new Uri("https://api.powerbi.com/"), tokenCredentials);
            GenerateTokenRequestV2 tokenRequestV2 = new GenerateTokenRequestV2();
            tokenRequestV2.Datasets = new List<GenerateTokenRequestV2Dataset>();
            tokenRequestV2.Reports = new List<GenerateTokenRequestV2Report>();
            tokenRequestV2.Datasets.Add(new GenerateTokenRequestV2Dataset(azureViewModel.DataSetId));
            tokenRequestV2.Reports.Add(new GenerateTokenRequestV2Report(new Guid(azureViewModel.ReportId)));
            var embedToken = await client.EmbedToken.GenerateTokenAsync(tokenRequestV2);
            return embedToken;

        }
    }
}