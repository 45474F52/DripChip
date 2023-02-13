using System.Text.Json.Serialization;

namespace DripChip.Models
{
    public partial class Account
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [JsonIgnore]
        public string Password { get; set; } = string.Empty;

        public bool Contains(Account? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            bool isContains = true;

            foreach (var thisProp in this.GetType().GetProperties())
            {
                if (thisProp.Name != nameof(Id))
                {
                    foreach (var otherProp in other.GetType().GetProperties())
                    {
                        if (thisProp.Name == otherProp.Name)
                        {
                            if (thisProp.GetValue(this)!.ToString()!.ToLower().Contains(otherProp.GetValue(other)!.ToString()!.ToLower()))
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

        public override string ToString() =>
            $"Id: {Id},\nFirstName: {FirstName}|,\nLastName: {LastName},\nEmail: {Email},nPassword: {Password}\n";
    }
}