

using IdentityAndJwt.InitExtention;
using IdentityAndJwt.jwt;
using SignalRHttps.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(); // 添加控制器
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//初始化Swagger
builder.Services.InitSwagger();

//初始jwt
builder.Services.InitJwt(builder.Configuration.GetSection("JWT").Get<JWTSettings>());

//初始identity
string connStr = builder.Configuration.GetConnectionString("Dbcomnection");
builder.Services.InitIdentity(connStr);

//配置Ioptions
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWT"));

//跨域
string[] urls = new[] { "http://localhost:3000", "http://localhost:8080" };
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder => builder.WithOrigins(urls).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});

// 引用SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// 使用跨域
app.UseCors();

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting(); // 路由中间件

// 将路径请求传入到SingalR.Hub类型中
// 泛型中的名字对应于Hubs文件夹下的类文件的名字
// 启用SignalR中间件
app.MapHub<ProgressHub>("/progressHub");

app.MapControllers(); // 路由配置
app.Run();