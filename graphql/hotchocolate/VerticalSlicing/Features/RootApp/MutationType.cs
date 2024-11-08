using System.Reflection;
using SimpleApp.Common;

namespace SimpleApp.Features.RootApp;

public class MutationType: ObjectType
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Mutation);
        
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly
            .GetTypes()
            .Where(t => 
                t is { IsClass: true, IsAbstract: false }
                && typeof(FeatureMutationTypeBase).IsAssignableFrom(t)) 
            .ToList();

        foreach (var type in types)
        {
            var instance = ActivatorUtilities.CreateInstance(descriptor.Extend().Context.Services, type);
            if (instance is FeatureMutationTypeBase featureQueryType)
            {
                featureQueryType.Configure(descriptor);
            }
        }
    }
}