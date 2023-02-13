using DripChip.Models;

namespace DripChip.DataBase.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAll();
        Account? Get(int id);
        Task Create(Account account);
        Task Update(Account account);
        Task<Account> Delete(int id);
    }
}