using Abp.Dependency;
using GraphQL;
using GraphQL.Types;
using Infogroup.IDMS.Queries.Container;

namespace Infogroup.IDMS.Schemas
{
    public class MainSchema : Schema, ITransientDependency
    {
        public MainSchema(IDependencyResolver resolver) :
            base(resolver)
        {
            Query = resolver.Resolve<QueryContainer>();
        }
    }
}