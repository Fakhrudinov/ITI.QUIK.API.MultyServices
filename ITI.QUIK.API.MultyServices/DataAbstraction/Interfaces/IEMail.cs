using DataAbstraction.Models.EMail;

namespace DataAbstraction.Interfaces
{
    public interface IEMail
    {
        public Task Send(NewEMail message);
    }
}
