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

        public LinksController(ILinkRepository linkRepository)
        {
            _linkRepository = linkRepository;
        }

        [HttpPost]
        public async Task<ActionResult<Link>> CreateLink([FromBody] Link link)
        {
            if (link == null)
            {
                return BadRequest();
            }

            var createdLink = await _linkRepository.AddLink(link);
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
    }
}