using DataAbstraction.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITI.QUIK.API.MultyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewUserController : ControllerBase
    {
        private ILogger<NewUserController> _logger;
        private IDataBaseRepository _repository;

        public NewUserController(ILogger<NewUserController> logger, IDataBaseRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }


        // new OW user = input is (rf codes pair)+(key)
        // get personal info
        // check RF - not in MO


        // new non EDP user = input is (code)+(key)
        // get personal info
        // get BO personal info
        // at liast one of codes must be set       
        // get all spot portfolios exlude = MO RS SF OT - list at settings
        // get all rf codes pair

    }
}
