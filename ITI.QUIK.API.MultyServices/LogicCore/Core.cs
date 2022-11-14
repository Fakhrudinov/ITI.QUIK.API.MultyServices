using CommonServices;
using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Models.EMail;
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
        private CoreSettings _settings;
        private System.Timers.Timer _timerUpdateFileCurrClnts;
        private IEMail _sender;

        public Core(ILogger<Core> logger, IHttpApiRepository repository, IOptions<CoreSettings> settings, IEMail sender)
        {
            _logger = logger;
            _repository = repository;
            _settings = settings.Value;

            _timerUpdateFileCurrClnts = new System.Timers.Timer(120000);
            _timerUpdateFileCurrClnts.Elapsed += WaitAndGenerateNewFileCurrClnts;
            _timerUpdateFileCurrClnts.AutoReset = false;
            _sender=sender;
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
                newClient.NewClient.MatrixClientPortfolios = spotCodes.MatrixClientCodesList.ToArray();
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

            BoolResponse isClientHasOptionWorkshop = await _repository.GetIsClientHasOptionWorkshop(clientCode);
            newClientOW.NewOWClient.HasOptionWorkShop = isClientHasOptionWorkshop.IsTrue;
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetInfoNewUserOptionWorkShop isClientHasOptionWorkshop is {isClientHasOptionWorkshop.IsTrue}");

            return newClientOW;
        }

        public async Task<ListStringResponseModel> PostNewClientOptionWorkshop(NewClientOptionWorkShopModel newClientModel)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore PostNewClientOptionWorkshop Called for {newClientModel.Client.FirstName}");

            ListStringResponseModel createResponse = new ListStringResponseModel();

            //check if user already exist
            List<string> clientCodesArray = new List<string>();
            foreach (var forts in newClientModel.CodesPairRF)
            {
                clientCodesArray.Add(forts.FortsClientCode);
            }
            ListStringResponseModel checkUserExist = await _repository.GetIsUserAlreadyExistByCodeArray(clientCodesArray.ToArray());

            if (checkUserExist.IsSuccess)//success - client already exist
            {
                createResponse.IsSuccess = false;
                createResponse.Messages.Add("PostNewClientOptionWorkshop terminated. User already exist.");
                createResponse.Messages.AddRange(checkUserExist.Messages);

                return createResponse;
            }

            // create new user
            createResponse = await _repository.CreateNewClientOptionWorkshop(newClientModel);

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
            createResponse.NewClient = newClientModel;

            //check if user already exist
            List<string> clientCodesArray = new List<string>();
            if (newClientModel.CodesPairRF != null)
            {
                foreach (var forts in newClientModel.CodesPairRF)
                {
                    clientCodesArray.Add(forts.FortsClientCode);
                }
            }
            if (newClientModel.MatrixClientPortfolios != null)
            {
                foreach (var spot in newClientModel.MatrixClientPortfolios)
                {
                    clientCodesArray.Add(spot.MatrixClientPortfolio);
                }
            }

            ListStringResponseModel checkUserExist = await _repository.GetIsUserAlreadyExistByCodeArray(clientCodesArray.ToArray());

            if (checkUserExist.IsSuccess && newClientModel.isExistanceOverraid == false)//success - client already exist and no overrraid option selected
            {
                createResponse.IsNewClientCreationSuccess = false;
                createResponse.NewClientCreationMessages.Add("PostNewClientOptionWorkshop terminated. User already exist.");
                createResponse.NewClientCreationMessages.AddRange(checkUserExist.Messages);

                return createResponse;
            }
            else
            {
                createResponse.NewClientCreationMessages.AddRange(checkUserExist.Messages);
            }

            // create new user

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
            newMNPClient.MatrixClientPortfolios = newClientModel.MatrixClientPortfolios;
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
            if (newClientModel.MatrixClientPortfolios != null)
            {
                //codes ini
                //сначала очистим от совпадений по quik кодам клиента, например в codes.ini портфели равны MS=FX

                List<string> portfolios = new List<string>();
                foreach (var portfolio in newClientModel.MatrixClientPortfolios)
                {
                    string portfolioEqualized = portfolio.MatrixClientPortfolio;

                    if (portfolioEqualized.Contains("-FX-"))
                    {
                        portfolioEqualized = portfolioEqualized.Replace("-FX-", "-MS-");
                    }

                    if (!portfolios.Contains(portfolioEqualized))
                    {
                        portfolios.Add(portfolioEqualized);
                    }
                }

                CodesArrayModel codesArray = new CodesArrayModel();
                codesArray.MatrixClientPortfolios = new MatrixClientPortfolioModel[portfolios.Count];

                for (int i = 0; i < portfolios.Count; i++)
                {
                    codesArray.MatrixClientPortfolios[i] = new MatrixClientPortfolioModel() { MatrixClientPortfolio = portfolios[i] };
                }

                ListStringResponseModel fillCodesIniResponse = await _repository.FillCodesIniFile(codesArray);
                createResponse.IsCodesIniSuccess = fillCodesIniResponse.IsSuccess;
                createResponse.CodesIniMessages = fillCodesIniResponse.Messages;


                // ? CD reg
                bool totalSucces = true;
                foreach (var code in newClientModel.MatrixClientPortfolios)
                {
                    if (code.MatrixClientPortfolio.Contains("-CD-"))
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
            foreach (string deleteWord in _settings.RemoveTextArray)
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

        public async Task<FindedQuikClientResponse> GetIsUserAlreadyExistInAllQuikByMatrixClientAccount(string matrixClientAccount)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetIsUserAlreadyExistInAllQuikByMatrixClientAccount Called for {matrixClientAccount}");

            FindedQuikClientResponse result = new FindedQuikClientResponse();

            // найти все портфели - фильтрованные
            MatrixClientCodeModelResponse spotCodes = await _repository.GetClientAllSpotCodesFiltered(matrixClientAccount);
            if (spotCodes.Response.IsSuccess)
            {
                result.MatrixClientPortfolios = spotCodes.MatrixClientCodesList.ToArray();
            }
            else
            {
                result.MatrixClientPortfoliosMessages.AddRange(spotCodes.Response.Messages);
            }
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetIsUserAlreadyExistInAllQuikByMatrixClientAccount " +
                $"GetClientAllSpotCodesFiltered result {spotCodes.Response.IsSuccess}");

            MatrixToFortsCodesMappingResponse fortsCodes = await _repository.GetClientAllFortsCodes(matrixClientAccount);
            if (fortsCodes.Response.IsSuccess)
            {
                result.CodesPairRF = fortsCodes.MatrixToFortsCodesList.ToArray();
            }
            else
            {
                result.CodesPairRFMessages.AddRange(fortsCodes.Response.Messages);
            }
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetIsUserAlreadyExistInAllQuikByMatrixClientAccount " +
                $"GetClientAllFortsCodes result {fortsCodes.Response.IsSuccess}");
            // проверить, что хоть какие то портфели вернулись
            if (result.CodesPairRF is null && result.MatrixClientPortfolios is null)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetIsUserAlreadyExistInAllQuikByMatrixClientAccount " +
                    $"(404) no portfolios found for {matrixClientAccount}");
                result.Messages.Add($"(404) no portfolios found for {matrixClientAccount}");
                result.IsSuccess = false;

                return result;
            }

            //все портфели и коды срочного в список
            List<string> allportfolios = new List<string>();
            if (result.MatrixClientPortfolios is not null)
            {
                foreach (var portfolio in result.MatrixClientPortfolios)
                {
                    allportfolios.Add(portfolio.MatrixClientPortfolio);
                }
            }
            if (result.CodesPairRF is not null)
            {
                foreach (var fortsCode in result.CodesPairRF)
                {
                    allportfolios.Add(fortsCode.FortsClientCode);
                }
            }

            // INSTR TW - получить зарегистрированные в бд строки
            result.InstrTWDBRecords = await _repository.GetRecordsFromInstrTwDataBase(allportfolios);
            result.InstrTWDBRecordsMessages = result.InstrTWDBRecords.Messages;

            // проверить наличие в файле CurrClients (QAdmin)
            ListStringResponseModel findedInQadmin = await _repository.GetIsUserAlreadyExistByCodeArray(allportfolios.ToArray());
            if (findedInQadmin.IsSuccess)
            {
                result.isQuikQAdminClientFinded = true;
                result.QuikQAdminClient = GetClientsFromMessages(findedInQadmin.Messages);
            }
            else
            {
                result.QuikQAdminClientMessages.AddRange(findedInQadmin.Messages);
            }

            // проверить CD портфели - есть ли в шаблонах
            if (result.MatrixClientPortfolios is not null)
            {
                foreach (var portfolio in result.MatrixClientPortfolios)
                {
                    if (portfolio.MatrixClientPortfolio.Contains("-CD-"))
                    {
                        MatrixPortfolioAtTemplates codeAtTemplates = new MatrixPortfolioAtTemplates() { MatrixClientPortfolios = portfolio };
                        string quikCdportfolio = CommonServices.PortfoliosConvertingService.GetQuikCdPortfolio(portfolio.MatrixClientPortfolio);

                        string[] tempatesName = new string[]
                        {
                            "CD_portfolio", "CD_OnlyClose"
                        };

                        // ищем в шаблонах по комиссии
                        for (int i = 0; i < tempatesName.Length; i++)
                        {
                            ListStringResponseModel isAddedToTemplate = await _repository.GetAllClientsFromTemplatePoKomissii(tempatesName[i]);
                            if (isAddedToTemplate.IsSuccess)
                            {
                                //search code in result
                                if (isAddedToTemplate.Messages.Contains(quikCdportfolio))
                                {
                                    codeAtTemplates.TemplatePoKomissii = tempatesName[i];
                                    break;
                                }
                            }
                            else
                            {
                                result.IsCdPortfoliosAddedToTemplateMessages.AddRange(isAddedToTemplate.Messages);
                            }
                        }

                        // ищем в шаблоне по плечу
                        ListStringResponseModel isAddedToTemplatePoPlechu = await _repository.GetAllClientsFromTemplatePoPlechu(tempatesName[0]);
                        if (isAddedToTemplatePoPlechu.IsSuccess)
                        {
                            //search code in result
                            if (isAddedToTemplatePoPlechu.Messages.Contains(quikCdportfolio))
                            {
                                codeAtTemplates.TemplatePoPlechu = tempatesName[0];
                            }
                        }
                        else
                        {
                            result.IsCdPortfoliosAddedToTemplateMessages.AddRange(isAddedToTemplatePoPlechu.Messages);
                        }

                        result.PortfolioAtTemplates.Add(codeAtTemplates);
                    }
                }

                // check is all templates filled. if not - set false at IsCdPortfoliosAddedToTemplates
                foreach (var cdPortfolio in result.PortfolioAtTemplates)
                {
                    if (cdPortfolio.TemplatePoPlechu == null || cdPortfolio.TemplatePoKomissii == null)
                    {
                        result.IsCdPortfoliosAddedToTemplates = false;
                    }
                }
            }

            // проверить коды в codes.ini
            if (result.MatrixClientPortfolios is not null)
            {
                List<string> spotPortfolios = new List<string>();
                foreach (var portfolio in result.MatrixClientPortfolios)
                {
                    spotPortfolios.Add(portfolio.MatrixClientPortfolio);
                }

                BoolResponse isCodesIniFilled = await _repository.GetIsAllSpotPortfoliosPresentInFileCodesIni(spotPortfolios);
                result.IsCodesIniFilled = isCodesIniFilled.IsTrue;
                result.IsCodesIniFilledMessages = isCodesIniFilled.Messages;
            }

            // ? общий анализ ?

            return result;
        }

        public async Task<FindedQuikQAdminClientResponse> GetIsUserAlreadyExistByMatrixClientAccount(string matrixClientAccount)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetIsUserAlreadyExistByMatrixClientAccount Called for {matrixClientAccount}");
            
            ListStringResponseModel findedClients = await _repository.GetIsUserAlreadyExistByMatrixClientAccount(matrixClientAccount);
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore GetIsUserAlreadyExistByMatrixClientAccount result is {findedClients.IsSuccess}");

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


        public async Task<ListStringResponseModel> BlockUserByMatrixClientCode(MatrixClientPortfolioModel model)
        {
            //check if user already exist and only 1
            ListStringResponseModel checkUserExist = await _repository.GetIsUserAlreadyExistByMatrixClientAccount(model.MatrixClientPortfolio.Split('-').First());

            if (checkUserExist.IsSuccess && checkUserExist.Messages.Count > 1)//success - client already exist && messages - 1 message for 1 founded
            {
                checkUserExist.IsSuccess = false;
                checkUserExist.Messages.Insert(0, "BlockUserByMatrixClientCode terminated. Founded more then one user:");

                return checkUserExist;
            }

            // block unique user
            return await _repository.BlockUserByMatrixClientCode(model);
        }
        public async Task<ListStringResponseModel> BlockUserByFortsClientCode(FortsClientCodeModel model)
        {
            //check if user already exist and only 1
            ListStringResponseModel checkUserExist = await _repository.GetIsUserAlreadyExistByFortsCode(model.FortsClientCode);

            if (checkUserExist.IsSuccess && checkUserExist.Messages.Count > 1)//success - client already exist && messages - 1 message for 1 founded
            {
                checkUserExist.IsSuccess = false;
                checkUserExist.Messages.Insert(0, "BlockUserByFortsClientCode terminated. Founded more then one user:");

                return checkUserExist;
            }

            // block unique user
            return await _repository.BlockUserByFortsClientCode(model);
        }
        public async Task<ListStringResponseModel> BlockUserByUID(int uid)
        {
            return await _repository.BlockUserByUID(uid);
        }


        public async Task<ListStringResponseModel> SetNewPubringKeyByMatrixClientCode(MatrixCodeAndPubringKeyModel model)
        {
            //check if user already exist and only 1
            ListStringResponseModel checkUserExist = await _repository.GetIsUserAlreadyExistByMatrixClientAccount(model.MatrixClientPortfolio.MatrixClientPortfolio.Split('-').First());

            if (checkUserExist.IsSuccess && checkUserExist.Messages.Count > 1)//success - client already exist && messages - 1 message for 1 founded
            {
                checkUserExist.IsSuccess = false;
                checkUserExist.Messages.Insert(0, "SetNewPubringKeyByMatrixClientCode terminated. Founded more then one user:");

                return checkUserExist;
            }

            return await _repository.SetNewPubringKeyByMatrixClientCode(model);
        }
        public async Task<ListStringResponseModel> SetNewPubringKeyByFortsClientCode(FortsCodeAndPubringKeyModel model)
        {
            //check if user already exist and only 1
            ListStringResponseModel checkUserExist = await _repository.GetIsUserAlreadyExistByFortsCode(model.ClientCode.FortsClientCode);

            if (checkUserExist.IsSuccess && checkUserExist.Messages.Count > 1)//success - client already exist && messages - 1 message for 1 founded
            {
                checkUserExist.IsSuccess = false;
                checkUserExist.Messages.Insert(0, "SetNewPubringKeyByFortsClientCode terminated. Founded more then one user:");

                return checkUserExist;
            }

            return await _repository.SetNewPubringKeyByFortsClientCode(model);
        }


        public async Task<ListStringResponseModel> SetAllTradesByMatrixClientCode(MatrixClientPortfolioModel model)
        {
            //check if user already exist and only 1
            ListStringResponseModel checkUserExist = await _repository.GetIsUserAlreadyExistByMatrixClientAccount(model.MatrixClientPortfolio.Split('-').First());

            if (checkUserExist.IsSuccess && checkUserExist.Messages.Count > 1)//success - client already exist && messages - 1 message for 1 founded
            {
                checkUserExist.IsSuccess = false;
                checkUserExist.Messages.Insert(0, "SetAllTradesByMatrixClientCode terminated. Founded more then one user:");

                return checkUserExist;
            }

            return await _repository.SetAllTradesByMatrixClientCode(model);
        }
        public async Task<ListStringResponseModel> SetAllTradesByFortsClientCode(FortsClientCodeModel model)
        {
            //check if user already exist and only 1
            ListStringResponseModel checkUserExist = await _repository.GetIsUserAlreadyExistByFortsCode(model.FortsClientCode);

            if (checkUserExist.IsSuccess && checkUserExist.Messages.Count > 1)//success - client already exist && messages - 1 message for 1 founded
            {
                checkUserExist.IsSuccess = false;
                checkUserExist.Messages.Insert(0, "SetAllTradesByFortsClientCode terminated. Founded more then one user:");

                return checkUserExist;
            }

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

        public async Task<ListStringResponseModel> RenewLimLimFile()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewLimLimFile Called");

            return await _repository.DownloadLimLimFile();
        }

        private async Task DownloadNewFileCurrClnts()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore DownloadNewFileCurrClnts Called");

            await _repository.DownloadNewFileCurrClnts();
        }

        public async Task<ListStringResponseModel> RenewClientsInSpotTemplatesPoKomissii(bool sendReport)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInSpotTemplatesPoKomissii Called");

            ListStringResponseModel result = new ListStringResponseModel();
            NewEMail message = new NewEMail();
            message.Subject = "QUIK обновление шаблонов 'По комиссии' в Qadmin MC0138200000 библиотеке";
            message.Body = "<html><body><h2>QUIK обновление шаблонов По комиссии спот</h2>";

            //установка MS и FX портфелей

            string[] templateNames = new string[]
            {
                "FoeNeRes", // 0 список злых нерезов
                "FrnNeResKval",// 1 список добрых нерезов квалов 
                "FrnNeResNeKv",// 2 список добрых нерезов Не квалов 
                "KSUR_NeKval", // 3 список неквалов КСУР
                "KPUR_NeKval", // 4 список неквалов КПУР
                "KPUR_Kval",   // 5 список квалов КПУР
                "CD_Restrict", // 6 для CD портфелей все нерезы-не дружественные.
                "CD_portfolio" // 7 для CD портфелей все кпур и ксур клиенты, нерезы-дружественные, квалы и не квалы
            };

            for (int i = 0; i < templateNames.Length; i++)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInSpotTemplatesPoKomissii work with {templateNames[i]}");

                message.Body = message.Body + $"<h3>Установка в шаблон {templateNames[i]}</h3>";
                //запросить список клиентов в БД матрицы
                MatrixClientCodeModelResponse clientportfolios = new MatrixClientCodeModelResponse();
                switch (i)
                {
                    case 0://запросить список злых нерезов
                        clientportfolios = await _repository.GetAllEnemyNonResidentSpotPortfolios();
                        message.Body = message.Body + $"<p>Найдено в БД Матрицы вражеских нерезидентов спот портфелей {clientportfolios.MatrixClientCodesList.Count}</p>";
                        break;
                    case 1://запросить список добрых нерезов квалов
                        clientportfolios = await _repository.GetAllFrendlyNonResidentKvalSpotPortfolios();
                        message.Body = message.Body + $"<p>Найдено в БД Матрицы Дружественных нерезидентов квалов спот портфелей {clientportfolios.MatrixClientCodesList.Count}</p>";
                        break;
                    case 2://запросить список добрых нерезов не квалов
                        clientportfolios = await _repository.GetAllFrendlyNonResidentNonKvalSpotPortfolios();
                        message.Body = message.Body + $"<p>Найдено в БД Матрицы Дружественных нерезидентов не квалов спот портфелей {clientportfolios.MatrixClientCodesList.Count}</p>";
                        break;
                    case 3://запросить список неквалов КСУР
                        clientportfolios = await _repository.GetAllNonKvalKsurUsersSpotPortfolios();
                        message.Body = message.Body + $"<p>Найдено в БД Матрицы Неквалов КСУР спот портфелей {clientportfolios.MatrixClientCodesList.Count}</p>";
                        break;
                    case 4://запросить список неквалов КПУР
                        clientportfolios = await _repository.GetAllNonKvalKpurUsersSpotPortfolios();
                        message.Body = message.Body + $"<p>Найдено в БД Матрицы Неквалов КПУР спот портфелей {clientportfolios.MatrixClientCodesList.Count}</p>";
                        break;
                    case 5://запросить список квалов КПУР
                        clientportfolios = await _repository.GetAllKvalKpurUsersSpotPortfolios();
                        message.Body = message.Body + $"<p>Найдено в БД Матрицы Квалов КПУР спот портфелей {clientportfolios.MatrixClientCodesList.Count}</p>";
                        break;
                    case 6://6 запрет торговли на валюте для CD портфелей
                        //clientportfolios = await _repository.GetAllEnemyNonResidentCdPortfolios();//запросить список злых нерезов
                        clientportfolios = await _repository.GetAllRestrictedCDPortfolios();//запросить список портфелей для шаблона CD_Restrict
                        message.Body = message.Body + $"<p>Найдено в БД Матрицы CD портфелей для шаблона CD_Restrict: {clientportfolios.MatrixClientCodesList.Count}</p>";

                        //6 запрет торговли на валюте - для всех нерезидентов, дружественных и не дружественных.
                        //MatrixClientCodeModelResponse frendlyNeRez = await _repository.GetAllFrendlyNonResidentCdPortfolios();//запросить список добрых нерезов
                        //MatrixClientCodeModelResponse enemyNeRez = await _repository.GetAllEnemyNonResidentCdPortfolios();//запросить список злых нерезов

                        //message.Body = message.Body + 
                        //    $"<p>Найдено в БД Матрицы " +
                        //    $"<br />Вражеских нерезидентов CD портфелей {enemyNeRez.MatrixClientCodesList.Count}" +
                        //    $"<br />Дружественных нерезидентов CD портфелей {frendlyNeRez.MatrixClientCodesList.Count}</p>";

                        ////объеденить списки в общий список clientportfolios
                        //if (frendlyNeRez.Response.IsSuccess)
                        //{
                        //    if (frendlyNeRez.MatrixClientCodesList is not null)
                        //    {
                        //        clientportfolios.MatrixClientCodesList.AddRange(frendlyNeRez.MatrixClientCodesList);
                        //    }
                        //}
                        //else
                        //{
                        //    result.IsSuccess = false;
                        //    result.Messages.AddRange(frendlyNeRez.Response.Messages);
                        //}

                        //if (enemyNeRez.Response.IsSuccess)
                        //{
                        //    if (enemyNeRez.MatrixClientCodesList is not null)
                        //    {
                        //        clientportfolios.MatrixClientCodesList.AddRange(enemyNeRez.MatrixClientCodesList);
                        //    }
                        //}
                        //else
                        //{
                        //    result.IsSuccess = false;
                        //    result.Messages.AddRange(enemyNeRez.Response.Messages);
                        //}

                        break;

                    case 7://7 все CD портфели с разрешением торговать
                        clientportfolios = await _repository.GetAllAllowedCDPortfolios();//запросить список портфелей для шаблона CD_portfolio
                        message.Body = message.Body + $"<p>Найдено в БД Матрицы CD портфелей для шаблона CD_portfolio: {clientportfolios.MatrixClientCodesList.Count}</p>";
                        // все кпур и ксур клиенты, добрые нерезы квалы и не квалы - в общий список Cd портфелей
                        //MatrixClientCodeModelResponse kvalKPUR = await _repository.GetAllKvalKpurUsersCdPortfolios();//запросить список квалов КПУР
                        //MatrixClientCodeModelResponse kvalKSUR = await _repository.GetAllKvalKsurUsersCdPortfolios();//запросить список квалов КСУР
                        //MatrixClientCodeModelResponse nonKvalKPUR = await _repository.GetAllNonKvalKpurUsersCdPortfolios();//запросить список НЕ квалов КПУР
                        //MatrixClientCodeModelResponse nonKvalKSUR = await _repository.GetAllNonKvalKsurUsersCdPortfolios();//запросить список НЕ квалов КСУР
                        //MatrixClientCodeModelResponse frendlyNeRez = await _repository.GetAllFrendlyNonResidentCdPortfolios();//запросить список добрых нерезов

                        //message.Body = message.Body +
                        //    $"<p>Найдено в БД Матрицы " +
                        //    $"<br />Квалов КПУР CD портфелей {kvalKPUR.MatrixClientCodesList.Count}" +
                        //    $"<br />Квалов КСУР CD портфелей {kvalKSUR.MatrixClientCodesList.Count}" +
                        //    $"<br />НЕ квалов КПУР CD портфелей {nonKvalKPUR.MatrixClientCodesList.Count}" +
                        //    $"<br />НЕ квалов КСУР CD портфелей {nonKvalKSUR.MatrixClientCodesList.Count}" +
                        //    $"<br />всех дружественных нерезидентов CD портфелей {frendlyNeRez.MatrixClientCodesList.Count}</p>";

                        ////объеденить списки в общий список clientportfolios
                        //if (kvalKPUR.Response.IsSuccess)
                        //{
                        //    if (kvalKPUR.MatrixClientCodesList is not null)
                        //    {
                        //        clientportfolios.MatrixClientCodesList.AddRange(kvalKPUR.MatrixClientCodesList);
                        //    }
                        //}
                        //else
                        //{
                        //    clientportfolios.Response.IsSuccess = false;
                        //    clientportfolios.Response.Messages.AddRange(kvalKPUR.Response.Messages);
                        //}

                        //if (kvalKSUR.Response.IsSuccess)
                        //{
                        //    if (kvalKSUR.MatrixClientCodesList is not null)
                        //    {
                        //        clientportfolios.MatrixClientCodesList.AddRange(kvalKSUR.MatrixClientCodesList);
                        //    }
                        //}
                        //else
                        //{
                        //    clientportfolios.Response.IsSuccess = false;
                        //    clientportfolios.Response.Messages.AddRange(kvalKSUR.Response.Messages);
                        //}

                        //if (nonKvalKPUR.Response.IsSuccess)
                        //{
                        //    if (nonKvalKPUR.MatrixClientCodesList is not null)
                        //    {
                        //        clientportfolios.MatrixClientCodesList.AddRange(nonKvalKPUR.MatrixClientCodesList);
                        //    }
                        //}
                        //else
                        //{
                        //    clientportfolios.Response.IsSuccess = false;
                        //    clientportfolios.Response.Messages.AddRange(nonKvalKPUR.Response.Messages);
                        //}

                        //if (nonKvalKSUR.Response.IsSuccess)
                        //{
                        //    if (nonKvalKSUR.MatrixClientCodesList is not null)
                        //    {
                        //        clientportfolios.MatrixClientCodesList.AddRange(nonKvalKSUR.MatrixClientCodesList);
                        //    }
                        //}
                        //else
                        //{
                        //    clientportfolios.Response.IsSuccess = false;
                        //    clientportfolios.Response.Messages.AddRange(nonKvalKSUR.Response.Messages);
                        //}

                        //if (frendlyNeRez.Response.IsSuccess)
                        //{
                        //    if (frendlyNeRez.MatrixClientCodesList is not null)
                        //    {
                        //        clientportfolios.MatrixClientCodesList.AddRange(frendlyNeRez.MatrixClientCodesList);
                        //    }
                        //}
                        //else
                        //{
                        //    clientportfolios.Response.IsSuccess = false;
                        //    clientportfolios.Response.Messages.AddRange(frendlyNeRez.Response.Messages);
                        //}

                        break;
                }

                //установить список в шаблон по комиссии Quik БРЛ
                if (clientportfolios.Response.IsSuccess)
                {
                    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInSpotTemplatesPoKomissii " +
                        $"clientportfolios count {clientportfolios.MatrixClientCodesList.Count}");

                    // это нужно чтобы затереть старые данные любым кодом клиента
                    if (clientportfolios.MatrixClientCodesList.Count < 1)
                    {
                        message.Body = message.Body + $"<p>Пустой список заменяем на подставного клиента BP00001-MS-99, для перезаписи ранее установленного списка</p>";
                        _logger.LogDebug($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInSpotTemplatesPoKomissii " +
                            $"Пустой список заменяем на подставного клиента BP00001-MS-99");
                        clientportfolios.MatrixClientCodesList.Add(
                            new MatrixClientPortfolioModel() 
                            { 
                                MatrixClientPortfolio = "BP00001-MS-99"
                            });
                    }

                    TemplateAndMatrixCodesModel templateAndMatrixCodes = new TemplateAndMatrixCodesModel()
                    {
                        MatrixClientPortfolio = clientportfolios.MatrixClientCodesList.ToArray(),
                        Template = templateNames[i]
                    };

                    ListStringResponseModel setCodeToTemplatePoKomissii = await _repository.SetClientsToTemplatePoKomissii(templateAndMatrixCodes);
                    if (setCodeToTemplatePoKomissii.IsSuccess == false)
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInSpotTemplatesPoKomissii " +
                            $"Ошибка установки списка клиентов в шаблон {templateNames[i]} ");

                        foreach (var text in setCodeToTemplatePoKomissii.Messages)
                        {
                            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} - {text}");
                            message.Body = message.Body + $"<p style='color:red'>Ошибка установки в {templateNames[i]}: {text}</p>";
                        }

                        result.IsSuccess = false;
                        result.Messages.AddRange(setCodeToTemplatePoKomissii.Messages);
                    }
                    else
                    {
                        message.Body = message.Body + $"<p>Установка в {templateNames[i]} успешна</p>";
                    }
                }
                else
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInSpotTemplatesPoKomissii Ошибка запроса списка клиентов ");

                    foreach (var text in clientportfolios.Response.Messages)
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} - {text}");
                        message.Body = message.Body + $"<p style='color:red'>Ошибка запроса списка клиентов {text}</p>";
                    }

                    result.IsSuccess = false;
                    result.Messages.AddRange(clientportfolios.Response.Messages);
                }
            }

            message.Body = message.Body + "</body></html>";

            if (sendReport)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInSpotTemplatesPoKomissii отправляем почту");
                await _sender.Send(message);
            }

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInSpotTemplatesPoKomissii all done");
            return result;
        }

        private List<string> RemoveCodeIfFinded(List<string> codesFromCurrClntsXML, List<FortsClientCodeModel> source)
        {
            //идем от конца, чтобы удалять из list можно было без ошибок
            for (int i = codesFromCurrClntsXML.Count - 1; i >= 0; i--)
            {
                string matrixCode = PortfoliosConvertingService.GetMatrixFortsCode(codesFromCurrClntsXML[i]);

                foreach (var code in source)
                {
                    if (code.FortsClientCode.Equals(matrixCode))
                    {
                        codesFromCurrClntsXML.RemoveAt(i);
                    }
                }
            }

            return codesFromCurrClntsXML;
        }

        private List<string> GetFortsCodesFromCurrClntsXml(string filePathToCurrClntsXml)
        {
            List<string> result = new List<string>();

            XmlDocument xDoc = new XmlDocument();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var sr = new StreamReader(filePathToCurrClntsXml, Encoding.GetEncoding("windows-1251"))) xDoc.Load(sr);
            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;
            // обход всех узлов в корневом элементе
            foreach (XmlNode xnode in xRoot)
            {
                string futCodes = "";

                // обходим все дочерние узлы
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "Classes")
                    {
                        if (childnode.Attributes.GetNamedItem("FirmID").Value.Contains("SPBFUT"))
                        {
                            if (!childnode.InnerText.Equals("INSTR_RF"))
                            {
                                futCodes = childnode.Attributes.GetNamedItem("ClientCodes").Value;

                                if (futCodes.Length > 0)
                                {
                                    string[] codesSplitted = futCodes.Split(", ");
                                    foreach (string code in codesSplitted)
                                    {
                                        if (code.Contains("SPBFUT"))
                                        {
                                            if (!result.Contains(code))
                                            {
                                                result.Add(code);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
        
        public async Task<ListStringResponseModel> RenewClientsInFortsTemplatesPoKomissii(bool sendReport)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInFortsTemplatesPoKomissii Called");

            ListStringResponseModel result = new ListStringResponseModel();
            NewEMail message = new NewEMail();
            message.Subject = "QUIK обновление шаблонов 'По комиссии' в Qadmin SPBFUT библиотеке";
            message.Body = "<html><body><h2>QUIK обновление шаблона По комиссии фортс 'RF_Restrict'</h2>";

            // запрет торговли по срочному рынку

            // Все QUIK коды срочки берутся из файла xml

            // 1. берем список ВСЕХ вражеских нерезов не зависимо от Q и оставляем только те что есть в XML
            // должен остаться allEnemyNonResident(list) с кодами встреченными в codesFromCurrClntsXML
            // 2. берем всех
            //      квалов
            //      неквалов сдавших тест 16
            // и вычитаем эти коды из файла xml - должен остаться codesFromCurrClntsXML без кодов квалов и тестированных
            // остатки 1 и 2 объеденяем и загружаем в шаблон RF_Restrict

            string filePathToCurrClntsXml = _settings.PathToCurrClntsXml;
            if (!File.Exists(filePathToCurrClntsXml))
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} Error! File CurrClnts.xml not found at " + filePathToCurrClntsXml);
                message.Body = message.Body + $"<p style='color:red'>Error! File CurrClnts.xml not found at {filePathToCurrClntsXml}</p>";
            }
            else
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInFortsTemplatesPoKomissii " +
                    $"CurrClnts.xml from {File.GetCreationTime(filePathToCurrClntsXml)} finded");
                message.Body = message.Body + $"<p>Файл CurrClnts.xml создан: {File.GetLastWriteTime(filePathToCurrClntsXml)}</p>";
                List<string> codesFromCurrClntsXML = GetFortsCodesFromCurrClntsXml(filePathToCurrClntsXml);//список всех кодов срочки из CurrClnts.xml
                message.Body = message.Body + $"<p>Найдено кодов срочного рынка в CurrClnts.xml: {codesFromCurrClntsXML.Count}</p>";

                //1
                FortsClientCodeModelResponse allEnemyNonResident = await _repository.GetAllEnemyNonResidentFortsCodes();// список ВСЕХ вражеских нерезов не зависимо от Q
                //2
                FortsClientCodeModelResponse allKvalClientsFortsCodes = await _repository.GetAllKvalClientsFortsCodes();//всех квалов фортс                                                                                                                 //      квалов
                FortsClientCodeModelResponse allNonKvalWithTest16FortsCodes = await _repository.GetAllNonKvalWithTest16FortsCodes();//всех неквалов фортс сдавших тест 16

                if (allEnemyNonResident.Response.IsSuccess)
                {
                    message.Body = message.Body + $"<p>Найдено кодов срочного рынка всех вражеских нерезов не зависимо от торговой системы: " +
                        $"{allEnemyNonResident.FortsClientCodesList.Count}</p>";
                }
                else
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInTemplatesPoKomissii Ошибка " +
                        $"при получении кодов срочного рынка ВСЕХ вражеских нерезов не зависимо от торговой системы");

                    message.Body = message.Body + $"<p style='color:red'>Ошибка при получении кодов срочного рынка ВСЕХ вражеских нерезов не зависимо от торговой системы</p>";
                    foreach (var text in allEnemyNonResident.Response.Messages)
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInFortsTemplatesPoKomissii Ошибка = {text}");
                        message.Body = message.Body + $"<p style='color:red'> - {text}</p>";
                    }
                }

                if (allKvalClientsFortsCodes.Response.IsSuccess)
                {
                    message.Body = message.Body + $"<p>Найдено кодов срочного рынка всех квалов фортс не зависимо от торговой системы: " +
                        $"{allKvalClientsFortsCodes.FortsClientCodesList.Count}</p>";
                }
                else
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInFortsTemplatesPoKomissii Ошибка " +
                        $"при получении кодов срочного рынка всех квалов фортс не зависимо от торговой системы");

                    message.Body = message.Body + $"<p style='color:red'>Ошибка при получении кодов срочного рынка всех квалов фортс не зависимо от торговой системы</p>";
                    foreach (var text in allKvalClientsFortsCodes.Response.Messages)
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInFortsTemplatesPoKomissii Ошибка = {text}");
                        message.Body = message.Body + $"<p style='color:red'> - {text}</p>";
                    }
                }

                if (allNonKvalWithTest16FortsCodes.Response.IsSuccess)
                {
                    message.Body = message.Body + $"<p>Найдено кодов срочного рынка всех неквалов фортс сдавших тест 16 не зависимо от торговой системы: " +
                        $"{allNonKvalWithTest16FortsCodes.FortsClientCodesList.Count}</p>";
                }
                else
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInFortsTemplatesPoKomissii Ошибка " +
                        $"при получении кодов срочного рынка всех неквалов фортс сдавших тест 16 не зависимо от торговой системы");

                    message.Body = message.Body + $"<p style='color:red'>Ошибка при получении кодов срочного рынка всех неквалов фортс сдавших тест 16 не зависимо от торговой системы</p>";
                    foreach (var text in allNonKvalWithTest16FortsCodes.Response.Messages)
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInFortsTemplatesPoKomissii Ошибка = {text}");
                        message.Body = message.Body + $"<p style='color:red'> - {text}</p>";
                    }
                }

                if (allEnemyNonResident.Response.IsSuccess && allKvalClientsFortsCodes.Response.IsSuccess && allNonKvalWithTest16FortsCodes.Response.IsSuccess)
                {
                    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInFortsTemplatesPoKomissii all db request is ok, work with data");

                    message.Body = message.Body + $"<h3>Обработка полученных из БД матрицы данных</h3>";
                    // 1. берем список ВСЕХ вражеских нерезов не зависимо от Q и оставляем только те что есть в XML
                    //идем от конца, чтобы удалять из list можно было без ошибок
                    for (int i = allEnemyNonResident.FortsClientCodesList.Count - 1; i >= 0; i--)
                    {
                        //переведем в формат Q для сравнения
                        string quikCode = PortfoliosConvertingService.GetQuikFortsCode(allEnemyNonResident.FortsClientCodesList[i].FortsClientCode);

                        //если кода нет, удаляем, нам он не интересен
                        if (!codesFromCurrClntsXML.Contains(quikCode))
                        {
                            allEnemyNonResident.FortsClientCodesList.RemoveAt(i);
                        }
                    }

                    message.Body = message.Body + $"<p>Совпадений всех вражеских нерезов с CurrClnts.xml: {allEnemyNonResident.FortsClientCodesList.Count}</p>";

                    // 2. берем всех
                    //      квалов
                    //      неквалов сдавших тест 16
                    // и вычитаем эти коды из файла xml - должен остаться codesFromCurrClntsXML без кодов квалов и тестированных
                    codesFromCurrClntsXML = RemoveCodeIfFinded(codesFromCurrClntsXML, allKvalClientsFortsCodes.FortsClientCodesList);
                    message.Body = message.Body + $"<p>Осталось в CurrClnts.xml после удаления кодов всех квалов: {codesFromCurrClntsXML.Count}</p>";
                    codesFromCurrClntsXML = RemoveCodeIfFinded(codesFromCurrClntsXML, allNonKvalWithTest16FortsCodes.FortsClientCodesList);
                    message.Body = message.Body + $"<p>Осталось в CurrClnts.xml после удаления кодов всех неквалов с тестом 16: {codesFromCurrClntsXML.Count}</p>";

                    //коды quik формата заменяем на формат матрицы
                    for (int i = 0; i < codesFromCurrClntsXML.Count; i++)
                    {
                        codesFromCurrClntsXML[i] = PortfoliosConvertingService.GetMatrixFortsCode(codesFromCurrClntsXML[i]);
                    }

                    //объеденяем списки
                    foreach (var code in codesFromCurrClntsXML)
                    {
                        FortsClientCodeModel model = new FortsClientCodeModel();
                        model.FortsClientCode = code;
                        allEnemyNonResident.FortsClientCodesList.Add(model);
                    }

                    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInFortsTemplatesPoKomissii отправляем список с кодами " +
                        $"{allEnemyNonResident.FortsClientCodesList.Count} в QUIK БРЛ SPBEX");
                    message.Body = message.Body + $"<p>В общем списке запрещенных кодов: {allEnemyNonResident.FortsClientCodesList.Count}</p>";                    

                    //выгружаем результат в forts шаблон по комиссии RF_Restrict
                    TemplateAndMatrixFortsCodesModel templateAndMatrixFortsCodesModel = new TemplateAndMatrixFortsCodesModel();
                    templateAndMatrixFortsCodesModel.Template = "RF_Restrict";
                    templateAndMatrixFortsCodesModel.FortsClientCodes = allEnemyNonResident.FortsClientCodesList.ToArray();
                    message.Body = message.Body + $"<h3>Выгрузка в QUIK БРЛ SPBFUT шаблон ео комиссии {templateAndMatrixFortsCodesModel.Template}</h3>";

                    ListStringResponseModel setCodeToTemplatePoKomissii = await _repository.SetClientsToFortsTemplatePoKomissii(templateAndMatrixFortsCodesModel);
                    if (setCodeToTemplatePoKomissii.IsSuccess == false)
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInFortsTemplatesPoKomissii Ошибка " +
                            $"при установке кодов в Forts шаблон по комиссии {templateAndMatrixFortsCodesModel.Template}");

                        foreach (var text in setCodeToTemplatePoKomissii.Messages)
                        {
                            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInFortsTemplatesPoKomissii Ошибка = {text}");

                            message.Body = message.Body + $"<p style='color:red'>Ошибка установки в forts BRL RF_Restrict: {text}</p>";
                        }

                        result.IsSuccess = false;
                        result.Messages.AddRange(setCodeToTemplatePoKomissii.Messages);
                    }
                    else
                    {
                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInFortsTemplatesPoKomissii в forts BRL RF_Restrict успешна");
                        message.Body = message.Body + $"<p>Установка в forts BRL RF_Restrict успешна</p>";
                    }
                }
            }

            message.Body = message.Body + "</body></html>";

            if (sendReport)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInFortsTemplatesPoKomissii Send message");
                await _sender.Send(message);
            }

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewClientsInFortsTemplatesPoKomissii all done");
            return result;
        }



        public async Task<ListStringResponseModel> RenewRestrictedSecuritiesInTemplatesPoKomissii(bool sendReport)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewRestrictedSecuritiesInTemplatesPoKomissii Called");

            ListStringResponseModel result = new ListStringResponseModel();
            NewEMail message = new NewEMail();
            message.Subject = "QUIK обновление запрещенных для неквалов инструментов в шаблонах 'По комиссии' в Qadmin MC013820000";
            message.Body = "<html><body>";
            message.Body = message.Body + $"<h3>Получение данных по бумагам/бордам из матрицы</h3>";
            // запросить список инструментов в бд матрицы
            SecurityAndBoardResponse secAndBoards = await _repository.GetRestrictedSecuritiesAndBoards();// список ВСЕХ запрещенных нерезам бумаг
            if (secAndBoards.Response.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewRestrictedSecuritiesInTemplatesPoKomissii " +
                    $"Найдено запрещенных для неквалов инструментов: {secAndBoards.SecurityAndBoardList.Count}");

                message.Body = message.Body + $"<p>Найдено запрещенных для неквалов инструментов: " +
                    $"{secAndBoards.SecurityAndBoardList.Count}</p>";
            }
            else
            {
                result.IsSuccess = false;

                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewRestrictedSecuritiesInTemplatesPoKomissii " +
                    $"Ошибка получения запрещенных для неквалов инструментов:");

                message.Body = message.Body + $"<p style='color:red'>Ошибка получения запрещенных для неквалов инструментов</p>";
                foreach (var text in secAndBoards.Response.Messages)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} - {text}");
                    message.Body = message.Body + $"<p style='color:red'> - {text}</p>";
                    result.Messages.Add(text);
                }

                return result;
            }

            //разбить список по бордам, формируя отдельные RestrictedSecuritiesArraySetForBoardInTemplatesModel
            List<RestrictedSecuritiesArraySetForBoardInTemplatesModel> secAndBoardsSeparatesList = GenerateRestrictedSecuritiesArraySetForBoard(secAndBoards.SecurityAndBoardList);

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewRestrictedSecuritiesInTemplatesPoKomissii " +
                $"после сортировки количество бордов {secAndBoardsSeparatesList.Count}");

            message.Body = message.Body + $"<p>После сортировки количество бордов {secAndBoardsSeparatesList.Count}</p>";

            //если не пустой, выгрузить в QUIK
            string[] templateNames = new string[]
            {
                "KSUR_NeKval", // 0 список неквалов КСУР
                "KPUR_NeKval", // 1 список неквалов КПУР
                "FrnNeResNeKv" // 2 список неквалов дружественных нерезов
            };

            foreach (string template in templateNames)
            {
                message.Body = message.Body + $"<h3> Отправка списков в шаблон {template}</h3>";

                foreach (RestrictedSecuritiesArraySetForBoardInTemplatesModel board in secAndBoardsSeparatesList)
                {
                    board.TemplateName = template;

                    ListStringResponseModel setRestrictedSecurityes = await _repository.SetRestrictedSecuritiesInTemplatesPoKomissii(board);

                    if (setRestrictedSecurityes.IsSuccess == false)
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewRestrictedSecuritiesInTemplatesPoKomissii Ошибка " +
                            $"отправки списка в {board.TemplateName} {board.SecBoard} с количеством {board.Securities.Count}:");

                        message.Body = message.Body + $"<p style='color:red'> Ошибка отправки списка в {board.TemplateName} {board.SecBoard} с количеством {board.Securities.Count}:</p>";

                        foreach (var text in setRestrictedSecurityes.Messages)
                        {
                            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} - {text}");

                            message.Body = message.Body + $"<p style='color:red'> - {text}</p>";
                        }

                        result.IsSuccess = false;
                        result.Messages.AddRange(setRestrictedSecurityes.Messages);
                    }
                    else
                    {
                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewRestrictedSecuritiesInTemplatesPoKomissii " +
                            $"Отправка списка в {board.TemplateName} {board.SecBoard} с количеством {board.Securities.Count} успешна");

                        message.Body = message.Body + $"<p>Отправка списка в {board.TemplateName} {board.SecBoard} с количеством {board.Securities.Count} успешна</p>";
                    }
                }
            }

            message.Body = message.Body + "</body></html>";

            if (sendReport)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewRestrictedSecuritiesInTemplatesPoKomissii Send message");
                await _sender.Send(message);
            }

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore RenewRestrictedSecuritiesInTemplatesPoKomissii all done");
            return result;
        }

        private List<RestrictedSecuritiesArraySetForBoardInTemplatesModel> GenerateRestrictedSecuritiesArraySetForBoard(List<SecurityAndBoardModel> securityAndBoardList)
        {
            List<RestrictedSecuritiesArraySetForBoardInTemplatesModel> result = new List<RestrictedSecuritiesArraySetForBoardInTemplatesModel>();
            foreach (SecurityAndBoardModel securityAndBoard in securityAndBoardList)
            {
                //сначала смотрим есть ли такой борд в листе result.
                bool boardIsPresent = false;
                foreach(var model in result)
                {
                    if (model.SecBoard.Contains(securityAndBoard.Board))
                    {
                        //просто добавим бумагу в этот список
                        model.Securities.Add(securityAndBoard.Secutity);
                        boardIsPresent = true;
                        break;
                    }
                }

                // не нашли такого борда, добавляем новый в список
                if (!boardIsPresent)
                {
                    RestrictedSecuritiesArraySetForBoardInTemplatesModel newModel = new RestrictedSecuritiesArraySetForBoardInTemplatesModel();
                    newModel.SecBoard = securityAndBoard.Board;
                    newModel.Securities.Add(securityAndBoard.Secutity);
                    
                    result.Add(newModel);
                }
            }

            return result;
        }

        public async Task<NewClientCreationResponse> AddNewMatrixPortfolioToExistingClientByUID(NewMatrixPortfolioToExistingClientModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore AddNewMatrixPortfolioToExistingClientByUID Called, " +
                $"UID={model.UID} portfolio={model.MatrixPortfolio.MatrixClientPortfolio}");

            MatrixClientPortfolioModel[] newMatrixPortfolioArray = new MatrixClientPortfolioModel[1];
            newMatrixPortfolioArray[0] = model.MatrixPortfolio;

            string clientAccount = model.MatrixPortfolio.MatrixClientPortfolio.Split("-").First();

            NewClientCreationResponse createResponse = new NewClientCreationResponse();

            // проверить - совпадает ли клиент в UID
            if (model.CheckIsUserHaveEqualsPortfolio)
            {
                BoolResponse isExist = await CheckIsUserAlreadyExistAtUID(model.UID, clientAccount);

                // если клиента нет в UID - отказываем
                if (isExist.IsTrue == false)
                {
                    createResponse.IsSftpUploadSuccess = false;
                    createResponse.SftpUploadMessages.AddRange(isExist.Messages);

                    return createResponse;
                }
            }

            //// проверить - совпадает ли клиент в UID
            //if (model.CheckIsUserHaveEqualsPortfolio)
            //{
            //    FindedQuikQAdminClientResponse findedInQ = await GetIsUserAlreadyExistByMatrixClientAccount(clientAccount);

            //    if (findedInQ.QuikQAdminClient != null)
            //    {
            //        QuikQAdminClientModel isClienExist = findedInQ.QuikQAdminClient.Find(x => x.UID.Equals(model.UID));

            //        if (isClienExist==null)
            //        {
            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore AddNewMatrixPortfolioToExistingClientByUID " +
            //                $"SFTP: UID {model.UID} not contain any porfolios for account {clientAccount}");

            //            createResponse.IsSftpUploadSuccess = false;
            //            createResponse.SftpUploadMessages.Add($"Присланный UID {model.UID} не имеет ранее присвоенных портфелей клиента {clientAccount}. " +
            //                $"Добавление невозможно, т.к. включена проверка на строгое наличие клиента в UID");

            //            return createResponse;
            //        }
            //    }
            //    else
            //    {
            //        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore AddNewMatrixPortfolioToExistingClientByUID " + 
            //            $"SFTP: for account {clientAccount} not found any UID in CurrClnt");

            //        createResponse.IsSftpUploadSuccess = false;
            //        createResponse.SftpUploadMessages.Add($"Присланный портфель клиента {clientAccount} не найден во всем файле CurrClnt. " +
            //            $"Добавление невозможно, т.к. включена проверка на строгое наличие клиента в UID {model.UID}");

            //        return createResponse;
            //    }
            //}

            //SFTP update - добавить портфель
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore AddNewMatrixPortfolioToExistingClientByUID " +
                $"SFTP for {model.UID} added porfolio {model.MatrixPortfolio.MatrixClientPortfolio}");

            MatrixPortfolioAndUidModel matrixPortfolioAndUid = new MatrixPortfolioAndUidModel
            {
                MatrixPortfolio = model.MatrixPortfolio,
                UID = model.UID
            };

            ListStringResponseModel createSftpResponse = await _repository.AddNewMatrixPortfolioToExistingClientByUID(matrixPortfolioAndUid);
            createResponse.IsSftpUploadSuccess = createSftpResponse.IsSuccess;
            createResponse.SftpUploadMessages = createSftpResponse.Messages;


            //InstrTw register
            NewMNPClientModel newMNPClient = new NewMNPClientModel()
            {
                MatrixClientPortfolios = newMatrixPortfolioArray
            };

            ListStringResponseModel clientInfoRequest = await GetClientInformationForInstrTwRegister(newMNPClient, createResponse.NewClient, clientAccount);
            if (clientInfoRequest.IsSuccess)
            {
                ListStringResponseModel fillDataBaseInstrTWResponse = await _repository.FillDataBaseInstrTW(newMNPClient);
                createResponse.IsInstrTwSuccess = fillDataBaseInstrTWResponse.IsSuccess;
                createResponse.InstrTwMessages = fillDataBaseInstrTWResponse.Messages;
            }
            else
            {
                createResponse.InstrTwMessages.AddRange(clientInfoRequest.Messages);
                createResponse.IsInstrTwSuccess = false;
            }

            ////InstrTw register
            //ClientBOInformationResponse clientBOInformation = await _repository.GetClientBOInformation(clientAccount);
            //ClientInformationResponse clientInformation = await _repository.GetClientInformation(clientAccount);

            //NewMNPClientModel newMNPClient = new NewMNPClientModel();

            //if (clientInformation.Response.IsSuccess)
            //{
            //    newMNPClient.Client = clientInformation.ClientInformation;
            //    createResponse.NewClient.Client = newMNPClient.Client;
            //}

            //if (clientBOInformation.Response.IsSuccess)
            //{
            //    newMNPClient.RegisterDate = clientBOInformation.ClientBOInformation.RegisterDate;
            //    newMNPClient.Address = clientBOInformation.ClientBOInformation.Address;
            //    newMNPClient.isClientResident = clientBOInformation.ClientBOInformation.isClientResident;
            //    newMNPClient.isClientPerson = clientBOInformation.ClientBOInformation.isClientPerson;

            //    createResponse.NewClient.RegisterDate = clientBOInformation.ClientBOInformation.RegisterDate;
            //    createResponse.NewClient.Address = clientBOInformation.ClientBOInformation.Address;
            //    createResponse.NewClient.isClientResident = clientBOInformation.ClientBOInformation.isClientResident;
            //    createResponse.NewClient.isClientPerson = clientBOInformation.ClientBOInformation.isClientPerson;
            //}


            //if (clientInformation.Response.IsSuccess && clientBOInformation.Response.IsSuccess)
            //{
            //    newMNPClient.MatrixClientPortfolios = newMatrixPortfolioArray;

            //    ListStringResponseModel fillDataBaseInstrTWResponse = await _repository.FillDataBaseInstrTW(newMNPClient);
            //    createResponse.IsInstrTwSuccess = fillDataBaseInstrTWResponse.IsSuccess;
            //    createResponse.InstrTwMessages = fillDataBaseInstrTWResponse.Messages;
            //}
            //else
            //{
            //    createResponse.InstrTwMessages.AddRange(clientInformation.Response.Messages);
            //    createResponse.InstrTwMessages.AddRange(clientBOInformation.Response.Messages);

            //    createResponse.IsInstrTwSuccess = false;
            //}

            // codes ini, CD reg
            //codes ini
            CodesArrayModel codesArray = new CodesArrayModel() { MatrixClientPortfolios = newMatrixPortfolioArray };

            ListStringResponseModel fillCodesIniResponse = await _repository.FillCodesIniFile(codesArray);
            createResponse.IsCodesIniSuccess = fillCodesIniResponse.IsSuccess;
            createResponse.CodesIniMessages = fillCodesIniResponse.Messages;

            // ? CD reg
            bool totalSucces = true;

            if (model.MatrixPortfolio.MatrixClientPortfolio.Contains("-CD-"))
            {
                ListStringResponseModel addCdToKomissiiResponse = await _repository.AddCdPortfolioToTemplateKomissii(model.MatrixPortfolio);
                if (!addCdToKomissiiResponse.IsSuccess)
                {
                    totalSucces = false;
                }
                createResponse.AddToTemplatesMessages.AddRange(addCdToKomissiiResponse.Messages);

                ListStringResponseModel addCdToPoPlechuResponse = await _repository.AddCdPortfolioToTemplatePoPlechu(model.MatrixPortfolio);
                if (!addCdToPoPlechuResponse.IsSuccess)
                {
                    totalSucces = false;
                }
                createResponse.AddToTemplatesMessages.AddRange(addCdToPoPlechuResponse.Messages);
            }
            if (totalSucces)
            {
                createResponse.IsAddToTemplatesSuccess = true;
            }

            //IsSuccess total ?
            if (createResponse.IsSftpUploadSuccess && createResponse.IsCodesIniSuccess && createResponse.IsAddToTemplatesSuccess && createResponse.IsInstrTwSuccess)
            {
                createResponse.IsNewClientCreationSuccess = true;
            }

            return createResponse;
        }

        public async Task<NewClientCreationResponse> AddNewFortsPortfolioToExistingClientByUID(NewFortsPortfolioToExistingClientModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore AddNewFortsPortfolioToExistingClientByUID Called, " +
                $"UID={model.UID} portfolio={model.MatrixToFortsCodes.MatrixClientCode}");

            MatrixToFortsCodesMappingModel[] newMatrixPortfolioArray = new MatrixToFortsCodesMappingModel[1];
            newMatrixPortfolioArray[0] = model.MatrixToFortsCodes;

            string clientAccount = model.MatrixToFortsCodes.MatrixClientCode.Split("-").First();

            NewClientCreationResponse createResponse = new NewClientCreationResponse();

            // проверить - совпадает ли клиент в UID
            if (model.CheckIsUserHaveEqualsPortfolio)
            {
                BoolResponse isExist = await CheckIsUserAlreadyExistAtUID(model.UID, clientAccount);

                // если клиента нет в UID - отказываем
                if (isExist.IsTrue == false)
                {
                    createResponse.IsSftpUploadSuccess = false;
                    createResponse.SftpUploadMessages.AddRange(isExist.Messages);

                    return createResponse;
                }
            }

            //SFTP update - добавить портфель
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore AddNewFortsPortfolioToExistingClientByUID " +
                $"SFTP for {model.UID} added code {model.MatrixToFortsCodes.FortsClientCode} from porfolio {model.MatrixToFortsCodes.MatrixClientCode}");

            FortsCodeAndUidModel fortsCodeAndUid = new FortsCodeAndUidModel
            {
                MatrixFortsCode = model.MatrixToFortsCodes.FortsClientCode,
                UID = model.UID
            };

            ListStringResponseModel createSftpResponse = await _repository.AddNewFortsPortfolioToExistingClientByUID(fortsCodeAndUid);
            createResponse.IsSftpUploadSuccess = createSftpResponse.IsSuccess;
            createResponse.SftpUploadMessages = createSftpResponse.Messages;

            //InstrTw register
            NewMNPClientModel newMNPClient = new NewMNPClientModel()
            {
                CodesPairRF = newMatrixPortfolioArray
            };

            ListStringResponseModel clientInfoRequest = await GetClientInformationForInstrTwRegister(newMNPClient, createResponse.NewClient, clientAccount);
            if (clientInfoRequest.IsSuccess)
            {
                ListStringResponseModel fillDataBaseInstrTWResponse = await _repository.FillDataBaseInstrTW(newMNPClient);
                createResponse.IsInstrTwSuccess = fillDataBaseInstrTWResponse.IsSuccess;
                createResponse.InstrTwMessages = fillDataBaseInstrTWResponse.Messages;
            }
            else
            {
                createResponse.InstrTwMessages.AddRange(clientInfoRequest.Messages);
                createResponse.IsInstrTwSuccess = false;
            }

            // codes ini, CD reg - no actions needed, skip
            createResponse.IsCodesIniSuccess = true;
            createResponse.IsAddToTemplatesSuccess = true;


            //IsSuccess total ?
            if (createResponse.IsSftpUploadSuccess && createResponse.IsCodesIniSuccess && createResponse.IsAddToTemplatesSuccess && createResponse.IsInstrTwSuccess)
            {
                createResponse.IsNewClientCreationSuccess = true;
            }

            return createResponse;
        }

        private async Task<ListStringResponseModel> GetClientInformationForInstrTwRegister(NewMNPClientModel newMNPClient, NewClientModel newClient, string clientAccount)
        {

            ListStringResponseModel result = new ListStringResponseModel();

            ClientBOInformationResponse clientBOInformation = await _repository.GetClientBOInformation(clientAccount);
            ClientInformationResponse clientInformation = await _repository.GetClientInformation(clientAccount);

            if (clientInformation.Response.IsSuccess)
            {
                newMNPClient.Client = clientInformation.ClientInformation;
                newClient.Client = newMNPClient.Client;
            }

            if (clientBOInformation.Response.IsSuccess)
            {
                newMNPClient.RegisterDate = clientBOInformation.ClientBOInformation.RegisterDate;
                newMNPClient.Address = clientBOInformation.ClientBOInformation.Address;
                newMNPClient.isClientResident = clientBOInformation.ClientBOInformation.isClientResident;
                newMNPClient.isClientPerson = clientBOInformation.ClientBOInformation.isClientPerson;

                newClient.RegisterDate = clientBOInformation.ClientBOInformation.RegisterDate;
                newClient.Address = clientBOInformation.ClientBOInformation.Address;
                newClient.isClientResident = clientBOInformation.ClientBOInformation.isClientResident;
                newClient.isClientPerson = clientBOInformation.ClientBOInformation.isClientPerson;
            }

            if (clientInformation.Response.IsSuccess && clientBOInformation.Response.IsSuccess)
            {
                result.IsSuccess = true;
            }
            else
            {
                result.Messages.AddRange(clientInformation.Response.Messages);
                result.Messages.AddRange(clientBOInformation.Response.Messages);

                result.IsSuccess = false;
            }

            return result;
        }

        private async Task<BoolResponse> CheckIsUserAlreadyExistAtUID(int uID, string clientAccount)
        {
            BoolResponse isExist = new BoolResponse();
            
            // список портфелей и кодов клиента для поиска по UID
            List<string> clientCodesArray = new List<string>();

            // получим портфели спот
            MatrixClientCodeModelResponse spotCodes = await _repository.GetClientAllSpotCodesFiltered(clientAccount);

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore AddNewFortsPortfolioToExistingClientByUID " +
                $"GetClientAllSpotCodesFiltered result {spotCodes.Response.IsSuccess}");

            if (spotCodes.MatrixClientCodesList.Count > 0)
            {
                foreach (var spot in spotCodes.MatrixClientCodesList)
                {
                    clientCodesArray.Add(spot.MatrixClientPortfolio);
                }
            }

            // получим портфели фортс
            MatrixToFortsCodesMappingResponse fortsCodes = await _repository.GetClientAllFortsCodes(clientAccount);

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore AddNewFortsPortfolioToExistingClientByUID " +
                $"GetClientAllFortsCodes result {fortsCodes.Response.IsSuccess}");

            if (fortsCodes.MatrixToFortsCodesList.Count > 0)
            {
                foreach (var forts in fortsCodes.MatrixToFortsCodesList)
                {
                    clientCodesArray.Add(forts.FortsClientCode);
                }
            }

            // если портфелей нет - вернем ответ
            if (clientCodesArray.Count == 0)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore AddNewFortsPortfolioToExistingClientByUID " + 
                    $" return false, not any portfolios found in matrix for {clientAccount}");

                isExist.Messages.Add($"Портфели клиента {clientAccount} не найдены во всем файле CurrClnt. " +
                    $"Добавление невозможно, т.к. включена проверка на строгое наличие клиента в UID {uID}");

                isExist.IsTrue = false;
                return isExist;
            }

            // запросим UID
            ListStringResponseModel checkUserExist = await _repository.GetIsUserAlreadyExistByCodeArray(clientCodesArray.ToArray());

            if (checkUserExist.IsSuccess == false)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore AddNewFortsPortfolioToExistingClientByUID GetIsUserAlreadyExistByCodeArray " +
                    $" return IsSuccess false");

                isExist.IsSuccess = false;
                isExist.IsTrue = false;
                isExist.Messages.AddRange(checkUserExist.Messages);                
                return isExist;
            }

            foreach (string xmlClient in checkUserExist.Messages)
            {
                if (xmlClient.Contains("UID=\"" + uID + "\""))
                {
                    isExist.IsTrue = true;
                    return isExist;
                }
            }

            isExist.IsTrue = false;
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore AddNewFortsPortfolioToExistingClientByUID " +
                    $"Account {clientAccount} not found at UID {uID}. ");
            isExist.Messages.Add($"Портфели клиента {clientAccount} не найдены в UID {uID}. " +
                    $"Добавление невозможно, т.к. включена проверка на строгое наличие клиента в UID {uID}");


            //if (findedInQ.QuikQAdminClient != null)
            //{
            //    QuikQAdminClientModel isClienExist = findedInQ.QuikQAdminClient.Find(x => x.UID.Equals(model.UID));

            //    if (isClienExist==null)
            //    {
            //        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore AddNewFortsPortfolioToExistingClientByUID " +
            //            $"SFTP: UID {model.UID} not contain any porfolios for account {clientAccount}");

            //        createResponse.IsSftpUploadSuccess = false;
            //        createResponse.SftpUploadMessages.Add($"Присланный UID {model.UID} не имеет ранее присвоенных портфелей клиента {clientAccount}. " +
            //            $"Добавление невозможно, т.к. включена проверка на строгое наличие клиента в UID");

            //        return createResponse;
            //    }
            //}
            //else
            //{
            //    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICore AddNewFortsPortfolioToExistingClientByUID " +
            //        $"SFTP: for account {clientAccount} not found any UID in CurrClnt");

            //    createResponse.IsSftpUploadSuccess = false;
            //    createResponse.SftpUploadMessages.Add($"Присланный портфель клиента {clientAccount} не найден во всем файле CurrClnt. " +
            //        $"Добавление невозможно, т.к. включена проверка на строгое наличие клиента в UID {model.UID}");

            //    return createResponse;
            //}


            return isExist;
        }
    }
}