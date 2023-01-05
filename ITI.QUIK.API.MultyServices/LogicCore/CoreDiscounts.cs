using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Models.Discounts;
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

        public CoreDiscounts(
            ILogger<CoreDiscounts> logger,
            IHttpMatrixRepository repoMatrix,
            IHttpQuikRepository repoQuik,
            IOptions<DiscountsSettings> settings,
            IOptions<LimLImCreationSettings> settingsCurrency
            )
        {
            _logger = logger;
            _repoMatrix = repoMatrix;
            _repoQuik = repoQuik;
            _settings = settings.Value;
            _settingsCurrencyArray = settingsCurrency.Value.PositionAsMoneyArray;
        }

        public async Task<BoolResponse> CheckSingleDiscount(string security)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CheckSingleDiscount Called {security}");
            BoolResponse result = new BoolResponse 
            {
                IsTrue = true
            };
            // эталонные дисконты
            DiscountModel ksur = null;
            DiscountModel kpur = null;
            DiscountModel nolvrg = null;

            /// plan
            // получим информацию из матрицы
            //  считаем по ней эталонные дисконты КСУР, КПУР и NoLvrg
            //      если null и ошибка = "Не найдено данных по дисконтам ***"
            //          - то смотрим чтобы данных не было и в QUIK, эталонные дисконты = null 
            //      если null и другое - прервать!
            // получим информацию из Quik global, сравниваем
            // получим информацию из Quik templates, сравниваем. (ввод дисконта по базовой валюте (USD CNY etc) - их не должно быть в темплейтах)

            // To do !! - - -  - - - -  - - - -  - - - -  - - - -  - - - -  - - - -  - - - -  - - - -  - - - -  - - - -  - - - -  - - - -  - 
            // validation security

            // получим информацию из матрицы
            DiscountMatrixSingleResponse matrixDiscount = null;
            if (!security.EndsWith("*")) // если фондовый или валютный
            {
                matrixDiscount = await _repoMatrix.GetDiscountSingle(security);
            }
            else // если срочка
            {
                matrixDiscount = await _repoMatrix.GetDiscountSingleForts(security);
            }

            if (matrixDiscount.IsSuccess) // считаем эталонные дисконты
            {
                ksur = CalculateDiscountKSUR(matrixDiscount.Discount);
                kpur = CalculateDiscountKPUR(matrixDiscount.Discount);
                nolvrg = new DiscountModel
                {
                    DLong       = 1,
                    DShort      = -1,
                    DLong_min   = -1,
                    DShort_min  = -1,
                    KLong       = 1,
                    KShort      = 1
                };

                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CheckSingleDiscount " +
                    $"{security} matrix discounts is " +
                    $"KSUR {ksur.ToString()} KPUR {kpur.ToString()} NoLvrg {nolvrg.ToString()}");

                result.Messages.Add($"Matrix: " +
                    $"KSUR {ksur.ToString()} KPUR {kpur.ToString()} NoLvrg {nolvrg.ToString()}");
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
                if (quikGlobalDiscount.Discount.Equals(ksur))
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

                    if (ksur == null)
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
                    await CompareSingleDiscountFromTemplateWithEthalon(template, security, ksur, result);
                }
            }
            //получим информацию из Quik templates KPUR
            if (_settings.TemlpatesArrayKPUR is not null)
            {
                foreach (string template in _settings.TemlpatesArrayKPUR)
                {
                    await CompareSingleDiscountFromTemplateWithEthalon(template, security, kpur, result);
                }
            }
            //получим информацию из Quik templates NoLvrg
            if (_settings.TemlpatesArrayNoLeverage is not null)
            {
                foreach (string template in _settings.TemlpatesArrayNoLeverage)
                {
                    await CompareSingleDiscountFromTemplateWithEthalon(template, security, nolvrg, result);
                }
            }

            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} CoreDiscounts CheckSingleDiscount {security} " +
                $"IsSuccess={result.IsSuccess} IsTrue={result.IsTrue}");
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

                    result.Messages.AddRange(quikTemplateDiscount.Messages);
                }
            }
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
    }
}
