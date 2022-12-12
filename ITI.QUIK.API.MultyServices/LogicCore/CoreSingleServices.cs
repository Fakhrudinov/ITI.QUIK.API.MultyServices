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
        private IHttpMatrixRepository _repository;
        private IHttpQuikRepository _repoQuik;
        private CoreSettings _coreSettings;
        private LimLImCreationSettings _limSettings;
        private IEMail _sender;

        public CoreSingleServices(
            ILogger<CoreSingleServices> logger, 
            IHttpMatrixRepository repository,
            IHttpQuikRepository repoQuik,
            IOptions<CoreSettings> coreSettings,
            IOptions<LimLImCreationSettings> limSettings,
            IEMail sender)
        {
            _logger = logger;
            _repository = repository;
            _repoQuik = repoQuik;
            _coreSettings = coreSettings.Value;
            _limSettings = limSettings.Value;
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

        public async Task<BoolResponse> CheckIsFileCorrectLimLim(bool sendReport, bool checkExactMoney)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsFileCorrectLimLim Called");

            BoolResponse result = new BoolResponse();
            NewEMail message = new NewEMail();

            string check = "Проверка";
            if (checkExactMoney)
            {
                check = "Точная проверка";
            }

            message.Body = $"<html><body><h2>{check} корректности файла лимитов lim.lim</h2>";

            // проверить что файл lim.lim есть
            if (!File.Exists(_coreSettings.PathToLimLim))
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} Error! File lim.lim not found at " + _coreSettings.PathToLimLim);
                result.Messages.Add($"Error! File lim.lim not found at {_coreSettings.PathToLimLim}");
                result.IsSuccess = false;
                result.IsTrue = false;

                message.Body = message.Body + $"<p style='color:red'>Error! File lim.lim not found at {_coreSettings.PathToLimLim}</p>";
                message.Subject = $"Fail! QUIK {check} корректности (заранее скачанного) файла лимитов lim.lim";
                await SendMessageFinalize(message, sendReport);

                return result;
            }

            // считать все строки файла lim.lim
            string[] fileLimLim = await File.ReadAllLinesAsync(_coreSettings.PathToLimLim);

            // lim.lim старый? 
            ListStringResponseModel lastWriteTime = await _repoQuik.GetSftpFileLastWriteTime("lim.lim");

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
                message.Subject = $"Nobody to check! QUIK {check} корректности (заранее скачанного) файла лимитов lim.lim";
                await SendMessageFinalize(message, sendReport);

                return result;
            }

            message.Body = message.Body + $"<p>В матрице найдено {matrixClients.Count} портфелей от клиентов, торговавших вчера</p>";

            //запросить позиции из матрицы у клиентов с сделками
            await AddPositionsToClientsWhoTradeYesterday(matrixClients, portfoliosToHTTPRequestDepoPositions);

            List<ClientAssetsModel> limlimClients = await GetClientLimitsFromLimLim(matrixClients, fileLimLim, result, message);

            message.Body = message.Body + $"<p>Получили {limlimClients.Count} портфелей от клиентов из файла lim.lim</p>";
            message.Body = message.Body + $"<h3>Сравниваем результаты</h3>";
            //сравниваем данные матрицы и файла lim.lim
            CompareMatrixAndLimlimAndRemoveEquals(matrixClients, limlimClients, checkExactMoney);

            // смотрим остатки, заполняем отчет о различиях
            AnalizeLeftoversAndFillResult(matrixClients, limlimClients, result);
            if (sendReport)
            {
                AnalizeLeftoversAndFillEmail(matrixClients, limlimClients, message);
            }

            if (result.IsSuccess)
            {
                if (result.Messages.Count > 0)
                {
                    message.Subject = $"Differences! QUIK {check} корректности (заранее скачанного) файла лимитов lim.lim";
                }
                else
                {
                    message.Subject = $"OK! QUIK {check} корректности (заранее скачанного) файла лимитов lim.lim";
                }
            }
            else
            {
                message.Subject = $"Failed! QUIK {check} корректности (заранее скачанного) файла лимитов lim.lim";
            }

            await SendMessageFinalize(message, sendReport);          

            return result;
        }

        private void AnalizeLeftoversAndFillEmail(List<ClientAssetsModel> matrixClients, List<ClientAssetsModel> limlimClients, NewEMail message)
        {
            if (matrixClients.Count > 0 || limlimClients.Count > 0)
            {
                List<ClientAssetsComparitionModel> clientsWithDiff = new List<ClientAssetsComparitionModel>();

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

                foreach (var mClient in matrixClients)
                {
                    foreach (var money in mClient.Moneys)
                    {
                        result.Messages.Add($"matrix: {mClient.ClientPortfolio.MatrixClientPortfolio} " +
                            $"has different money {money.Currency} {money.Balance} {money.Tag}");
                    }

                    foreach (var depo in mClient.Positions)
                    {
                        result.Messages.Add($"matrix: {mClient.ClientPortfolio.MatrixClientPortfolio} " +
                            $"has different position {depo.Seccode} {depo.OpenBalance} {depo.AveragePrice} {depo.TKS}");
                    }
                }

                foreach (var qClient in limlimClients)
                {
                    foreach (var money in qClient.Moneys)
                    {
                        result.Messages.Add($"lim.lim: {qClient.ClientPortfolio.MatrixClientPortfolio} " +
                            $"has different money {money.Currency} {money.Balance} {money.Tag}");
                    }

                    foreach (var depo in qClient.Positions)
                    {
                        result.Messages.Add($"lim.lim: {qClient.ClientPortfolio.MatrixClientPortfolio} " +
                            $"has different position {depo.Seccode} {depo.OpenBalance} {depo.AveragePrice} {depo.TKS}");
                    }
                }
            }
            else
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices CheckIsFileCorrectLimLim " +
                    $"- OK расхождений не найдено");

                result.IsTrue = true;
            }
        }

        private void CompareMatrixAndLimlimAndRemoveEquals(List<ClientAssetsModel> matrixClients, List<ClientAssetsModel> limlimClients, bool checkExactMoney)
        {
            //сравниваем данные матрицы и файла lim.lim
            for (int i = matrixClients.Count - 1; i >= 0; i--)
            {
                // для дебага - ловить конкретного клиента
                //if (matrixClients[i].ClientPortfolio.MatrixClientPortfolio.Equals("BP3871-MS-01"))
                //{
                //    Console.WriteLine();
                //}

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
                                    else if (!checkExactMoney)// если не включена проверка точного соответствия рублей
                                    {
                                        //получаем 1% от денег матрицы
                                        decimal onePercent = matrixClients[i].Moneys[m].Balance / 100;

                                        if (compareToLimLim.Moneys[qm].Balance + onePercent >= matrixClients[i].Moneys[m].Balance &&
                                            compareToLimLim.Moneys[qm].Balance - onePercent <= matrixClients[i].Moneys[m].Balance)
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
                                        else if (!checkExactMoney)// если не включена проверка точного соответствия рублей
                                        {
                                            //получаем 1% от AveragePrice матрицы
                                            decimal onePercent = matrixClients[i].Positions[p].AveragePrice / 100;

                                            if (compareToLimLim.Positions[qp].AveragePrice + onePercent >= matrixClients[i].Positions[p].AveragePrice &&
                                                compareToLimLim.Positions[qp].AveragePrice - onePercent <= matrixClients[i].Positions[p].AveragePrice)
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
                string tag = CommonServices.LimLimService.GetTagByPortfolio(matrixClients[i].ClientPortfolio.MatrixClientPortfolio);
                string tks = matrixClients[i].TKS;

                // пусть будет пока. хотя ткс из позиции по идее должно быть = ткс из портфеля.
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
                $"получено позицийу клиентов с сделками - " + matrixDepoPositions.PortfoliosAndPosition.Count);
            //позиции записать в List<ClientAssetsModel> matrixClients
            foreach (ClientDepoPositionModel position in matrixDepoPositions.PortfoliosAndPosition)
            {
                // если USD EUR etc - то в деньги <<-------------------------------------------------------------------------
                if (_limSettings.PositionAsMoneyArray.Contains(position.SecCode))
                {
                    string tag = CommonServices.LimLimService.GetTagByPortfolio(position.MatrixClientPortfolio);

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

                    int indexOfMatrixClient = matrixClients.FindIndex(r => r.ClientPortfolio.MatrixClientPortfolio == position.MatrixClientPortfolio);
                    if (indexOfMatrixClient == -1)
                    {
                        indexOfMatrixClient = matrixClients.FindIndex(r => r.ClientPortfolio.MatrixClientPortfolio == position.MatrixClientPortfolio.Replace("-MS-", "-MO-"));
                    }

                    matrixClients[indexOfMatrixClient].Positions.Add(newDepoPosition);
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
                ClientAssetsModel newClient = new ClientAssetsModel
                {
                    TKS = client.TKS
                };

                newClient.ClientPortfolio = new MatrixClientPortfolioModel 
                { 
                    MatrixClientPortfolio = client.MatrixClientPortfolio,
                };

                ClientAssetsMoneyPositionModel clientMoneyPosition = new ClientAssetsMoneyPositionModel 
                { 
                    Balance = client.Money
                };

                clientMoneyPosition.Tag = CommonServices.LimLimService.GetTagByPortfolio(newClient.ClientPortfolio.MatrixClientPortfolio);

                newClient.Moneys.Add(clientMoneyPosition);

                matrixClients.Add(newClient);

                portfoliosToHTTPRequestDepoPositions = portfoliosToHTTPRequestDepoPositions + $"&portfolios={client.MatrixClientPortfolio.Replace("-MO-", "-MS-")}";
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
                    if (!tks.Equals("")) // если пустое - ничего не удаляем.
                    {
                        if (!clientInLimLim[i].Contains(tks))
                        {
                            clientInLimLim.RemoveAt(i);
                        }
                    }
                }
            }
        }

        private List<string> GetStringsWithClient(string matrixPortfolio, string[] fileLimLim)
        {
            List<string> clientInLimLim = new List<string>();

            foreach (string line in fileLimLim)
            {
                if (line.Contains(matrixPortfolio + ";"))
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

        public async Task<ListStringResponseModel> GetSingleClientSpotLimitsToFileByMatrixAccount(string matrixClientAccount, bool oldPositionMustBeZeroing)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices GetSingleClientSpotLimitsToFileByMatrixAccount {matrixClientAccount} Called");

            ListStringResponseModel result = new ListStringResponseModel();            

            // проверка наличия директории
            if (!Directory.Exists(_limSettings.PathToSingleClientLimLim))
            {
                try
                {
                    Directory.CreateDirectory(_limSettings.PathToSingleClientLimLim);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices GetSingleClientSpotLimitsToFileByMatrixAccount Error! " +
                        $"Path directory '{_limSettings.PathToSingleClientLimLim}' unavaliable");

                    result.IsSuccess = false;
                    result.Messages.Add($"Error! Path directory '{_limSettings.PathToSingleClientLimLim}' unavaliable. {ex.Message}");
                    return result;
                }
            }

            //запрос всех денег клиента по портфелям
            SingleClientPortfoliosMoneyResponse clientPortfolios = await _repository.GetClientSpotPortfoliosAndMoneyForLimLim(matrixClientAccount);
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices GetSingleClientSpotLimitsToFileByMatrixAccount " +
                $"for {matrixClientAccount} finded {clientPortfolios.PortfoliosAndMoney.Count} portfolios");
            result.Messages.AddRange(clientPortfolios.Response.Messages);

            //если нет портфелей с деньгами то запрос позиций не имеет смысла
            if (clientPortfolios.PortfoliosAndMoney.Count > 0)
            {
                List<ClientDepoPositionModel> clientPositions = new List<ClientDepoPositionModel>();

                //запрос привязки к ТКС пустыми позициями.
                ClientDepoPositionsResponse clientInitialZeroToTKSPositions = await _repository.GetClientInitialDepoToTksSpotPositionsForLimLim(matrixClientAccount);
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices GetSingleClientSpotLimitsToFileByMatrixAccount " +
                    $"for {matrixClientAccount} finded {clientInitialZeroToTKSPositions.PortfoliosAndPosition.Count} TKS to portfolio positions");
                result.Messages.AddRange(clientInitialZeroToTKSPositions.Response.Messages);
                clientPositions.AddRange(clientInitialZeroToTKSPositions.PortfoliosAndPosition);

                //запрос всех РАНЕЕ ЗАКРЫТЫХ позиций клиента
                if (oldPositionMustBeZeroing)
                {
                    //дней когда последний раз торговал
                    for (int dayShift = 1; dayShift <= 7; dayShift++)
                    {
                        BoolResponse GetIsClientTrade = await _repository.GetBoolIsClientTradeDaysAgoByClientAccountAndDays(matrixClientAccount, dayShift);
                        if (GetIsClientTrade.IsTrue)
                        {
                            //запрос зануленных закрытых позиций
                            ClientDepoPositionsResponse clientZeroedPositions = await _repository.GetClientZeroedClosedSpotPositionsForLimLim(matrixClientAccount, dayShift);
                            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices GetSingleClientSpotLimitsToFileByMatrixAccount " +
                                $"for {matrixClientAccount} finded {clientZeroedPositions.PortfoliosAndPosition.Count} Zeroed positions");
                            result.Messages.AddRange(clientZeroedPositions.Response.Messages);
                            clientPositions.AddRange(clientZeroedPositions.PortfoliosAndPosition);

                            break;
                        }
                    }
                }

                //запрос всех актуальных позимций клиента                
                ClientDepoPositionsResponse clientActualPositions = await _repository.GetClientActualSpotPositionsForLimLim(matrixClientAccount);
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices GetSingleClientSpotLimitsToFileByMatrixAccount " +
                    $"for {matrixClientAccount} finded {clientActualPositions.PortfoliosAndPosition.Count} actual positions");
                result.Messages.AddRange(clientActualPositions.Response.Messages);
                clientPositions.AddRange(clientActualPositions.PortfoliosAndPosition);

                //create lim.lim file
                List<string> fileText = new List<string>();
                //Деньги:
                foreach (PortfoliosAndMoneyModel portfolio in clientPortfolios.PortfoliosAndMoney)
                {
                    fileText.Add(CreateMoneyStringForLimLim(portfolio));
                }
                //позиции
                foreach (ClientDepoPositionModel position in clientPositions)
                {
                    fileText.Add(CreatePositionStringForLimLim(position));
                }

                //добавить имя файла к пути
                string filePath = Path.Combine(_limSettings.PathToSingleClientLimLim, $"lim_{matrixClientAccount}_{DateTime.Now.ToString("yy-MM-dd_HH-mm")}.lim");
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices GetSingleClientSpotLimitsToFileByMatrixAccount " + 
                    $"try write file to {filePath}");
                // пробуем записать
                try
                {
                    await File.WriteAllLinesAsync(filePath, fileText);

                    result.Messages.Add($"File saved at path: {filePath}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} ICoreSingleServices GetSingleClientSpotLimitsToFileByMatrixAccount Exception when write file! " +
                        ex.Message);

                    result.IsSuccess = false;
                    result.Messages.Add($"Exception when write file! " + ex.Message);
                }                
            }
            else
            {
                result.IsSuccess = false;                
                result.Messages.Add($"Для клиента {matrixClientAccount} не найдено данных по наличию портфелей!");
            }

            return result;
        }

        private string CreatePositionStringForLimLim(ClientDepoPositionModel position)
        {
            //если это деньги - делаем деньги, иначе обычную позицию
            if (_limSettings.PositionAsMoneyArray.Contains(position.SecCode))
            {
                PortfoliosAndMoneyModel portfolio = new PortfoliosAndMoneyModel();
                portfolio.MatrixClientPortfolio = position.MatrixClientPortfolio;
                portfolio.Leverage = 1;
                portfolio.MoneyOpenBalanse = position.OpenBalance;

                string resultMoney = CreateMoneyStringForLimLim(portfolio, position.SecCode);
                return resultMoney;
            }

            //делаем обычную позицию
            string positionResult = "";

            //DEPO:  FIRM_ID = MC0138200000; SECCODE = HYDR; CLIENT_CODE = BP1234/01; OPEN_BALANCE = -1000; OPEN_LIMIT = 0; TRDACCID = L01+00000F00; WA_POSITION_PRICE = 0.745100; LIMIT_KIND = 2;
            for (int i = 1; i <= 2; i++)
            {
                positionResult = positionResult +
                    $"DEPO: " +
                    $" FIRM_ID = MC0138200000;" +
                    $" LIMIT_KIND = {i};" +
                    $" CLIENT_CODE = {CommonServices.PortfoliosConvertingService.GetQuikSpotCode(position.MatrixClientPortfolio)};" +
                    $" SECCODE = {position.SecCode};" +
                    $" OPEN_BALANCE = {position.OpenBalance};" +
                    $" OPEN_LIMIT = 0;" +
                    $" TRDACCID = {position.TKS};" +
                    $" WA_POSITION_PRICE = {position.AveragePrice};"
                    ;
                if (i == 1)
                {
                    positionResult = positionResult + "\r\n";
                }
            }

            positionResult = CheckRepareDelimeters(positionResult);

            return positionResult;
        }

        private string CreateMoneyStringForLimLim(PortfoliosAndMoneyModel portfolio, string currency = "SUR")
        {
            string moneyResult = "";

            //MONEY:  FIRM_ID = MC0138200000; TAG = EQTV; CURR_CODE = SUR; CLIENT_CODE = BP21609/01; OPEN_BALANCE = 0.00; LEVERAGE = 2.00; LIMIT_KIND = 2;
            for (int i = 1; i <= 2; i++)
            {
                moneyResult = moneyResult +
                    $"MONEY: " +
                    $" FIRM_ID = MC0138200000;" +
                    $" LIMIT_KIND = {i};" +
                    $" CLIENT_CODE = {CommonServices.PortfoliosConvertingService.GetQuikSpotCode(portfolio.MatrixClientPortfolio)};" +
                    $" CURR_CODE = {currency};" +
                    $" OPEN_BALANCE = {portfolio.MoneyOpenBalanse};" +
                    $" TAG = {CommonServices.LimLimService.GetTagByPortfolio(portfolio.MatrixClientPortfolio)};" +  
                    $" LEVERAGE = {portfolio.Leverage};"
                    ;
                if (i == 1)
                {
                    moneyResult = moneyResult + "\r\n";
                }
            }

            moneyResult = CheckRepareDelimeters(moneyResult);

            return moneyResult;
        }

        private string CheckRepareDelimeters(string str)
        {
            if (_limSettings.Delimiter.Equals("."))
            {
                str = str.Replace(",", ".");
            }
            else
            {
                str = str.Replace(".", ",");
            }

            return str;
        }
    }
}
