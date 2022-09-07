using DataAbstraction.Interfaces;
using DataAbstraction.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ITI.QUIK.API.MultyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KvalInvestorsAndTestsController : ControllerBase
    {
        private ILogger<KvalInvestorsAndTestsController> _logger;
        private ICoreKval _core;

        public KvalInvestorsAndTestsController(ILogger<KvalInvestorsAndTestsController> logger, ICoreKval core)
        {
            _logger = logger;
            _core = core;
        }

        [HttpPost("RenewClients/{sendReport}")]
        public async Task<IActionResult> RenewClients(bool sendReport)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} Httppost RenewClients Call, sendReport={sendReport}");

            ListStringResponseModel result = await _core.RenewClients(sendReport);

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} Httppost RenewClients result isOK={result.IsSuccess}");

            return Ok(result);
        }
    }
}
