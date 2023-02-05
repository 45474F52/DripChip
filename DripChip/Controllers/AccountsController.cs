using DripChip.Core.Serialization;
using DripChip.DataBase;
using DripChip.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DripChip.Controllers
{
    [ApiController]
    [Route("Accounts")]
    public sealed class AccountsController : BaseController<Account>
    {
        protected internal override string PathToCurrentEntities => "DataBase/Accounts.json";

        private IAsyncSerializer<IEnumerable<Account>>? _serializer;
        protected internal override IAsyncSerializer<IEnumerable<Account>> Serializer
        {
            get => _serializer ?? throw new NullReferenceException("Сериализатор не был создан");
            set => _serializer = value;
        }

        public AccountsController()
        {
            Serializer = new JsonAsyncSerializer<IEnumerable<Account>>()
            { Path = Path.Combine(Environment.CurrentDirectory, PathToCurrentEntities) };

            InitializeEntities();
            foreach (var account in Entities)
                account.InitializeFilterModel();
        }

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
        public ActionResult<IEnumerable<AccountFilterModel>> SearchAccounts(
            [FromQuery] string firstName,
            [FromQuery] string lastName,
            [FromQuery] string email,
            [FromQuery] int? from,
            [FromQuery] int? size)
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

            foreach (var account in Entities.Where(account => account.Model.Contains(sendedFilterModel)))
            {
                filterModels = filterModels.Append(account.Model);
            }

            filterModels = filterModels.Skip(from ?? 0);
            filterModels = filterModels.Take(size ?? 100);
            filterModels = filterModels.OrderBy(a => a.Id);

            return Ok(filterModels);
        }

        [HttpPost("registration")]
        public async Task<ActionResult<Account>> RegistrationAccount(string? fName, string? lName, string? email, string? password)
        {
            if (ValidateRequestDatas(fName, lName, email, password))
            {
                IEnumerable<Account>? accounts = await new GetEntities<Account>(Serializer).ReceiveEnumerable();
                if (accounts != null)
                {
                    Account? account = accounts.FirstOrDefault(a => a.Email.ToLower() == email!.ToLower());

                    if (account == null)
                    {
                        account = new()
                        {
                            Id = SetNewId(),
                            FirstName = fName!,
                            LastName = lName!,
                            Email = email!,
                            Password = password!
                        };
                        account.InitializeFilterModel();
                        //Сохранить новый аккаунт в файл
                        return Ok(account.Model);
                    }

                    return Conflict();
                }
            }

            return BadRequest();
        }

        private static bool ValidateRequestDatas(string? fName, string? lName, string? email, string? password)
        {
            var emailChecker = new EmailAddressAttribute();
            if (!string.IsNullOrWhiteSpace(fName) &&
                !string.IsNullOrWhiteSpace(lName) &&
                !string.IsNullOrWhiteSpace(email) &&
                !string.IsNullOrWhiteSpace(password))
            {
                if (emailChecker.IsValid(email))
                {
                    return true;
                }
            }

            return false;
        }
        /*
        [HttpPut]
        public async Task<ActionResult<Account>> UpdateAccount(int? id, string? fName, string? lName, string? email, string? password)
        {

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Account>> DeleteAccount(int? id)
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
        private int SetNewId() => Entities.Select(x => x.Id).Max() + 1;
    }
}