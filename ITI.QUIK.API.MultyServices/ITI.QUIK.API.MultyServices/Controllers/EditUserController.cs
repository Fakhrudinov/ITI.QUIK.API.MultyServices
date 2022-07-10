using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ITI.QUIK.API.MultyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditUserController : ControllerBase
    {
        private ILogger<EditUserController> _logger;
        private ICore _core;

        public EditUserController(ILogger<EditUserController> logger, ICore core)
        {
            _logger = logger;
            _core = core;
        }

        [HttpGet("Get/IsUser/AlreadyExist/ByMatrixPortfolio/{clientPortfolio}")]
        public async Task<IActionResult> GetIsUserAlreadyExistByMatrixPortfolio(string clientPortfolio)
        {
            _logger.LogInformation($"HttpGet Get/IsUser/AlreadyExist/ByMatrixPortfolio/{clientPortfolio} Call");

            //validate clientPortfolio

            FindedQuikQAdminClientResponse findedUsers = await _core.GetIsUserAlreadyExistByMatrixPortfolio(clientPortfolio);

            return Ok(findedUsers);
        }

        [HttpGet("Get/IsUser/AlreadyExist/ByFortsCode/{fortsClientCode}")]
        public async Task<IActionResult> GetIsUserAlreadyExistByFortsCode(string fortsClientCode)
        {
            _logger.LogInformation($"HttpGet Get/IsUser/AlreadyExist/ByFortsCode/{fortsClientCode} Call");

            //validate fortsClientCode

            FindedQuikQAdminClientResponse findedUsers = await _core.GetIsUserAlreadyExistByFortsCode(fortsClientCode);

            return Ok(findedUsers);
        }

        [HttpDelete("BlockUserBy/MatrixClientCode")]
        public async Task<IActionResult> BlockUserByMatrixClientCode([FromBody] MatrixClientCodeModel model)
        {
            _logger.LogInformation($"HttpDelete BlockUserBy/MatrixClientCode/{model.MatrixClientCode} Call");

            //validate MatrixClientCodeModel model

            ListStringResponseModel result = await _core.BlockUserByMatrixClientCode(model);

            return Ok(result);
        }
        [HttpDelete("BlockUserBy/FortsClientCode")]
        public async Task<IActionResult> BlockUserByFortsClientCode([FromBody] FortsClientCodeModel model)
        {
            _logger.LogInformation($"HttpDelete BlockUserBy/FortsClientCode/{model.FortsClientCode} Call");

            //validate FortsClientCodeModel model

            ListStringResponseModel result = await _core.BlockUserByFortsClientCode(model);

            return Ok(result);
        }
        [HttpDelete("BlockUserBy/UID/{uid}")]
        public async Task<IActionResult> BlockUserByUID(int uid)
        {
            _logger.LogInformation($"HttpDelete BlockUserBy/UID/{uid} Call");

            //validate uid

            ListStringResponseModel result = await _core.BlockUserByUID(uid);

            return Ok(result);
        }


        [HttpPut("SetNewPubringKeyBy/MatrixClientCode")]
        public async Task<IActionResult> SetNewPubringKeyByMatrixClientCode([FromBody] MatrixCodeAndPubringKeyModel model)
        {
            _logger.LogInformation("HttpPut SetNewPubringKey/ByMatrixClientCode Call " + model.ClientCode);

            ////проверим корректность входных данных

            ListStringResponseModel result = await _core.SetNewPubringKeyByMatrixClientCode(model);

            return Ok(result);
        }
        [HttpPut("SetNewPubringKeyBy/FortsClientCode")]
        public async Task<IActionResult> SetNewPubringKeyByFortsClientCode([FromBody] FortsCodeAndPubringKeyModel model)
        {
            _logger.LogInformation("HttpPut SetNewPubringKey/ByFortsClientCode Call " + model.ClientCode);

            ////проверим корректность входных данных

            ListStringResponseModel result = await _core.SetNewPubringKeyByFortsClientCode(model);

            return Ok(result);
        }


        [HttpPut("SetAllTrades/ByMatrixClientCode")]
        public async Task<IActionResult> SetAllTradesByMatrixClientCode([FromBody] MatrixClientCodeModel model)
        {
            _logger.LogInformation("HttpPut SetAllTrades/ByMatrixClientCode Call " + model.MatrixClientCode);

            //проверим корректность входных данных

            ListStringResponseModel result = await _core.SetAllTradesByMatrixClientCode(model);

            return Ok(result);
        }

        [HttpPut("SetAllTradesBy/FortsClientCode")]
        public async Task<IActionResult> SetAllTradesByFortsClientCode([FromBody] FortsClientCodeModel model)
        {
            _logger.LogInformation("HttpPut SetAllTradesBy/FortsClientCode Call " + model.FortsClientCode);

            //проверим корректность входных данных

            ListStringResponseModel result = await _core.SetAllTradesByFortsClientCode(model);

            return Ok(result);
        }
    }
}
