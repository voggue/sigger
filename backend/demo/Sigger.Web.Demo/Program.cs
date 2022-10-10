using Microsoft.EntityFrameworkCore;
using Sigger;
using Sigger.Generator;
using Sigger.Web.Demo.Data;
using Sigger.Web.Demo.Hubs;
using Sigger.UI;
using Sigger.Web.Demo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.AddAppIdentity();

builder.Services.AddSigger(o => o
    .WithHub<ChatHub>("/hubs/v1/chat")
);
builder.Services.AddSiggerRepository();

builder.Services.AddControllers();

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

app.UseAuthentication();
app.UseAuthorization();

app.UseSigger();
app.UseSiggerUi();

app.Run();