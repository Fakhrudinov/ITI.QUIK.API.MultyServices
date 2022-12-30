namespace DataAbstraction.Connections
{
    public class SMTPMailConfig
    {
        public string SMTPHost { get; set; }
        public int SMTPPort { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }

        public string MainReciever { get; set; }

        public string SenderEmail { get; set; }
        public string SenderEmailAlias { get; set; }

        public string [] CCReceiversEmail { get; set; }

        public string ServerType { get; set; } = "Undefined";
    }
}
