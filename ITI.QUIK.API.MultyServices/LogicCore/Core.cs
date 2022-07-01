using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Responses;
using Microsoft.Extensions.Logging;

namespace LogicCore
{
    public class Core : ICore
    {
        private ILogger<Core> _logger;
        private IHttpApiRepository _repository;

        public Core(ILogger<Core> logger, IHttpApiRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<NewClientCreationResponse> PostNewClient(NewClientModel newClientModel)
        {
            _logger.LogInformation($"ICore PostNewClient Called for {newClientModel.Client.FirstName}");

            NewClientCreationResponse createResponse = new NewClientCreationResponse();

            //SFTP create
            _logger.LogInformation($"ICore SFTP register for {newClientModel.Client.FirstName}");
            ListStringResponseModel createSftpResponse = await _repository.CreateNewClient(newClientModel);
            createResponse.IsSftpUploadSuccess = createSftpResponse.IsSuccess;
            createResponse.SftpUploadMessages = createSftpResponse.Messages;

            //InstrTw register
            _logger.LogInformation($"ICore InstrTw register for {newClientModel.Client.FirstName}");
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
            }

            return createResponse;
        }
    }
}