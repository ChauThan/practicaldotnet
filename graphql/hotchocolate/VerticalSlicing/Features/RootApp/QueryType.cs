using System.Reflection;
using SimpleApp.Common;

namespace SimpleApp.Features.RootApp;

public class QueryType: ObjectType
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Query);
        
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly
            .GetTypes()
            .Where(t => 
                t is { IsClass: true, IsAbstract: false }
                && typeof(FeatureQueryTypeBase).IsAssignableFrom(t)) 
            .ToList();

        foreach (var type in types)
        {
            var instance = ActivatorUtilities.CreateInstance(descriptor.Extend().Context.Services, type);
            if (instance is FeatureQueryTypeBase featureQueryType)
            {
                featureQueryType.Configure(descriptor);
            }
        }
    }
}