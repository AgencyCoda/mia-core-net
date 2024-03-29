using System.Threading.Tasks;
using MiaCore.Features;
using MiaCore.Features.CreateUser;
using MiaCore.Features.CurrentUserPlan;
using MiaCore.Features.FetchEntityById;
using MiaCore.Features.FetchProfile;
using MiaCore.Features.Firebase;
using MiaCore.Features.GenerictList;
using MiaCore.Features.Login;
using MiaCore.Features.MiaDevices.Register;
using MiaCore.Features.MiaPlan.Update;
using MiaCore.Features.RecoveryPassword;
using MiaCore.Features.Register;
using MiaCore.Features.RemoveEntityById;
using MiaCore.Features.RemoveUserMe;
using MiaCore.Features.UpdateProfile;
using MiaCore.Models;
using MiaCore.Utils;
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
        public async Task<IActionResult> Me()
        => Ok(await Mediator.Send(new FetchProfileRequest()));

        [HttpPost("user/save")]
        [Authorize(Roles = Roles.Admin)]
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
        [AllowAnonymous]
        public async Task<IActionResult> RecoveryPassword(RecoveryPasswordRequest request)
        => Ok(await Mediator.Send(request));

        [HttpPost("change-password-recovery")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword(ChangePasswordRecoveryRequest request)
        => Ok(await Mediator.Send(request));

        [HttpPost("user/list")]
        [Authorize(Roles = Roles.AdminOrAdministrator)]
        public async Task<IActionResult> UserList(GenerictListRequest<MiaUser> request)
        => Ok(await Mediator.Send(request.SetReturnType<MiauserDetailedDto>()));

        [HttpPost("user/plan/list")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UserPlanList(GenerictListRequest<MiaUserPlan> request)
        => Ok(await Mediator.Send(request));

        [HttpGet("user/plan/current/{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UserPlanCurrent(int id, [FromQuery] string withs)
        {
            return Ok(await Mediator.Send(new CurrentUserPlanRequest { Id = id, Withs = withs }));
        }

        [HttpDelete("user/remove/{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RemoveUser(long id)
        {
            return Ok(await Mediator.Send(new RemoveEntityByIdRequest<MiaUser> { Id = id }));
        }

        [HttpDelete("user/remove")]
        public async Task<IActionResult> RemoveUserMe()
        => Ok(await Mediator.Send(new RemoveUserMeRequest()));

        [HttpGet("user/fetch/{id}")]
        public async Task<IActionResult> FetchUser(long id, [FromQuery] string withs)
        {
            return Ok(await Mediator.Send(
                new FetchEntityByIdRequest<MiaUser> { Id = id, Withs = withs }
                    .SetReturnType<MiaUserDto>()
                ));
        }

        [HttpPost("register-device")]
        public async Task<IActionResult> RegisterDevice(RegisterMiaDeviceRequest request)
        {
            return Ok(await Mediator.Send(request));
        }

        [HttpPost("mia-plan/list")]
        [AllowAnonymous]
        public async Task<IActionResult> ListMiaPlan(GenerictListRequest<MiaPlan> request)
        => Ok(await Mediator.Send(request));

        [HttpPost("mia-plan/save")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> SaveMiaPlan(MiaPlanUpdateRequest request)
        => Ok(await Mediator.Send(request));
    }
}