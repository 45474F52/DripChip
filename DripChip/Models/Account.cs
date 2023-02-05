using System.Text.Json.Serialization;

namespace DripChip.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        [JsonIgnore]
        public AccountFilterModel Model { get; private set; } = new AccountFilterModel();

        public void InitializeFilterModel()
        {
            Model = new()
            {
                Id = this.Id,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email
            };
        }

        public override string ToString() =>
            $"Id: {Id},\nFirstName: {FirstName}|,\nLastName: {LastName},\nEmail: {Email},nPassword: {Password}\n";
    }

    public sealed class AccountFilterModel
    {
        public int Id { get; set; } = -1;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public bool Contains(AccountFilterModel? fm)
        {
            if (fm is null)
                return false;

            if (ReferenceEquals(this, fm))
                return true;

            bool isContains = true;

            foreach (var fm1props in this.GetType().GetProperties())
            {
                if (fm1props.Name != nameof(Id))
                {
                    foreach (var fm2props in fm.GetType().GetProperties())
                    {
                        if (fm1props.Name == fm2props.Name)
                        {
                            if (fm1props.GetValue(this)!.ToString()!.ToLower().Contains(fm2props.GetValue(fm)!.ToString()!.ToLower()))
                            {
                                isContains = true;
                                break;
                            }
                            else
                                return false;
                        }
                        else
                            continue;
                    }
                }
                else
                    continue;                
            }

            return isContains;
        }

        public override bool Equals(object? obj)
        {
            if (obj is AccountFilterModel filterModel)
                return this == filterModel;
            else
                return false;
        }

        public static bool operator ==(AccountFilterModel? fm1, AccountFilterModel? fm2)
        {
            if (fm1 is null || fm2 is null)
                return false;

            if (ReferenceEquals(fm1, fm2))
                return true;

            bool isEqulas = true;

            foreach (var fm1props in fm1.GetType().GetProperties())
            {
                foreach (var fm2props in fm2.GetType().GetProperties())
                {
                    if (fm1props.Name == fm2props.Name && fm1props.Name != nameof(Id))
                    {
                        if (fm1props.GetValue(fm1)!.Equals(fm2props.GetValue(fm2)))
                        {
                            isEqulas = true;
                            break;
                        }
                        else
                            return false;
                    }
                    else
                        continue;
                }
            }

            return isEqulas;
        }

        public static bool operator !=(AccountFilterModel? fm1, AccountFilterModel? fm2)
        {
            if (fm1 is null || fm2 is null)
                return false;

            if (ReferenceEquals(fm1, fm2))
                return false;

            bool isNotEqulas = false;

            foreach (var fm1props in fm1.GetType().GetProperties())
            {
                foreach (var fm2props in fm2.GetType().GetProperties())
                {
                    if (fm1props.Name == fm2props.Name)
                    {
                        if (!fm1props.GetValue(fm1)!.Equals(fm2props.GetValue(fm2)))
                        {
                            isNotEqulas = true;
                            break;
                        }
                        else
                            return false;
                    }
                    else
                        continue;
                }
            }

            return isNotEqulas;
        }

        public override int GetHashCode() => GetHashCode();
    }
}