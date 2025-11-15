## Plan: Implement `LinkService` for Short Link Management

Create a public `LinkService` class in `ShortLink.Core/Services/LinkService.cs` that manages short links using an injected `ILinkRepository`. It will generate unique short codes, create links, and provide retrieval and deletion methods.

### Steps
1. Define the public `LinkService` class in `ShortLink.Core/Services/LinkService.cs`.
2. Inject `ILinkRepository` via the constructor and store it as a private field.
3. Implement `CreateShortLink(string longUrl)`:
   - Generate a random 6-character alphanumeric code.
   - Check for uniqueness using the repository; retry if needed.
   - Create and save a new `Link` object via the repository.
   - Return the created `Link`.
4. Implement `GetLinkByShortCode(string shortCode)` and `DeleteLink(string shortCode)` methods, delegating to the repository.
5. Ensure all methods are public and follow .NET naming conventions.

### Further Considerations
1. Should the short code generation be case-sensitive (A-Z, a-z, 0-9) or only uppercase/lowercase?
2. Should `CreateShortLink` throw an exception or return null if the URL is invalid?
3. Are there thread-safety or performance requirements for high-volume link creation?
