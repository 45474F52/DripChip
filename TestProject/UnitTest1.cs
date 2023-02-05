using DripChip.Models;

namespace TestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestFilterModelsIsEqulas()
        {
            Account account2 = new()
            {
                Id = 2,
                FirstName = "FName",
                LastName = "LName",
                Email = "Email"
            };
            account2.InitializeFilterModel();

            Account account1 = new()
            {
                Id = 1,
                FirstName = "FName",
                LastName = "LName",
                Email = "Email"
            };
            account1.InitializeFilterModel();

            Account account3 = new()
            {
                Id = 3,
                FirstName = "FName",
                LastName = "LName",
                Email = "Email"
            };
            account3.InitializeFilterModel();

            Account account4 = new()
            {
                Id = 4,
                FirstName = "FName3",
                LastName = "LName3",
                Email = "Email3"
            };
            account4.InitializeFilterModel();

            Assert.IsTrue(account2.Model == account1.Model);
            Assert.IsFalse(account2.Model != account3.Model);
            Assert.IsTrue(account1.Model != account4.Model);
            Assert.IsTrue(account1.Model.Equals(account3.Model));
        }

        [TestMethod]
        public void TestFilterModelContains()
        {
            Account account = new()
            {
                Id = 0,
                FirstName = "FName",
                LastName = "LName",
                Email = "Email"
            };
            account.InitializeFilterModel();

            Account account1 = new()
            {
                Id = 1,
                FirstName = "FName",
                LastName = "LName",
                Email = "Email"
            };
            account1.InitializeFilterModel();

            Account account2 = new()
            {
                Id = 2,
                FirstName = "FName",
                LastName = "LName",
                Email = "Email"
            };
            account2.InitializeFilterModel();

            Account account3 = new()
            {
                Id = 3,
                FirstName = "FName3",
                LastName = "LName3",
                Email = "Email3"
            };
            account3.InitializeFilterModel();

            Assert.IsTrue(account1.Model.Contains(account2.Model));
            Assert.IsFalse(account2.Model.Contains(account3.Model));
            Assert.IsTrue(account3.Model.Contains(account.Model));
        }
    }
}