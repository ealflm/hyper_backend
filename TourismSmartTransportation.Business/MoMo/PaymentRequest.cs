using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.MoMo
{
    class PaymentRequest
    {
        public PaymentRequest()
        {
        }
        public async static Task<string> sendPaymentRequest(string endpoint, string postJsonString)
        {

                HttpClient client = new HttpClient();
                var postData = postJsonString;
                var data = Encoding.UTF8.GetBytes(postData);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(endpoint),
                    Content=new StringContent(postJsonString, Encoding.UTF8, "application/json")
                };
                string jsonresponse = "";
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    jsonresponse = await response.Content.ReadAsStringAsync();
                }
                //todo parse it
                return jsonresponse;
                //return new MomoResponse(mtid, jsonresponse);
        }
    }
}
