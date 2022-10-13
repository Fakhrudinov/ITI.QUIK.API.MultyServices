using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Models.EMail;
using DataAbstraction.Models.MoneyAndDepo;
using DataAbstraction.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace LogicCore
{
    public class CoreSingleServices : ICoreSingleServices
    {
        private ILogger<CoreSingleServices> _logger;
        private IHttpApiRepository _repository;
        private CoreSettings _settings;
        private IEMail _sender;

        public CoreSingleServices(ILogger<CoreSingleServices> logger, IHttpApiRepository repository, IOptions<CoreSettings> settings, IEMail sender)
        {
            _logger = logger;
            _repository = repository;
            _settings = settings.Value;
            _sender=sender;
        }

        public Task<BoolResponse> CheckIsAnyFortsCodesFromOptionWorkshopInEDP()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsAnyFortsCodesFromOptionWorkshopInEDP Called");

            BoolResponse result = new BoolResponse();
            result.IsSuccess = false;
            result.Messages.Add("Not implemenyed yet");

            return Task.FromResult(result);
        }

        public async Task<BoolResponse> CheckIsFileCorrectLimLim(bool sendReport)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsFileCorrectLimLim Called");

            BoolResponse result = new BoolResponse();

            NewEMail message = new NewEMail();
            message.Subject = "QUIK проверка корректности (заранее скачанного) файла лимитов lim.lim";
            message.Body = "<html><body><h2>Проверка корректности файла лимитов lim.lim</h2>";

            // проверить что файл lim.lim есть
            if (!File.Exists(_settings.PathToLimLim))
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} Error! File lim.lim not found at " + _settings.PathToLimLim);
                result.Messages.Add($"Error! File lim.lim not found at {_settings.PathToLimLim}");
                result.IsSuccess = false;
                result.IsTrue = false;

                message.Body = message.Body + $"<p style='color:red'>Error! File lim.lim not found at {_settings.PathToLimLim}</p>";
                await SendMessageFinalize(message, sendReport);

                return result;
            }

            // считать все строки файла lim.lim
            string[] fileLimLim = await File.ReadAllLinesAsync(_settings.PathToLimLim);

            // lim.lim старый? 
            ListStringResponseModel lastWriteTime = await _repository.GetSftpFileLastWriteTime("lim.lim");

            if (lastWriteTime.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsFileCorrectLimLim " +
                    $"Файл lim.lim прочитан, всего строк={fileLimLim.Length}, {lastWriteTime.Messages[0]}");
                message.Body = message.Body + $"<p>Файл lim.lim прочитан, всего строк={fileLimLim.Length}, {lastWriteTime.Messages[0]}</p>";
            }
            else
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsFileCorrectLimLim error at request lim.lim write time:");
                message.Body = message.Body + $"<p style='color:red'>Ошибка при запросе даты записи файла lim.lim:</p>";
                foreach (var str in lastWriteTime.Messages)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsFileCorrectLimLim  - {str}");
                    message.Body = message.Body + $"<p style='color:red'> - {str}</p>";
                }                
            }


            // собираем в list из матрицы клиентов, торговавших вчера
            List<ClientAssetsModel> matrixClients = new List<ClientAssetsModel>();
            string portfoliosToHTTPRequestDepoPositions = await GetClientsWithMoneyWhoTradeYesterday(matrixClients);

            message.Body = message.Body + $"<p>Строка запроса с списком клиентов:" +
                $"<br />{portfoliosToHTTPRequestDepoPositions}</p>";


            if (matrixClients.Count == 0)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsFileCorrectLimLim no client with deals found for last week");
                result.Messages.Add($"ICoreSingleServices CheckIsFileCorrectLimLim no client with deals found for last week");
                result.IsSuccess = false;
                result.IsTrue = false;

                message.Body = message.Body + $"<p style='color:red'>Не найдено ни одного клиента, торговавшего за последнюю неделю</p>";
                await SendMessageFinalize(message, sendReport);

                return result;
            }

            message.Body = message.Body + $"<p>В матрице найдено {matrixClients.Count} клиентов, торговавших вчера</p>";

            //запросить позиции из матрицы у клиентов с сделками
            await AddPositionsToClientsWhoTradeYesterday(matrixClients, portfoliosToHTTPRequestDepoPositions);

            List<ClientAssetsModel> limlimClients = await GetClientLimitsFromLimLim(matrixClients, fileLimLim, result, message);

            message.Body = message.Body + $"<p>Получили {limlimClients.Count} клиентов из файла lim.lim</p>";
            message.Body = message.Body + $"<h3>Сравниваем результаты</h3>";
            //сравниваем данные матрицы и файла lim.lim
            CompareMatrixAndLimlimAndRemoveEquals(matrixClients, limlimClients);

            // смотрим остатки, заполняем отчет о различиях
            AnalizeLeftoversAndFillResult(matrixClients, limlimClients, result);
            if (sendReport)
            {
                AnalizeLeftoversAndFillEmail(matrixClients, limlimClients, message);
            }

            await SendMessageFinalize(message, sendReport);

            return result;
        }

        private void AnalizeLeftoversAndFillEmail(List<ClientAssetsModel> matrixClients, List<ClientAssetsModel> limlimClients, NewEMail message)
        {
            if (matrixClients.Count > 0 || limlimClients.Count > 0)
            {
                List<ClientAssetsComparitionModel> clientsWithDiff = new List<ClientAssetsComparitionModel>();

                //message.Body = message.Body + $"<p>После чистки совпадающих позиций осталось с расхождениями " +
                //    $"<br />matrix клиентов: {matrixClients.Count}" +
                //    $"<br />lim.lim клиентов: {limlimClients.Count}</p>";

                foreach (var mClient in matrixClients)
                {
                    ClientAssetsComparitionModel newDiff = new ClientAssetsComparitionModel();
                    newDiff.ClientPortfolio = mClient.ClientPortfolio;

                    foreach (var money in mClient.Moneys)
                    {
                        ClientMoneyComparitionModel newMoney = new ClientMoneyComparitionModel();
                        newMoney.Currency = money.Currency;

                        ClientMoneyComparitionMoneyModel matrixMoney = new ClientMoneyComparitionMoneyModel();
                        matrixMoney.TradeSystem = "matrix";
                        matrixMoney.Balance = money.Balance;
                        matrixMoney.Tag = money.Tag;

                        newMoney.MoneyAtTradeSystem.Add(matrixMoney);
                        newDiff.Money.Add(newMoney);
                    }

                    foreach (var depo in mClient.Positions)
                    {
                        ClientPositionComparitionModel newDepo = new ClientPositionComparitionModel();
                        newDepo.Seccode = depo.Seccode;

                        ClientPositionComparitionPositionModel matrixPos = new ClientPositionComparitionPositionModel();
                        matrixPos.TradeSystem = "matrix";
                        matrixPos.AveragePrice = depo.AveragePrice;
                        matrixPos.OpenBalance = depo.OpenBalance;
                        matrixPos.TKS = depo.TKS;

                        newDepo.PositionAtTradeSystem.Add(matrixPos);
                        newDiff.Position.Add(newDepo);
                    }

                    clientsWithDiff.Add(newDiff);
                }

                foreach (var qClient in limlimClients)
                {
                    //проверим, есть ли уже такой клиент в списке.
                    if (clientsWithDiff.FindIndex(r =>
                                    r.ClientPortfolio.MatrixClientPortfolio == qClient.ClientPortfolio.MatrixClientPortfolio) == -1)
                    {
                        //если клиента нет - добавим.
                        ClientAssetsComparitionModel newDiff = new ClientAssetsComparitionModel();
                        newDiff.ClientPortfolio = qClient.ClientPortfolio;

                        clientsWithDiff.Add(newDiff);
                    }

                    int clientIndex = clientsWithDiff.FindIndex(r => r.ClientPortfolio.MatrixClientPortfolio == qClient.ClientPortfolio.MatrixClientPortfolio);
                    foreach (var money in qClient.Moneys)
                    {
                        //проверить, что такие деньги существуют.
                        if (clientsWithDiff[clientIndex].Money.FindIndex(m => m.Currency == money.Currency) == -1)
                        {
                            // не найдено, добавим.
                            ClientMoneyComparitionModel newMoney = new ClientMoneyComparitionModel();
                            newMoney.Currency = money.Currency;

                            clientsWithDiff[clientIndex].Money.Add(newMoney);
                        }


                        ClientMoneyComparitionMoneyModel matrixMoney = new ClientMoneyComparitionMoneyModel();
                        matrixMoney.TradeSystem = "limlim";
                        matrixMoney.Balance = money.Balance;
                        matrixMoney.Tag = money.Tag;

                        int moneyIndex = clientsWithDiff[clientIndex].Money.FindIndex(mm => mm.Currency == money.Currency);
                        clientsWithDiff[clientIndex].Money[moneyIndex].MoneyAtTradeSystem.Add(matrixMoney);
                    }

                    foreach (var depo in qClient.Positions)
                    {
                        //проверить, что такая позиция уже существует
                        if (clientsWithDiff[clientIndex].Position.FindIndex(p => p.Seccode == depo.Seccode) == -1)
                        {
                            // не найдено, добавим.
                            ClientPositionComparitionModel newDepo = new ClientPositionComparitionModel();
                            newDepo.Seccode = depo.Seccode;

                            clientsWithDiff[clientIndex].Position.Add(newDepo);
                        }

                        ClientPositionComparitionPositionModel matrixPos = new ClientPositionComparitionPositionModel();
                        matrixPos.TradeSystem = "limlim";
                        matrixPos.AveragePrice = depo.AveragePrice;
                        matrixPos.OpenBalance = depo.OpenBalance;
                        matrixPos.TKS = depo.TKS;

                        int posIndex = clientsWithDiff[clientIndex].Position.FindIndex(pp => pp.Seccode == depo.Seccode);
                        clientsWithDiff[clientIndex].Position[posIndex].PositionAtTradeSystem.Add(matrixPos);
                    }
                }

                AddDiferencesToEMail(clientsWithDiff, message);

            }
            else
            {
                message.Body = message.Body + $"<h4>Расхождений не найдено</h4>";
            }
        }

        private void AddDiferencesToEMail(List<ClientAssetsComparitionModel> clientsWithDiff, NewEMail message)
        {
            
            message.Body = message.Body + $"<h4 style='color:red'>Расхождения!</h4>";

            foreach (ClientAssetsComparitionModel client in clientsWithDiff)
            {
                message.Body = message.Body + $"<p><strong>{client.ClientPortfolio.MatrixClientPortfolio}:</strong></p>";

                message.Body = message.Body + $"<ul>";
                    foreach (var money in client.Money)
                    {                    
                        message.Body = message.Body + $"<li>{money.Currency}</li>";
                    
                        message.Body = message.Body + $"<ul>";
                        foreach (var moneyDetails in money.MoneyAtTradeSystem)
                        {                        
                            message.Body = message.Body + $"<li>{moneyDetails.TradeSystem} {moneyDetails.Tag} {moneyDetails.Balance}</li>";                        
                        }
                        message.Body = message.Body + $"</ul>";
                    }
               

                    foreach (var depo in client.Position)
                    {
                        message.Body = message.Body + $"<li>{depo.Seccode}</li>";

                        message.Body = message.Body + $"<ul>";
                        foreach (var depoDetails in depo.PositionAtTradeSystem)
                        {
                            message.Body = message.Body + $"<li>{depoDetails.TradeSystem} {depoDetails.TKS} " +
                                $"OpenBalance={depoDetails.OpenBalance} AveragePrice={depoDetails.AveragePrice}</li>";
                        }
                        message.Body = message.Body + $"</ul>";
                    }
                message.Body = message.Body + $"</ul>";
            }

            message.Body = message.Body + $"<h4 style='color:red'>Действия при расхождениях</h4>";
            message.Body = message.Body + $"<p>Если время создания файла lim.lim - ночь(00:10) и расхождения только в SUR и AveragePrice и небольшие " +
                $"- можно ничего не делать, загружен файл с лимитами по состоянию на закрытие вчерашнего дня. </p>";
            message.Body = message.Body + $"<p>Если есть различия в позициях или расхождения по SUR большие - проверить, во сколько сформирован файл lim.lim в бэке - " +
                $"<br /> на сервере 172.16.18.51 в /u03/work/quik. Обычно проблемы, если бэк формирует файл поздно, уже после выгрузки на сервер QUIK.</p>";
            message.Body = message.Body + $"<p>Взять свежий файл lim.lim, загрузить его на сервер QUIK, в Qmonitor нажать на Actions/Load limits from file, " +
                $"в рабочее время еще нажать на Actions/Recalculates limits для фирмы MC***</p>";
        }

        private async Task SendMessageFinalize(NewEMail message, bool sendReport)
        {
            message.Body = message.Body + "</body></html>";

            if (sendReport)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsFileCorrectLimLim Send message");
                await _sender.Send(message);
            }
        }

        private void AnalizeLeftoversAndFillResult(List<ClientAssetsModel> matrixClients, List<ClientAssetsModel> limlimClients, BoolResponse result)
        {
            // смотрим остатки, заполняем отчет о различиях
            if (matrixClients.Count > 0 || limlimClients.Count > 0)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsFileCorrectLimLim " +
                    $"- расхождения по клиентам остались: по матрице={matrixClients.Count}, по lim.lim={limlimClients.Count}");

                //message.Body = message.Body + $"<p>После чистки совпадающих позиций осталось с расхождениями " +
                //    $"<br />matrix клиентов: {matrixClients.Count}" +
                //    $"<br />lim.lim клиентов: {limlimClients.Count}</p>";

                foreach (var mClient in matrixClients)
                {
                    foreach (var money in mClient.Moneys)
                    {
                        result.Messages.Add($"matrix: {mClient.ClientPortfolio.MatrixClientPortfolio} " +
                            $"has different money {money.Currency} {money.Balance} {money.Tag}");
                        //message.Body = message.Body + $"<p style='color:red'>matrix: {mClient.ClientPortfolio.MatrixClientPortfolio} " +
                        //    $"has different money {money.Currency} {money.Balance} {money.Tag}</p>";
                    }

                    foreach (var depo in mClient.Positions)
                    {
                        result.Messages.Add($"matrix: {mClient.ClientPortfolio.MatrixClientPortfolio} " +
                            $"has different position {depo.Seccode} {depo.OpenBalance} {depo.AveragePrice} {depo.TKS}");
                        //message.Body = message.Body + $"<p style='color:red'>matrix: {mClient.ClientPortfolio.MatrixClientPortfolio} " +
                        //    $"has different position {depo.Seccode} {depo.OpenBalance} {depo.AveragePrice} {depo.TKS}</p>";
                    }
                }

                foreach (var qClient in limlimClients)
                {
                    foreach (var money in qClient.Moneys)
                    {
                        result.Messages.Add($"lim.lim: {qClient.ClientPortfolio.MatrixClientPortfolio} " +
                            $"has different money {money.Currency} {money.Balance} {money.Tag}");
                        //message.Body = message.Body + $"<p style='color:red'>lim.lim: {qClient.ClientPortfolio.MatrixClientPortfolio} " +
                        //    $"has different money {money.Currency} {money.Balance} {money.Tag}</p>";
                    }

                    foreach (var depo in qClient.Positions)
                    {
                        result.Messages.Add($"lim.lim: {qClient.ClientPortfolio.MatrixClientPortfolio} " +
                            $"has different position {depo.Seccode} {depo.OpenBalance} {depo.AveragePrice} {depo.TKS}");
                        //message.Body = message.Body + $"<p style='color:red'>lim.lim: {qClient.ClientPortfolio.MatrixClientPortfolio} " +
                        //    $"has different position {depo.Seccode} {depo.OpenBalance} {depo.AveragePrice} {depo.TKS}</p>";
                    }
                }
            }
            else
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsFileCorrectLimLim " +
                    $"- OK расхождений не найдено");

                //message.Body = message.Body + $"<p><strong>OK, расхождений не найдено</strong></p>";

                result.IsTrue = true;
            }
        }

        private void CompareMatrixAndLimlimAndRemoveEquals(List<ClientAssetsModel> matrixClients, List<ClientAssetsModel> limlimClients)
        {
            //сравниваем данные матрицы и файла lim.lim
            for (int i = matrixClients.Count - 1; i >= 0; i--)
            {
                if (limlimClients.FindIndex(r =>
                                    r.ClientPortfolio.MatrixClientPortfolio == matrixClients[i].ClientPortfolio.MatrixClientPortfolio) == -1)
                {
                    // такого клиента нет в list, ничего не делаем.
                }
                else
                {
                    ClientAssetsModel compareToLimLim = limlimClients[limlimClients.FindIndex(r =>
                        r.ClientPortfolio.MatrixClientPortfolio == matrixClients[i].ClientPortfolio.MatrixClientPortfolio)];

                    // сравниваем деньги, совпадения удаляем.
                    for (int m = matrixClients[i].Moneys.Count - 1; m >= 0; m--)
                    {
                        for (int qm = compareToLimLim.Moneys.Count - 1; qm >= 0; qm--)
                        {
                            if (compareToLimLim.Moneys[qm].Tag == matrixClients[i].Moneys[m].Tag)
                            {
                                if (compareToLimLim.Moneys[qm].Currency == matrixClients[i].Moneys[m].Currency)
                                {
                                    if (compareToLimLim.Moneys[qm].Balance == matrixClients[i].Moneys[m].Balance)
                                    {
                                        //удаляем lim.lim деньги
                                        limlimClients[limlimClients.FindIndex(r =>
                                            r.ClientPortfolio.MatrixClientPortfolio == matrixClients[i].ClientPortfolio.MatrixClientPortfolio)].Moneys.RemoveAt(qm);

                                        //удаляем матричные деньги
                                        matrixClients[i].Moneys.RemoveAt(m);

                                        break;
                                    }                                    
                                }
                            }
                        }
                    }

                    // сравниваем позиции, совпадения удаляем.
                    for (int p = matrixClients[i].Positions.Count - 1; p >= 0; p--)
                    {
                        for (int qp = compareToLimLim.Positions.Count - 1; qp >= 0; qp--)
                        {
                            if (compareToLimLim.Positions[qp].Seccode == matrixClients[i].Positions[p].Seccode)
                            {
                                if (compareToLimLim.Positions[qp].TKS == matrixClients[i].Positions[p].TKS)
                                {
                                    if (compareToLimLim.Positions[qp].OpenBalance == matrixClients[i].Positions[p].OpenBalance)
                                    {
                                        if (compareToLimLim.Positions[qp].AveragePrice == matrixClients[i].Positions[p].AveragePrice)
                                        {
                                            //удаляем lim.lim позицию
                                            limlimClients[limlimClients.FindIndex(r =>
                                                r.ClientPortfolio.MatrixClientPortfolio == matrixClients[i].ClientPortfolio.MatrixClientPortfolio)].Positions.RemoveAt(qp);

                                            //удаляем матричную позицию
                                            matrixClients[i].Positions.RemoveAt(p);

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // из lim.lim клиента удаляем все позиции и деньги с нулевым количеством
                    for (int qm = compareToLimLim.Moneys.Count - 1; qm >= 0; qm--)
                    {
                        if (compareToLimLim.Moneys[qm].Balance == 0)
                        {
                            //удаляем lim.lim деньги
                            limlimClients[limlimClients.FindIndex(r =>
                                r.ClientPortfolio.MatrixClientPortfolio == matrixClients[i].ClientPortfolio.MatrixClientPortfolio)].Moneys.RemoveAt(qm);
                        }
                    }
                    for (int qp = compareToLimLim.Positions.Count - 1; qp >= 0; qp--)
                    {
                        if (compareToLimLim.Positions[qp].OpenBalance == 0)
                        {
                            //удаляем lim.lim позицию
                            limlimClients[limlimClients.FindIndex(r =>
                                r.ClientPortfolio.MatrixClientPortfolio == matrixClients[i].ClientPortfolio.MatrixClientPortfolio)].Positions.RemoveAt(qp);
                        }
                    }

                    // если в lim.lim клиенте не осталось денег и позиций - удалем его
                    if (compareToLimLim.Positions.Count == 0 && compareToLimLim.Moneys.Count == 0)
                    {
                        limlimClients.RemoveAt(limlimClients.FindIndex(r =>
                                                r.ClientPortfolio.MatrixClientPortfolio == matrixClients[i].ClientPortfolio.MatrixClientPortfolio));
                    }

                    // если в матричном клиенте не осталось денег и позиций - удалем его
                    if (matrixClients[i].Positions.Count == 0 && matrixClients[i].Moneys.Count == 0)
                    {
                        matrixClients.RemoveAt(i);
                    }
                } 
            }
        }

        private async Task<List<ClientAssetsModel>> GetClientLimitsFromLimLim(List<ClientAssetsModel> matrixClients, string[] fileLimLim, BoolResponse result, NewEMail message)
        {
            List<ClientAssetsModel> limlimClients = new List<ClientAssetsModel>();

            for (int i = matrixClients.Count - 1; i >= 0; i--)
            {
                string tag = "EQTV";
                string tks = "L01+00000F00";
                if (!matrixClients[i].ClientPortfolio.MatrixClientPortfolio.Contains("-MS-"))
                {
                    tag = "EUSR";
                    tks = "MB0138204947";
                }

                if (matrixClients[i].Positions.Count > 0)
                {
                    tks = matrixClients[i].Positions[0].TKS;
                }

                ClientAssetsModel newLimLimClient = await GetAssetsFromFileLimLimByPortfolio(
                    matrixClients[i].ClientPortfolio.MatrixClientPortfolio,
                    tks,
                    tag,
                    fileLimLim);

                if (newLimLimClient is null)
                {
                    result.IsTrue = false;
                    result.Messages.Add($"В файле lim.lim не найдено данных по портфелю {matrixClients[i].ClientPortfolio.MatrixClientPortfolio}");
                    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsFileCorrectLimLim " +
                        $"В файле lim.lim не найдено данных по портфелю {matrixClients[i].ClientPortfolio.MatrixClientPortfolio}");

                    message.Body = message.Body + $"<p style='color:red'>В файле lim.lim не найдено данных по портфелю {matrixClients[i].ClientPortfolio.MatrixClientPortfolio}</p>";

                    // удалить из matrixClients этого клиента. что его сравнивать то? === не удаляем, иначе isTrue в респонсе = true
                    //matrixClients.RemoveAt(i);
                }
                else
                {
                    limlimClients.Add(newLimLimClient);
                }
            }
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsFileCorrectLimLim " +
                $"всего клиентов из lim.lim получено - " + limlimClients.Count);

            return limlimClients;
        }

        private async Task AddPositionsToClientsWhoTradeYesterday(List<ClientAssetsModel> matrixClients, string portfoliosToHTTPRequestDepoPositions)
        {
            //запросить позиции из матрицы у клиентов с сделками
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsFileCorrectLimLim " +
                $"запросить позиции из матрицы у клиентов с сделками");
            portfoliosToHTTPRequestDepoPositions = portfoliosToHTTPRequestDepoPositions.Substring(1);//убрать первый & из строки
            ClientDepoPositionsResponse matrixDepoPositions = await _repository.GetClientsPositionsByMatrixPortfolioList(portfoliosToHTTPRequestDepoPositions);
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsFileCorrectLimLim " +
                $"получено позицийу клиентов с сделками - " + matrixDepoPositions.Clients.Count);
            //позиции записать в List<ClientAssetsModel> matrixClients
            foreach (ClientDepoPositionModel position in matrixDepoPositions.Clients)
            {
                // если USD EUR etc - то в деньги <<-------------------------------------------------------------------------
                if (position.SecCode.Equals("USD") ||
                    position.SecCode.Equals("EUR") ||
                    position.SecCode.Equals("GBP") ||
                    position.SecCode.Equals("HKD"))
                {
                    string tag = "EQTV";

                    if (!position.MatrixClientPortfolio.Contains("-MS-"))
                    {
                        tag = "EUSR";
                    }

                    ClientAssetsMoneyPositionModel newMoneyPosition = new ClientAssetsMoneyPositionModel
                    {
                        Balance = position.OpenBalance,
                        Currency = position.SecCode,
                        Tag = tag
                    };

                    matrixClients[matrixClients.FindIndex(r => r.ClientPortfolio.MatrixClientPortfolio == position.MatrixClientPortfolio)].Moneys.Add(newMoneyPosition);
                }
                else
                {
                    ClientAssetsDepoPositionModel newDepoPosition = new ClientAssetsDepoPositionModel();
                    newDepoPosition.AveragePrice = position.AveragePrice;
                    newDepoPosition.Seccode = position.SecCode;
                    newDepoPosition.OpenBalance = position.OpenBalance;
                    newDepoPosition.TKS = position.TKS;

                    matrixClients[matrixClients.FindIndex(r => r.ClientPortfolio.MatrixClientPortfolio == position.MatrixClientPortfolio)].Positions.Add(newDepoPosition);
                }
            }
        }

        private async Task<string> GetClientsWithMoneyWhoTradeYesterday(List<ClientAssetsModel> matrixClients)
        {
            string portfoliosToHTTPRequestDepoPositions = "";

            //запросить в матрице клиентов с сделками
            ClientAndMoneyResponse matrixclientAndMoney = new ClientAndMoneyResponse();//GetClientAndMoneyResponse(7);//7 = last week
            for (int i = 1; i <= 7; i++)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsFileCorrectLimLim " +
                    $"запрос матрице клиентов с сделками, попытка " + i);
                matrixclientAndMoney = await _repository.GetClientsSpotPortfoliosWhoTradesYesterday(i);
                if (matrixclientAndMoney.Clients.Count > 0)
                {
                    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsFileCorrectLimLim " +
                        $"найдены в матрице клиенты с сделками - " + matrixclientAndMoney.Clients.Count);
                    break;
                }
            }
            //добавить найденных в модель List<ClientAssetsModel> matrixClients
            foreach (ClientAndMoneyModel client in matrixclientAndMoney.Clients)
            {
                ClientAssetsModel newClient = new ClientAssetsModel();
                newClient.ClientPortfolio = new MatrixClientPortfolioModel { MatrixClientPortfolio = client.MatrixClientPortfolio };
                ClientAssetsMoneyPositionModel clientMoneyPosition = new ClientAssetsMoneyPositionModel { Balance = client.Money };
                if (newClient.ClientPortfolio.MatrixClientPortfolio.Contains("-MS-"))
                {
                    clientMoneyPosition.Tag = "EQTV";
                }
                else
                {
                    clientMoneyPosition.Tag = "EUSR";
                }

                newClient.Moneys.Add(clientMoneyPosition);

                matrixClients.Add(newClient);

                portfoliosToHTTPRequestDepoPositions = portfoliosToHTTPRequestDepoPositions + $"&portfolios={client.MatrixClientPortfolio}";
            }

            return portfoliosToHTTPRequestDepoPositions;
        }

        private async Task<ClientAssetsModel> GetAssetsFromFileLimLimByPortfolio(string matrixPortfolio, string tks, string tag, string[] fileLimLim)
        {
            ClientAssetsModel clientAssetsModel = new ClientAssetsModel();
            clientAssetsModel.ClientPortfolio = new MatrixClientPortfolioModel { MatrixClientPortfolio = matrixPortfolio };

            //портфель в формат QUIK
            if (matrixPortfolio.Contains("-CD-"))
            {
                matrixPortfolio = CommonServices.PortfoliosConvertingService.GetQuikCdPortfolio(matrixPortfolio);
            }
            else
            {
                matrixPortfolio = CommonServices.PortfoliosConvertingService.GetQuikSpotPortfolio(matrixPortfolio);
            }

            //получить строки для конкретного клиента
            List<string> clientInLimLim = GetStringsWithClient(matrixPortfolio, fileLimLim);

            //очистить строки = оставить только для MS или FX/CD
            LeftOnlyStringsBelongingToPortfolio(clientInLimLim, tks, tag);

            // если не найдено строк с клиентом, вернем null 
            if (clientInLimLim.Count == 0)
            {
                return null;
            }

            //собираем модель клиента из строк
            foreach (string str in clientInLimLim)
            {
                if (str.Contains("MONEY:"))
                {
                    //MONEY:  FIRM_ID = MC0138200000; TAG = EQTV; CURR_CODE = SUR; CLIENT_CODE = BP33736/01; OPEN_BALANCE = 959.27; OPEN_LIMIT = 0; LEVERAGE =  2.00; LIMIT_KIND = 2;

                    ClientAssetsMoneyPositionModel clientMoney = new ClientAssetsMoneyPositionModel();
                    clientMoney.Tag = GetStringValueFromStringByTerm("TAG=", str);
                    clientMoney.Currency = GetStringValueFromStringByTerm("CURR_CODE=", str);
                    clientMoney.Balance = GetDecimalValueFromStringByTerm("OPEN_BALANCE=", str);

                    clientAssetsModel.Moneys.Add(clientMoney);
                }
                else if (str.Contains("DEPO:"))
                {
                    //DEPO:  FIRM_ID = MC0138200000; SECCODE = SPBE; CLIENT_CODE =BP33736/01; OPEN_BALANCE = 300; OPEN_LIMIT = 0; TRDACCID =L01+00000F00; WA_POSITION_PRICE=93.400000; LIMIT_KIND = 2;
                    ClientAssetsDepoPositionModel clientDepo = new ClientAssetsDepoPositionModel();
                    clientDepo.Seccode = GetStringValueFromStringByTerm("SECCODE=", str);
                    clientDepo.TKS = GetStringValueFromStringByTerm("TRDACCID=", str);
                    clientDepo.OpenBalance = GetDecimalValueFromStringByTerm("OPEN_BALANCE=", str);
                    clientDepo.AveragePrice = GetDecimalValueFromStringByTerm("WA_POSITION_PRICE=", str);

                    clientAssetsModel.Positions.Add(clientDepo);
                }
            }

            return clientAssetsModel;
        }

        private decimal GetDecimalValueFromStringByTerm(string nameOfTerm, string str)
        {
            string stringResult = GetStringValueFromStringByTerm(nameOfTerm, str);

            if (stringResult == "")
            {
                return 0;
            }

            stringResult = stringResult.Replace(".", ",");

            decimal result = Convert.ToDecimal(stringResult);

            return result;
        }

        private string GetStringValueFromStringByTerm(string nameOfTerm, string str)
        {
            if (!str.Contains(nameOfTerm))
            {
                return "";
            }

            int startPosition = str.IndexOf(nameOfTerm) + nameOfTerm.Length;
            int endPosition = str.Substring(startPosition).IndexOf(";");

            return str.Substring(startPosition, endPosition);
        }

        private void LeftOnlyStringsBelongingToPortfolio(List<string> clientInLimLim, string tks, string tag)
        {
            for (int i = clientInLimLim.Count-1; i >= 0; i--)
            {
                if (clientInLimLim[i].Contains("MONEY:"))
                {
                    //MONEY:  FIRM_ID = MC0138200000; TAG = EQTV; CURR_CODE = SUR; CLIENT_CODE = BP33736/01; OPEN_BALANCE = 959.27; OPEN_LIMIT = 0; LEVERAGE =  2.00; LIMIT_KIND = 2;
                    if (!clientInLimLim[i].Contains(tag))
                    {
                        clientInLimLim.RemoveAt(i);
                    }
                }
                else
                {
                    //DEPO:  FIRM_ID = MC0138200000; SECCODE = SPBE; CLIENT_CODE =BP33736/01; OPEN_BALANCE = 300; OPEN_LIMIT = 0; TRDACCID =L01+00000F00; WA_POSITION_PRICE=93.400000; LIMIT_KIND = 2;
                    if (!clientInLimLim[i].Contains(tks))
                    {
                        clientInLimLim.RemoveAt(i);
                    }
                }
            }
        }

        private List<string> GetStringsWithClient(string matrixPortfolio, string[] fileLimLim)
        {
            List<string> clientInLimLim = new List<string>();

            foreach (string line in fileLimLim)
            {
                if (line.Contains(matrixPortfolio))
                {
                    string lineNoSpaces = line.Replace(" ", "");

                    if (lineNoSpaces.Contains("LIMIT_KIND=2"))
                    {
                        clientInLimLim.Add(lineNoSpaces);
                    }
                }
            }

            return clientInLimLim;
        }
    }
}
