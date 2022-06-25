using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ITI.QUIK.API.MultyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewUserController : ControllerBase
    {
        private ILogger<NewUserController> _logger;
        private IHttpApiRepository _repository;
        private PubringKeyIgnoreWords _ignoreWords;

        public NewUserController(ILogger<NewUserController> logger, IHttpApiRepository repository, IOptions<PubringKeyIgnoreWords> ignoreWords)
        {
            _logger = logger;
            _repository = repository;
            _ignoreWords = ignoreWords.Value;
        }

        [HttpGet("GetInfo/OptionWorkShop/{clientCode}")]
        public async Task<IActionResult> GetInfoNewUserOptionWorkShop(string clientCode)
        {
            _logger.LogInformation($"HttpGet GetInfo/NewUser/ForOptionWorkShop/{clientCode} Call");

            NewClientOptionWorkShopModelResponse newClientOW = new NewClientOptionWorkShopModelResponse();
            newClientOW.NewOWClient.Key = new PubringKeyModel();

            //ListStringResponseModel validationResult = Validator.ValidateClientCode(clientCode);
            //if (!validationResult.IsSuccess)
            //{
            //    _logger.LogWarning($"HttpGet GetUser/SpotPortfolios {clientCode} Validation Fail: {validationResult.Messages[0]}");
            //    return BadRequest(validationResult);
            //}

            ClientInformationResponse clientInformation = await _repository.GetClientInformation(clientCode);
            if (clientInformation.Response.IsSuccess)
            {
                newClientOW.NewOWClient.Client = clientInformation.ClientInformation;
            }
            else
            {
                newClientOW.Response.IsSuccess = false;
                newClientOW.Response.Messages.AddRange(clientInformation.Response.Messages);
            }

            MatrixToFortsCodesMappingResponse fortsCodes = await _repository.GetClientNonEdpFortsCodes(clientCode);
            if (fortsCodes.Response.IsSuccess)
            {
                newClientOW.NewOWClient.CodesPairRF = fortsCodes.MatrixToFortsCodesList.ToArray();
            }
            else
            {
                newClientOW.Response.IsSuccess = false;
                newClientOW.Response.Messages.AddRange(fortsCodes.Response.Messages);
            }

            return Ok(newClientOW);
        }

        [HttpGet("GetInfo/{clientCode}")]
        public async Task<IActionResult> GetInfoNewUserNonEDP(string clientCode)
        {
            _logger.LogInformation($"HttpGet GetInfo/NewUser/NonEDP/{clientCode} Call");

            NewClientModelResponse newClient = new NewClientModelResponse();
            newClient.NewClient.Key = new PubringKeyModel();


            ClientInformationResponse clientInformation = await _repository.GetClientInformation(clientCode);
            if (clientInformation.Response.IsSuccess)
            {
                newClient.NewClient.Client = clientInformation.ClientInformation;
            }
            else
            {
                newClient.Response.IsSuccess = false;
                newClient.Response.Messages.AddRange(clientInformation.Response.Messages);
            }

            ClientBOInformationResponse clientBOInformation = await _repository.GetClientBOInformation(clientCode);
            if (clientBOInformation.Response.IsSuccess)
            {
                newClient.NewClient.isClientPerson = clientBOInformation.ClientBOInformation.isClientPerson;
                newClient.NewClient.isClientResident = clientBOInformation.ClientBOInformation.isClientResident;
                newClient.NewClient.Address = clientBOInformation.ClientBOInformation.Address;
                newClient.NewClient.RegisterDate = clientBOInformation.ClientBOInformation.RegisterDate;
            }
            else
            {
                newClient.Response.IsSuccess = false;
                newClient.Response.Messages.AddRange(clientBOInformation.Response.Messages);
            }

            MatrixClientCodeModelResponse spotCodes = await _repository.GetClientAllSpotCodesFiltered(clientCode);
            if (spotCodes.Response.IsSuccess)
            {
                newClient.NewClient.CodesMatrix = spotCodes.MatrixClientCodesList.ToArray();
            }
            else
            {
                newClient.Response.IsSuccess = false;
                newClient.Response.Messages.AddRange(spotCodes.Response.Messages);
            }

            MatrixToFortsCodesMappingResponse fortsCodes = await _repository.GetClientAllFortsCodes(clientCode);
            if (fortsCodes.Response.IsSuccess)
            {
                newClient.NewClient.CodesPairRF = fortsCodes.MatrixToFortsCodesList.ToArray();
            }
            else
            {
                newClient.Response.IsSuccess = false;
                newClient.Response.Messages.AddRange(fortsCodes.Response.Messages);
            }

            return Ok(newClient);
        }

        [HttpGet("GetKeyModel/FromQuery")]
        public async Task<IActionResult> GetKeyModelFromQuery([FromQuery] string keyText)
        {
            _logger.LogInformation($"HttpGet GetKeyModel/FromQuery Call, Text=" + keyText);

            return Ok(GetKeyFromString(keyText));
        }

        [HttpGet("GetKeyModel/FromFile")]
        public async Task<IActionResult> GetKeyFromFileModel([FromQuery] string filePath)
        {
            _logger.LogInformation($"HttpGet GetKeyModel/FromFile Call, filePath=" + filePath);

            if (!System.IO.File.Exists(filePath))
            {
                _logger.LogWarning("HttpGet GetKeyModel/FromFile Error - file not found: " + filePath);

                PubringKeyModelResponse key = new PubringKeyModelResponse();

                key.Response.IsSuccess = false;
                key.Response.Messages.Add("HttpGet GetKeyModel/FromFile Error - file not found: " + filePath);
                
                return Ok(key);
            }

            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > 600)//normal size is about 310 - 350. 600 for long login string
            {
                PubringKeyModelResponse key = new PubringKeyModelResponse();

                key.Response.IsSuccess = false;
                key.Response.Messages.Add($"HttpGet GetKeyModel/FromFile Error - file size '{fileInfo.Length}'byte anomally big: " + filePath);

                return Ok(key);
            }

            string keyText = "";
            using (StreamReader reader = new StreamReader(filePath))
            {
                keyText = await reader.ReadToEndAsync();
            }

            return Ok(GetKeyFromString(keyText));
        }

        [HttpGet("GetResult/FromQuikSFTP/FileUpload")]
        public async Task<IActionResult> GetResultFromQuikSFTPFileUpload([FromQuery] string file)
        {
            _logger.LogInformation($"HttpGet GetResult/FromQuikSFTP/FileUpload Call, file=" + file);

            ListStringResponseModel result = await _repository.GetResultFromQuikSFTPFileUpload(file);

            return Ok(result);
        }

        [HttpPost("Post/NewClient/OptionWorkshop")]
        public async Task<IActionResult> PostNewClientOptionWorkshop([FromBody] NewClientOptionWorkShopModel newClientModel)
        {
            _logger.LogInformation($"HttpPost Post/NewClient/OptionWorkshop Call for " + newClientModel.Client.FirstName);

            //validate newClientModel

            ListStringResponseModel createResponse = await _repository.CreateNewClientOptionWorkshop(newClientModel);

            if (createResponse.IsSuccess)
            {
                ListStringResponseModel searchFileResult = await _repository.GetResultFromQuikSFTPFileUpload(createResponse.Messages[0]);

                createResponse.Messages.AddRange(searchFileResult.Messages);
            }

            return Ok(createResponse);
        }

        [HttpPost("Post/NewClient")]
        public async Task<IActionResult> PostNewClient([FromBody] NewClientModel newClientModel)
        {
            _logger.LogInformation($"HttpPost Post/NewClient Call for " + newClientModel.Client.FirstName);

            //validate newClientModel

            NewClientCreationResponse createResponse = new NewClientCreationResponse();
            createResponse.NewClient = newClientModel;

            //SFTP create
            ListStringResponseModel createSftpResponse = await _repository.CreateNewClient(newClientModel);
            createResponse.IsSftpUploadSuccess = createSftpResponse.IsSuccess;
            createResponse.SftpUploadMessages = createSftpResponse.Messages;

            //codes ini
            ListStringResponseModel fillCodesIniResponse = await _repository.FillCodesIniFile(newClientModel);
            createResponse.IsCodesIniSuccess = fillCodesIniResponse.IsSuccess;
            createResponse.CodesInMessages = fillCodesIniResponse.Messages;

            //InstrTw register
            ListStringResponseModel fillDataBaseInstrTWResponse = await _repository.FillDataBaseInstrTW(newClientModel);
            createResponse.IsInstrTwSuccess = fillDataBaseInstrTWResponse.IsSuccess;
            createResponse.InstrTwMessages = fillDataBaseInstrTWResponse.Messages;

            // ? CD reg

            // по плечу - в NoLeverage 

            //заполним результаты в IsSuccess


            //добавить строки с именем файла

            //поискать файл в результатах

            return Ok(createResponse);
        }


        private PubringKeyModelResponse GetKeyFromString(string keyText)
        {
            PubringKeyModelResponse key = new PubringKeyModelResponse();

            // чистим от переносов строк и табов
            keyText = keyText.Replace(Environment.NewLine, " ");
            keyText = keyText.Replace("\r\n", " ");
            keyText = keyText.Replace("\n", " ");
            keyText = keyText.Replace("\t", " ");

            //чистим от списка слов из appsettings.json секция PubringKeyIgnoreWords
            var deleteWordsArray = _ignoreWords.RemoveText.Split(" ");
            foreach (string deleteWord in deleteWordsArray)
            {
                keyText = keyText.Replace(deleteWord, "");
            }

            // чистим от лишних пробелов
            while (keyText.Contains("  "))
            {
                keyText = keyText.Replace("  ", " ");
            }

            //разбиваем строки
            var keyTextArray = keyText.Split(" ");
            foreach (string line in keyTextArray)
            {
                if (line.Contains("KEYID="))
                {
                    key.Key.KeyID = line.Replace("KEYID=", "");
                }

                if (line.Contains("KEY="))
                {
                    key.Key.RSAKey = line.Replace("KEY=", "");
                }

                if (line.Contains("TIME="))
                {
                    string time = line.Replace("TIME=", "");
                    try
                    {

                        key.Key.Time = Int32.Parse(time);
                    }
                    catch (Exception)
                    {
                        key.Response.IsSuccess = false;
                        key.Response.Messages.Add($"Error at parse Time value '{time}'");
                    }
                }
            }

            // проверим, всё ли есть в ключе
            if (key.Key.KeyID is null)
            {
                key.Response.IsSuccess = false;
                key.Response.Messages.Add($"Error: KeyID value is empty");
            }

            if (key.Key.RSAKey is null)
            {
                key.Response.IsSuccess = false;
                key.Response.Messages.Add($"Error: RSAKey value is empty");
            }

            if (key.Key.Time == 0)
            {
                key.Response.IsSuccess = false;
                key.Response.Messages.Add($"Error: Time value is empty");
            }

            return key;
        }
    }
}
