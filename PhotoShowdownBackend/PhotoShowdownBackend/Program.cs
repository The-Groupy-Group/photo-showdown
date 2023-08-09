global using PhotoShowdownBackend.Models;
global using PhotoShowdownBackend.Data;
global using PhotoShowdownBackend.Services.Users;
global using PhotoShowdownBackend.Repositories.Repository;
global using PhotoShowdownBackend.Dtos.Users;
global using PhotoShowdownBackend.Exceptions.Users;
global using Microsoft.EntityFrameworkCore;
using PhotoShowdownBackend.Repositories.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<PhotoShowdownDbContext>(options =>
    {
        options.UseInMemoryDatabase("PhotoShowdownDB");

        // In due time amen
        //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
);

builder.Services.AddControllers();

// Add repositories
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
