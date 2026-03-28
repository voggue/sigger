using Sigger.Generator;
using Sigger.Generator.Server;
using Sigger.UI;
using Sigger.Web.GettingStarted.Hubs.Chat;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSigger(o => o
    .WithHub<ChatHub>("/hubs/v1/chat")
    .WithSchemaEndpointMode(SiggerSchemaEndpointMode.DevelopmentOnly));

builder.Services.AddCors(o =>
    o.AddPolicy("DevCors", p => p
        .WithOrigins("https://localhost:4200", "http://localhost:4200", "https://localhost:7178", "http://localhost:5178")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

var app = builder.Build();
app.UseCors("DevCors");
app.UseSigger();
app.UseSiggerUi(o => o.WithVisibility(SiggerUiVisibility.Always));
app.Run();
