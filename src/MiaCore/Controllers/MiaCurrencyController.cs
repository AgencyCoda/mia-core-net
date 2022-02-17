using System.Threading.Tasks;
using MiaCore.Features.GenerictList;
using MiaCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiaCore.Controllers
{
    [Route("mia-currency")]
    public class MiaCurrencyController : MiaControllerBase
    {
        [HttpPost("list")]
        [AllowAnonymous]
        public async Task<IActionResult> List(GenerictListRequest<MiaCurrency> request)
        => Ok(await Mediator.Send(request));
    }
}