using DripChip.Models;
using DripChip.Core.Helper;
using DripChip.DataBase.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Nodes;
using System.ComponentModel.DataAnnotations;

namespace DripChip.Controllers
{
    [ApiController]
    [Route("Accounts")]
    public sealed class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly List<Account> _accounts;

        public AccountsController(IAccountRepository accountRepository, IAnimalRepository animalRepository)
        {
            _accountRepository = accountRepository;
            _animalRepository = animalRepository;
            _accounts = _accountRepository.GetAll().ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccount(int? id)
        {
            if (id == null || id <= 0)
                return BadRequest();

            Account? account = _accounts.FirstOrDefault(a => a.Id == id);
            return account != null ? Ok(account) : NotFound();
        }

        [Authorize]
        [HttpGet("search")]
        public ActionResult<IEnumerable<Account>> SearchAccounts(
            [FromQuery] string firstName,
            [FromQuery] string lastName,
            [FromQuery] string email,
            [FromQuery] int? from,
            [FromQuery] int? size)
        {
            if (from < 0 || from == null || size <= 0 || size == null)
                return BadRequest();

            Account sendedAccount = new()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email
            };

            IEnumerable<Account> accounts = new List<Account>();

            foreach (var account in _accounts.Where(account => account.Contains(sendedAccount)))
                accounts = accounts.Append(account);

            accounts = accounts.Skip(from ?? 0);
            accounts = accounts.Take(size ?? 100);
            accounts = accounts.OrderBy(a => a.Id);

            return Ok(accounts);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<Account>> UpdateAccount(int? id, [FromBody] JsonObject data)
        {
            string? firstName = JsonObjectParser<string>.Parse(data, nameof(firstName));
            string? lastName = JsonObjectParser<string>.Parse(data, nameof(lastName));
            string? email = JsonObjectParser<string>.Parse(data, nameof(email));
            string? password = JsonObjectParser<string>.Parse(data, nameof(password));

            if (ValidateRequestData(firstName, lastName, email, password))
            {
                if (_accounts.FirstOrDefault(a => a.Email.ToLower() == email!.ToLower()) == null)
                {
                    Account? account = _accounts.FirstOrDefault(a => a.Id == id);
                    if (account != null)
                    {
                        int index = _accounts.IndexOf(account);

                        _accounts.ElementAt(index).FirstName = firstName!;
                        _accounts.ElementAt(index).LastName = lastName!;
                        _accounts.ElementAt(index).Email = email!;
                        _accounts.ElementAt(index).Password = password!;

                        await _accountRepository.Update(_accounts.ElementAt(index));
                        return Ok(_accounts.ElementAt(index));
                    }

                    return Forbid();
                }

                return Conflict();
            }

            return BadRequest();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAccount(int? id)
        {
            if (id != null && id > 0)
            {
                if (!IsAnimalsUsed(id))
                {
                    Account? account = _accounts.FirstOrDefault(a => a.Id == id);
                    if (account != null)
                    {
                        _accounts.Remove(account);
                        await _accountRepository.Delete((int)id);
                        return Ok();
                    }

                    return Forbid();
                }
            }

            return BadRequest();
        }

        private bool IsAnimalsUsed(int? id)
        {
            if (id != null)
            {
                IEnumerable<Animal>? animals = _animalRepository.GetAll();

                if (animals != null)
                    return animals.FirstOrDefault(a => a.ChipperId == id) != null;
            }

            return false;
        }

        private static bool ValidateRequestData(string? fName, string? lName, string? email, string? password)
        {
            EmailAddressAttribute emailChecker = new();

            if (!string.IsNullOrWhiteSpace(fName) &&
                !string.IsNullOrWhiteSpace(lName) &&
                !string.IsNullOrWhiteSpace(email) &&
                !string.IsNullOrWhiteSpace(password))
            {
                if (emailChecker.IsValid(email))
                    return true;
            }

            return false;
        }
    }
}