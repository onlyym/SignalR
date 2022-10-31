using Microsoft.AspNetCore.Identity;
namespace IdentityAndJwt.Identity
{
    public class User:IdentityUser<long>
    {
        public DateTime CreationTime { get; set; }
        public string? NickName { get; set; }
    }
}
