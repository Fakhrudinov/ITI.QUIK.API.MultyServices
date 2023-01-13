﻿using DataAbstraction.Interfaces;
using DataAbstraction.Responses;
using DataValidationService;
using Microsoft.AspNetCore.Mvc;

namespace ITI.QUIK.API.MultyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : ControllerBase
    {
        private ILogger<DiscountsController> _logger;
        private ICoreDiscounts _core;

        public DiscountsController(ILogger<DiscountsController> logger, ICoreDiscounts core)
        {
            _logger = logger;
            _core = core;
        }

        [HttpGet("Check/SingleDiscount/{security}")]
        public async Task<IActionResult> CheckSingleDiscount(string security)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Check/SingleDiscount/{security} Call");

            //проверим корректность входных данных
            ListStringResponseModel validateResult = ValidateData.ValidateSecurityName(security);
            if (!validateResult.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Check/SingleDiscount/{security} " +
                    $"validate Error: {validateResult.Messages[0]}");

                BoolResponse validationResult = new BoolResponse();

                validationResult.IsSuccess = false;
                validationResult.Messages.AddRange(validateResult.Messages);
                return Ok(validationResult);
            }

            BoolResponse result = await _core.CheckSingleDiscount(security);

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Check/SingleDiscount/{security} " +
                $"result isOK={result.IsSuccess}");

            return Ok(result);
        }

        [HttpPost("Post/SingleDiscount/{security}")]
        public async Task<IActionResult> PostSingleDiscount(string security)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPost Post/SingleDiscount/{security} Call");

            //проверим корректность входных данных
            ListStringResponseModel result = ValidateData.ValidateSecurityName(security);
            if (!result.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPost Post/SingleDiscount/{security} " +
                    $"validate Error: {result.Messages[0]}");

                return Ok(result);
            }

           result = await _core.PostSingleDiscount(security);

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpPost Post/SingleDiscount/{security} " +
                $"result isOK={result.IsSuccess}");

            return Ok(result);
        }

        [HttpDelete("Delete/SingleDiscount/{security}")]
        public async Task<IActionResult> DeleteSingleDiscount(string security)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpDelete Delete/SingleDiscount/{security} Call");

            //проверим корректность входных данных
            ListStringResponseModel validateResult = ValidateData.ValidateSecurityName(security);
            if (!validateResult.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpDelete Delete/SingleDiscount/{security} " +
                    $"validate Error: {validateResult.Messages[0]}");

                BoolResponse validationResult = new BoolResponse();

                validationResult.IsSuccess = false;
                validationResult.Messages.AddRange(validateResult.Messages);
                return Ok(validationResult);
            }

            BoolResponse result = await _core.DeleteSingleDiscount(security);

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpDelete Delete/SingleDiscount/{security} " +
                $"result isOK={result.IsSuccess}");

            return Ok(result);
        }

        [HttpGet("Check/AllDiscounts")]
        public async Task<IActionResult> CheckSingleDiscount()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Check/AllDiscounts Call");

            BoolResponse result = await _core.CheckAllDiscounts();

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet Check/AllDiscounts " +
                $"result isOK={result.IsSuccess}");

            return Ok(result);
        }
    }
}