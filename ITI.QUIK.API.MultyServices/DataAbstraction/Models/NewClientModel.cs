namespace DataAbstraction.Models
{
    public class NewClientModel
    {
        public bool isEDP { get; set; } = false;

        public ClientInformationModel Client { get; set; }

        public bool isClientPerson { get; set; }//P //O Тип клиента. «P» для физического лица, «O» для юридического лица, «?» для прочих типов.
        public bool isClientResident { get; set; }//N //R Тип клиента(резидент/нерезидент). Принимает значения «R» для резидентов, «N» для нерезидентов.
        public string Address { get; set; } //A35E3P1, Респ. Казахстан, г. Алматы, ул. Баймагамбетова, д. 206
        public int RegisterDate { get; set; }//20160714 Дата заключения договора.Формат: ГГГГММДД. 

        public MatrixClientCodeModel[]? CodesMatrix { get; set; }// can be null if MatrixToFortsCodesMappingModel exist
        public MatrixToFortsCodesMappingModel[]? CodesPairRF { get; set; }// can be null if MatrixClientCodeModel exist

        public PubringKeyModel Key { get; set; }

        public string? Manager { get; set; } = null;//NULL Содержит имя менеджера, соответствующего данному договору обслуживания.
        //public string? Number { get; set; } //Дог.BP17178  Номер договора.
        public string SubAccount { get; set; } = "";//   //SubAccount: Субсчёт(пустая строка при отсутствии субсчёта).
        public string Depositary { get; set; } = "НДЦ";//НДЦ Депозитарий
        public bool isClientDepo { get; set; } = false;//0  Тип договора: 0 – договор обслуживания, 1 – депозитарный договор.
        public string DepoClientAccountsManager { get; set; } = "ITinvest"; //ITinvest Распорядитель.
    }
}
