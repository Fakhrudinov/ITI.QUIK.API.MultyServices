using DataAbstraction.Interfaces;
using DataAbstraction.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITI.QUIK.API.MultyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QMonitorController : ControllerBase
    {
        private ILogger<QMonitorController> _logger;
        private IHttpQuikRepository _repository;

        public QMonitorController(ILogger<QMonitorController> logger, IHttpQuikRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet("Reload/SpotBRL/MC013820000")]
        public async Task<IActionResult> ReloadSpotBrlMC013820000()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Reload/SpotBRL/MC013820000 Call");

            ListStringResponseModel result = await _repository.ReloadSpotBrlMC013820000();

            return Ok(result);
        }

        [HttpGet("Reload/FortsBRL/SPBFUT")]
        public async Task<IActionResult> ReloadFortsBrlSPBFUT()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Reload/FortsBRL/SPBFUT Call");

            ListStringResponseModel result = await _repository.ReloadFortsBrlSPBFUT();

            return Ok(result);
        }
    }
}
