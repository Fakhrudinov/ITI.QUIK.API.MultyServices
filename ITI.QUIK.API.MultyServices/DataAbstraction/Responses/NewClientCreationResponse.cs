using DataAbstraction.Models;

namespace DataAbstraction.Responses
{
    public class NewClientCreationResponse
    {
        public NewClientModel NewClient { get; set; } = new NewClientModel();

        public bool IsNewClientCreationSuccess { get; set; } = false;
        public List<string> NewClientCreationMessages { get; set; } = new List<string>();

        public bool IsSftpUploadSuccess { get; set; } = false;
        public List<string> SftpUploadMessages { get; set; } = new List<string>();

        public bool IsCodesIniSuccess { get; set; } = false;
        public List<string> CodesIniMessages { get; set; } = new List<string>();

        public bool IsInstrTwSuccess { get; set; } = false;
        public List<string> InstrTwMessages { get; set; } = new List<string>();

        public bool IsAddToTemplatesSuccess { get; set; } = false;
        public List<string> AddToTemplatesMessages { get; set; } = new List<string>();
    }
}
