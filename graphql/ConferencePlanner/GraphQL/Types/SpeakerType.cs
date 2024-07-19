using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Types
{
    public class SpeakerType : InterfaceType<Speaker>
    {
        protected override void Configure(IInterfaceTypeDescriptor<Speaker> descriptor)
        {
            descriptor.Field(x => x.Id).ID();

            descriptor.ResolveAbstractType((context, result) =>
            {
                if (result is Speaker speaker)
                {
                    IReadOnlyCollection<ObjectType>? types = null;
                    types ??= context.Schema.GetPossibleTypes(this);
                    foreach (var type in types)
                    {
                        if (type.Name == speaker.JobType.ToString())
                        {
                            return type;
                        }
                    }
                }

                return null;
            });
        }
    }
}