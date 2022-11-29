using DataAbstraction.Interfaces;
using DataAbstraction.Responses;
using DataValidationService;
using Microsoft.AspNetCore.Mvc;

namespace ITI.QUIK.API.MultyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoneyController : ControllerBase
    {
        private ILogger<MoneyController> _logger;
        private ICoreSingleServices _core;

        public MoneyController(ILogger<MoneyController> logger, ICoreSingleServices core)
        {
            _logger = logger;
            _core = core;
        }

        [HttpGet("Get/SingleClient/SpotLimits/ToFile/ByMatrixAccount/{matrixClientAccount}/DoZeroingToOldPosition/{oldPositionMustBeZeroing}")]
        public async Task<IActionResult> GetSingleClientSpotLimitsToFileByMatrixAccount(string matrixClientAccount, bool oldPositionMustBeZeroing)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Get/SingleClient/SpotLimits/ToFile/ByMatrixAccount/{matrixClientAccount} Call");

            //проверим корректность входных данных
            ListStringResponseModel result = ValidateData.ValidateMatrixClientAccount(matrixClientAccount);
            if (!result.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Get/IsUser/AlreadyExist/inQAdmin/ByMatrixClientAccount/{matrixClientAccount} " +
                    $"Error: {result.Messages[0]}");
                return Ok(result);
            }

            result = await _core.GetSingleClientSpotLimitsToFileByMatrixAccount(matrixClientAccount, oldPositionMustBeZeroing);

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Get/SingleClient/SpotLimits/ToFile/ByMatrixAccount/{matrixClientAccount} " +
                $"result isOK={result.IsSuccess}");

            return Ok(result);
        }
    }
}
