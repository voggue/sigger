using Sigger.Generator;
using Sigger.UI;
using Sigger.Web.GettingStarted.Hubs.Chat;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSigger(o => o
    .WithHub<ChatHub>("/hubs/v1/chat")
);
builder.Services.AddCors(o =>
    o.AddDefaultPolicy(c => c
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    ));

var app = builder.Build();
app.UseCors();
app.UseSigger();
app.UseSiggerUi();
app.Run();