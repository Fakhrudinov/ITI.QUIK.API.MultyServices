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

            // codes ini, CD reg, NoLeverage reg
            if (newClientModel.CodesMatrix != null)
            {
                //codes ini
                ListStringResponseModel fillCodesIniResponse = await _repository.FillCodesIniFile(newClientModel);
                createResponse.IsCodesIniSuccess = fillCodesIniResponse.IsSuccess;
                createResponse.CodesIniMessages = fillCodesIniResponse.Messages;
                // ? CD reg

                // по плечу - в NoLeverage 
            }
            else
            {
                createResponse.IsCodesIniSuccess = true;
                createResponse.CodesInMessages
                createResponse.IsAddToTemplatesSuccess = true
            }


            //заполним результаты в IsSuccess


            //добавить строки с именем файла

            //поискать файл в результатах

            return createResponse;
        }
    }
}