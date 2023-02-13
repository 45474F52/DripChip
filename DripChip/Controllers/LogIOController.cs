using DripChip.Models;
using DripChip.Core.Helper;
using DripChip.DataBase.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using System.ComponentModel.DataAnnotations;

namespace DripChip.Controllers
{
    [ApiController]
    [Route("/registration")]
    public class LogIOController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly List<Account> _accounts;

        public LogIOController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _accounts = _accountRepository.GetAll().ToList();
        }

        [HttpPost]
        public async Task<ActionResult<Account>> RegisterAccount([FromBody] JsonObject data)
        {
            if (User.Identity!.IsAuthenticated)
                return Forbid();

            string? firstName = JsonObjectParser<string>.Parse(data, nameof(firstName));
            string? lastName = JsonObjectParser<string>.Parse(data, nameof(lastName));
            string? email = JsonObjectParser<string>.Parse(data, nameof(email));
            string? password = JsonObjectParser<string>.Parse(data, nameof(password));

            if (ValidateRequestDatas(firstName, lastName, email, password))
            {
                if (_accounts != null)
                {
                    Account? account = _accounts.FirstOrDefault(a => a.Email.ToLower() == email!.ToLower());
                    if (account == null)
                    {
                        account = new()
                        {
                            Id = SetNewId(),
                            FirstName = firstName!,
                            LastName = lastName!,
                            Email = email!,
                            Password = password!
                        };
                        _accounts.Add(account);
                        await _accountRepository.Create(account);
                        
                        return CreatedAtAction(nameof(RegisterAccount), account);
                    }

                    return Conflict();
                }
            }

            return BadRequest();
        }

        private int SetNewId() => _accounts.Select(x => x.Id).Max() + 1;
        private static bool ValidateRequestDatas(string? fName, string? lName, string? email, string? password)
        {
            var emailChecker = new EmailAddressAttribute();
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