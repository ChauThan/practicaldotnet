using System;
using System.Linq;
using System.Threading.Tasks;
using ShortLink.Core.Models;
using ShortLink.Core.Services;
using Xunit;

namespace ShortLink.Core.Tests
{
    public class LinkServiceTests
    {
        [Fact]
        public async Task CreateShortLink_WithValidUrl_ReturnsLink()
        {
            // Arrange
            var repo = new InMemoryLinkRepository();
            var service = new LinkService(repo);
            var url = "https://example.com/hello";

            // Act
            var created = await service.CreateShortLink(url);

            // Assert
            Assert.NotNull(created);
            Assert.Equal(url, created.OriginalUrl);
            Assert.False(string.IsNullOrEmpty(created.ShortCode));
            Assert.Equal(6, created.ShortCode.Length);
        }

        [Fact]
        public async Task CreateShortLink_WithInvalidUrl_ThrowsArgumentException()
        {
            // Arrange
            var repo = new InMemoryLinkRepository();
            var service = new LinkService(repo);
            var badUrl = "not-a-url";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateShortLink(badUrl));
        }

        [Fact]
        public async Task GetLinkByShortCode_ReturnsCreatedLink()
        {
            // Arrange
            var repo = new InMemoryLinkRepository();
            var service = new LinkService(repo);
            var url = "https://example.com/2";

            // Act
            var created = await service.CreateShortLink(url);
            var found = await service.GetLinkByShortCode(created.ShortCode);

            // Assert
            Assert.NotNull(found);
            Assert.Equal(created.Id, found!.Id);
        }

        [Fact]
        public async Task ResolveAndTrack_IncrementsHitCountAndLastAccessed()
        {
            // Arrange
            var repo = new InMemoryLinkRepository();
            var service = new LinkService(repo);
            var url = "https://example.com/track";

            // Act
            var created = await service.CreateShortLink(url);
            Assert.Equal(0, created.HitCount);

            var resolved = await service.ResolveAndTrackAsync(created.ShortCode);

            // Assert
            Assert.NotNull(resolved);
            Assert.Equal(1, resolved!.HitCount);
            Assert.True(resolved.LastAccessed.HasValue);
        }
    }
}