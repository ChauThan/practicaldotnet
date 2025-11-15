using Microsoft.AspNetCore.Mvc;
using ShortLink.Core.Interfaces;

namespace ShortLink.Api.Controllers
{
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly ILinkService _linkService;

        public RedirectController(ILinkService linkService)
        {
            _linkService = linkService;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> RedirectToOriginal(string code)
        {
            var link = await _linkService.ResolveAndTrackAsync(code);
            if (link == null)
            {
                return NotFound();
            }

            return RedirectPermanent(link.OriginalUrl);
        }
    }
}
