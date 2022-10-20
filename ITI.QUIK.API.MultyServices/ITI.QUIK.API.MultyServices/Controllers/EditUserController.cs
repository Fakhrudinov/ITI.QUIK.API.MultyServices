using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Responses;
using DataValidationService;
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

        [HttpGet("Get/IsUser/AlreadyExist/inAllQuik/ByMatrixClientAccount/{matrixClientAccount}")]
        public async Task<IActionResult> GetIsUserAlreadyExistInAllQuikByMatrixClientAccount(string matrixClientAccount)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Get/IsUser/AlreadyExist/inAllQuik/ByMatrixClientAccount/{matrixClientAccount} Call");

            //проверим корректность входных данных
            FindedQuikClientResponse result = ValidateData.ValidateMatrixClientAccountToFindedQuik(matrixClientAccount);
            if (!result.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Get/IsUser/AlreadyExist/inAllQuik/ByMatrixClientAccount/{matrixClientAccount} Error: {result.Messages[0]}");
                return Ok(result);
            }

            result = await _core.GetIsUserAlreadyExistInAllQuikByMatrixClientAccount(matrixClientAccount);

            return Ok(result);
        }

        [HttpGet("Get/IsUser/AlreadyExist/inQAdmin/ByMatrixClientAccount/{matrixClientAccount}")]
        public async Task<IActionResult> GetIsUserAlreadyExistByMatrixClientAccount(string matrixClientAccount)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Get/IsUser/AlreadyExist/inQAdmin/ByMatrixClientAccount/{matrixClientAccount} Call");

            //проверим корректность входных данных
            ListStringResponseModel result = ValidateData.ValidateMatrixClientAccount(matrixClientAccount);
            if (!result.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Get/IsUser/AlreadyExist/inQAdmin/ByMatrixClientAccount/{matrixClientAccount} Error: {result.Messages[0]}");
                return Ok(result);
            }

            FindedQuikQAdminClientResponse findedUsers = await _core.GetIsUserAlreadyExistByMatrixClientAccount(matrixClientAccount);

            return Ok(findedUsers);
        }

        [HttpGet("Get/IsUser/AlreadyExist/inQAdmin/ByFortsCode/{fortsClientCode}")]
        public async Task<IActionResult> GetIsUserAlreadyExistByFortsCode(string fortsClientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Get/IsUser/AlreadyExist/inQAdmin/ByFortsCode/{fortsClientCode} Call");

            //проверим корректность входных данных
            ListStringResponseModel result = ValidateData.ValidateMatrixFortsCode(fortsClientCode);
            if (!result.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Get/IsUser/AlreadyExist/inQAdmin/ByFortsCode/{fortsClientCode} Error: {result.Messages[0]}");
                return Ok(result);
            }

            FindedQuikQAdminClientResponse findedUsers = await _core.GetIsUserAlreadyExistByFortsCode(fortsClientCode);

            return Ok(findedUsers);
        }

        [HttpDelete("BlockUserBy/MatrixClientPortfolio/inQAdmin")]
        public async Task<IActionResult> BlockUserByMatrixClientCode([FromBody] MatrixClientPortfolioModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpDelete BlockUserBy/MatrixClientPortfolio/inQAdmin/{model.MatrixClientPortfolio} Call");

            //проверим корректность входных данных
            ListStringResponseModel result = ValidateData.ValidateMatrixClientPortfolioModel(model);
            if (!result.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpDelete BlockUserBy/MatrixClientPortfolio/inQAdmin/{model.MatrixClientPortfolio} " +
                    $"Error: {result.Messages[0]}");
                return Ok(result);
            }

            result = await _core.BlockUserByMatrixClientCode(model);

            return Ok(result);
        }
        [HttpDelete("BlockUserBy/FortsClientCode/inQAdmin")]
        public async Task<IActionResult> BlockUserByFortsClientCode([FromBody] FortsClientCodeModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpDelete BlockUserBy/FortsClientCode/inQAdmin/{model.FortsClientCode} Call");

            //проверим корректность входных данных
            ListStringResponseModel result = ValidateData.ValidateMatrixFortsCode(model.FortsClientCode);
            if (!result.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpDelete BlockUserBy/FortsClientCode/inQAdmin/{model.FortsClientCode} Error: {result.Messages[0]}");
                return Ok(result);
            }

            result = await _core.BlockUserByFortsClientCode(model);

            return Ok(result);
        }
        [HttpDelete("BlockUserBy/UID/inQAdmin/{uid}")]
        public async Task<IActionResult> BlockUserByUID(int uid)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpDelete BlockUserBy/UID/inQAdmin/{uid} Call");

            //validate uid?

            ListStringResponseModel result = await _core.BlockUserByUID(uid);

            return Ok(result);
        }


        [HttpPut("SetNewPubringKeyBy/MatrixClientPortfolio/inQAdmin")]
        public async Task<IActionResult> SetNewPubringKeyByMatrixClientCode([FromBody] MatrixCodeAndPubringKeyModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPut SetNewPubringKey/MatrixClientPortfolio/inQAdmin Call " + model.MatrixClientPortfolio);

            //проверим корректность входных данных
            ListStringResponseModel result = ValidateData.ValidateMatrixCodeAndPubringKeyModel(model);
            if (!result.IsSuccess)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPut SetNewPubringKey/MatrixClientPortfolio/inQAdmin Failed with " + result.Messages[0]);
                return Ok(result);
            }

            result = await _core.SetNewPubringKeyByMatrixClientCode(model);

            return Ok(result);
        }
        [HttpPut("SetNewPubringKeyBy/FortsClientCode/inQAdmin")]
        public async Task<IActionResult> SetNewPubringKeyByFortsClientCode([FromBody] FortsCodeAndPubringKeyModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPut SetNewPubringKey/ByFortsClientCode/inQAdmin Call " + model.ClientCode);

            //проверим корректность входных данных
            ListStringResponseModel result = ValidateData.ValidateFortsCodeAndPubringKeyModel(model);
            if (!result.IsSuccess)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPut SetNewPubringKey/ByFortsClientCode/inQAdmin Failed with " + result.Messages[0]);
                return Ok(result);
            }

            result = await _core.SetNewPubringKeyByFortsClientCode(model);

            return Ok(result);
        }


        [HttpPut("SetAllTrades/ByMatrixClientPortfolio/inQAdmin")]
        public async Task<IActionResult> SetAllTradesByMatrixClientCode([FromBody] MatrixClientPortfolioModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPut SetAllTrades/ByMatrixClientPortfolio/inQAdmin Call " + model.MatrixClientPortfolio);

            //проверим корректность входных данных
            ListStringResponseModel result = ValidateData.ValidateMatrixClientPortfolioModel(model);
            if (!result.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPut SetAllTrades/ByMatrixClientPortfolio/inQAdmin/{model.MatrixClientPortfolio} " +
                    $"Error: {result.Messages[0]}");
                return Ok(result);
            }

            result = await _core.SetAllTradesByMatrixClientCode(model);

            return Ok(result);
        }

        [HttpPut("SetAllTradesBy/FortsClientCode/inQAdmin")]
        public async Task<IActionResult> SetAllTradesByFortsClientCode([FromBody] FortsClientCodeModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPut SetAllTradesBy/FortsClientCode/inQAdmin Call " + model.FortsClientCode);

            //проверим корректность входных данных
            ListStringResponseModel result = ValidateData.ValidateMatrixFortsCode(model.FortsClientCode);
            if (!result.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPut SetAllTradesBy/FortsClientCode/inQAdmin/{model.FortsClientCode} Error: {result.Messages[0]}");
                return Ok(result);
            }

            result = await _core.SetAllTradesByFortsClientCode(model);

            return Ok(result);
        }

        [HttpPut("AddNew/MatrixPortfolio/ToExistingClient/ByUID")]
        public async Task<IActionResult> AddNewMatrixPortfolioToExistingClientByUID([FromBody] NewPortfolioToExistingClientModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPut AddNew/MatrixPortfolio/ToExistingClient/ByUID Call, " +
                $"UID={model.UID} portfolio={model.MatrixPortfolio.MatrixClientPortfolio}");

            ////проверим корректность входных данных
            //NewClientCreationResponse result = ValidateData.ValidateMatrixFortsCode(model.FortsClientCode);
            //if (!result.IsSuccess)
            //{
            //    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPut AddNew/MatrixPortfolio/ToExistingClient/ByUID
            //      {model.FortsClientCode} Error: {result.Messages[0]}");
            //    return Ok(result);
            //}

            NewClientCreationResponse result = await _core.AddNewMatrixPortfolioToExistingClientByUID(model);

            return Ok(result);
        }
    }
}
