namespace DataAbstraction.Interfaces
{
    public interface IResponseDirect
    {
        public bool IsSuccess { get; set; }
        public List<string> Messages { get; set; }
    }
}
