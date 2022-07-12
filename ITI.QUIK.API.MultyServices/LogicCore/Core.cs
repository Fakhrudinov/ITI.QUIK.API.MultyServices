using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Xml;

namespace LogicCore
{
    public class Core : ICore
    {
        private ILogger<Core> _logger;
        private IHttpApiRepository _repository;
        private PubringKeyIgnoreWords _ignoreWords;
        private System.Timers.Timer _timerUpdateFileCurrClnts;

        public Core(ILogger<Core> logger, IHttpApiRepository repository, IOptions<PubringKeyIgnoreWords> ignoreWords)
        {
            _logger = logger;
            _repository = repository;
            _ignoreWords = ignoreWords.Value;

            _timerUpdateFileCurrClnts = new System.Timers.Timer(120000);
            _timerUpdateFileCurrClnts.Elapsed += WaitAndGenerateNewFileCurrClnts;
            _timerUpdateFileCurrClnts.AutoReset = false;
        }

        public async Task<NewClientModelResponse> GetInfoNewUserNonEDP(string clientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetInfoNewUserNonEDP Called for {clientCode}");

            // этот запрос помогает авторизоваться в сторонней бэкофисной БД и предотвратит ошибки:
            // ORA - 02396: превышено максимальное время ожидания, повторите соединение еще раз
            // ORA - 02063: предшествующий line из BOFFCE_MOFF_LINK",
            await _repository.WarmUpBackOfficeDataBase();

            // далее все по плану
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
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetInfoNewUserNonEDP GetClientInformation result {clientInformation.Response.IsSuccess}");

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
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetInfoNewUserNonEDP GetClientAllSpotCodesFiltered result {spotCodes.Response.IsSuccess}");

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
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetInfoNewUserNonEDP GetClientAllFortsCodes result {fortsCodes.Response.IsSuccess}");

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
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetInfoNewUserNonEDP GetClientAllFortsCodes result {clientBOInformation.Response.IsSuccess}");

            return newClient;
        }

        public async Task<NewClientOptionWorkShopModelResponse> GetInfoNewUserOptionWorkShop(string clientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetInfoNewUserOptionWorkShop Called for {clientCode}");

            NewClientOptionWorkShopModelResponse newClientOW = new NewClientOptionWorkShopModelResponse();
            newClientOW.NewOWClient.Key = new PubringKeyModel();

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
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetInfoNewUserOptionWorkShop GetClientInformation result {clientInformation.Response.IsSuccess}");

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
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetInfoNewUserOptionWorkShop GetClientNonEdpFortsCodes result {fortsCodes.Response.IsSuccess}");

            return newClientOW;
        }

        public async Task<ListStringResponseModel> PostNewClientOptionWorkshop(NewClientOptionWorkShopModel newClientModel)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore PostNewClientOptionWorkshop Called for {newClientModel.Client.FirstName}");

            ListStringResponseModel createResponse = await _repository.CreateNewClientOptionWorkshop(newClientModel);

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore PostNewClientOptionWorkshop result is {createResponse.IsSuccess}");

            if (createResponse.IsSuccess)
            {
                ListStringResponseModel searchFileResult = await _repository.GetResultFromQuikSFTPFileUpload(createResponse.Messages[0]);

                createResponse.Messages.AddRange(searchFileResult.Messages);

                _timerUpdateFileCurrClnts.Enabled = true;
            }

            return createResponse;
        }

        public async Task<NewClientCreationResponse> PostNewClient(NewClientModel newClientModel)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore PostNewClient Called for {newClientModel.Client.FirstName}");

            NewClientCreationResponse createResponse = new NewClientCreationResponse();

            //SFTP create
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore SFTP register for {newClientModel.Client.FirstName}");
            ListStringResponseModel createSftpResponse = await _repository.CreateNewClient(newClientModel);
            createResponse.IsSftpUploadSuccess = createSftpResponse.IsSuccess;
            createResponse.SftpUploadMessages = createSftpResponse.Messages;

            //InstrTw register
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore InstrTw register for {newClientModel.Client.FirstName}");
            NewMNPClientModel newMNPClient = new NewMNPClientModel();
            newMNPClient.Client = newClientModel.Client;
            newMNPClient.isClientPerson = newClientModel.isClientPerson;
            newMNPClient.isClientResident = newClientModel.isClientResident;
            newMNPClient.Address = newClientModel.Address;
            newMNPClient.RegisterDate = newClientModel.RegisterDate;
            newMNPClient.CodesMatrix = newClientModel.CodesMatrix;
            newMNPClient.CodesPairRF = newClientModel.CodesPairRF;
            newMNPClient.Manager = newClientModel.Manager;
            newMNPClient.SubAccount = newClientModel.SubAccount;
            newMNPClient.Depositary = newClientModel.Depositary;
            newMNPClient.isClientDepo = newClientModel.isClientDepo;
            newMNPClient.DepoClientAccountsManager = newClientModel.DepoClientAccountsManager;

            ListStringResponseModel fillDataBaseInstrTWResponse = await _repository.FillDataBaseInstrTW(newMNPClient);
            createResponse.IsInstrTwSuccess = fillDataBaseInstrTWResponse.IsSuccess;
            createResponse.InstrTwMessages = fillDataBaseInstrTWResponse.Messages;

            // codes ini, CD reg
            if (newClientModel.CodesMatrix != null)
            {
                //codes ini
                CodesArrayModel codesArray = new CodesArrayModel();
                codesArray.ClientCodes = new MatrixClientCodeModel[newClientModel.CodesMatrix.Length];

                for (int i = 0; i < newClientModel.CodesMatrix.Length; i++)
                {
                    codesArray.ClientCodes[i] = newClientModel.CodesMatrix[i];
                }

                ListStringResponseModel fillCodesIniResponse = await _repository.FillCodesIniFile(codesArray);
                createResponse.IsCodesIniSuccess = fillCodesIniResponse.IsSuccess;
                createResponse.CodesIniMessages = fillCodesIniResponse.Messages;


                // ? CD reg
                bool totalSucces = true;
                foreach (var code in newClientModel.CodesMatrix)
                {
                    if (code.MatrixClientCode.Contains("-CD-"))
                    {
                        ListStringResponseModel addCdToKomissiiResponse = await _repository.AddCdPortfolioToTemplateKomissii(code);
                        if (!addCdToKomissiiResponse.IsSuccess)
                        {
                            totalSucces = false;
                        }
                        createResponse.AddToTemplatesMessages.AddRange(addCdToKomissiiResponse.Messages);

                        ListStringResponseModel addCdToPoPlechuResponse = await _repository.AddCdPortfolioToTemplatePoPlechu(code);
                        if (!addCdToPoPlechuResponse.IsSuccess)
                        {
                            totalSucces = false;
                        }
                        createResponse.AddToTemplatesMessages.AddRange(addCdToPoPlechuResponse.Messages);
                    }
                }

                if (totalSucces)
                {
                    createResponse.IsAddToTemplatesSuccess = true;
                }

            }
            else
            {
                createResponse.IsCodesIniSuccess = true;
                createResponse.CodesIniMessages.Add("Codes.ini - No action required, newClientModel.CodesMatrix is null");
                
                createResponse.IsAddToTemplatesSuccess = true;
                createResponse.AddToTemplatesMessages.Add("Add to templates - No action required, newClientModel.CodesMatrix is null");
            }

            //IsSuccess total ?
            if (createResponse.IsSftpUploadSuccess && createResponse.IsCodesIniSuccess && createResponse.IsAddToTemplatesSuccess && createResponse.IsInstrTwSuccess)
            {
                createResponse.IsNewClientCreationSuccess = true;
            }

            //проверим выполнение SFTP создания учетки в Qadmin - скорре всего результат будет "еще не обработан"
            if (createResponse.IsSftpUploadSuccess)
            {
                ListStringResponseModel searchFileResult = await _repository.GetResultFromQuikSFTPFileUpload(createSftpResponse.Messages[0]);

                createResponse.NewClientCreationMessages.Add(createSftpResponse.Messages[0]);
                createResponse.NewClientCreationMessages.AddRange(searchFileResult.Messages);

                _timerUpdateFileCurrClnts.Enabled = true;
            }

            return createResponse;
        }

        public async Task<ListStringResponseModel> GetResultFromQuikSFTPFileUpload(string fileName)
        {
            return await _repository.GetResultFromQuikSFTPFileUpload(fileName);
        }

        public PubringKeyModelResponse GetKeyFromFile(string filePath)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetKeyFromFile Called for {filePath}");

            if (!System.IO.File.Exists(filePath))
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet GetKeyModel/FromFile Error - file not found: " + filePath);

                PubringKeyModelResponse key = new PubringKeyModelResponse();

                key.Response.IsSuccess = false;
                key.Response.Messages.Add("HttpGet GetKeyModel/FromFile Error - file not found: " + filePath);

                return key;
            }

            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > 600)//normal size is about 310 - 350. 600 for long login string
            {
                PubringKeyModelResponse key = new PubringKeyModelResponse();

                key.Response.IsSuccess = false;
                key.Response.Messages.Add($"HttpGet GetKeyModel/FromFile Error - file size '{fileInfo.Length}'byte anomally big: " + filePath);

                return key;
            }

            string keyText = "";
            using (StreamReader reader = new StreamReader(filePath))
            {
                keyText = reader.ReadToEnd();
            }

            return GetKeyFromString(keyText);
        }

        public PubringKeyModelResponse GetKeyFromString(string keyText)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetKeyFromString Called for {keyText}");

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

        public async Task<FindedQuikQAdminClientResponse> GetIsUserAlreadyExistByMatrixPortfolio(string clientPortfolio)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetIsUserAlreadyExistByMatrixPortfolio Called for {clientPortfolio}");
            
            ListStringResponseModel findedClients = await _repository.GetIsUserAlreadyExistByMatrixPortfolio(clientPortfolio);
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetIsUserAlreadyExistByMatrixPortfolio result is {findedClients.IsSuccess}");

            FindedQuikQAdminClientResponse findedResponse = new FindedQuikQAdminClientResponse();

            if (findedClients.IsSuccess)
            {
                findedResponse.IsSuccess = true;
                findedResponse.QuikQAdminClient = GetClientsFromMessages(findedClients.Messages);
            }
            else
            {
                findedResponse.IsSuccess = false;
                findedResponse.Messages.AddRange(findedClients.Messages);
            }

            return findedResponse;
        }
        public async Task<FindedQuikQAdminClientResponse> GetIsUserAlreadyExistByFortsCode(string fortsClientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetIsUserAlreadyExistByFortsCode Called for {fortsClientCode}");

            ListStringResponseModel findedClients = await _repository.GetIsUserAlreadyExistByFortsCode(fortsClientCode);
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetIsUserAlreadyExistByMatrixPortfolio result is {findedClients.IsSuccess}");

            FindedQuikQAdminClientResponse findedResponse = new FindedQuikQAdminClientResponse();

            if (findedClients.IsSuccess)
            {
                findedResponse.IsSuccess = true;
                findedResponse.QuikQAdminClient = GetClientsFromMessages(findedClients.Messages);
            }
            else
            {
                findedResponse.IsSuccess = false;
                findedResponse.Messages.AddRange(findedClients.Messages);
            }

            return findedResponse;
        }
        private List<QuikQAdminClientModel> GetClientsFromMessages(List<string> messages)
        {
            List<QuikQAdminClientModel> clientList = new List<QuikQAdminClientModel>();

            foreach (string message in messages)
            {
                if (message.Contains("<UserDescription UID="))
                {
                    QuikQAdminClientModel client = new QuikQAdminClientModel();

                    XmlDocument doc = new XmlDocument();
                    XmlNamespaceManager nsmanager = new XmlNamespaceManager(doc.NameTable);
                    nsmanager.AddNamespace("qx", "urn:quik:user-rights-import");
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    doc.LoadXml(message);

                    XmlElement root = doc.DocumentElement;
                    client.UID = Int32.Parse(root.Attributes.GetNamedItem("UID").InnerText);
                    
                    client.FirstName = doc.SelectSingleNode("//qx:FirstName", nsmanager).InnerText;
                    client.MiddleName = doc.SelectSingleNode("//qx:MiddleName", nsmanager).InnerText;
                    client.LastName = doc.SelectSingleNode("//qx:LastName", nsmanager).InnerText;
                    client.Comment = doc.SelectSingleNode("//qx:Comment", nsmanager).InnerText;

                    string blockedValue = doc.SelectSingleNode("//qx:Blocked", nsmanager).InnerText;
                    if (int.TryParse(blockedValue, out int number))
                    {
                        if (number == 0)
                        {
                            client.IsBlocked = false;
                        }
                        else
                        {
                            client.IsBlocked = true;
                        }
                    }

                    //данные ключей
                    XmlNodeList userKeys = doc.SelectNodes("//qx:RSAKey", nsmanager);
                    if (userKeys.Count > 0)
                    {
                        client.Keys = new List<PubringKeyModel>();

                        foreach (XmlNode userKey in userKeys)
                        {
                            PubringKeyModel newKey = new PubringKeyModel();

                            newKey.KeyID = userKey.Attributes.GetNamedItem("KeyID").InnerText;
                            newKey.Time = Int32.Parse(userKey.Attributes.GetNamedItem("Time").InnerText);
                            newKey.RSAKey = userKey.InnerText;

                            client.Keys.Add(newKey);
                        }
                    }

                    //коды клиента 
                    XmlNodeList nodesWithCodes = doc.SelectNodes("//qx:Classes", nsmanager);
                    foreach (XmlNode node in nodesWithCodes)
                    {
                        string clientCodes = node.Attributes.GetNamedItem("ClientCodes").InnerText;
                        if (clientCodes.Contains(","))
                        {
                            string [] codesArray = clientCodes.Split(", ");
                            foreach (string code in codesArray)
                            {
                                if (!client.ClientCodes.Contains(code))
                                {
                                    client.ClientCodes.Add(code);
                                }
                            }
                        }
                        else
                        {
                            if (!client.ClientCodes.Contains(clientCodes))
                            {
                                client.ClientCodes.Add(clientCodes);
                            }                            
                        }
                    }


                    clientList.Add(client);
                }
            }

            return clientList;
        }


        public async Task<ListStringResponseModel> BlockUserByMatrixClientCode(MatrixClientCodeModel model)
        {
            return await _repository.BlockUserByMatrixClientCode(model);
        }
        public async Task<ListStringResponseModel> BlockUserByFortsClientCode(FortsClientCodeModel model)
        {
            return await _repository.BlockUserByFortsClientCode(model);
        }
        public async Task<ListStringResponseModel> BlockUserByUID(int uid)
        {
            return await _repository.BlockUserByUID(uid);
        }


        public async Task<ListStringResponseModel> SetNewPubringKeyByMatrixClientCode(MatrixCodeAndPubringKeyModel model)
        {
            return await _repository.SetNewPubringKeyByMatrixClientCode(model);
        }
        public async Task<ListStringResponseModel> SetNewPubringKeyByFortsClientCode(FortsCodeAndPubringKeyModel model)
        {
            return await _repository.SetNewPubringKeyByFortsClientCode(model);
        }


        public async Task<ListStringResponseModel> SetAllTradesByMatrixClientCode(MatrixClientCodeModel model)
        {
            return await _repository.SetAllTradesByMatrixClientCode(model);
        }
        public async Task<ListStringResponseModel> SetAllTradesByFortsClientCode(FortsClientCodeModel model)
        {
            return await _repository.SetAllTradesByFortsClientCode(model);
        }

        private void WaitAndGenerateNewFileCurrClnts(Object source, System.Timers.ElapsedEventArgs e)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore Timer WaitAndGenerateNewFileCurrClnts Called");

            RenewAllClientFile();

            _timerUpdateFileCurrClnts.Stop();
        }

        public async void RenewAllClientFile()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewAllClientFile Called");
            
            //послать запрос на формирование нового файла с всеми клиентами            
            ListStringResponseModel generateNewCurrClnts = await _repository.GenerateNewFileCurrClnts();

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewAllClientFile generateNewCurrClnts.IsSuccess={generateNewCurrClnts.IsSuccess}");

            //проверим выполнение SFTP запроса на создание нового CurrClnts
            if (generateNewCurrClnts.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewAllClientFile generateNewCurrClnts file is ={generateNewCurrClnts.Messages[0]}");

                //ждем минуту и делаем 10 попыток найти файл через каждые 20 секунд

                Thread.Sleep(60000);

                bool isSuccess = false;

                for (int i = 0; i < 10; i++)
                {
                    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewAllClientFile try {i} find complete request");

                    ListStringResponseModel getFileResult = await _repository.GetResultFromQuikSFTPFileUpload(generateNewCurrClnts.Messages[0]);
                    if(getFileResult.IsSuccess && getFileResult.Messages[0].Contains("обработан и исполнен"))
                    {
                        //если успешно - запускаем скачивание файла
                        isSuccess = true;
                        await DownloadNewFileCurrClnts();
                        break;
                    }
                    else
                    {
                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewAllClientFile current status is {getFileResult.Messages[0]}");
                    }

                    Thread.Sleep(20000);
                }

                if (!isSuccess)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewAllClientFile any search of complete request is failed.");
                }

            }
            else
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewAllClientFile generateNewCurrClnts.IsSuccess={generateNewCurrClnts.IsSuccess}, " +
                    $"{generateNewCurrClnts.Messages[0]}");
            }
        }

        private async Task DownloadNewFileCurrClnts()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore DownloadNewFileCurrClnts Called");

            await _repository.DownloadNewFileCurrClnts();
        }
    }
}