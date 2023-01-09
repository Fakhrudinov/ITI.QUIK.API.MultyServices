using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Models.Discounts;
using DataAbstraction.Models.EMail;
using DataAbstraction.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace LogicCore
{
    public class CoreDiscounts : ICoreDiscounts
    {
        private ILogger<CoreDiscounts> _logger;
        private IHttpMatrixRepository _repoMatrix;
        private IHttpQuikRepository _repoQuik;
        private DiscountsSettings _settings;
        private string[] _settingsCurrencyArray;
        private IEMail _sender;

        // эталонные дисконты
        private DiscountModel _ksur = null;
        private DiscountModel _kpur = null;
        private DiscountModel _nolvrg = null;

        private enum _discontType
        { 
            KSUR = 0,
            KPUR = 1,
            NoLvrg = 2
        };

        private DiscountModel[] discontEthalonArray = new DiscountModel[3];// по числу в enum


        public CoreDiscounts(
            ILogger<CoreDiscounts> logger,
            IHttpMatrixRepository repoMatrix,
            IHttpQuikRepository repoQuik,
            IOptions<DiscountsSettings> settings,
            IOptions<LimLImCreationSettings> settingsCurrency,
            IEMail sender
            )
        {
            _logger = logger;
            _repoMatrix = repoMatrix;
            _repoQuik = repoQuik;
            _settings = settings.Value;
            _settingsCurrencyArray = settingsCurrency.Value.PositionAsMoneyArray;
            _sender = sender;
        }

        public async Task<BoolResponse> CheckSingleDiscount(string security)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CheckSingleDiscount Called {security}");
            BoolResponse result = new BoolResponse 
            {
                IsTrue = true
            };

            /// plan
            // получим информацию из матрицы
            //  считаем по ней эталонные дисконты КСУР, КПУР и NoLvrg
            //      если null и ошибка = "Не найдено данных по дисконтам ***"
            //          - то смотрим чтобы данных не было и в QUIK, эталонные дисконты = null 
            //      если null и другое - прервать!
            // получим информацию из Quik global, сравниваем
            // получим информацию из Quik templates, сравниваем. (ввод дисконта по базовой валюте (USD CNY etc) - их не должно быть в темплейтах)


            // получим информацию из матрицы
            DiscountMatrixSingleResponse matrixDiscount = await GetDiscountValueFromMatrix(security);

            if (matrixDiscount.IsSuccess) // считаем эталонные дисконты
            {
                CalculateEthalonDiscounts(matrixDiscount.Discount);

                result.Messages.Add($"Matrix: " +
                    $"KSUR {_ksur.ToString()} KPUR {_kpur.ToString()} NoLvrg {_nolvrg.ToString()}");
            }
            else // ошибка
            {
                if (matrixDiscount.Messages[0].StartsWith("Не найдено данных по дисконтам "))
                {
                    // ничего тут не делаем, эталонные дисконты остаются null.
                    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CheckSingleDiscount " +
                        $"{security} discount not found");
                    result.Messages.Add($"Matrix discount not found.");
                }
                else
                {
                    // ошибка в запросе, завершаем выполнение.
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CheckSingleDiscount " +
                        $"{security} failed at matrix data request");

                    result.Messages.AddRange(matrixDiscount.Messages);

                    result.IsTrue = false;
                    result.IsSuccess = false;

                    return result;
                }
            }


            //получим информацию из Quik global
            DiscountSingleResponse quikGlobalDiscount = await _repoQuik.GetDiscountSingleFromGlobal(security);
            if(quikGlobalDiscount.IsSuccess )
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CheckSingleDiscount " +
                    $"Quik global discount {quikGlobalDiscount.Discount.ToString()}");

                // сравниваем с эталонами
                if (quikGlobalDiscount.Discount.Equals(_ksur))
                {
                    result.Messages.Add($"OK = Quik global discount equal to KSUR. {quikGlobalDiscount.Discount.ToString()}");
                }
                else
                {
                    result.Messages.Add($"Error = Quik global discount NOT equal to KSUR. {quikGlobalDiscount.Discount.ToString()}");
                    result.IsTrue = false;
                }
            }
            else
            {
                if (quikGlobalDiscount.Messages[0].StartsWith("QAS110 Error!"))
                {
                    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CheckSingleDiscount " +
                        $"Quik global discounts {security} not found. ");

                    if (_ksur == null)
                    {
                        // ok, так и должно быть, в матрице тоже нет
                        result.Messages.Add($"OK = Quik global discount not found.");
                    }
                    else
                    {
                        result.Messages.Add($"Error = Quik global discount not found.");
                        result.IsTrue = false;
                    }
                }
                else
                {
                    // ошибка в запросе
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CheckSingleDiscount " +
                        $"{security} failed at Quik global request");

                    result.Messages.AddRange(quikGlobalDiscount.Messages);
                }
            }


            //получим информацию из Quik templates KSUR
            if (_settings.TemlpatesArrayKSUR is not null)
            {
                foreach (string template in _settings.TemlpatesArrayKSUR)
                {
                    await CompareSingleDiscountFromTemplateWithEthalon(template, security, _ksur, result);
                }
            }
            //получим информацию из Quik templates KPUR
            if (_settings.TemlpatesArrayKPUR is not null)
            {
                foreach (string template in _settings.TemlpatesArrayKPUR)
                {
                    await CompareSingleDiscountFromTemplateWithEthalon(template, security, _kpur, result);
                }
            }
            //получим информацию из Quik templates NoLvrg
            if (_settings.TemlpatesArrayNoLeverage is not null)
            {
                foreach (string template in _settings.TemlpatesArrayNoLeverage)
                {
                    await CompareSingleDiscountFromTemplateWithEthalon(template, security, _nolvrg, result);
                }
            }

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CheckSingleDiscount {security} " +
                $"IsSuccess={result.IsSuccess} IsTrue={result.IsTrue}");
            return result;
        }    

        public async Task<BoolResponse> PostSingleDiscount(string security)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CheckSingleDiscount Called {security}");
            BoolResponse result = new BoolResponse();

            /// plan
            // получим информацию из матрицы
            //  считаем по ней эталонные дисконты КСУР, КПУР и NoLvrg
            //      если null или ошибка - прервать!
            // запишем КСУР в Quik global
            // запишем в Quik templates (ввод дисконта по базовой валюте (USD CNY etc) - их не должно быть в темплейтах)


            // получим информацию из матрицы
            DiscountMatrixSingleResponse matrixDiscount = await GetDiscountValueFromMatrix(security);

            if (matrixDiscount.IsSuccess) // считаем эталонные дисконты
            {
                CalculateEthalonDiscounts(matrixDiscount.Discount);

                result.Messages.Add($"Matrix: " +
                    $"KSUR {_ksur.ToString()} KPUR {_kpur.ToString()} NoLvrg {_nolvrg.ToString()}");
            }
            else
            {
                // ошибка в запросе или данные не найдены, завершаем выполнение.
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts PostSingleDiscount " +
                    $"{security} failed at matrix data request");

                result.Messages.AddRange(matrixDiscount.Messages);

                result.IsTrue = false;
                result.IsSuccess = false;

                return result;
            }
            
            //отправка данных
            DiscountAndSecurityModel modelToQuik = new DiscountAndSecurityModel { Security = security };

            // запишем КСУР в Quik global
            modelToQuik.Discount = _ksur;

            ListStringResponseModel postToGlobal = await _repoQuik.PostSingleDiscountToGlobal(modelToQuik);
            if (!postToGlobal.IsSuccess)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts PostSingleDiscount " +
                    $"{security} POST to global failed! {postToGlobal.Messages[0]}");

                result.Messages.AddRange(postToGlobal.Messages);

                result.IsTrue = false;
                result.IsSuccess = false;
            }
            else
            {
                result.Messages.Add($"Discount set to Quik global {modelToQuik.Security} - {modelToQuik.Discount.ToString()}.");
            }

            // отправка данных в шаблоны
            //отправка данных в шаблоны Quik templates KSUR
            if (_settings.TemlpatesArrayKSUR is not null)
            {
                foreach (string template in _settings.TemlpatesArrayKSUR)
                {
                    await PostSingleDiscountToTemplate(template, modelToQuik, result);
                }
            }
            //отправка данных в шаблоны Quik templates KPUR
            if (_settings.TemlpatesArrayKPUR is not null)
            {
                modelToQuik.Discount = _kpur;

                foreach (string template in _settings.TemlpatesArrayKPUR)
                {
                    await PostSingleDiscountToTemplate(template, modelToQuik, result);
                }
            }
            //отправка данных в шаблоны NoLvrg
            if (_settings.TemlpatesArrayNoLeverage is not null)
            {
                modelToQuik.Discount = _nolvrg;

                foreach (string template in _settings.TemlpatesArrayNoLeverage)
                {
                    await PostSingleDiscountToTemplate(template, modelToQuik, result);
                }
            }            

            return result;
        }

        public async Task<BoolResponse> DeleteSingleDiscount(string security)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts DeleteSingleDiscount Called {security}");
            BoolResponse result = new BoolResponse
            {
                IsTrue = true
            };

            //удаляем из глобал
            ListStringResponseModel deleteFromGlobal = await _repoQuik.DeleteDiscountFromGlobal(security);
            if (deleteFromGlobal.IsSuccess)
            {
                result.Messages.Add($"Delete from global is success.");
            }
            else // ошибка
            {
                if (deleteFromGlobal.Messages[0].EndsWith("1004 Данные не найдены"))
                {
                    result.Messages.Add($"Delete from global - discount not found. Nothing to delete.");
                }
                else
                {
                    // ошибка в запросе
                    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts DeleteSingleDiscount {security} " +
                        $"from global - Error {deleteFromGlobal.Messages[0]}");

                    result.Messages.AddRange(deleteFromGlobal.Messages);

                    result.IsTrue = false;
                    result.IsSuccess = false;
                }
            }

            // удаляем из щаблонов
            //пробуем удалить из Quik templates KSUR
            if (_settings.TemlpatesArrayKSUR is not null)
            {
                foreach (string template in _settings.TemlpatesArrayKSUR)
                {
                    await DeleteSingleDiscountFromTemplate(template, security, result);
                }
            }
            //пробуем удалить из Quik templates KPUR
            if (_settings.TemlpatesArrayKPUR is not null)
            {
                foreach (string template in _settings.TemlpatesArrayKPUR)
                {
                    await DeleteSingleDiscountFromTemplate(template, security, result);
                }
            }
            //пробуем удалить из Quik templates NoLvrg
            if (_settings.TemlpatesArrayNoLeverage is not null)
            {
                foreach (string template in _settings.TemlpatesArrayNoLeverage)
                {
                    await DeleteSingleDiscountFromTemplate(template, security, result);
                }
            }

            return result;
        }

        public async Task<BoolResponse> CheckAllDiscounts()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CheckAllDiscounts Called");
            BoolResponse result = new BoolResponse
            {
                IsTrue = true
            };
            NewEMail message = new NewEMail();
            message.Subject = "Quik Проверка списков дисконтов";
            message.Body = $"<html><body><h2>Проверка списков дисконтов</h2>";

            /// план 
            // получим из матрицы списки ЦБ+дисконтов по рынкам, указанным в настройках CheckMarketsArray.
            // при неудаче - прерываем.

            // запросим список бумаг из глобал/шаболонов
            // сравним по бумагам, лишние/недостающие указать отдельно, \
            // по оставшимся сравить значения дисконотов.

            List<DiscountMatrixModel> matrixDiscounts = new List<DiscountMatrixModel>();

            foreach (string market in _settings.CheckMarketsArray)
            {
                DiscountsListResponse discounts = await _repoMatrix.GetDiscountsListFromMarket(market);
                if (discounts.IsSuccess)
                {
                    matrixDiscounts.AddRange(discounts.Discounts);

                    message.Body = message.Body + $"<p>From Matrix {market} added {discounts.Discounts.Count} discounts.</p>";
                    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CheckAllDiscounts " +
                        $"- from Matrix market {market} added {discounts.Discounts.Count} discounts");
                }
                else
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CheckAllDiscounts " +
                        $"failed get Matrix discounts from market {market}. {discounts.Messages[0]}");

                    message.Body = message.Body + $"<p style='color:red'>Error! Matrix discounts from market {market} failed.</p>";
                    foreach (var str in discounts.Messages)
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CheckAllDiscounts - {str}");
                        message.Body = message.Body + $"<p style='color:red'> - {str}</p>";
                    }

                    result.IsTrue = false;
                    result.IsSuccess = false;
                    result.Messages.AddRange(discounts.Messages);

                    message.Subject = "Failed! " + message.Subject;

                    await SendMessageFinalize(message);

                    return result;
                }
            }
            // сравниваем в глобал
            await CompareDiscountsList("global", matrixDiscounts, result, message, _discontType.KSUR);
            // запросить из всех шаблонов ------------------------------------------------
            //получим информацию из Quik templates KSUR
            if (_settings.TemlpatesArrayKSUR is not null)
            {
                foreach (string template in _settings.TemlpatesArrayKSUR)
                {
                    await CompareDiscountsList(template, matrixDiscounts, result, message, _discontType.KSUR);
                }
            }
            //получим информацию из Quik templates KPUR
            if (_settings.TemlpatesArrayKPUR is not null)
            {
                foreach (string template in _settings.TemlpatesArrayKPUR)
                {
                    await CompareDiscountsList(template, matrixDiscounts, result, message, _discontType.KPUR);
                }
            }
            //получим информацию из Quik templates NoLvrg
            if (_settings.TemlpatesArrayNoLeverage is not null)
            {
                foreach (string template in _settings.TemlpatesArrayNoLeverage)
                {
                    await CompareDiscountsList(template, matrixDiscounts, result, message, _discontType.NoLvrg);
                }
            }


            if (result.IsTrue)
            {
                message.Subject = "Success! " + message.Subject;
            }
            else
            {
                message.Subject = "Errors! " + message.Subject;
            }

            await SendMessageFinalize(message);
            return result;
        }

        private async Task CompareDiscountsList(string templateName, List<DiscountMatrixModel> matrixDiscounts, BoolResponse result, NewEMail message, _discontType discontType)
        {
            // получить список цб из шаблона
            // сравнить списки
            SecuritysListResponse securitiesFromQuik = null;
            message.Body = message.Body + $"<h2>Сравниваем шаблон {templateName}</h2>";

            // get
            if (templateName == "global")//from global
            {
                securitiesFromQuik = await _repoQuik.GetDiscountSecurityesListFromGlobal();
            }
            else // from template
            {
                securitiesFromQuik = await _repoQuik.GetDiscountSecurityesListFromTemplate(templateName);
            }

            // compare
            if (!securitiesFromQuik.IsSuccess) // error at request
            {
                result.IsTrue = false;

                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CompareDiscountsList " +
                    $"failed to get discounts securities list from {templateName}. {securitiesFromQuik.Messages[0]}");

                message.Body = message.Body + $"<p style='color:red'>Error when get discounts securities list from {templateName}.</p>";
                foreach (var str in securitiesFromQuik.Messages)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CompareDiscountsList - {str}");
                    message.Body = message.Body + $"<p style='color:red'> - {str}</p>";
                }
            }
            else // ok
            {
                message.Body = message.Body + $"<p>In {templateName} finded {securitiesFromQuik.Securitys.Count} securities.</p>";
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CompareDiscountsList " +
                    $"- In {templateName} finded {securitiesFromQuik.Securitys.Count} securities.");

                // получить список недостающих в quik securityes
                // получить список лишних в quik securityes - этот список будет как остаток от securitiesFromQuik
                // получить список по совпадающим securityes - по ним сравнить дисконты

                List<string> absentInQuik= new List<string>();
                List<string> matchInBoth = new List<string>();

                if (securitiesFromQuik.Securitys is not null && securitiesFromQuik.Securitys.Count > 0)
                {
                    //заполнить недостающие и совпадающие
                    for (int i = securitiesFromQuik.Securitys.Count - 1; i >= 0; i--)
                    {
                        foreach (DiscountMatrixModel security in matrixDiscounts)
                        {
                            if (security.Tiker == securitiesFromQuik.Securitys[i])
                            {
                                matchInBoth.Add(securitiesFromQuik.Securitys[i]);
                                securitiesFromQuik.Securitys.RemoveAt(i);
                                break;
                            }
                        }

                    }
                    // заполнит абсент
                    foreach (DiscountMatrixModel securityInMatrix in matrixDiscounts)
                    {
                        bool found = false;
                        foreach (string securityInQuik in matchInBoth)
                        {
                            if (securityInMatrix.Tiker == securityInQuik)
                            {
                                found = true;
                                break;
                            }
                        }
                        //если не нашли - добавим
                        if (!found)
                        {
                            absentInQuik.Add(securityInMatrix.Tiker);
                        }
                    }
                }
                else // тогда все в список отсутствующих
                {
                    foreach (DiscountMatrixModel security in matrixDiscounts)
                    {
                        absentInQuik.Add(security.Tiker);
                    }
                }

                if (absentInQuik.Count > 0 || securitiesFromQuik.Securitys.Count > 0)
                {
                    result.IsTrue = false;
                }

                // Выведем списки недостающих и лишних:
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CompareDiscountsList " +
                    $"After comparision lists is: " +
                    $"Both={matchInBoth.Count}, " +
                    $"AbsentInQuik={absentInQuik.Count}, " +
                    $"unnecessaryInQuik={securitiesFromQuik.Securitys.Count}");

                message.Body = message.Body + $"<h3>Отсутствует в QUIK шаблоне {templateName} - {absentInQuik.Count} бумаг.</h3><p>";
                foreach (var str in absentInQuik)
                {
                    message.Body = message.Body + $"{str}, ";
                }
                message.Body = message.Body + $"</p>";

                message.Body = message.Body + $"<h3>Лишние в QUIK шаблоне {templateName} - {securitiesFromQuik.Securitys.Count} бумаг.</h3><p>";
                foreach (var str in securitiesFromQuik.Securitys)
                {
                    message.Body = message.Body + $"{str}, ";
                }
                message.Body = message.Body + $"</p>";

                // сравним совпавшие по дисконтам.
                await CompareDiscontListWithActualValues(matrixDiscounts, matchInBoth, templateName, discontType, message, result);
            }
        }

        private async Task CompareDiscontListWithActualValues(
            List<DiscountMatrixModel> matrixDiscounts, //значения дисконтов для инструмента
            List<string> matchInBoth, // список инструментов для проверки
            string templateName, // в каком шаблоне квик сравнивать
            _discontType discontType, // с каким типом дисконтом сверять
            NewEMail message, // сюда результаты
            BoolResponse result) // сюда результаты
        {
            // план 
            // foreach matchInBoth
            // находим его значения в matrixDiscounts и формируем данные для quik
            // получить данные из QUIK из нужного темплейта
            // сравниваем с заданным типом дисконта discontType


            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CompareDiscontListWithActualValues " +
                $"started for {templateName} {discontType}");

            foreach (string tiker in matchInBoth)
            {
                _logger.LogDebug($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CompareDiscontListWithActualValues " +
                    $"work with {templateName} {tiker}");
                //находим его значения в matrixDiscounts и формируем данные для quik
                DiscountMatrixModel matrixSingleDiscount = matrixDiscounts.Where(disc => disc.Tiker == tiker).FirstOrDefault();

                _logger.LogDebug($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CompareDiscontListWithActualValues " +
                    $"finded in matrix list {matrixSingleDiscount.Tiker} {matrixSingleDiscount.Discount}");

                CalculateEthalonDiscounts(matrixSingleDiscount);

                // получить данные из QUIK из нужного темплейта
                DiscountSingleResponse quikDiscount = null;
                if (templateName == "global")
                {
                    //получим информацию из Quik global
                    quikDiscount = await _repoQuik.GetDiscountSingleFromGlobal(tiker);
                }
                else
                {
                    // из шаблона 
                    quikDiscount = await _repoQuik.GetDiscountSingleFromMarginTemplate(templateName, tiker);
                }

                // сравниваем с заданным типом дисконта discontType
                if (!quikDiscount.IsSuccess)//при ошибке - записать инфу,
                {

                }
                else // сравниваем 
                {

                }
            }
        }

        private async Task SendMessageFinalize(NewEMail message)
        {
            message.Body = message.Body + "</body></html>";

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts SendMessageFinalize try to Send message");
            await _sender.Send(message);

        }

        private async Task DeleteSingleDiscountFromTemplate(string template, string security, BoolResponse result)
        {
            //удаляем из глобал
            ListStringResponseModel deleteFromTemplate= await _repoQuik.DeleteDiscountFromTemplate(template, security);
            if (deleteFromTemplate.IsSuccess)
            {
                result.Messages.Add($"Delete {security} from template {template} is success.");
            }
            else // ошибка
            {
                if (deleteFromTemplate.Messages[0].EndsWith("1004 Данные не найдены"))
                {
                    result.Messages.Add($"Delete {security} from template {template} - discount not found. Nothing to delete.");
                }
                else
                {
                    // ошибка в запросе
                    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts DeleteSingleDiscount " +
                        $"{security} from template {template} - Error {deleteFromTemplate.Messages[0]}");

                    result.Messages.AddRange(deleteFromTemplate.Messages);

                    result.IsTrue = false;
                    result.IsSuccess = false;
                }
            }
        }

        private async Task<DiscountMatrixSingleResponse> GetDiscountValueFromMatrix(string security)
        {
            DiscountMatrixSingleResponse matrixDiscount = null;

            if (!security.EndsWith("*")) // если фондовый или валютный
            {
                matrixDiscount = await _repoMatrix.GetDiscountSingle(security);
            }
            else // если срочка
            {
                matrixDiscount = await _repoMatrix.GetDiscountSingleForts(security);
            }

            return matrixDiscount;
        }

        private void CalculateEthalonDiscounts(DiscountMatrixModel discount)
        {
            _ksur = CalculateDiscountKSUR(discount);
            _kpur = CalculateDiscountKPUR(discount);
            _nolvrg = new DiscountModel
            {
                DLong       = 1,
                DShort      = -1,
                DLong_min   = -1,
                DShort_min  = -1,
                KLong       = 1,
                KShort      = 1
            };

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CalculateEthalonDiscounts " +
                $"matrix discounts for {discount.Tiker} is " +
                $"KSUR {_ksur.ToString()} KPUR {_kpur.ToString()} NoLvrg {_nolvrg.ToString()}");
        }

        private DiscountModel CalculateDiscountKSUR(DiscountMatrixModel discount)
        {
            DiscountModel result = new DiscountModel
            {
                KLong = 1,
                KShort = 1,
            };

            //KSUR d long =((1+0,17)*(1+0,17))-1
            double dLong = ((1 + discount.Discount) * (1 + discount.Discount)) - 1;
            if (dLong > 1)
            {
                dLong = 1;
            }
            result.DLong = Math.Round(dLong, 4, MidpointRounding.AwayFromZero);

            //KSUR d min long = d long / 2
            result.DLong_min = Math.Round(result.DLong / 2, 4, MidpointRounding.AwayFromZero);

            // шорт ?
            if (discount.IsShort)
            {
                result.DShort = result.DLong;
                result.DShort_min = result.DLong_min;
            }
            else
            {
                result.DShort = -1;
                result.DShort_min = -1;
            }

            return result;
        }

        private DiscountModel CalculateDiscountKPUR(DiscountMatrixModel discount)
        {
            DiscountModel result = new DiscountModel
            {
                KLong = 1,
                KShort = 1,
            };

            result.DLong = Math.Round(discount.Discount, 4, MidpointRounding.AwayFromZero);
            result.DLong_min = Math.Round(discount.Discount / 2, 4, MidpointRounding.AwayFromZero);

            // шорт ?
            if (discount.IsShort)
            {
                result.DShort = result.DLong;
                result.DShort_min = result.DLong_min;
            }
            else
            {
                result.DShort = -1;
                result.DShort_min = -1;
            }

            return result;
        }

        private async Task CompareSingleDiscountFromTemplateWithEthalon(string template, string security, DiscountModel? ethalon, BoolResponse result)
        {
            DiscountSingleResponse quikTemplateDiscount = await _repoQuik.GetDiscountSingleFromMarginTemplate(template, security);

            if (quikTemplateDiscount.IsSuccess)
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CompareSingleDiscountFromTemplateWithEthalon " +
                    $"Quik template {template} {security} discount {quikTemplateDiscount.Discount.ToString()}");

                if (_settingsCurrencyArray.Contains(security)) //ввод дисконта по базовой валюте(USD CNY etc) - их не должно быть в темплейтах
                {
                    result.IsTrue = false;
                    result.Messages.Add($"Error = Quik template {template} {security} discount finded. " +
                        $"For base currency it is NOT correct - must be no discount in templates");
                }
                else
                {
                    // сравниваем с эталонами
                    if (quikTemplateDiscount.Discount.Equals(ethalon))
                    {
                        result.Messages.Add($"OK = Quik template {template} {security} discount equal to ethalon. " +
                            $"{quikTemplateDiscount.Discount.ToString()}");
                    }
                    else
                    {
                        result.Messages.Add($"Error = Quik template {template} {security} discount NOT equal to ethalon. " +
                            $"{quikTemplateDiscount.Discount.ToString()}");
                        result.IsTrue = false;
                    }
                }
            }
            else
            {
                if (quikTemplateDiscount.Messages[0].StartsWith("QAS110 Error!"))
                {
                    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CompareSingleDiscountFromTemplateWithEthalon " +
                        $"Quik template {template} {security} discounts not found. ");

                    if (ethalon == null)
                    {
                        // ok, так и должно быть, в матрице тоже нет
                        result.Messages.Add($"OK = Quik template {template} {security} discount not found.");
                    }
                    else
                    {
                        if (_settingsCurrencyArray.Contains(security)) //ввод дисконта по базовой валюте(USD CNY etc) - их не должно быть в темплейтах
                        {
                            result.Messages.Add($"OK = Quik template {template} {security} discount not found. " +
                                $"For base currency it is correct - must be no discount");
                        }
                        else
                        {
                            result.Messages.Add($"Error = Quik template {template} {security} discount not found.");
                            result.IsTrue = false;
                        }
                    }
                }
                else
                {
                    // ошибка в запросе
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CompareSingleDiscountFromTemplateWithEthalon " +
                        $"failed at Quik template {template} {security} request");

                    result.IsTrue = false;
                    result.IsSuccess = false;
                    result.Messages.AddRange(quikTemplateDiscount.Messages);
                }
            }
        }

        private async Task PostSingleDiscountToTemplate(string template, DiscountAndSecurityModel modelToQuik, BoolResponse result)
        {
            if (_settingsCurrencyArray.Contains(modelToQuik.Security)) //ввод дисконта по базовой валюте(USD CNY etc) - их не должно быть в темплейтах
            {
                // запросим, есть ли? если есть - удалим.
                DiscountSingleResponse isExist = await _repoQuik.GetDiscountSingleFromMarginTemplate(template, modelToQuik.Security);
                if (isExist.IsSuccess)
                {
                    // удаляем
                    ListStringResponseModel deleteResult = await _repoQuik.DeleteDiscountFromTemplate(template, modelToQuik.Security);
                    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts PostSingleDiscountToTemplate " +
                        $"Try to delete base currency {modelToQuik.Security} from template {template}. Delete succesful = {deleteResult.IsSuccess}");
                    
                    if (deleteResult.IsSuccess)
                    {
                        result.Messages.Add($"Discount {modelToQuik.Security} deleted from Quik template {template}. " +
                            $"For base currency it is correct - must be no discount");
                    }
                    else
                    {
                        result.IsTrue = false;
                        result.IsSuccess = false;
                        result.Messages.Add($"Error at delete Discount {modelToQuik.Security} from Quik template {template}. " +
                            $"For base currency - must be no discount.");
                        result.Messages.AddRange(deleteResult.Messages);
                    }
                }
                else
                {
                    result.Messages.Add($"Discount {modelToQuik.Security} NOT set to Quik templates. For base currency it is correct - must be no discount");
                }
            }
            else
            {
                ListStringResponseModel postResult = await _repoQuik.PostSingleDiscountToTemplate(template, modelToQuik);

                if (postResult.IsSuccess)
                {
                    result.Messages.Add($"Discount set to Quik template {template} {modelToQuik.Security} - {modelToQuik.Discount.ToString()}.");

                    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts PostSingleDiscountToTemplate " +
                        $"Quik template {template} {modelToQuik.Security} discount {modelToQuik.Discount.ToString()} Successful");
                }
                else
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts PostSingleDiscountToTemplate " +
                        $"Quik template {template} {modelToQuik.Security} discount {modelToQuik.Discount.ToString()} Error! {postResult.Messages[0]}");

                    result.IsTrue = false;
                    result.IsSuccess = false;
                    result.Messages.AddRange(postResult.Messages);
                }
            }
        }
    }
}
