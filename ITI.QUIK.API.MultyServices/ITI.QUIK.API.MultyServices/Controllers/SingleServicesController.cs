using DataAbstraction.Interfaces;
using DataAbstraction.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ITI.QUIK.API.MultyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SingleServicesController : ControllerBase
    {
        private ILogger<SingleServicesController> _logger;
        private ICoreSingleServices _core;

        public SingleServicesController(ILogger<SingleServicesController> logger, ICoreSingleServices core)
        {
            _logger = logger;
            _core = core;
        }

        [HttpGet("Check/IsAnyFortsCodes/InEDP")]
        public async Task<IActionResult> RenewClientsInSpotTemplatesPoKomissii()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Check/IsAnyFortsCodes/InEDP Call");

            BoolResponse result = await _core.CheckIsAnyFortsCodesFromOptionWorkshopInEDP();

            return Ok(result);
        }

        [HttpGet("Check/IsFileCorrect/LimLim/{sendReport}")]
        public async Task<IActionResult> CheckIsFileCorrectLimLim(bool sendReport)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Check/IsFileCorrect/LimLim Call");

            BoolResponse result = await _core.CheckIsFileCorrectLimLim(sendReport);

            return Ok(result);
        }
    }
}
