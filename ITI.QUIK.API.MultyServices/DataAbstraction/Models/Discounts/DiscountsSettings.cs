namespace DataAbstraction.Models.Discounts
{
    public class DiscountsSettings
    {
        public string[] CheckMarketsArray { get; set; }//какие рынки получать из матрицы для полного сравнения.
        public string[] TemlpatesArrayKSUR { get; set; } // в каких шаблонах Quik искать дисконты КСУР
        public string[] TemlpatesArrayKPUR { get; set; } // в каких шаблонах Quik искать дисконты КПУР
        public string[] TemlpatesArrayNoLeverage { get; set; } // в каких шаблонах Quik искать дисконты "без плечей"
    }
}
