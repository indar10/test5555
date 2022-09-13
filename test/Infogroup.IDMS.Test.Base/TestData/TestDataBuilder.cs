using Infogroup.IDMS.EntityFrameworkCore;

namespace Infogroup.IDMS.Test.Base.TestData
{
    public class TestDataBuilder
    {
        private readonly IDMSDbContext _context;
        private readonly int _tenantId;

        public TestDataBuilder(IDMSDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            new TestOrganizationUnitsBuilder(_context, _tenantId).Create();
            new TestSubscriptionPaymentBuilder(_context, _tenantId).Create();
            new TestEditionsBuilder(_context).Create();

            _context.SaveChanges();
        }
    }
}
