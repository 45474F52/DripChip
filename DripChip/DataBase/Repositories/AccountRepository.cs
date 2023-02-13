using DripChip.Models;

namespace DripChip.DataBase.Repositories
{
    public sealed class AccountRepository : IAccountRepository
    {
        private readonly DataContext _context;

        public AccountRepository(DataContext context) => _context = context;

        public IEnumerable<Account> GetAll() => _context.Accounts;

        public Account? Get(int id) => _context.Accounts.Find(id);

        public async Task Create(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task<Account> Delete(int id)
        {
            Account current = Get(id)!;
            _context.Accounts.Remove(current);
            await _context.SaveChangesAsync();
            return current;
        }

        public async Task Update(Account account)
        {
            Account current = Get(account.Id)!;

            current.FirstName = account.FirstName;
            current.LastName = account.LastName;
            current.Email = account.Email;
            current.Password = account.Password;

            _context.Accounts.Update(current);
            await _context.SaveChangesAsync();
        }
    }
}