using System.Threading.Tasks;
using MiaCore.Features.AssignCategoryToUser;
using MiaCore.Features.GenerictList;
using MiaCore.Features.SaveCategory;
using MiaCore.Models;
using MiaCore.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiaCore.Controllers
{
    [Route("mia-category")]
    [Authorize]
    public class MiaCategoryController : MiaControllerBase
    {
        [HttpPost("list")]
        [AllowAnonymous]
        public async Task<IActionResult> List(GenerictListRequest<MiaCategory> request)
        => Ok(await Mediator.Send(request));

        [HttpPost("save")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Save(SaveCategoryRequest request)
        => Ok(await Mediator.Send(request));

        [HttpPost("assign-to-user")]
        public async Task<IActionResult> Assign(AssignCategoryToUserRequest request)
        => Ok(await Mediator.Send(request));
    }
}