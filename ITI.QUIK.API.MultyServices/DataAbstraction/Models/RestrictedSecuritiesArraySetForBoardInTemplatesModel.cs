namespace DataAbstraction.Models
{
    public class RestrictedSecuritiesArraySetForBoardInTemplatesModel
    {
        public string TemplateName { get; set; }
        public string SecBoard { get; set; }
        public List<string> Securities { get; set; } = new List<string>();
    }
}
