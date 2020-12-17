using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Weekday.Data;
using Weekday.Data.Core;
using Weekday.Data.Models;
using Xunit;

namespace Weekday.UnitTests.Data
{
    public class AccountManagerTests
    {
        private Mock<IApplicationDbContext> _context;

        public AccountManagerTests()
        {
            _context = new Mock<IApplicationDbContext>();
        }

        [Fact]
        public async Task CreateRoleAsync_Success()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            roleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(new TestIdentityResult());
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.CreateRoleAsync(It.IsAny<IdentityRole>());
            
            // assert
            result.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task CreateRoleAsync_Fail()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            roleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(new TestIdentityResult(false));
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.CreateRoleAsync(It.IsAny<IdentityRole>());
            
            // assert
            result.Succeeded.Should().BeFalse();
        }
        
        [Fact]
        public async Task DeleteUserAsync_Success()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());
            userManager.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new TestIdentityResult());

            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.DeleteUserAsync(It.IsAny<string>());
            
            // assert
            result.Succeeded.Should().BeTrue();
        }
        
        [Fact]
        public async Task DeleteUserAsync_NoUser()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.DeleteUserAsync(It.IsAny<string>());
            
            // assert
            result.Succeeded.Should().BeTrue();
        }
        
        [Fact]
        public async Task CheckPasswordAsync_Success()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true);

            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>());
            
            // assert
            result.Should().BeTrue();
        }
        
        [Fact]
        public async Task CheckPasswordAsync_Fail()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(false);

            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>());
            
            // assert
            result.Should().BeFalse();
        }
        
        [Fact]
        public async Task CheckPasswordAsync_FailSupportsUserLockout()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(false);
            userManager.Setup(x => x.SupportsUserLockout).Returns(false);
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>());
            
            // assert
            result.Should().BeFalse();
        }
        
        [Fact]
        public async Task GetUser_Success()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();
            var user = new ApplicationUser();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.GetUserByIdAsync(It.IsAny<string>());
            
            // assert
            result.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task GetUserAndRolesAsync_NoResult()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.GetUserAndRolesAsync(It.IsAny<string>());
            
            // assert
            result.Should().BeNull();
        }
        
        [Fact]
        public async Task GetUserAndRolesAsync_Success()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();
           
            var user = new ApplicationUser();
            _context.Setup(x => x.GetUserById(It.IsAny<string>())).ReturnsAsync(user);
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.GetUserAndRolesAsync(It.IsAny<string>());
            
            // assert
            result.Value.User.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task GetUsersAndRolesAsync_Success()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();
           
            var user = new ApplicationUser
            {
                Id = "1",
                Roles = new List<IdentityUserRole<string>>
                {
                    new IdentityUserRole<string>
                    {
                        RoleId = "2",
                        UserId = "1"
                    }
                }
            };
 
            _context.Setup(x => x.GetRoleFiltered(It.IsAny<List<string>>())).ReturnsAsync(new[]
            {
                new IdentityRole
                {
                    Id = "2",
                    Name = "my role"
                },
            });
            _context.Setup(x => x.GetUsers()).Returns(new List<ApplicationUser>
            { user });
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.GetUsersAndRolesAsync(1,1);
            
            // assert
            result.Count.Should().Be(1);
            result.Single().User.Id.Should().Be(user.Id);
            result.First().User.Roles.Should().BeEquivalentTo(user.Roles);
            result.First().Roles.First().Should().Be("my role");
        }

        [Fact]
        public async Task CreateUserAsync_Failed()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            var user = new ApplicationUser
            {
                Id = "1",
                UserName = "vika",
                Roles = new List<IdentityUserRole<string>>
                {
                    new IdentityUserRole<string>
                    {
                        RoleId = "2",
                        UserId = "1"
                    }
                }
            };
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(new TestIdentityResult());
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            userManager.Setup(x => x.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(new TestIdentityResult());
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            bool exceptionTriggered = false;
            try
            {
                
                // act
                var result = await target.CreateUserAsync(user, It.IsAny<IEnumerable<string>>(), It.IsAny<string>());
            }
            catch (Exception e)
            {
                exceptionTriggered = true;
            }

            // assert
            exceptionTriggered.Should().BeTrue();
        }
        
        
        [Fact]
        public async Task CreateUserAsync_Success()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            var user = new ApplicationUser
            {
                Id = "1",
                UserName = "vika",
                Roles = new List<IdentityUserRole<string>>
                {
                    new IdentityUserRole<string>
                    {
                        RoleId = "2",
                        UserId = "1"
                    }
                }
            };
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(new TestIdentityResult());
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            userManager.Setup(x => x.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(new TestIdentityResult());
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.CreateUserAsync(user, new List<string>(), It.IsAny<string>());

            // assert
            result.Succeeded.Should().BeTrue();
        }
        
        [Fact]
        public async Task CreateUserAsync_NoResult()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            var user = new ApplicationUser
            {
                Id = "1",
                UserName = "vika",
                Roles = new List<IdentityUserRole<string>>
                {
                    new IdentityUserRole<string>
                    {
                        RoleId = "2",
                        UserId = "1"
                    }
                }
            };
            userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(new TestIdentityResult());
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            userManager.Setup(x => x.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(new TestIdentityResult(false));
            userManager.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new TestIdentityResult(false));
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.CreateUserAsync(user, new List<string>(), It.IsAny<string>());

            // assert
            result.Succeeded.Should().BeFalse();
        }
        
        [Fact]
        public async Task UpdateUserAsync_Fail()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            var user = new ApplicationUser
            {
                Id = "1",
                UserName = "vika",
                Roles = new List<IdentityUserRole<string>>
                {
                    new IdentityUserRole<string>
                    {
                        RoleId = "2",
                        UserId = "1"
                    }
                }
            };

            userManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new TestIdentityResult(false));
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.UpdateUserAsync(user, new List<string>());

            // assert
            result.Succeeded.Should().BeFalse();
        }
        
        [Fact]
        public async Task UpdateUserAsync_Success()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            var user = new ApplicationUser
            {
                Id = "1",
                UserName = "vika",
                Roles = new List<IdentityUserRole<string>>
                {
                    new IdentityUserRole<string>
                    {
                        RoleId = "2",
                        UserId = "1"
                    }
                }
            };

            userManager.Setup(x => x.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(new TestIdentityResult());
            userManager.Setup(x => x.RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(new TestIdentityResult());
            userManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>{"user role"});
            userManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new TestIdentityResult());
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.UpdateUserAsync(user, new List<string>{"test"});

            // assert
            result.Succeeded.Should().BeTrue();
        }
        
        [Fact]
        public async Task UpdateUserAsync_FailToRemove()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            var user = new ApplicationUser
            {
                Id = "1",
                UserName = "vika",
                Roles = new List<IdentityUserRole<string>>
                {
                    new IdentityUserRole<string>
                    {
                        RoleId = "2",
                        UserId = "1"
                    }
                }
            };

            userManager.Setup(x => x.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(new TestIdentityResult());
            userManager.Setup(x => x.RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(new TestIdentityResult(false));
            userManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>{"user role"});
            userManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new TestIdentityResult());
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.UpdateUserAsync(user, new List<string>{"test"});

            // assert
            result.Succeeded.Should().BeFalse();
        }
        
        [Fact]
        public async Task UpdateUserAsync_FailToAdd()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            var user = new ApplicationUser
            {
                Id = "1",
                UserName = "vika",
                Roles = new List<IdentityUserRole<string>>
                {
                    new IdentityUserRole<string>
                    {
                        RoleId = "2",
                        UserId = "1"
                    }
                }
            };

            userManager.Setup(x => x.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(new TestIdentityResult(false));
            userManager.Setup(x => x.RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(new TestIdentityResult());
            userManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>{"user role"});
            userManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new TestIdentityResult());
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.UpdateUserAsync(user, new List<string>{"test"});

            // assert
            result.Succeeded.Should().BeFalse();
        }
        
        [Fact]
        public async Task UpdateUserAsync_NoRoles()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            var user = new ApplicationUser
            {
                Id = "1",
                UserName = "vika",
                Roles = new List<IdentityUserRole<string>>
                {
                    new IdentityUserRole<string>
                    {
                        RoleId = "2",
                        UserId = "1"
                    }
                }
            };

            userManager.Setup(x => x.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(new TestIdentityResult(false));
            userManager.Setup(x => x.RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(new TestIdentityResult());
            userManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>{"user role"});
            userManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new TestIdentityResult());
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.UpdateUserAsync(user);

            // assert
            result.Succeeded.Should().BeTrue();
        }
        
        [Fact]
        public async Task ResetPasswordAsync_Success()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            var user = new ApplicationUser
            {
                Id = "1",
                UserName = "vika",
                Roles = new List<IdentityUserRole<string>>
                {
                    new IdentityUserRole<string>
                    {
                        RoleId = "2",
                        UserId = "1"
                    }
                }
            };

            userManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("lalala");
            userManager.Setup(x => x.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new TestIdentityResult());
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.ResetPasswordAsync(user, "pass");

            // assert
            result.Succeeded.Should().BeTrue();
        }
        
        [Fact]
        public async Task ResetPasswordAsync_Fail()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            var user = new ApplicationUser
            {
                Id = "1",
                UserName = "vika",
                Roles = new List<IdentityUserRole<string>>
                {
                    new IdentityUserRole<string>
                    {
                        RoleId = "2",
                        UserId = "1"
                    }
                }
            };

            userManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("lalala");
            userManager.Setup(x => x.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new TestIdentityResult(false));
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.ResetPasswordAsync(user, "pass");

            // assert
            result.Succeeded.Should().BeFalse();
        }

        [Fact]
        public async Task UpdatePasswordAsync_Success()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            var user = new ApplicationUser
            {
                Id = "1",
                UserName = "vika",
                Roles = new List<IdentityUserRole<string>>
                {
                    new IdentityUserRole<string>
                    {
                        RoleId = "2",
                        UserId = "1"
                    }
                }
            };

            userManager.Setup(x =>
                    x.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new TestIdentityResult());
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.UpdatePasswordAsync(user, "pass", "pass");

            // assert
            result.Succeeded.Should().BeTrue();
        }
        [Fact]
        public async Task FindByNameAsync_Success()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            var role = new IdentityRole
            {
                Id = "id"
            };
            roleManager.Setup(x =>
                    x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(role);
            
            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.GetRoleByNameAsync("name");

            // assert
            result.Should().BeEquivalentTo(role);
        }

        [Fact]
        public async Task GetRoleByIdAsync_Success()
        {
            // arrange
            var roleManager = MockHelpers.MockRoleManager<IdentityRole>();
            var userManager = MockHelpers.MockUserManager<ApplicationUser>();

            var role = new IdentityRole
            {
                Id = "id"
            };
            roleManager.Setup(x =>
                    x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(role);

            var target = new AccountManager(_context.Object, userManager.Object, roleManager.Object);

            // act
            var result = await target.GetRoleByIdAsync("name");

            // assert
            result.Should().BeEquivalentTo(role);
        }
    }
}