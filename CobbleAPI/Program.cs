using Microsoft.EntityFrameworkCore;
using CobbleAPI.Data;
using CobbleAPI.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JSON serialization options
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Register the DbContext with SQL Server provider
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configure Kestrel to listen on all interfaces
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Always enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Only use HTTPS redirection when not in Docker
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
{
    app.UseHttpsRedirection();
}

// Enable CORS
app.UseCors();

// Apply migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        
        // Wait for SQL Server to be ready (useful in Docker environments)
        var maxRetryCount = 15;  // Increased retry count
        var retryDelayMs = 5000; // 5 seconds
        
        for (int retry = 0; retry < maxRetryCount; retry++)
        {
            try
            {
                // Check database connection
                var canConnect = context.Database.CanConnect();
                Console.WriteLine($"Database connection test: {(canConnect ? "Successful" : "Failed")}");
                
                if (canConnect)
                {
                    // Get and log the actual connection string being used (without password)
                    var connectionString = context.Database.GetConnectionString();
                    if (connectionString != null)
                    {
                        var sanitizedConnectionString = connectionString.Contains("Password=") 
                            ? connectionString.Replace(connectionString.Substring(connectionString.IndexOf("Password="), 
                                connectionString.IndexOf(";", connectionString.IndexOf("Password=")) - connectionString.IndexOf("Password=")), 
                                "Password=***")
                            : connectionString;
                            
                        Console.WriteLine($"Using connection string: {sanitizedConnectionString}");
                    }
                    
                    // Check if we can create the database if it doesn't exist
                    try
                    {
                        context.Database.EnsureCreated();
                        Console.WriteLine("Database exists or was created successfully");
                    }
                    catch (Exception dbCreateEx)
                    {
                        Console.WriteLine($"Error creating database: {dbCreateEx.Message}");
                    }
                    
                    // Apply pending migrations
                    try
                    {
                        context.Database.Migrate();
                        Console.WriteLine("Database migration complete");
                    }
                    catch (Exception migrateEx)
                    {
                        Console.WriteLine($"Error applying migrations: {migrateEx.Message}");
                        throw; // Re-throw to trigger retry
                    }
                    
                    break;
                }
                else
                {
                    throw new Exception("Database connection test failed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration attempt {retry+1} failed: {ex.Message}");
                
                if (retry < maxRetryCount - 1)
                {
                    Console.WriteLine($"Waiting {retryDelayMs/1000} seconds before retrying...");
                    Thread.Sleep(retryDelayMs);
                }
                else
                {
                    Console.WriteLine("Max retry attempts reached. Could not apply migrations.");
                    // Continue anyway - don't let migration failure prevent the app from starting
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
    }
}

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Make sure we can connect to the database
        bool canConnect = false;
        for (int i = 0; i < 10; i++)
        {
            try 
            {
                canConnect = dbContext.Database.CanConnect();
                if (canConnect) break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection attempt {i+1} failed: {ex.Message}");
                Thread.Sleep(2000); // Wait 2 seconds before retry
            }
        }
        
        if (!canConnect)
        {
            Console.WriteLine("Could not connect to the database after multiple attempts");
            // Continue anyway - don't prevent app startup
        }
        else
        {
            // Create the database if it doesn't exist
            dbContext.Database.EnsureCreated();
            Console.WriteLine("Database created or already exists");
            
            // Check if the Accounts table exists
            bool tableExists = false;
            try 
            {
                // Try a simple query to see if the table exists
                _ = dbContext.Accounts.Count();
                tableExists = true;
            }
            catch 
            {
                tableExists = false;
            }
            
            if (!tableExists)
            {
                Console.WriteLine("Creating Accounts table...");
                // Create the Accounts table
                dbContext.Database.ExecuteSqlRaw(@"
                    CREATE TABLE Accounts (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        Email NVARCHAR(450) NOT NULL,
                        Password NVARCHAR(MAX) NOT NULL
                    );
                    CREATE UNIQUE INDEX IX_Accounts_Email ON Accounts(Email);
                ");
                Console.WriteLine("Accounts table created successfully");
            }
            else
            {
                Console.WriteLine("Accounts table already exists");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing database: {ex.Message}");
        // Continue to start the app even if db init fails
    }
}

// Basic error handler endpoint
app.MapGet("/error", () => Results.Problem("An error occurred."))
   .ExcludeFromDescription();

// Root endpoint
app.MapGet("/", () => Results.Ok(new { Message = "CobbleAPI is running", Timestamp = DateTime.UtcNow }))
   .WithName("Root")
   .WithOpenApi();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck")
   .WithOpenApi();

// Explicitly register endpoint methods
var accountPostEndpoint = app.MapPost("/account", async (CreateAccountRequest request, AppDbContext dbContext) =>
{
    Console.WriteLine($"POST /account called with email: {request.Email}");

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
});
accountPostEndpoint.WithName("CreateAccount").WithOpenApi();

// GET endpoint to retrieve account info by ID
var accountGetEndpoint = app.MapGet("/account/{id}", async (int id, AppDbContext dbContext) =>
{
    Console.WriteLine($"GET /account/{id} called");
    
    try {
        var account = await dbContext.Accounts.FindAsync(id);
        
        if (account == null)
        {
            return Results.NotFound("Account not found");
        }

        return Results.Ok(new AccountResponse
        {
            Id = account.Id,
            Email = account.Email,
            // Don't return the password in production
            Password = app.Environment.IsDevelopment() ? account.Password : null
        });
    }
    catch (Exception ex) {
        Console.WriteLine($"Error retrieving account {id}: {ex.Message}");
        return Results.Problem($"Error retrieving account: {ex.Message}");
    }
});
accountGetEndpoint.WithName("GetAccount").WithOpenApi();

// GET endpoint to see all accounts (for development/admin purposes)
var accountsGetEndpoint = app.MapGet("/accounts", async (AppDbContext dbContext) =>
{
    Console.WriteLine("GET /accounts called");
    
    try {
        var accounts = await dbContext.Accounts.ToListAsync();
        return Results.Ok(accounts.Select(a => new AccountResponse
        {
            Id = a.Id,
            Email = a.Email,
            // Don't return the password in production
            Password = app.Environment.IsDevelopment() ? a.Password : null
        }));
    }
    catch (Exception ex) {
        Console.WriteLine($"Error retrieving accounts: {ex.Message}");
        return Results.Problem($"Error retrieving accounts: {ex.Message}");
    }
});
accountsGetEndpoint.WithName("GetAllAccounts").WithOpenApi();

// DELETE endpoint to remove an account
app.MapDelete("/account/{id}", async (int id, AppDbContext dbContext) =>
{
    var account = await dbContext.Accounts.FindAsync(id);
    
    if (account == null)
    {
        return Results.NotFound("Account not found");
    }

    dbContext.Accounts.Remove(account);
    await dbContext.SaveChangesAsync();

    return Results.Ok(new { Message = "Account deleted successfully" });
})
.WithName("DeleteAccount")
.WithOpenApi();

// PUT endpoint to update an account
app.MapPut("/account/{id}", async (int id, UpdateAccountRequest request, AppDbContext dbContext) =>
{
    var account = await dbContext.Accounts.FindAsync(id);
    
    if (account == null)
    {
        return Results.NotFound("Account not found");
    }

    // Update account properties if provided
    if (!string.IsNullOrEmpty(request.Email))
    {
        // Check if the new email is already in use by another account
        if (request.Email != account.Email && 
            await dbContext.Accounts.AnyAsync(a => a.Email == request.Email))
        {
            return Results.BadRequest("Email already in use");
        }
        
        account.Email = request.Email;
    }

    if (!string.IsNullOrEmpty(request.Password))
    {
        account.Password = request.Password; // In production, hash the password
    }

    await dbContext.SaveChangesAsync();

    return Results.Ok(new AccountResponse
    {
        Id = account.Id,
        Email = account.Email
    });
})
.WithName("UpdateAccount")
.WithOpenApi();

app.Run();