using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Responses;
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

        [HttpGet("GetInfo/OptionWorkShop/{clientCode}")]
        public async Task<IActionResult> GetInfoNewUserOptionWorkShop(string clientCode)
        {
            _logger.LogInformation($"HttpGet GetInfo/NewUser/ForOptionWorkShop/{clientCode} Call");

            NewClientOptionWorkShopModelResponse newClientOW = await _core.GetInfoNewUserOptionWorkShop(clientCode);

            return Ok(newClientOW);
        }

        [HttpGet("GetInfo/{clientCode}")]
        public async Task<IActionResult> GetInfoNewUserNonEDP(string clientCode)
        {
            _logger.LogInformation($"HttpGet GetInfo/NewUser/NonEDP/{clientCode} Call");

            NewClientModelResponse newClient = await _core.GetInfoNewUserNonEDP(clientCode);

            return Ok(newClient);
        }

        [HttpGet("GetKeyModel/FromQuery")]
        public async Task<IActionResult> GetKeyModelFromQuery([FromQuery] string keyText)
        {
            _logger.LogInformation($"HttpGet GetKeyModel/FromQuery Call, Text=" + keyText);

            return Ok(_core.GetKeyFromString(keyText));
        }

        [HttpGet("GetKeyModel/FromFile")]
        public async Task<IActionResult> GetKeyFromFileModel([FromQuery] string filePath)
        {
            _logger.LogInformation($"HttpGet GetKeyModel/FromFile Call, filePath=" + filePath);

            PubringKeyModelResponse key = _core.GetKeyFromFile(filePath);

            return Ok(key);
        }

        [HttpGet("GetResult/FromQuikSFTP/FileUpload")]
        public async Task<IActionResult> GetResultFromQuikSFTPFileUpload([FromQuery] string fileName)
        {
            _logger.LogInformation($"HttpGet GetResult/FromQuikSFTP/FileUpload Call, file=" + fileName);

            ListStringResponseModel result = await _core.GetResultFromQuikSFTPFileUpload(fileName);

            return Ok(result);
        }

        [HttpPost("Post/NewClient/OptionWorkshop")]
        public async Task<IActionResult> PostNewClientOptionWorkshop([FromBody] NewClientOptionWorkShopModel newClientModel)
        {
            _logger.LogInformation($"HttpPost Post/NewClient/OptionWorkshop Call for " + newClientModel.Client.FirstName);

            //validate newClientModel

            ListStringResponseModel createResponse = await _core.PostNewClientOptionWorkshop(newClientModel);

            return Ok(createResponse);
        }

        [HttpPost("Post/NewClient")]
        public async Task<IActionResult> PostNewClient([FromBody] NewClientModel newClientModel)
        {
            _logger.LogInformation($"HttpPost Post/NewClient Call for " + newClientModel.Client.FirstName);

            //validate newClientModel

            NewClientCreationResponse createResponse = await _core.PostNewClient(newClientModel);

            return Ok(createResponse);
        }
    }
}
