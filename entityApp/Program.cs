using System.Text.Json.Serialization;
using EntityApp.Entities;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.Configure<JsonOptions>(options =>
// {
//     options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
// });
builder.Services.AddDbContext<MyBoardsContext>(
    option => option
    // .UseLazyLoadingProxies()
    .UseSqlServer(builder.Configuration.GetConnectionString("MyBoardsConnectionStrings")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetService<MyBoardsContext>();

var pendingMigrations = dbContext.Database.GetPendingMigrations();

if (pendingMigrations.Any())
{
    dbContext.Database.Migrate();
}

var users = dbContext.Users.ToList();
if (!users.Any())
{
    var user1 = new User()
    {
        Email = "user1@test.com",
        FullName = "User One",
        Address = new Address()
        {
            City = "Warszawa",
            Street = "Szeroka"
        }
    };

    var user2 = new User()
    {
        Email = "user2@test.com",
        FullName = "User Two",
        Address = new Address()
        {
            City = "Kraków",
            Street = "Długa"
        }
    };

    dbContext.Users.AddRange(user1, user2);
    dbContext.SaveChanges();
}

app.MapGet("data", async (MyBoardsContext db) =>
{
    var user = await db.Users
    .Include(u => u.Comments)
    .FirstAsync(user => user.Id == Guid.Parse("68366dbe-0809-490f-cc1d-08da10ab0e61"));

    Console.WriteLine(user.Comments.Count);
    return user;
});

app.MapPost("update", async (MyBoardsContext db) =>
{
    var epic = await db.Epics.FirstAsync(epic => epic.Id == 1);
    epic.StateId = 1;
    // epic.Area = "Updated area";
    // epic.Priority = 1;
    // epic.StartDate = DateTime.Now;
    await db.SaveChangesAsync();

    return epic;
});

app.MapPut("create", async (MyBoardsContext db) =>
{
    Console.WriteLine("ELOOOO");
    var address = new Address()
    {
        Id = Guid.Parse("482dbbd8-6032-4aad-b419-eb2e17bf8bf6"),
        City = "Krupia Wolka",
        Country = "Poland",
        Street = "Gorna"
    };

    var user = new User()
    {
        Email = "user12@test.com",
        FullName = "Test User",
        Address = address
    };

    db.Users.Add(user);

    await db.SaveChangesAsync();
    Console.WriteLine("UDALO SIE");

    return;
});

app.Run();
