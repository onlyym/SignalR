using System.Text;
using IdentityAndJwt.Identity;
using IdentityAndJwt.jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace IdentityAndJwt.InitExtention
{
    public static class ProjectInit
    {
        /// <summary>
        /// 初始化Swagger
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static IServiceCollection InitSwagger(this IServiceCollection service)
        {
            service.AddSwaggerGen(c =>
            {
                var scheme = new OpenApiSecurityScheme()
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Authorization"
                    },
                    Scheme = "oauth2",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                };
                c.AddSecurityDefinition("Authorization", scheme);
                var requirement = new OpenApiSecurityRequirement();
                requirement[scheme] = new List<string>();
                c.AddSecurityRequirement(requirement);
            });
            return service;
        }

        /// <summary>
        /// 初始化jwt
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static IServiceCollection InitJwt(this IServiceCollection service, JWTSettings jwtOpt)
        {
            service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    //var jwtOpt = builder.Configuration.GetSection("JWT").Get<JWTSettings>();
                    byte[] keyBytes = Encoding.UTF8.GetBytes(jwtOpt.SecKey);
                    var secKey = new SymmetricSecurityKey(keyBytes);
                    x.TokenValidationParameters = new()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = secKey
                    };

                    //新增signalR jwt令牌认证
                    x.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/progressHub")))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            return service;
        }

        public static IServiceCollection InitIdentity(this IServiceCollection services,string connStr)
        {
            services.AddDbContext<IDbContext>(opt => {
                //string connStr = builder.Configuration.GetConnectionString("Dbcomnection");
                opt.UseSqlServer(connStr);
            });
            services.AddDataProtection();
            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            });
            var idBuilder = new IdentityBuilder(typeof(User), typeof(Role), services);
            idBuilder.AddEntityFrameworkStores<IDbContext>()
                .AddDefaultTokenProviders()
                .AddRoleManager<RoleManager<Role>>()
                .AddUserManager<UserManager<User>>();
            return services;
        }
    }
}
