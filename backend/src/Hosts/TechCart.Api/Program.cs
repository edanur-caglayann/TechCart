using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TechCart.Api.Common;
using TechCart.Users.Application.Abstractions;
using TechCart.Users.Infrastructure;
using TechCart.Users.Infrastructure.Security;
using TechCart.Addresses.Infrastructure;
using TechCart.Categories.Infrastructure;
using TechCart.Brands.Infrastructure;
using TechCart.CartItems.Infrastructure;
using TechCart.Products.Infrastructure;
using TechCart.ProductImages.Infrastructure;
using TechCart.Inventory.Infrastructure;
using TechCart.OrderItems.Infrastructure;
using TechCart.Orders.Infrastructure;
using TechCart.Payments.Infrastructure;
using MassTransit;
using TechCart.Products.Infrastructure.Consumers;
using TechCart.Search.Infrastructure;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddAddressesModule(builder.Configuration); 
builder.Services.AddCategoriesModule(builder.Configuration);
builder.Services.AddBrandsModule(builder.Configuration);
builder.Services.AddProductsModule(builder.Configuration);
builder.Services.AddProductImagesModule(builder.Configuration);
builder.Services.AddInventoryModule(builder.Configuration);
builder.Services.AddCartItemsModule(builder.Configuration);
builder.Services.AddOrdersModule(builder.Configuration);
builder.Services.AddPaymentsModule(builder.Configuration);
builder.Services.AddOrderItemsModule(builder.Configuration);
builder.Services.AddSearchModule(builder.Configuration);

// RabbitMQ bağlantı bilgilerini appsettings'ten okuyoruz.
var rabbitMqHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
var rabbitMqUsername = builder.Configuration["RabbitMQ:Username"] ?? "guest";
var rabbitMqPassword = builder.Configuration["RabbitMQ:Password"] ?? "guest";

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductStockChangedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqHost, "/", host =>
        {
            host.Username(rabbitMqUsername);
            host.Password(rabbitMqPassword);
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(kvp => kvp.Value?.Errors.Count > 0)
            .SelectMany(kvp => kvp.Value!.Errors.Select(error =>
                new FieldError(
                    char.ToLowerInvariant(kvp.Key[0]) + kvp.Key[1..],
                    error.ErrorMessage)))
            .ToList();

        return new BadRequestObjectResult(new { errors });
    };
});

// AppException hatalarını yakalayan global handler.
builder.Services.AddExceptionHandler<AppExceptionHandler>();
builder.Services.AddProblemDetails();

// JWT ayarlarını appsettings.json dosyasından okur.
var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "Jwt konfigürasyonu appsettings.json'da tanımlı değil.");

// JWT doğrulama ayarları.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var jti = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Jti);
                if (string.IsNullOrWhiteSpace(jti))
                {
                    context.Fail("Token missing jti.");
                    return;
                }

                var revokedTokenRepository = context.HttpContext.RequestServices
                    .GetRequiredService<IRevokedTokenRepository>();

                if (await revokedTokenRepository.IsRevokedAsync(jti, context.HttpContext.RequestAborted))
                {
                    context.Fail("Token revoked.");
                }
            }
        };
    });

builder.Services.AddAuthorization();

// Frontend'in backend'e istek gönderebilmesi için CORS politikası.
const string FrontendCorsPolicy = "FrontendCorsPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TechCart.Api",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Login işleminden dönen JWT token'ını giriniz."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Global exception handler.
app.UseExceptionHandler();

// Development ortamında OpenAPI endpoint'ini açar.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// CORS, Authentication'dan önce çalışmalıdır.
app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
