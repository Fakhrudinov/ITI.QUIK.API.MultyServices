using DataAbstraction.Interfaces;
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
    }
}
