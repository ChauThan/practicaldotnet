using HotChocolate.Resolvers;

namespace Enhance1;

/// <summary>
/// Defines resolver that can be used to resolve a field.
/// </summary>
public interface IResolver
{
    Task<object> InvokeAsync(IResolverContext context);
}

/// <summary>
/// Defines resolver that can be used to resolve a field.
/// </summary>
public interface IResolver<TResult> : IResolver
{
    /// <summary>
    /// Resolver handling method
    /// </summary>
    /// <param name="context">The <see cref="IResolverContext"/></param>
    /// <returns>A field data</returns>
    new Task<TResult> InvokeAsync(IResolverContext context);
    
    async Task<object> IResolver.InvokeAsync(IResolverContext context) => await InvokeAsync(context);
}