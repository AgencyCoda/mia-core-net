using System.Threading.Tasks;
using MiaCore.Features.CreateUser;
using MiaCore.Features.CurrentUserPlan;
using MiaCore.Features.FetchEntityById;
using MiaCore.Features.FetchProfile;
using MiaCore.Features.Firebase;
using MiaCore.Features.GenerictList;
using MiaCore.Features.Login;
using MiaCore.Features.RecoveryPassword;
using MiaCore.Features.Register;
using MiaCore.Features.RemoveEntityById;
using MiaCore.Features.UpdateProfile;
using MiaCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiaCore.Controllers
{
    [Route("mia-auth")]
    [Authorize]
    public class MiaAuthController : MiaControllerBase
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request)
        => Ok(await Mediator.Send(request));

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest request)
        => Ok(await Mediator.Send(request));

        [HttpGet("me")]
        public async Task<IActionResult> Me(FetchProfileRequest request)
        => Ok(await Mediator.Send(request));

        [HttpPost("user/save")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SaveUser(SaveUserRequest request)
        => Ok(await Mediator.Send(request));

        [HttpPost("update-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
        => Ok(await Mediator.Send(request));

        [HttpPost("login-with-firebase")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithFirebase(FirebaseAuthenticationRequest request)
        => Ok(await Mediator.Send(request));

        [HttpPost("login-with-google")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithGoogle(FirebaseAuthenticationRequest request)
        => Ok(await Mediator.Send(request));

        [HttpPost("login-with-facebook")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithFacebook(FirebaseAuthenticationRequest request)
        => Ok(await Mediator.Send(request));

        [HttpPost("recovery")]
        public async Task<IActionResult> RecoveryPassword(RecoveryPasswordRequest request)
        => Ok(await Mediator.Send(request));

        [HttpPost("change-password-recovery")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRecoveryRequest request)
        => Ok(await Mediator.Send(request));

        [HttpPost("user/list")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UserList(GenerictListRequest<MiaUser> request)
        => Ok(await Mediator.Send(request));

        [HttpPost("user/plan/list")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UserPlanList(GenerictListRequest<MiaUserPlan> request)
        => Ok(await Mediator.Send(request));

        [HttpGet("user/plan/current/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UserPlanCurrent(int id, [FromQuery] string withs)
        {
            return Ok(await Mediator.Send(new CurrentUserPlanRequest { Id = id, Withs = withs }));
        }

        [HttpPost("user/remove/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RemoveUser(long id)
        {
            return Ok(await Mediator.Send(new RemoveEntityByIdRequest<MiaUser> { Id = id }));
        }

        [HttpGet("user/fetch/{id}")]
        public async Task<IActionResult> FetchUser(long id, [FromQuery] string withs)
        {
            return Ok(await Mediator.Send(new FetchEntityByIdRequest<MiaUser> { Id = id, Withs = withs }));
        }
    }
}