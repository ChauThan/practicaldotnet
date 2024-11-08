using SimpleApp.Common;

namespace HotChocolate.Types;

public static class ObjectFieldDescriptorExtensions
{
    public static IObjectFieldDescriptor Resolve<TResolver>(this IObjectFieldDescriptor descriptor)
        where TResolver : IResolver
    {
        return Resolve(descriptor, typeof(TResolver));
    }

    public static IObjectFieldDescriptor Resolve(this IObjectFieldDescriptor descriptor, Type resolverType)
    {
        if (!resolverType
                .GetInterfaces()
                .Any(i => i.IsGenericType 
                          && i.GetGenericTypeDefinition() == typeof(IResolver<>)))
        {
            throw new InvalidOperationException($"The resolver type {resolverType.Name} does not implement IResolver<>.");
        }

        return descriptor.Resolve(async ctx =>
            {
                var resolver = ActivatorUtilities.CreateInstance(ctx.Services, resolverType);
                if (resolver is null)
                {
                    throw new InvalidOperationException($"Resolver {resolverType.Name} not found.");
                }

                return await ((IResolver)resolver).InvokeAsync(ctx);

            },
            GetReturnType(resolverType));

    }
    
    private static Type GetReturnType(Type resolverType)
    {
        var resolverInterface = resolverType
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType 
                                 && i.GetGenericTypeDefinition() == typeof(IResolver<>));
        
        if (resolverInterface is null)
        {
            throw new InvalidOperationException($"Could not determine return type for resolver {resolverType.Name}");
            
        }
        
        return resolverInterface.GetGenericArguments()[0];
    }
}