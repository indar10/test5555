using Infogroup.IDMS.Auditing;
using Infogroup.IDMS.Test.Base;
using Shouldly;
using Xunit;

namespace Infogroup.IDMS.Tests.Auditing
{
    // ReSharper disable once InconsistentNaming
    public class NamespaceStripper_Tests: AppTestBase
    {
        private readonly INamespaceStripper _namespaceStripper;

        public NamespaceStripper_Tests()
        {
            _namespaceStripper = Resolve<INamespaceStripper>();
        }

        [Fact]
        public void Should_Stripe_Namespace()
        {
            var controllerName = _namespaceStripper.StripNameSpace("Infogroup.IDMS.Web.Controllers.HomeController");
            controllerName.ShouldBe("HomeController");
        }

        [Theory]
        [InlineData("Infogroup.IDMS.Auditing.GenericEntityService`1[[Infogroup.IDMS.Storage.BinaryObject, Infogroup.IDMS.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null]]", "GenericEntityService<BinaryObject>")]
        [InlineData("CompanyName.ProductName.Services.Base.EntityService`6[[CompanyName.ProductName.Entity.Book, CompanyName.ProductName.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],[CompanyName.ProductName.Services.Dto.Book.CreateInput, N...", "EntityService<Book, CreateInput>")]
        [InlineData("Infogroup.IDMS.Auditing.XEntityService`1[Infogroup.IDMS.Auditing.AService`5[[Infogroup.IDMS.Storage.BinaryObject, Infogroup.IDMS.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],[Infogroup.IDMS.Storage.TestObject, Infogroup.IDMS.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],]]", "XEntityService<AService<BinaryObject, TestObject>>")]
        public void Should_Stripe_Generic_Namespace(string serviceName, string result)
        {
            var genericServiceName = _namespaceStripper.StripNameSpace(serviceName);
            genericServiceName.ShouldBe(result);
        }
    }
}
