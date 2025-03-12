using Microsoft.EntityFrameworkCore;
using CobbleAPI.Data;
using CobbleAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the DbContext with SQL Server provider
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// POST endpoint to create a new account
app.MapPost("/account", async (CreateAccountRequest request, AppDbContext dbContext) =>
{
    // Validate input
    if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
    {
        return Results.BadRequest("Email and password are required");
    }

    // Check if email already exists
    if (await dbContext.Accounts.AnyAsync(a => a.Email == request.Email))
    {
        return Results.BadRequest("Email already in use");
    }

    // Create new account
    var account = new Account
    {
        Email = request.Email,
        Password = request.Password // In production, this should be hashed
    };

    dbContext.Accounts.Add(account);
    await dbContext.SaveChangesAsync();

    // Return the account with its assigned ID
    return Results.Created($"/account/{account.Id}", new AccountResponse
    {
        Id = account.Id,
        Email = account.Email
    });
})
.WithName("CreateAccount")
.WithOpenApi();

// GET endpoint to retrieve account info by ID
app.MapGet("/account/{id}", async (int id, AppDbContext dbContext) =>
{
    var account = await dbContext.Accounts.FindAsync(id);
    
    if (account == null)
    {
        return Results.NotFound("Account not found");
    }

    return Results.Ok(new AccountResponse
    {
        Id = account.Id,
        Email = account.Email,
        Password = account.Password // In production, you might not want to return the password
    });
})
.WithName("GetAccount")
.WithOpenApi();

app.Run();