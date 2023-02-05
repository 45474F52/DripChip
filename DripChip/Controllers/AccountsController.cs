using DripChip.Core.Serialization;
using DripChip.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace DripChip.Controllers
{
    [ApiController]
    [Route("Accounts")]
    public sealed class AccountsController : BaseController<Account>
    {
        protected internal override string CurrentPath => "DataBase/Accounts.json";

        private IAsyncSerializer<IEnumerable<Account>>? _serializer;
        protected internal override IAsyncSerializer<IEnumerable<Account>> Serializer
        {
            get => _serializer ?? throw new NullReferenceException("Сериализатор не был создан");
            set => _serializer = value;
        }

        public AccountsController()
        {
            Serializer = new JsonAsyncSerializer<IEnumerable<Account>>(new System.Text.Json.JsonSerializerOptions()
            { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) })
            { Path = Path.Combine(Environment.CurrentDirectory, CurrentPath) };

            InitializeEntities();
            foreach (var account in Entities)
            {
                account.InitializeFilterModel();
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Account>> GetAccounts() => Get();

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccount(int? id)
        {
            if (id == null || id <= 0)
                return BadRequest();

            Account? account = Entities.FirstOrDefault(a => a.Id == id);
            if (account == null)
                return NotFound();

            return Ok(account);
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<AccountFilterModel>> SearchAccounts(string firstName, string lastName, string email, int? from, int? size)
        {
            if (Entities == null)
                return Unauthorized();

            if (from < 0 || from == null || size <= 0 || size == null)
                return BadRequest();

            AccountFilterModel sendedFilterModel = new()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email
            };

            IEnumerable<AccountFilterModel> filterModels = new List<AccountFilterModel>();

            foreach (var account in Entities)
            {
                if (account.Model.Contains(sendedFilterModel))
                {
                    filterModels = filterModels.Append(account.Model);
                }
            }

            filterModels = filterModels.Skip(from ?? 0);
            filterModels = filterModels.Take(size ?? 100);
            filterModels = filterModels.OrderBy(a => a.Id);

            return Ok(filterModels);
        }

        #region NotImplemented
        /*
        [HttpPost("{Account}")]
        [ActionName("Authentication")]
        public async Task<ActionResult<Account>> PostAsync([FromBody] Account account)
        {
            account.Id = SetNewId();
            Accounts.Add(account);
            await _serializer.OverwriteFileAsync(Accounts);
            return Ok(account);
        }

        [HttpPut("{Account}")]
        [ActionName("UpdateAccount")]
        public async Task<ActionResult<Account>> PutAsync([FromBody] Account accountData)
        {
            int listId = Accounts.IndexOf(accountData);
            if (listId != -1)
            {
                Accounts[listId] = accountData;
                await _serializer.OverwriteFileAsync(Accounts);
                return Ok(accountData);
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        [ActionName("Delete")]
        public async Task<ActionResult<Account>> DeleteAsync(int id)
        {
            Account? account = Accounts.FirstOrDefault(a => a.Id == id);
            if (account != null)
            {
                int listId = Accounts.IndexOf(account);
                if (listId != -1)
                {
                    Accounts.RemoveAt(listId);
                    await _serializer.OverwriteFileAsync(Accounts);
                    return Ok(account);
                }
            }

            return NotFound();
        }
        */
        #endregion NotImplemented

        private int SetNewId() => Entities.Select(x => x.Id).Max() + 1;
    }
}