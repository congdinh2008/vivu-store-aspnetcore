using Microsoft.EntityFrameworkCore;

namespace ViVuStore.Data;

public class ViVuStoreDbContext: DbContext
{
    public ViVuStoreDbContext(DbContextOptions<ViVuStoreDbContext> options): base(options)
    {
        
    }
}
