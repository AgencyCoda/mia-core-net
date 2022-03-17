using System.Threading.Tasks;
using MiaCore.Features.MiaBilling.FetchCurrentBillingInfo;
using MiaCore.Features.MiaBilling.FetchMiaBillingInfo;
using MiaCore.Features.MiaBilling.SaveBillingInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiaCore.Controllers
{
    [Route("mia-billing")]
    [Authorize]
    public class MiaBillingController : MiaControllerBase
    {
        [HttpPost("info/save")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Save(SaveBillingInfoRequest request)
        => Ok(await Mediator.Send(request));

        [HttpGet("info/fetch/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Fetch(long id)
        {
            return Ok(await Mediator.Send(new FetchMiaBillingInfoRequest { Id = id }));
        }

        [HttpGet("info/me")]
        public async Task<IActionResult> FetchCurrent()
        {
            return Ok(await Mediator.Send(new FetchCurrentBillingInfoRequest()));
        }
    }
}