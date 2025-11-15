using System;

namespace ShortLink.Core.Models
{
    // Public record to represent a shortened link
    public record Link
    {
        // Primary identifier
        public Guid Id { get; set; }

        // Original URL to be shortened
        public string OriginalUrl { get; set; } = string.Empty;

        // Generated short code
        public string ShortCode { get; set; } = string.Empty;

        // Creation timestamp
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}