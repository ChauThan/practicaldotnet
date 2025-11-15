using Microsoft.AspNetCore.Mvc;
using ShortLink.Core.Interfaces;
using ShortLink.Core.Models;

namespace ShortLink.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinksController : ControllerBase
    {
        private readonly ILinkRepository _linkRepository;
        private readonly ILinkService _linkService;

        public LinksController(ILinkRepository linkRepository, ILinkService linkService)
        {
            _linkRepository = linkRepository;
            _linkService = linkService;
        }

        [HttpPost]
        public async Task<ActionResult<Link>> CreateLink([FromBody] Link link)
        {
            if (link == null)
            {
                return BadRequest();
            }

            var createdLink = await _linkService.CreateShortLink(link.OriginalUrl);
            return CreatedAtAction(nameof(GetLinkById), new { id = createdLink.Id }, createdLink);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Link>> GetLinkById(Guid id)
        {
            var link = await _linkRepository.GetLinkById(id);
            if (link == null)
            {
                return NotFound();
            }
            return Ok(link);
        }

        [HttpGet("short/{shortCode}")]
        public async Task<ActionResult<Link>> GetLinkByShortCode(string shortCode)
        {
            var link = await _linkService.GetLinkByShortCode(shortCode);
            if (link == null)
            {
                return NotFound();
            }
            return Ok(link);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Link>>> GetAllLinks()
        {
            var links = await _linkRepository.GetAllLinks();
            return Ok(links);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLink(Guid id)
        {
            var link = await _linkRepository.GetLinkById(id);
            if (link == null)
            {
                return NotFound();
            }

            await _linkRepository.DeleteLink(id);
            return NoContent();
        }

        [HttpDelete("short/{shortCode}")]
        public async Task<IActionResult> DeleteLinkByShortCode(string shortCode)
        {
            var link = await _linkService.GetLinkByShortCode(shortCode);
            if (link == null)
            {
                return NotFound();
            }

            await _linkService.DeleteLink(shortCode);
            return NoContent();
        }
    }
}