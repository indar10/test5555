using Infogroup.IDMS.EntityFrameworkCore;

namespace Infogroup.IDMS.Migrations.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly IDMSDbContext _context;

        public InitialHostDbBuilder(IDMSDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
