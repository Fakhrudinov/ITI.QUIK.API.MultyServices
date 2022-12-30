using DataAbstraction.Interfaces;
using DataAbstraction.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ITI.QUIK.API.MultyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientTemplatesManagementController : ControllerBase
    {
        private ILogger<ClientTemplatesManagementController> _logger;
        private ICore _core;

        public ClientTemplatesManagementController(ILogger<ClientTemplatesManagementController> logger, ICore core)
        {
            _logger = logger;
            _core = core;
        }

        [HttpPost("RenewClients/InTemplates/Spot/PoKomissii/{sendReport}")]
        public async Task<IActionResult> RenewClientsInSpotTemplatesPoKomissii(bool sendReport)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} Httppost RenewClients/InTemplates/Spot/PoKomissii Call, sendReport={sendReport}");

            ListStringResponseModel result = await _core.RenewClientsInSpotTemplatesPoKomissii(sendReport);

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} Httppost RenewClients/InTemplates/Spot/PoKomissii result isOK={result.IsSuccess}");

            return Ok(result);
        }

        [HttpPost("RenewClients/InTemplates/Forts/PoKomissii/{sendReport}")]
        public async Task<IActionResult> RenewClientsInFortsTemplatesPoKomissii(bool sendReport)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} Httppost RenewClients/InTemplates/Forts/PoKomissii Call, sendReport={sendReport}");

            ListStringResponseModel result = await _core.RenewClientsInFortsTemplatesPoKomissii(sendReport);

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} Httppost RenewClients/InTemplates/Forts/PoKomissii result isOK={result.IsSuccess}");

            return Ok(result);
        }

        [HttpPost("Renew/RestrictedSecurities/InTemplates/PoKomissii/{sendReport}")]
        public async Task<IActionResult> RenewRestrictedSecuritiesInTemplatesPoKomissii(bool sendReport)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} Httppost Renew/RestrictedSecurities/InTemplates/PoKomissii/ Call, sendReport={sendReport}");

            ListStringResponseModel result = await _core.RenewRestrictedSecuritiesInTemplatesPoKomissii(sendReport);

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} Httppost Renew/RestrictedSecurities/InTemplates/PoKomissii/ result isOK={result.IsSuccess}");

            return Ok(result);
        }
    }
}
