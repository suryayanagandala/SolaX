using DbAccessaLayer.Infrastructure;
using SolarXNUnit.SqLite.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SolarXNUnit.SqLite.Infrstructure
{
    public class RoleRepositoryTest:SqliteTestBase
    {
        private RoleRepository roleRepository;

        [SetUp]
        public void SetUp()
        {
            roleRepository = new RoleRepository(_db);
        }

        [Test]
        public async Task GetAllRoles()
        {
            var result=await roleRepository.GetAllRoles();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetUsersBaedOnRole_Success()
        {
            string roleName = "Admin";
            var result= roleRepository.GetUsersFromUser(roleName);
            Assert.IsNotNull(result);   
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void GetUsersFromRoles_Failure()
        {
            string roleName = "fakerole";

            var result = roleRepository.GetUsersFromUser(roleName);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }
        [Test]
        public void GetUsersFromUser_NullRole_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                roleRepository.GetUsersFromUser(null));

            Assert.That(ex.Message, Is.EqualTo("Role name is required"));
        }
        [Test]
        public void GetUsersFromUser_InvalidRole_ThrowsException()
        {
            string roleName = "fakerole";

            var ex = Assert.Throws<Exception>(() =>
                roleRepository.GetUsersFromUser(roleName));

            Assert.That(ex.Message, Is.EqualTo("No users found for this role"));
        }

    }
}
