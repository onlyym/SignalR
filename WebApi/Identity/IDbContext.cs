using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityAndJwt.Identity
{
    public class IDbContext: IdentityDbContext<User, Role, long>
    {
        public IDbContext(DbContextOptions<IDbContext> options) : base(options)
        {

        }
    }
}
