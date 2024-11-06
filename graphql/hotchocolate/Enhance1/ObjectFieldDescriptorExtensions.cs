namespace Enhance1;

public static class ObjectFieldDescriptorExtensions
{
    public static IObjectFieldDescriptor Resolve<TResolver>(this IObjectFieldDescriptor descriptor)
        where TResolver : IResolver
    {
        return descriptor.Resolve(async ctx =>
        {
            var resolver = ActivatorUtilities.CreateInstance<TResolver>(ctx.Services);
            if (resolver is null)
            {
                throw new InvalidOperationException($"Resolver {typeof(TResolver).Name} not found.");
            }

            return await resolver.InvokeAsync(ctx);

        }, 
            GetReturnType<TResolver>());
    }

    private static Type GetReturnType<TResolver>() where TResolver : IResolver
    {
        var resolverInterface = typeof(TResolver)
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType 
                                 && i.GetGenericTypeDefinition() == typeof(IResolver<>));
        if (resolverInterface is null)
        {
            throw new InvalidOperationException($"Could not determine return type for resolver {typeof(TResolver).Name}");
            
        }
        return resolverInterface.GetGenericArguments()[0];
    }
}