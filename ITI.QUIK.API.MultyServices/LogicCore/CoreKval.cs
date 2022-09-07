using CommonServices;
using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Models.EMail;
using DataAbstraction.Responses;
using Microsoft.Extensions.Logging;


namespace LogicCore
{
    public class CoreKval : ICoreKval
    {
        private ILogger<CoreKval> _logger;
        private IHttpApiRepository _repository;
        private IEMail _sender;

        public CoreKval(ILogger<CoreKval> logger, IHttpApiRepository repository, IEMail sender)
        {
            _logger = logger;
            _repository = repository;
            _sender = sender;
        }

        public async Task<ListStringResponseModel> RenewClients(bool sendReport)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreKval RenewClients Called");

            ListStringResponseModel result = new ListStringResponseModel();
            NewEMail message = new NewEMail();
            message.Subject = "QUIK обновление списков Квал и сдавших тесты в Qadmin MC0138200000 библиотеке";
            message.Body = "<html><body><h2>QUIK обновление списков Квал инвесторов</h2>";

            // запросить в БД матрицы список всех квалов
            MatrixClientCodeModelResponse kval = await _repository.GetAllKvalSpotPortfolios();

            //ok?
            if (kval.Response.IsSuccess)
            {
                message.Body = message.Body + $"<p>Найдено в БД Матрицы спот портфелей Квалов {kval.MatrixClientCodesList.Count}</p>";

                CodesArrayModel codesArrayModel = new CodesArrayModel();
                codesArrayModel.MatrixClientPortfolios = kval.MatrixClientCodesList.ToArray();

                ListStringResponseModel setKval = await _repository.SetKvalClientsToComplexProductRestrictions(codesArrayModel);

                if (setKval.IsSuccess == false)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} Ошибка установки в БРЛ QUIK спот портфелей Квалов");

                    foreach (var text in setKval.Messages)
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} - {text}");
                        message.Body = message.Body + $"<p style='color:red'> - {text}</p>";
                    }

                    result.IsSuccess = false;
                    result.Messages.AddRange(setKval.Messages);
                }
                else
                {
                    message.Body = message.Body + $"<p>Установка в БРЛ QUIK спот портфелей Квалов успешна</p>";
                }
            }
            else
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreKval RenewClients " +
                            $"Ошибка получения списка квалов из бд:");
                message.Body = message.Body + $"<p style='color:red'>Ошибка получения списка квалов из бд:</p>";

                foreach (var text in kval.Response.Messages)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreKval RenewClients - {text}");
                    message.Body = message.Body + $"<p style='color:red'> - {text}</p>";
                }

                result.IsSuccess = false;
                result.Messages.AddRange(kval.Response.Messages);
            }

            message.Body = message.Body + "<h2>QUIK обновление списков не квалов, сдавших тесты</h2>";
            // запросить в БД матрицы список всех не квалов с тестами
            PortfoliosAndTestForComplexProductResponse nekval = await _repository.GetAllNonKvalWithTestsSpotPortfolios();

            //ok?
            if (nekval.Response.IsSuccess)
            {
                message.Body = message.Body + $"<p>Найдено в БД Матрицы спот портфелей неквалов с тестами {nekval.TestForComplexProductList.Count}</p>";

                //создать список для отправки в БРЛ QUIK
                List<QCodeAndListOfComplexProductsTestsModel> clientsForQuikBRL = CreateNonKvalClientsListForBRL(nekval.TestForComplexProductList);

                message.Body = message.Body + $"<p>После сортировки портфелей неквалов с тестами {clientsForQuikBRL.Count}</p>";

                ListStringResponseModel setNeKval = await _repository.SetNonKvalClientsWithTestsToComplexProductRestrictions(clientsForQuikBRL.ToArray());

                if (setNeKval.IsSuccess == false)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} Ошибка установки в БРЛ QUIK спот портфелей неквалов с тестами");

                    foreach (var text in setNeKval.Messages)
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} - {text}");
                        message.Body = message.Body + $"<p style='color:red'> - {text}</p>";
                    }

                    result.IsSuccess = false;
                    result.Messages.AddRange(setNeKval.Messages);
                }
                else
                {
                    message.Body = message.Body + $"<p>Установка в БРЛ QUIK спот портфелей неквалов с тестами успешна</p>";
                }
            }
            else
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreKval RenewClients " +
                            $"Ошибка получения списка неквалов с тестами из бд:");
                message.Body = message.Body + $"<p style='color:red'>Ошибка получения списка неквалов с тестами из бд:</p>";

                foreach (var text in nekval.Response.Messages)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreKval RenewClients - {text}");
                    message.Body = message.Body + $"<p style='color:red'> - {text}</p>";
                }

                result.IsSuccess = false;
                result.Messages.AddRange(nekval.Response.Messages);
            }

            message.Body = message.Body + "</body></html>";

            if (sendReport)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreKval RenewClients отправляем почту");
                await _sender.Send(message);
            }

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreKval RenewClients all done");
            return result;
        }


        private List<QCodeAndListOfComplexProductsTestsModel> CreateNonKvalClientsListForBRL(List<PortfoliosAndTestForComplexProductModel> nekvalList)
        {
            List<QCodeAndListOfComplexProductsTestsModel> result = new List<QCodeAndListOfComplexProductsTestsModel>();

            foreach (var client in nekvalList)
            {
                string clientCode = client.MatrixClientPortfolio;
                //Console.WriteLine($"Обрабатываем данные: {client.MatrixClientPortfolio} как {clientCode} с квал кодом {client.TestForComplexProduct}");

                // обрабатываем коды портфелей rf cd ms fx rs                
                if (clientCode.Length < 10)//это код срочного рынка С000123 - приводим к виду SPBFUT00123
                {
                    clientCode = PortfoliosConvertingService.GetQuikFortsCode(clientCode);
                }
                else if (clientCode.Contains("-CD-"))//это коды клиентов с счетом CD BP12345-CD-01 приводим к виду BP12345/D01
                {
                    clientCode = PortfoliosConvertingService.GetQuikCdPortfolio(clientCode);
                }
                else//это коды обычных спот клиентов BP12345-MS-01 приводим к виду BP12345/01
                {
                    clientCode = PortfoliosConvertingService.GetQuikSpotPortfolio(clientCode);
                }

                //проверим, что клиент уже есть в списке. Если нет - сразу добавляем клиента и код
                var isClienExist = result.Find(x => x.QuikClientCode.Equals(clientCode));
                if (isClienExist != null)
                {
                    //Console.WriteLine("Такой клиент уже есть, " + clientCode);

                    if (!isClienExist.RestrictionCodes.Exists(x => x.ToString().Equals(client.TestForComplexProduct.ToString())))
                    {
                        //Console.WriteLine("Добавим новый код " + client.TestForComplexProduct);
                        isClienExist.RestrictionCodes.Add(client.TestForComplexProduct.ToString());
                    }
                }
                else
                {
                    //Console.WriteLine($"добавляем нового клиента {clientCode} с кодом {client.TestForComplexProduct}");

                    List<string> newList = new List<string>();
                    newList.Add(client.TestForComplexProduct.ToString());

                    result.Add(new QCodeAndListOfComplexProductsTestsModel
                    {
                        QuikClientCode = clientCode,
                        RestrictionCodes = newList
                    });
                }
            }

            return result;
        }
    }
}
