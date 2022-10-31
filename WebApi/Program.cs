

using IdentityAndJwt.InitExtention;
using IdentityAndJwt.jwt;
using SignalRHttps.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(); // ��ӿ�����
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//��ʼ��Swagger
builder.Services.InitSwagger();

//��ʼjwt
builder.Services.InitJwt(builder.Configuration.GetSection("JWT").Get<JWTSettings>());

//��ʼidentity
string connStr = builder.Configuration.GetConnectionString("Dbcomnection");
builder.Services.InitIdentity(connStr);

//����Ioptions
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWT"));

//����
string[] urls = new[] { "http://localhost:3000", "http://localhost:8080" };
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder => builder.WithOrigins(urls).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});

// ����SignalR
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

// ʹ�ÿ���
app.UseCors();

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting(); // ·���м��

// ��·�������뵽SingalR.Hub������
// �����е����ֶ�Ӧ��Hubs�ļ����µ����ļ�������
// ����SignalR�м��
app.MapHub<ProgressHub>("/progressHub");

app.MapControllers(); // ·������
app.Run();