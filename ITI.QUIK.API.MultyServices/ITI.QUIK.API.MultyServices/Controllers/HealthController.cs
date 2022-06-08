using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ITI.QUIK.API.MultyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet("MyHealth")]
        public IActionResult IsOk()
        {
            return Ok("Ok");
        }

        [HttpGet("IsChildHealthOk")]
        public async Task<IActionResult> IsChildOk()
        {
            string _baseAddress = "http://172.16.6.11:8002/";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseAddress);
                //client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var responseMessage = await client.GetAsync(_baseAddress + "api/HealthState/OK");

                if (responseMessage.IsSuccessStatusCode)
                {
                    string responseBody = await responseMessage.Content.ReadAsStringAsync();
                    return Ok(responseBody);
                }
            }
            return NotFound();


            //HttpClient client = new HttpClient();

            //client.BaseAddress = new Uri("http://172.16.6.11:8002/");
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            //HttpResponseMessage response = await client.GetAsync(url.PathAndQuery);
            //if (response.IsSuccessStatusCode)
            //{
            //    var product = await response.Content.ReadAsAsync<Product>();
            //}

            ////return Ok("Yes");
        }
    }
}
