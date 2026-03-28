using Microsoft.EntityFrameworkCore;
using Sigger;
using Sigger.Generator;
using Sigger.Generator.Server;
using Sigger.Web.Demo.Data;
using Sigger.Web.Demo.Hubs;
using Sigger.Web.Demo.Models;
using Sigger.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSigger(o => o
    .WithHub<ChatHub>("/hubs/v1/chat")
    .WithSchemaEndpointMode(SiggerSchemaEndpointMode.DevelopmentOnly)
    .WithGeneratorOptions(g =>
    {
        if (builder.Environment.IsProduction())
            g.IncludeClrMetadataInSchema = false;
    })
);
builder.Services.AddSiggerRepository();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
                  ?? ["https://localhost:44496", "https://localhost:5001"];
builder.Services.AddCors(o =>
    o.AddDefaultPolicy(p => p
        .WithOrigins(corsOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

var app = builder.Build();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSiggerUi();
app.UseSigger();

app.Run();
