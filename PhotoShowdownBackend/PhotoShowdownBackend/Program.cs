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
using PhotoShowdownBackend.Models;
using PhotoShowdownBackend.Services.MatchConnections;
using PhotoShowdownBackend.Repositories.MatchConnections;
using PhotoShowdownBackend.Facades.MatchConnections;
using PhotoShowdownBackend.Facades.Matches;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
    .WriteTo.File("Logs\\log.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddDbContext<PhotoShowdownDbContext>(options =>
    {
        //options.UseInMemoryDatabase("PhotoShowdownDB");

        // In due time amen
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
);

builder.Services.AddControllers();

// Add facades
builder.Services.AddScoped<IMatchesFacade, MatchesFacade>();
builder.Services.AddScoped<IMatchConnectionsFacade, MatchConnectionsFacade>();

// Add services
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IPicturesService, PicturesService>();
builder.Services.AddScoped<IMatchesService, MatchesService>();
builder.Services.AddScoped<IMatchConnectionsService, MatchConnectionsService>();
builder.Services.AddScoped<ISessionService, SessionService>();

// Add repositories
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IPicturesRepository, PicturesRepository>();
builder.Services.AddScoped<IMatchesReporitory, MatchesReporitory>();
builder.Services.AddScoped<IMatchConnectionsRepository, MatchConnectionsRepository>();


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

builder.Services.AddSwaggerGen(options=>
{
    options.SwaggerDoc("v1",
            new OpenApiInfo
            {
                Title = "Donfil API",
                Version = "v1"
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
builder.Services.AddAuthentication().AddJwtBearer(options=>
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Allow static files
Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath,"wwwroot", "pictures"));
app.UseStaticFiles();

// Use Cors
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
