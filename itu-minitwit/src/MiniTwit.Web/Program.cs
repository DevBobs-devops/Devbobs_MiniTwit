using Chirp.Core;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Chirp.Services;
using Chirp.Infrastructure.data;
using Chirp.Web;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);


string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); //Takes default connection from appsettings.json to use for db

//If we are in production we use postgres, if we are testing, we use sqlite, as we did before
Console.WriteLine("postgres");
    builder.Services.AddDbContext<CheepDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddDefaultIdentity<Author>(options =>   
        options.SignIn.RequireConfirmedAccount = true)            
    .AddEntityFrameworkStores<CheepDbContext>(); 

builder.Services.AddSession();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IFollowRepository, FollowRepository>();
builder.Services.AddScoped<IChirpService, ChirpService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMetricServer();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//If we are in production, seed with an empty dump, else seed with template data
if (app.Environment.IsProduction())
{
    using (var serviceScope = app.Services.CreateScope())
    {
        var context = serviceScope.ServiceProvider.GetRequiredService<CheepDbContext>();
        //Outcommented since we don't need to seed the database as it is located elsewhere
        //context.Database.EnsureCreated();
        //Prod_DbInitializer.SeedDatabase(context); //Should probably not be here anymore
    }
}
else
{
    using (var serviceScope = app.Services.CreateScope())
    {
        var context = serviceScope.ServiceProvider.GetRequiredService<CheepDbContext>();

        //SQLite development db. Here we need to confirm it exists.
        context.Database.EnsureCreated();
        DbInitializer.SeedDatabase(context);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. 
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapRazorPages();

app.MapProductEndpoints();

app.Run(); 

public partial class Program {} 