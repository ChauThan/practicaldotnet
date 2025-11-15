using System;

namespace ShortLink.Api.Models
{
    public class CreateLinkRequest
    {
        public string OriginalUrl { get; set; } = string.Empty;
    }
}
