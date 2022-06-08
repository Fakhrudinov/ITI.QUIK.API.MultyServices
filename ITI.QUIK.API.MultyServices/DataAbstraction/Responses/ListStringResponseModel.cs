namespace DataAbstraction.Responses
{
    public class ListStringResponseModel
    {
        public ListStringResponseModel()
        {
            IsSuccess = true;
            Messages = new List<string>();
        }
        public bool IsSuccess { get; set; }
        public List<string> Messages { get; set; }
    }
}
