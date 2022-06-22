namespace DataAbstraction.Models
{
    public class ClientBOInformationModel
    {
        public bool isClientPerson { get; set; }//P //O Тип клиента. «P» для физического лица, «O» для юридического лица, «?» для прочих типов.
        public bool isClientResident { get; set; }//N //R Тип клиента(резидент/нерезидент). Принимает значения «R» для резидентов, «N» для нерезидентов.
        public string Address { get; set; } //A35E3P1, Респ. Казахстан, г. Алматы, ул. Баймагамбетова, д. 206
        public int RegisterDate { get; set; }//20160714 Дата заключения договора.Формат: ГГГГММДД. 
    }
}
