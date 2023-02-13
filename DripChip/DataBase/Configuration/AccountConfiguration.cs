using DripChip.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DripChip.DataBase.Configuration
{
    public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasData(
                new Account { Id = 1, FirstName = "Егор", LastName = "Антюхин", Email = "danchin276@mail.ru", Password = "09082015H" },
                new Account { Id = 2, FirstName = "Антон", LastName = "Шашков", Email = "dranik228@gmail.com", Password = "dragdiller228" },
                new Account { Id = 3, FirstName = "Никита", LastName = "Глушаков", Email = "glushn1k@mail.ru", Password = "666666" }
                );
        }
    }
}