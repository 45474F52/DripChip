using DripChip.Controllers;
using DripChip.Models;

namespace TestProject
{
    [TestClass]
    public class AccountsTests
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

    [TestClass]
    public class LocationPointsTests
    {
        [TestMethod]
        public void TestPointUsed()
        {
            long id1 = 2;
            long id2 = 3;
            long id3 = 11;

            long[] vislocs1 = new long[5] { 1, 2, 4, 5, 8 };
            long[] vislocs2 = new long[2] { 2, 5 };
            long[] vislocs3 = new long[6] { 1, 2, 3, 4, 5, 6 };

            IEnumerable<Animal> animals = new List<Animal>()
            {
                new Animal() { VisitedLocations = vislocs1 },
                new Animal() { VisitedLocations = vislocs2 },
                new Animal() { VisitedLocations = vislocs2 },
                new Animal() { VisitedLocations = vislocs3 },
                new Animal() { VisitedLocations = new long[3] { 7, 9, 10 } }
            };

            Assert.IsFalse(LocationsController.PointNotUsed(id1, animals));
            Assert.IsFalse(LocationsController.PointNotUsed(id2, animals));
            Assert.IsTrue(LocationsController.PointNotUsed(id3, animals));
        }
    }
}