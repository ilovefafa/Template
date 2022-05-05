using Microsoft.EntityFrameworkCore;

namespace Template.TaskScheduler
{
    public partial class XxContext : DbContext
    {
        public XxContext(DbContextOptions<XxContext> options)
            : base(options)
        {
        }

        public virtual DbSet<XxModel> Xx { get; set; } = null!;

    }
}
