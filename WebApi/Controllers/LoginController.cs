using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityAndJwt.Identity;
using IdentityAndJwt.jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SignalRHttps.Request;

namespace IdentityAndJwt.Controllers
{
    [Route("api/[Controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<JWTSettings> _options;

        public LoginController(UserManager<User> userManager, RoleManager<Role> roleManager, IWebHostEnvironment webHostEnvironment
            , IOptions<JWTSettings> options
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
            _options = options;
        }

        // GET
        /// <summary>
        /// 初始化admin用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            if (!await _roleManager.RoleExistsAsync("admin"))
            {
                Role role = new Role() { Name = "admin" };
                var res = await _roleManager.CreateAsync(role);
                if (!res.Succeeded) return BadRequest("fail create role");

            }

            var user = await _userManager.FindByNameAsync("admin");
            if (user == null)
            {
                User user1 = new User()
                {
                    UserName = "admin",
                };
                var res1 = await _userManager.CreateAsync(user1, "123456");
                if (!res1.Succeeded) return BadRequest("fail create user");
                await _userManager.AddToRoleAsync(user1, "admin");
            }
            return Ok();

        }

        [HttpGet]
        public async Task<ActionResult> SendResetPassWordToken(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("未找到用户");
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            Console.WriteLine(token);
            return Ok(token);
        }
        [HttpGet]
        public async Task<ActionResult> ResetPassWord(string username, string token, string newpassword)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("未找到用户");
            }

            var res = await _userManager.ResetPasswordAsync(user, token, newpassword);
            if (!res.Succeeded)
            {
                return BadRequest("fali reset password");
            }
            return Ok("修改成功");
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.username);
            if (user == null)
            {
                return BadRequest("未找到用户");
            }


            var res = await _userManager.CheckPasswordAsync(user, loginRequest.password);
            if (!res)
            {
                return BadRequest("登录失败");
            }

            return Ok(new { token = await GetToken(user) })  ;
        }
            //return await GetToken(user);
        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<string>> GetToken(User user)
        {
            
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));

            string _skey = _options.Value.SecKey;
            DateTime expire = DateTime.Now.AddHours(1);

            byte[] secBytes = Encoding.UTF8.GetBytes(_skey);
            var secKey = new SymmetricSecurityKey(secBytes);
            var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(claims: claims,
                expires: expire, signingCredentials: credentials);
            string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return jwt;
        }
    }
}
