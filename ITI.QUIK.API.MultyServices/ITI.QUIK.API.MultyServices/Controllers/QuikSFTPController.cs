using DataAbstraction.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ITI.QUIK.API.MultyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuikSFTPController : ControllerBase
    {
        private ILogger<QuikSFTPController> _logger;
        private ICore _core;

        public QuikSFTPController(ILogger<QuikSFTPController> logger, ICore core)
        {
            _logger = logger;
            _core = core;
        }

        [HttpGet("Renew/AllClientFile")]
        public IActionResult RenewAllClientFile()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Renew/AllClientFile Call");

            _core.RenewAllClientFile();

            return Ok();
        }
    }
}
