using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Hashing;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Token;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.QueryServices;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Hashing.BCrypt.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Persistence.Repositories;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Tokens.JWT.Configuration;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Tokens.JWT.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Pipeline.Middleware.Extensions;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.OutboundServices.FileStorage;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.QueryServices;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.FileStorage.Cloudinary.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.QueryServices;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Persistence.MongoDB.Repositories;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Application.Internal.ACL;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Application.Internal.OutBoundServices.FileStorage;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Application.QueryServices;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Infrastructure.FileStorage.Cloudinary.Services;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Infrastructure.Persistence.MongoDB.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Interfaces.ASP.Configuration.Namings;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Namings;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Seeding;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cortex.Mediator.Behaviors;
using Cortex.Mediator.DependencyInjection;
using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Application.ACL;
using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Application.Internal.QueryServices;
using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Domain.Services;
using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Infrastructure.Persistence.EFC.Repositories;
using LiquoTrack.StocksipPlatform.API.AlertsAndNotifications.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Authentication;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Email;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Email.Gmail.Confirguration;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Email.Gmail.Services;

using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.External.ACL;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.ACL;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.EventHandlers;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Events;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Application.ACL;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.Shared.Application.Internal.EventHandlers;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.FileStorage.Cloudinary.Configuration;
using Microsoft.AspNetCore.Authentication.Google;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Infrastructure.Persistence.MongoDB.Repositories;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Application.Internal.QueryServices;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Application.Internal.Services;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.OutBoundServices.Jobs.Hosted;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.OutBoundServices.PaymentProviders.services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Jobs.Hosted;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Jobs.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.PaymentProviders.MercadoPago.Configuration;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.PaymentProviders.MercadoPago.Services;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Application.ACL;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Application.Internal.QueryServices;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Infrastructure.Converters.JSON;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Infrastructure.Persistence.MongoDB.Repositories;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.ACL;

// Register MongoDB mappings
GlobalMongoMappingHelper.RegisterAllBoundedContextMappings();

// Create a builder for the web application
var builder = WebApplication.CreateBuilder(args);

// Add logger
var loggerFactory = LoggerFactory.Create(config =>
{
    config.AddConsole();
    config.AddDebug();
});
var logger = loggerFactory.CreateLogger("Program");

// Configuration shortcuts
var configuration = builder.Configuration;
var env = builder.Environment;

// Services 
builder.Services.AddRouting(o => o.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()));
builder.Services.AddEndpointsApiExplorer();

// HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register MongoDB conventions for camel case naming
CamelCaseFieldNamingConvention.UseCamelCaseNamingConvention();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policyBuilder =>
    {
        policyBuilder.WithOrigins(
                "https://localhost:7164",
                "http://localhost:5283",
                "https://localhost:44355")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Add Cortex Mediator
builder.Services.AddCortexMediator(
    builder.Configuration,
    [typeof(Program)],
    options => options.AddOpenCommandPipelineBehavior(typeof(LoggingCommandBehavior<>)));

// Dependency Injection

// Registers the MongoDB client as a singleton service
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var cs = builder.Configuration["MongoDB:ConnectionString"];
    return string.IsNullOrEmpty(cs)
        ? throw new InvalidOperationException("MongoDB connection string is not configured")
        : new MongoClient(cs);
});

// Register IMongoDatabase
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var dbName = configuration["MongoDB:DatabaseName"];
    return string.IsNullOrEmpty(dbName)
        ? throw new InvalidOperationException("MongoDB database name is not configured")
        : client.GetDatabase(dbName);
});

// Add service for MongoDB client
builder.Services.AddSingleton<AppDbContext>();

//
// Bounded Context Shared
//

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<DatabaseSeeder>();

// Shared JSON converters
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new AccountIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new UserIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EmailJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ImageUrlJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new InventoryIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new PurchaseOrderIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new CatalogIdJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new MoneyJsonConverter());
});

//
// Bounded Context Alerts and Notifications
//

builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IAlertCommandService, AlertCommandService>();
builder.Services.AddScoped<IAlertQueryService, AlertQueryService>();
builder.Services.AddScoped<IAlertsAndNotificationsContextFacade, AlertsAndNotificationsContextFacade>();

//
// Authentication Bounded Context
//

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();

// Google Identity Services configuration and validator
builder.Services.Configure<LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.Settings.GoogleAuthSettings>(
    builder.Configuration.GetSection("Authentication:Google"));
builder.Services.AddScoped<
    LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Authentication.IExternalAuthService,
    LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.GoogleTokenValidator>();

// JWT Configuration
builder.Services.Configure<TokenSettings>(
    builder.Configuration.GetSection("Jwt"));

// Register Token Service
builder.Services.AddScoped<ITokenService, TokenService>();

// Register Hashing Service
builder.Services.AddScoped<IHashingService, HashingService>();

// Register token validator and authentication services




// Using a fully qualified name to resolve ambiguity


// JWT Configuration
builder.Services.Configure<LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Tokens.JWT.Configuration.TokenSettings>(
    builder.Configuration.GetSection("Jwt"));

// Register Token Service
builder.Services.AddScoped<ITokenService, TokenService>();

// Google Auth Service 
 


// Cloudinary Settings Configuration
builder.Services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

// MercadoPago Settings Configuration
builder.Services.Configure<MercadoPagoSettings>(builder.Configuration.GetSection("MercadoPagoSettings"));

// Gmail Settings Configuration
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpServiceSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

// Subscriptions Job
builder.Services.AddSingleton<ISubscriptionsExpirationService, SubscriptionsExpirationService>();
builder.Services.AddHostedService<SubscriptionsExpirationJob>();

//
// Bounded context Inventory
//
builder.Services.AddScoped<ITypeRepository, TypeRepository>();
builder.Services.AddScoped<ITypeQueryService, TypeQueryService>();

builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IBrandQueryService, BrandQueryService>();

builder.Services.AddScoped<IProductExitQueryService, ProductExitQueryService>();
builder.Services.AddScoped<IProductExitRepository, ProductExitRepository>();

builder.Services.AddScoped<IProductTransferQueryService, ProductTransferQueryService>();
builder.Services.AddScoped<IProductTransferRepository, ProductTransferRepository>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductQueryService, ProductQueryService>();
builder.Services.AddScoped<IProductCommandService, ProductCommandService>();

builder.Services.AddScoped<IInventoryCommandService, InventoryCommandService>();
builder.Services.AddScoped<IInventoryQueryService, InventoryQueryService>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

builder.Services.AddScoped<IInventoryImageService, InventoryImageService>();
builder.Services.AddScoped<ExternalAlertsAndNotificationsService>();

builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IWarehouseCommandService, WarehouseCommandService>();
builder.Services.AddScoped<IWarehouseQueryService, WarehouseQueryService>();

builder.Services.AddScoped<ICareGuideRepository, CareGuideRepository>();
builder.Services.AddScoped<ICareGuideQueryService, CareGuideQueryService>();
builder.Services.AddScoped<ICareGuideCommandService, CareGuideCommandService>();

// Registers the events handlers for the events of the context
builder.Services.AddScoped<IEventHandler<ProductWithLowStockDetectedEvent>, ProductWithLowStockDetectedEventHandler>();
builder.Services.AddScoped<IEventHandler<ProductWithoutStockDetectedEvent>, ProductWithoutStockDetectedEventHandler>();

// Registers the JSON Converts of the context
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new EBrandNamesJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EProductStatesJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EProductTypesJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductContentJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductExpirationDateJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductMinimumStockJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductNameJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ProductStockJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new WarehouseAddressJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new WarehouseCapacityJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new WarehouseTemperatureJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new CareGuideJsonConverter());
});

//
// Bounded Context Procurement Ordering
//
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IPurchaseOrderCommandService, PurchaseOrderCommandService>();
builder.Services.AddScoped<IPurchaseOrderQueryService, PurchaseOrderQueryService>();

builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
builder.Services.AddScoped<ICatalogCommandService, CatalogCommandService>();
builder.Services.AddScoped<ICatalogQueryService, CatalogQueryService>();

builder.Services.AddScoped<IProductContextFacade, ProductContextFacade>();
builder.Services.AddScoped<IProcurementOrderingFacade, ProcurementOrderingFacade>();

// Purchase Order converters
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new EOrderStatusJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new PurchaseOrderItemJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new CatalogItemJsonConverter());
});

//
// Bounded Context Order Management
//

builder.Services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
builder.Services.AddScoped<ISalesOrderCommandService, SalesOrderCommandService>();
builder.Services.AddScoped<ISalesOrderQueryService, SalesOrderQueryService>();
builder.Services.AddScoped<IOrderManagementContextFacade, OrderManagementContextFacade>();
builder.Services.AddScoped<ILowStockService, LowStockService>();

//
// Bounded Context Profile Management
//
builder.Services.AddScoped<IProfilesImageService, ProfilesImageService>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileQueryService, ProfileQueryService>();
builder.Services.AddScoped<IProfileCommandService, ProfileCommandService>();

builder.Services.AddScoped<IProfileContextFacade, ProfileContextFacade>();

// Profile converters
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new PersonContactNumberJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new PersonNameJsonConverter());
});

//
// Bounded context Payment & Subscriptions
//

builder.Services.AddScoped<IPlanQueryService, PlanQueryService>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();

builder.Services.AddScoped<IAccountQueryService, AccountQueryService>();
builder.Services.AddScoped<IAccountCommandService, AccountCommandService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<IBusinessCommandService, BusinessCommandService>();
builder.Services.AddScoped<IBusinessQueryService, BusinessQueryService>();
builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();

builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();
builder.Services.AddScoped<ISubscriptionsCommandService, SubscriptionCommandService>();

builder.Services.AddScoped<IPaymentAndSubscriptionsFacade, PaymentAndSubscriptionsFacade>();

builder.Services.AddScoped<IMercadoPagoService, MercadoPagoService>();

// Payment converters
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new BusinessEmailJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new BusinessNameJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EAccountRoleJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EAccountStatusesJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EPaymentFrequencyJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new EPlanTypeJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new ESubscriptionStatusJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new PlanLimitsJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new RucJsonConverter());
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Authentication: hashing and token service
builder.Services.AddScoped<IHashingService, HashingService>(); // BCrypt
builder.Services.AddScoped<ITokenService, TokenService>();

// Token settings 
builder.Services.Configure<TokenSettings>(configuration.GetSection("Jwt"));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddSwaggerGen(c =>
{
    // SwaggerDoc
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LiquoTrack.StocksipPlatform.API",
        Version = "v1",
        Description = "API for LiquoTrack Stock Management System",
        TermsOfService = new Uri("https://stocksip.com/tos"),
        Contact = new OpenApiContact { Name = "StockSip", Email = "contact@stocksip.com" },
        License = new OpenApiLicense
        {
            Name = "Apache 2.0",
            Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
        }
    });

    // Annotations
    c.EnableAnnotations();
    c.CustomSchemaIds(t => t.FullName);

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter JWT Bearer token (without 'Bearer ' prefix)"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    // Add OAuth2 Configuration
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
                TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID" },
                    { "profile", "Profile" },
                    { "email", "Email" }
                }
            }
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Google credentials from settings
var googleAuthSection = builder.Configuration.GetSection("Authentication:Google");

var googleClientId = googleAuthSection["ClientId"] ??
                     throw new InvalidOperationException("Google ClientId no está configurado en appsettings.json");

var googleClientSecret = googleAuthSection["ClientSecret"] ??
                         throw new InvalidOperationException("Google ClientSecret no está configurado en appsettings.json");



if (!string.IsNullOrWhiteSpace(googleClientId))
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.IncludeErrorDetails = true;
        options.RequireHttpsMetadata = false; // For development only

        var jwtSettings = builder.Configuration.GetSection("Jwt");
        // JWT configuration
        var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ??
            throw new InvalidOperationException("JWT Secret no está configurado"));

        // Get JWT settings from configuration
        var validateIssuer = jwtSettings.GetValue<bool>("ValidateIssuer");
        var validateAudience = jwtSettings.GetValue<bool>("ValidateAudience");
        var validateLifetime = jwtSettings.GetValue<bool>("ValidateLifetime");
        var validateIssuerSigningKey = jwtSettings.GetValue<bool>("ValidateIssuerSigningKey");
        var requireExpirationTime = jwtSettings.GetValue<bool>("RequireExpirationTime", true);
        var clockSkew = TimeSpan.FromMinutes(jwtSettings.GetValue<int>("ClockSkew", 30));

        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validate issuer
            ValidateIssuer = validateIssuer,
            ValidIssuers = [jwtSettings["Issuer"], "https://accounts.google.com"],

            // Validate audience
            ValidateAudience = validateAudience,
            ValidAudiences = [jwtSettings["Audience"], jwtSettings["ClientId"]],

            // Validate token lifetime
            ValidateLifetime = validateLifetime,
            ClockSkew = clockSkew,

            // Configure issuer signing key
            ValidateIssuerSigningKey = validateIssuerSigningKey,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            // Other settings
            RequireExpirationTime = requireExpirationTime,
            RequireSignedTokens = jwtSettings.GetValue<bool>("RequireSignedTokens", false),

            // Configuración de claims
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };

        // Log the JWT validation parameters for debugging
        logger.LogInformation("JWT Validation Parameters:");
        logger.LogInformation("- ValidateIssuer: {ValidateIssuer}", validateIssuer);
        logger.LogInformation("- ValidateAudience: {ValidateAudience}", validateAudience);
        logger.LogInformation("- ValidateLifetime: {ValidateLifetime}", validateLifetime);
        logger.LogInformation("- ValidateIssuerSigningKey: {ValidateIssuerSigningKey}", validateIssuerSigningKey);
        logger.LogInformation("- ClockSkew: {ClockSkew}", clockSkew);

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Headers.Authorization.ToString();
                if (!string.IsNullOrEmpty(accessToken) && accessToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = accessToken["Bearer ".Length..].Trim();
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var tokenLogger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var identity = context.Principal?.Identity as ClaimsIdentity;

                tokenLogger.LogInformation("=== TOKEN VALIDADO ===");
                tokenLogger.LogInformation("Usuario autenticado: {User}", context.Principal?.Identity?.Name);
                tokenLogger.LogInformation("Autenticado: {IsAuthenticated}", context.Principal?.Identity?.IsAuthenticated);

                tokenLogger.LogInformation("=== CLAIMS DEL TOKEN ===");
                Debug.Assert(context.Principal != null, "context.Principal != null");

                foreach (var claim in context.Principal.Claims)
                {
                    tokenLogger.LogInformation("Claim - Tipo: {Type}, Valor: {Value}", claim.Type, claim.Value);

                    if (claim.Type is not ("role" or "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"))
                        continue;

                    tokenLogger.LogInformation("Rol encontrado: {Role}", claim.Value);

                    if (context.Principal.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == claim.Value))
                        continue;

                    var newClaim = new Claim(ClaimTypes.Role, claim.Value);
                    identity?.AddClaim(newClaim);
                    tokenLogger.LogInformation("Añadido claim de rol: {Role}", claim.Value);
                }

                var hasAdminRole = context.Principal.IsInRole("Admin");
                tokenLogger.LogInformation("¿Tiene rol Admin? {HasAdminRole}", hasAdminRole);

                tokenLogger.LogInformation("=== FIN DE VALIDACIÓN DE TOKEN ===");

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var localLogger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                localLogger.LogError(context.Exception, "Error de autenticación");

                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.Append("Token-Expired", "true");
                    logger.LogError("El token ha expirado");
                }

                switch (context.Exception)
                {
                    case SecurityTokenInvalidIssuerException:
                        logger.LogError("Emisor del token no válido. Se esperaba: {ValidIssuers}", context.Options.TokenValidationParameters.ValidIssuers);
                        break;

                    case SecurityTokenInvalidAudienceException:
                        logger.LogError("Audiencia del token no válida. Se esperaba: {Join}", string.Join(", ", context.Options.TokenValidationParameters.ValidAudiences));
                        break;
                }

                return Task.CompletedTask;
            }
        };
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException();
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? throw new InvalidOperationException();
        options.CallbackPath = "/signin-google";
        options.AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        options.TokenEndpoint = "https://oauth2.googleapis.com/token";
        options.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.SaveTokens = true;

        options.Events.OnCreatingTicket = async context =>
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

                var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                response.EnsureSuccessStatusCode();

                var user = await response.Content.ReadFromJsonAsync<JsonElement>();

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.GetString("id") ?? string.Empty),
                    new(ClaimTypes.Name, user.GetString("name") ?? string.Empty),
                    new(ClaimTypes.Email, user.GetString("email") ?? string.Empty),
                    new(ClaimTypes.GivenName, user.GetString("given_name") ?? string.Empty),
                    new(ClaimTypes.Surname, user.GetString("family_name") ?? string.Empty),
                    new("http://schemas.microsoft.com/identity/claims/identityprovider", "Google"),
                    new Claim(ClaimTypes.Role, "User")
                };

                var identity = new ClaimsIdentity(claims, context.Scheme.Name);
                context.Principal = new ClaimsPrincipal(identity);

                context.Success();
            }
            catch (Exception ex)
            {
                var localLogger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                localLogger.LogError(ex, "Error creating authentication ticket");
                context.Fail(ex);
            }
        };
    });
}
else
{
    logger.LogWarning("Authentication:Google:ClientId is not configured. The token validations of Google ARE NOT active");
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    try
    {
        await seeder.SeedAsync();
        logger.LogInformation("Database seeding finished with success.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error while executing DatabaseSeeder.SeedAsync(). Check the configuration/data of the method.");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LiquoTrack API V1");
        c.OAuthClientId("520776661353-aq0nbie37i8742tnn0167ak4bdadk2cu.apps.googleusercontent.com");
        c.OAuthAppName("LiquoTrack API - Swagger");
        c.OAuthUsePkce();
        c.OAuth2RedirectUrl("https://localhost:7164/swagger/oauth2-redirect.html");
    });
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();

    var enableSwaggerInProduction = configuration.GetValue<bool>("EnableSwaggerInProduction");
    if (enableSwaggerInProduction)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "LiquoTrack API V1");
        });
    }
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowSpecificOrigins");

if (!string.IsNullOrWhiteSpace(googleClientId))
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.Map("/error", (HttpContext http) =>
{
    var exFeature = http.Features.Get<IExceptionHandlerFeature>();
    if (exFeature?.Error == null) return Results.Problem("Unknown error");
    var err = exFeature.Error;
    http.Response.StatusCode = 500;
    return Results.Problem(detail: err.Message, title: "Unhandled exception");
});

// Configure the Authentication HTTP request pipeline.
app.UseRequestAuthorization();

app.MapControllers();

try
{
    logger.LogInformation("Starting application. Environment: {env}", env.EnvironmentName);
    app.Run();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Host terminated unexpectedly");
    throw;
}

app.Run();

// For usage of testing projects
public partial class Program { }