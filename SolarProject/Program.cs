using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.DTO_S;
using DbAccessaLayer.Helpers;
using DbAccessaLayer.Infrastructure;
using DbAccessaLayer.Infrastructure.Background;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SolarProject.Hubs;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty,
                b =>
                {
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    b.CommandTimeout(1000);
                }));
builder.Host.UseSerilog((ctx, service, lc) => lc.ReadFrom.Configuration(ctx.Configuration).ReadFrom.Services(service).Enrich.FromLogContext().WriteTo.File(
    path: $"Config/logs.json",
    shared: true));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder => builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

//read values from appjosn
var jwtSettings = builder.Configuration.GetSection("Jwt");

//add authentication service

builder.Services.AddAuthentication(options =>
{
    // This tells ASP.NET:
    // JWT is the default authentication method
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //  Verify who issued the token
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],

        //  Verify who the token is meant for
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],

        //  Verify token is not expired
        ValidateLifetime = true,

        //  Verify token signature
        ValidateIssuerSigningKey = true,

        //  Secret key used to validate signature
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
        ),

        // Remove default 5 min tolerance
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            // If the request is for SignalR hub
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/notifications"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Configuration.AddJsonFile("message.json",optional:false,reloadOnChange:true);


builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

//builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection("GeminiSettings"));

builder.Services.AddHttpClient<AiInterfaceRepository, AiRepository>();

builder.Services.AddScoped<IEmailService, EmailServiceRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();


builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IAdminRepository, AdminRepository>();

builder.Services.AddScoped<ITechnicianRepository, TechnicianRepository>();

builder.Services.AddScoped<IProductsRepository, ProductRepository>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();

builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<IHomeRepository, HomeRepository>();

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddScoped<IDeliveryInterface, DeliveryRepository>();

builder.Services.AddHostedService<DeliveryAssignmentWorker>();

builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

builder.Services.AddScoped<IInstallRepository, InstallRepository>();

builder.Services.AddScoped<INoticationRepositry, NotificationRepository>();

builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();

builder.Services.AddScoped<ISolarPerformaceRepository, SolarRepository>();

builder.Services.Configure<AssignmentSettings>(builder.Configuration.GetSection("AssignedDelivery"));

builder.Services.AddScoped<ISignalRRepository, SignalRService>();

builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomerUserIdProvider>();

builder.Services.AddHostedService<MaintenenceAssignmentBackGroundService>();

builder.Services.AddHostedService<AlertBackgroundService>();

//builder.Services.AddHostedService<WeatherForeCastSerice>();

builder.Services.AddScoped<IAccessRepository, AccessoryRepository>();

builder.Services.AddScoped<IAccessRepository2,AccessRepository2>();

//builder.Services.AddHostedService<BackgroundServiceForDelayOrders>();

builder.Services.AddHostedService<AddToPanels>();

builder.Services.AddHostedService<DbAccessaLayer.Infrastructure.Background.SolarPerformanceService>();

//We need logged-in user info.
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddControllers();

//builder.Services.Configure<OllamaSettings>( builder.Configuration.GetSection("Ollama"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();

MessageHelper.Initialize(app.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAngularApp");
app.UseAuthentication(); 
app.UseAuthorization();
app.MapHub<NotificationHub>("/notifications").RequireAuthorization();

app.MapControllers();

app.Run();
