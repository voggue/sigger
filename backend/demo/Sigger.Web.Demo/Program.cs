using Microsoft.EntityFrameworkCore;
using Sigger;
using Sigger.Generator;
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
);
builder.Services.AddSiggerRepository();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// builder.Services.AddIdentityServer()
//     .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

// builder.Services.AddAuthentication()
//     .AddIdentityServerJwt();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


builder.Services.AddCors(o =>
    o.AddDefaultPolicy(c => c
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    ));

var app = builder.Build();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSiggerUi();
app.UseSigger();

// app.UseAuthentication();
// app.UseIdentityServer();
// app.UseAuthorization();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller}/{action=Index}/{id?}");
// app.MapRazorPages();
// app.MapFallbackToFile("index.html");

app.Run();
