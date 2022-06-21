using DataAbstraction.Connections;
using DataAbstraction.Models;
using DataAbstraction.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace ITI.QUIK.API.MultyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewUserController : ControllerBase
    {
        private ILogger<NewUserController> _logger;
        private HttpConfigurations _connections;

        public NewUserController(ILogger<NewUserController> logger, IOptions<HttpConfigurations> connections)
        {
            _logger = logger;
            _connections = connections.Value;
        }

        [HttpGet("GetInfo/NewUser/OptionWorkShop/{clientCode}")]
        public async Task<IActionResult> GetInfoNewUserOptionWorkShop(string clientCode)
        {
            _logger.LogInformation($"HttpGet GetInfo/NewUser/OptionWorkShop/{clientCode} Call");

            //ListStringResponseModel validationResult = Validator.ValidateClientCode(clientCode);
            //if (!validationResult.IsSuccess)
            //{
            //    _logger.LogWarning($"HttpGet GetUser/SpotPortfolios {clientCode} Validation Fail: {validationResult.Messages[0]}");
            //    return BadRequest(validationResult);
            //}

            //MatrixToFortsCodesMappingResponse result = await _repository.GetUserSpotPortfolios(clientCode);

            NewClientOptionWorkShopModel newClientOW = new NewClientOptionWorkShopModel();

            


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/PersonalInfo/" + clientCode);

                if (response.IsSuccessStatusCode)
                {
                    ClientInformationResponse result = await response.Content.ReadFromJsonAsync<ClientInformationResponse>();

                    _logger.LogInformation($"HttpGet  GetInfo/NewUser/OptionWorkShop/ClientInformationResponse/{clientCode}  succes is {result.Response.IsSuccess}");
                    //return Ok(result);
                    newClientOW.Client = result.ClientInformation;
                }
            }

            _logger.LogWarning($"HttpGet GetInfo/NewUser/OptionWorkShop/ClientInformationResponse/{clientCode} NotFound");
            //return NotFound();



            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/FortsPortfolios/NoEDP/" + clientCode);

                if (response.IsSuccessStatusCode)
                {
                    MatrixToFortsCodesMappingResponse result = await response.Content.ReadFromJsonAsync<MatrixToFortsCodesMappingResponse>();

                    _logger.LogInformation($"HttpGet  GetInfo/NewUser/OptionWorkShop/MatrixToFortsCodesMappingResponse/{clientCode}  succes is {result.Response.IsSuccess}");
                    //return Ok(result);
                    newClientOW.CodesPairRF = result.MatrixToFortsCodesList.ToArray();
                }
            }

            _logger.LogWarning($"HttpGet GetInfo/NewUser/OptionWorkShop/MatrixToFortsCodesMappingResponse/{clientCode} NotFound");
            //return NotFound();

            return Ok(newClientOW);


            //if (result.Response.IsSuccess)
            //{
            //    //if (result.MatrixClientCodesList.Count == 0)
            //    //{
            //    //    return NotFound();
            //    //}

            //    return Ok(result);
            //}
            //else
            //{
            //    return BadRequest(result.Response.Messages);
            //}
        }

        // new OW user = input is (rf codes pair)+(key)
        // get personal info
        // check RF - not in MO


        // new non EDP user = input is (code)+(key)
        // get personal info
        // get BO personal info
        // at liast one of codes must be set       
        // get all spot portfolios exlude = MO RS SF OT - list at settings
        // get all rf codes pair

    }
}
