﻿using DataAbstraction.Connections;
using DataAbstraction.Interfaces;
using DataAbstraction.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ITI.QUIK.API.MultyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataBaseController : ControllerBase
    {
        private ILogger<DataBaseController> _logger;
        private IDataBaseRepository _repository;


        public DataBaseController(ILogger<DataBaseController> logger, IDataBaseRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet("CheckConnections/MatrixDataBase")]
        public async Task<IActionResult> CheckConnection()
        {
            _logger.LogInformation("HttpGet CheckConnections/MatrixDataBase Call");

            ListStringResponseModel result = await _repository.CheckConnections();

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}
