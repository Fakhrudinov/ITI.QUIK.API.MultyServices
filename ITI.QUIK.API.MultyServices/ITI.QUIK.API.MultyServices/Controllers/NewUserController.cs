using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ITI.QUIK.API.MultyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewUserController : ControllerBase
    {
        private ILogger<NewUserController> _logger;
        private IHttpApiRepository _repository;

        public NewUserController(ILogger<NewUserController> logger, IHttpApiRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet("GetInfo/OptionWorkShop/{clientCode}")]
        public async Task<IActionResult> GetInfoNewUserOptionWorkShop(string clientCode)
        {
            _logger.LogInformation($"HttpGet GetInfo/NewUser/ForOptionWorkShop/{clientCode} Call");

            NewClientOptionWorkShopModelResponse newClientOW = new NewClientOptionWorkShopModelResponse();
            newClientOW.NewOWClient.Key = new PubringKeyModel();

            //ListStringResponseModel validationResult = Validator.ValidateClientCode(clientCode);
            //if (!validationResult.IsSuccess)
            //{
            //    _logger.LogWarning($"HttpGet GetUser/SpotPortfolios {clientCode} Validation Fail: {validationResult.Messages[0]}");
            //    return BadRequest(validationResult);
            //}

            ClientInformationResponse clientInformation = await _repository.GetClientInformation(clientCode);
            if (clientInformation.Response.IsSuccess)
            {
                newClientOW.NewOWClient.Client = clientInformation.ClientInformation;
            }
            else
            {
                newClientOW.Response.IsSuccess = false;
                newClientOW.Response.Messages.AddRange(clientInformation.Response.Messages);
            }

            MatrixToFortsCodesMappingResponse fortsCodes = await _repository.GetClientNonEdpFortsCodes(clientCode);
            if (fortsCodes.Response.IsSuccess)
            {
                newClientOW.NewOWClient.CodesPairRF = fortsCodes.MatrixToFortsCodesList.ToArray();
            }
            else
            {
                newClientOW.Response.IsSuccess = false;
                newClientOW.Response.Messages.AddRange(fortsCodes.Response.Messages);
            }

            return Ok(newClientOW);
        }

        [HttpGet("GetInfo/{clientCode}")]
        public async Task<IActionResult> GetInfoNewUserNonEDP(string clientCode)
        {
            _logger.LogInformation($"HttpGet GetInfo/NewUser/NonEDP/{clientCode} Call");

            NewClientModelResponse newClient = new NewClientModelResponse();
            newClient.NewClient.Key = new PubringKeyModel();


            ClientInformationResponse clientInformation = await _repository.GetClientInformation(clientCode);
            if (clientInformation.Response.IsSuccess)
            {
                newClient.NewClient.Client = clientInformation.ClientInformation;
            }
            else
            {
                newClient.Response.IsSuccess = false;
                newClient.Response.Messages.AddRange(clientInformation.Response.Messages);
            }

            ClientBOInformationResponse clientBOInformation = await _repository.GetClientBOInformation(clientCode);
            if (clientBOInformation.Response.IsSuccess)
            {
                newClient.NewClient.isClientPerson = clientBOInformation.ClientBOInformation.isClientPerson;
                newClient.NewClient.isClientResident = clientBOInformation.ClientBOInformation.isClientResident;
                newClient.NewClient.Address = clientBOInformation.ClientBOInformation.Address;
                newClient.NewClient.RegisterDate = clientBOInformation.ClientBOInformation.RegisterDate;
            }
            else
            {
                newClient.Response.IsSuccess = false;
                newClient.Response.Messages.AddRange(clientBOInformation.Response.Messages);
            }

            MatrixClientCodeModelResponse spotCodes = await _repository.GetClientAllSpotCodesFiltered(clientCode);
            if (spotCodes.Response.IsSuccess)
            {
                newClient.NewClient.CodesMatrix = spotCodes.MatrixClientCodesList.ToArray();
            }
            else
            {
                newClient.Response.IsSuccess = false;
                newClient.Response.Messages.AddRange(spotCodes.Response.Messages);
            }

            MatrixToFortsCodesMappingResponse fortsCodes = await _repository.GetClientAllFortsCodes(clientCode);
            if (fortsCodes.Response.IsSuccess)
            {
                newClient.NewClient.CodesPairRF = fortsCodes.MatrixToFortsCodesList.ToArray();
            }
            else
            {
                newClient.Response.IsSuccess = false;
                newClient.Response.Messages.AddRange(fortsCodes.Response.Messages);
            }

            return Ok(newClient);
        }
    }
}
