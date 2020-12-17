using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Weekday.Data.Interfaces;
using Weekday.Data.Models;
using Weekday.DataContracts;

namespace Weekday.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    [Route("api/[controller]")]
    public class AdministrationController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private readonly ILogger<AdministrationController> _logger;

        public AdministrationController(IMapper mapper, IAccountManager accountManager,
            ILogger<AdministrationController> logger)
        {
            _mapper = mapper;
            _accountManager = accountManager;
            _logger = logger;
        }


        [HttpPost("users")]
        [ProducesResponseType(200, Type = typeof(UserDataContract))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateUser([FromBody] UserDataContract user)
        {
            if (ModelState.IsValid)
            {
                if (user == null)
                {
                    return BadRequest($"{nameof(user)} cannot be null");
                }

                ApplicationUser appUser = _mapper.Map<ApplicationUser>(user);

                var result = await _accountManager.CreateUserAsync(appUser, user.RoleIds, user.NewPassword);
                if (result.Succeeded)
                {
                    UserDataContract newUser = await GetUserFromDatabase(appUser.Id);
                    return Ok(newUser);
                }

                AddError(result.Errors);
            }

            return BadRequest(ModelState);
        }

        [HttpGet("users/{pageNumber:int}/{pageSize:int}")]
        [ProducesResponseType(200, Type = typeof(List<UserDataContract>))]
        public async Task<IActionResult> GetUsers(int pageNumber, int pageSize)
        {
            var usersAndRoles = await _accountManager.GetUsersAndRolesAsync(pageNumber, pageSize);

            List<UserDataContract> usersVM = new List<UserDataContract>();

            foreach (var item in usersAndRoles)
            {
                var user = _mapper.Map<UserDataContract>(item.User);
                user.RoleIds = item.Roles;

                usersVM.Add(user);
            }

            return Ok(usersVM);
        }

        [HttpGet("roles")]
        [ProducesResponseType(200, Type = typeof(List<RoleDataContract>))]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _accountManager.GetAllRolesAsync();

            return Ok(roles.Select(x => _mapper.Map<RoleDataContract>(x)));
        }


        [HttpPut("users/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserDataContract user)
        {
            ApplicationUser appUser = await _accountManager.GetUserByIdAsync(id);

            if (ModelState.IsValid)
            {
                if (user == null)
                    return BadRequest($"{nameof(user)} cannot be null");

                if (!string.IsNullOrWhiteSpace(user.Id) && id != user.Id)
                    return BadRequest("Conflicting user id in parameter and model data");

                if (appUser == null)
                    return NotFound(id);

                bool isPasswordChanged = !string.IsNullOrWhiteSpace(user.NewPassword);
                bool isUserNameChanged = !appUser.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase);

                if (GetUserId(this.User) == id)
                {
                    if (string.IsNullOrWhiteSpace(user.CurrentPassword))
                    {
                        if (isPasswordChanged)
                            AddError("Current password is required when changing your own password", "Password");

                        if (isUserNameChanged)
                            AddError("Current password is required when changing your own username", "Username");
                    }
                    else if (isPasswordChanged || isUserNameChanged)
                    {
                        if (!await _accountManager.CheckPasswordAsync(appUser, user.CurrentPassword))
                            AddError("The username/password couple is invalid.");
                    }
                }

                if (ModelState.IsValid)
                {
                    _mapper.Map(user, appUser);

                    var result = await _accountManager.UpdateUserAsync(appUser, user.RoleIds);
                    if (result.Succeeded)
                    {
                        if (isPasswordChanged)
                        {
                            if (!string.IsNullOrWhiteSpace(user.CurrentPassword))
                                result = await _accountManager.UpdatePasswordAsync(appUser, user.CurrentPassword, user.NewPassword);
                            else
                                result = await _accountManager.ResetPasswordAsync(appUser, user.NewPassword);
                        }

                        if (result.Succeeded)
                            return NoContent();
                    }

                    AddError(result.Errors);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("users/{id}")]
        [ProducesResponseType(200, Type = typeof(UserDataContract))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            ApplicationUser appUser = await _accountManager.GetUserByIdAsync(id);

            if (appUser == null)
            {
                return NotFound(id);
            }

            var result = await _accountManager.DeleteUserAsync(appUser);
            if (!result.Succeeded)
            {
                throw new Exception("The following errors occurred whilst deleting user: " + string.Join(", ", result.Errors));
            }

            return Ok();
        }

        private async Task<UserDataContract> GetUserFromDatabase(string userId)
        {
            var userAndRoles = await _accountManager.GetUserAndRolesAsync(userId);
            if (userAndRoles == null)
            {
                return null;
            }

            var userVM = _mapper.Map<UserDataContract>(userAndRoles.Value.User);
            userVM.RoleIds = userAndRoles.Value.Roles;

            return userVM;
        }

        private static string GetUserId(ClaimsPrincipal user)
        {
            return user.FindFirst(JwtClaimTypes.Subject)?.Value?.Trim();
        }

        private void AddError(IEnumerable<string> errors, string key = "")
        {
            foreach (var error in errors)
            {
                AddError(error, key);
            }
        }

        private void AddError(string error, string key = "")
        {
            ModelState.AddModelError(key, error);
        }
    }
}
