namespace DataAbstraction.Models
{
    public class QuikQAdminClientModel
    {
        public bool IsBlocked { get; set; }
        public int UID { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }

        public string? Comment { get; set; }

        public List<string> ClientCodes { get; set; } = new List<string>();

        public List<PubringKeyModel> Keys { get; set; }
    }
}
