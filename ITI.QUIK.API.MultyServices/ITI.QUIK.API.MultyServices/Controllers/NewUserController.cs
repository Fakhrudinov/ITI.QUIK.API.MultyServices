using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Responses;
using DataValidationService;
using Microsoft.AspNetCore.Mvc;

namespace ITI.QUIK.API.MultyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewUserController : ControllerBase
    {
        private ILogger<NewUserController> _logger;
        private ICore _core;

        public NewUserController(ILogger<NewUserController> logger, ICore core)
        {
            _logger = logger;
            _core = core;
        }

        [HttpGet("GetInfo/OptionWorkShop/{matrixClientAccount}")]
        public async Task<IActionResult> GetInfoNewUserOptionWorkShop(string matrixClientAccount)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet GetInfo/NewUser/ForOptionWorkShop/{matrixClientAccount} Call");

            //проверим корректность входных данных
            ListStringResponseModel result = ValidateData.ValidateMatrixClientAccount(matrixClientAccount);
            if (!result.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet GetInfo/NewUser/ForOptionWorkShop/{matrixClientAccount} Error: {result.Messages[0]}");
                return Ok(result);
            }

            NewClientOptionWorkShopModelResponse newClientOW = await _core.GetInfoNewUserOptionWorkShop(matrixClientAccount);

            return Ok(newClientOW);
        }

        [HttpGet("GetInfo/{matrixClientAccount}")]
        public async Task<IActionResult> GetInfoNewUserNonEDP(string matrixClientAccount)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet GetInfo/NewUser/NonEDP/{matrixClientAccount} Call");

            //проверим корректность входных данных
            ListStringResponseModel result = ValidateData.ValidateMatrixClientAccount(matrixClientAccount);
            if (!result.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet GetInfo/NewUser/NonEDP/{matrixClientAccount} Error: {result.Messages[0]}");
                return Ok(result);
            }

            NewClientModelResponse newClient = await _core.GetInfoNewUserNonEDP(matrixClientAccount);

            return Ok(newClient);
        }

        [HttpGet("GetKeyModel/FromQuery")]
        public async Task<IActionResult> GetKeyModelFromQuery([FromQuery] string keyText)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet GetKeyModel/FromQuery Call, Text=" + keyText);

            return Ok(_core.GetKeyFromString(keyText));
        }

        [HttpGet("GetKeyModel/FromFile")]
        public async Task<IActionResult> GetKeyFromFileModel([FromQuery] string filePath)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet GetKeyModel/FromFile Call, filePath=" + filePath);

            PubringKeyModelResponse key = _core.GetKeyFromFile(filePath);

            return Ok(key);
        }

        [HttpPost("Post/NewClient/OptionWorkshop")]
        public async Task<IActionResult> PostNewClientOptionWorkshop([FromBody] NewClientOptionWorkShopModel newClientModel)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPost Post/NewClient/OptionWorkshop Call for " + newClientModel.Client.FirstName);

            //проверим корректность входных данных
            ListStringResponseModel result = ValidateData.ValidateNewClientOptionWorkShopModel(newClientModel);
            if (!result.IsSuccess)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPost NewClient/OptionWorkshop Failed with " + result.Messages[0]);
                return Ok(result);
            }

            result = await _core.PostNewClientOptionWorkshop(newClientModel);

            return Ok(result);
        }

        [HttpPost("Post/NewClient")]
        public async Task<IActionResult> PostNewClient([FromBody] NewClientModel newClientModel)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPost Post/NewClient Call for " + newClientModel.Client.FirstName);

            NewClientCreationResponse createResponse = new NewClientCreationResponse();

            //проверим корректность входных данных
            ListStringResponseModel validationResult = ValidateData.ValidateNewClientModel(newClientModel);
            if (!validationResult.IsSuccess)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPost NewClient Failed with " + validationResult.Messages[0]);

                createResponse.IsNewClientCreationSuccess = false;
                createResponse.NewClientCreationMessages.AddRange(validationResult.Messages);

                return Ok(createResponse);
            }

            createResponse = await _core.PostNewClient(newClientModel);

            return Ok(createResponse);
        }
    }
}
