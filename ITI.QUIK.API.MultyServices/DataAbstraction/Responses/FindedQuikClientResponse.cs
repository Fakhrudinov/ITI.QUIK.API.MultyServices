using DataAbstraction.Models;
using DataAbstraction.Models.InstrTw;

namespace DataAbstraction.Responses
{
    public class FindedQuikClientResponse
    {
        public bool IsSuccess { get; set; } = true;
        public List<string> Messages { get; set; } = new List<string>();


        public InstrTWDataBaseRecords ? InstrTWDBRecords { get; set; }
        public List<string> InstrTWDBRecordsMessages { get; set; } = new List<string>();


        public MatrixClientPortfolioModel[]? MatrixClientPortfolios { get; set; }
        public List<string> MatrixClientPortfoliosMessages { get; set; } = new List<string>();


        public MatrixToFortsCodesMappingModel[]? CodesPairRF { get; set; }
        public List<string> CodesPairRFMessages { get; set; } = new List<string>();


        public List<QuikQAdminClientModel>? QuikQAdminClient { get; set; }
        public List<string> QuikQAdminClientMessages { get; set; } = new List<string>();
    }
}
