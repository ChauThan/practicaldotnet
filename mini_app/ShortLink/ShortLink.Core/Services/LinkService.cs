using System.Security.Cryptography;
using System.Text;
using ShortLink.Core.Interfaces;
using ShortLink.Core.Models;

namespace ShortLink.Core.Services
{
    /// <summary>
    /// Higher-level link service that manages short-code generation and delegates storage
    /// operations to an injected <see cref="ILinkRepository"/> implementation.
    /// </summary>
    public class LinkService : ILinkService
    {
        private const int ShortCodeLength = 6;
        private static readonly char[] _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        private readonly ILinkRepository _repository;

        public LinkService(ILinkRepository repository)
        {
            _repository = repository;
        }

        public async Task<Link> CreateShortLink(string longUrl)
        {
            if (string.IsNullOrWhiteSpace(longUrl) || !Uri.TryCreate(longUrl, UriKind.Absolute, out var uriResult) || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("Invalid URL. Must be a valid absolute http(s) url.", nameof(longUrl));
            }

            // Ensure a unique short code
            var attempts = 0;
            string shortCode;
            do
            {
                shortCode = GenerateShortCode(ShortCodeLength);
                attempts++;
                if (attempts > 10)
                {
                    throw new InvalidOperationException("Unable to generate a unique short code after multiple attempts.");
                }

                // Try to find by short code to avoid costly enumeration
                var collision = await _repository.GetByShortCode(shortCode);
                if (collision == null)
                {
                    break;
                }
            } while (true);

            var link = new Link
            {
                OriginalUrl = longUrl,
                ShortCode = shortCode,
                DateCreated = DateTime.UtcNow
            };

            var created = await _repository.AddLink(link);
            return created;
        }

        public async Task<Link?> GetLinkByShortCode(string shortCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode)) return null;
            return await _repository.GetByShortCode(shortCode);
        }

        public async Task<Link?> ResolveAndTrackAsync(string shortCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode)) return null;
            var link = await _repository.GetByShortCode(shortCode);
            if (link == null) return null;

            link.HitCount++;
            link.LastAccessed = DateTime.UtcNow;
            await _repository.UpdateLink(link);

            return link;
        }

        public async Task DeleteLink(string shortCode)
        {
            var link = await GetLinkByShortCode(shortCode);
            if (link == null)
            {
                throw new KeyNotFoundException("Link with the specified short code does not exist.");
            }

            await _repository.DeleteLink(link.Id);
        }

        private static string GenerateShortCode(int length)
        {
            var data = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }

            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                var idx = data[i] % _alphabet.Length;
                sb.Append(_alphabet[idx]);
            }

            return sb.ToString();
        }
    }
}
