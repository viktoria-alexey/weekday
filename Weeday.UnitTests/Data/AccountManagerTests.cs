using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Weekday.Data;
using Weekday.Data.Core;
using Weekday.Data.Models;
using Xunit;

namespace Weeday.UnitTests
{
    public class TestIdentityResult:IdentityResult
    {
        public TestIdentityResult()
        {
            Succeeded = true;
        }
    }
    public class AccountManagerTests
    {
        private Mock<ApplicationDbContext> _context;
        private Mock<UserManager<ApplicationUser>> _userManager;
        private Mock<RoleManager<IdentityRole>> _roleManager;
        public AccountManagerTests()
        {
            _context = new Mock<ApplicationDbContext>();
            _userManager = new Mock<UserManager<ApplicationUser>>();
            _roleManager = new Mock<RoleManager<IdentityRole>>();
            
        }
        [Fact]
        public async Task CreateRoleAsync_Success()
        {
            // arrange
            _roleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(new TestIdentityResult());
            var target = new AccountManager(_context.Object, _userManager.Object, _roleManager.Object);

            // act
            var result = await target.CreateRoleAsync(It.IsAny<IdentityRole>());
            
            // assert
            result.Succeeded.Should().BeTrue();
        }
    }
}