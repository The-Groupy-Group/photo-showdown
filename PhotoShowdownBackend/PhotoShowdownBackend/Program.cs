using System.Text;
using PhotoShowdownBackend.Data;
using PhotoShowdownBackend.Services.Users;
using PhotoShowdownBackend.Repositories.Users;
using PhotoShowdownBackend.Consts;
using PhotoShowdownBackend.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using Serilog;
using System.Reflection;
using PhotoShowdownBackend.Services.Session;
using PhotoShowdownBackend.Services.Pictures;
using PhotoShowdownBackend.Repositories.Pictures;
using PhotoShowdownBackend.Services.Matches;
using PhotoShowdownBackend.Services.MatchConnections;
using PhotoShowdownBackend.Repositories.MatchConnections;
using System.Text.Json.Serialization;
using PhotoShowdownBackend.WebSockets;
using PhotoShowdownBackend.Middlewares;
using System.Text.Json;
using PhotoShowdownBackend.Repositories.Rounds;
using PhotoShowdownBackend.Services.Rounds;
using PhotoShowdownBackend.Services.CustomSentences;
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Repositories.CustomSentences;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using PhotoShowdownBackend.Repositories.RoundPictures;
using PhotoShowdownBackend.Repositories.RoundVotes;

var builder = WebApplication.CreateBuilder(args);

// Create all required folders if they dont exist
Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "wwwroot", SystemSettings.PicturesFolderName));

// Add Serilog
string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}\n";
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
        outputTemplate: outputTemplate)
    .WriteTo.File(
        "Logs\\log.txt",
        outputTemplate: outputTemplate,
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
        retainedFileCountLimit: 3)
    .CreateLogger();

builder.Host.UseSerilog();

// Build the DB
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
try
{
    Log.Logger.Information("Building the DB");
    var migrationExecutor = new SQLServerMigrationExecutor(connectionString);
    migrationExecutor.ExecutePendingScripts();
}
catch (Exception ex)
{
    Log.Logger.Error(ex, "Failed to build the DB");
    throw;
}


// Add services to the container.
builder.Services.AddDbContext<PhotoShowdownDbContext>(options =>
    {
        // Swap between InMemoryDatabase and SqlServer
        //options.UseInMemoryDatabase("PhotoShowdownDB");

        options.UseSqlServer(connectionString);
    }
);

// Init the DB by ending all in progress matches (In case the server was restarted)
Log.Logger.Information("Initializing the DB");
DbContextOptions<PhotoShowdownDbContext> options = new DbContextOptionsBuilder<PhotoShowdownDbContext>()
    .UseSqlServer(connectionString)
    .Options;
var dbContext = new PhotoShowdownDbContext(options);
await DatabaseInitializer.Initialize(dbContext);
await dbContext.DisposeAsync();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());

    // Save options.JsonSerializerOptions to be glovaly accessible
    SystemSettings.JsonSerializerOptions = options.JsonSerializerOptions;
});


// Add services
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IPicturesService, PicturesService>();
builder.Services.AddScoped<IMatchesService, MatchesService>();
builder.Services.AddScoped<IMatchConnectionsService, MatchConnectionsService>();
builder.Services.AddScoped<IRoundsService, RoundsService>();
builder.Services.AddScoped<ISentencesService, SentencesService>();

// Add repositories
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IPicturesRepository, PicturesRepository>();
builder.Services.AddScoped<IMatchesRepository, MatchesRepository>();
builder.Services.AddScoped<IMatchConnectionsRepository, MatchConnectionsRepository>();
builder.Services.AddScoped<IRoundsRepository, RoundsRepository>();
builder.Services.AddScoped<ICustomSentencesRepository, CustomSentencesRepository>();
builder.Services.AddScoped<IRoundPicturesRepository, RoundPicturesRepository>();
builder.Services.AddScoped<IRoundVotesRepository, RoundVotesRepository>();

// Add SessionService
builder.Services.AddScoped<ISessionService, SessionService>();

// Add WebSocketManager
builder.Services.AddSingleton<WebSocketRoomManager>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingConfig).Assembly);

// Adding HttpContextAccessor as injectable so services can access HttpContext
builder.Services.AddHttpContextAccessor();

// Add Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", build =>
    {
        build.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
            new OpenApiInfo
            {
                Title = "Donfil API",
                Version = "0.5.0"
            });

    // Add JWT support for swagger
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' following by space and JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();


    // Include the XML comments in Swagger documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// Add authentication
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    // Configure JWT bearer options
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
        RoleClaimType = UserClaims.Roles,
        NameClaimType = UserClaims.Username
    };
});

// Add RateLimiter
builder.Services.AddRateLimiter(options =>
{
    options.AddTokenBucketLimiter("token", options =>
    {
        options.TokenLimit = 100;
        options.ReplenishmentPeriod = TimeSpan.FromSeconds(5);
        options.TokensPerPeriod = 10;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Allow static files
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers",
          "Origin, X-Requested-With, Content-Type, Accept");
    }
});

// Use Cors
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseWebSockets();

app.UseRateLimiter();

app.Run();
