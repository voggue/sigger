using Sigger.Generator;
using Sigger.UI;
using Sigger.Web.GettingStarted.Hubs.Chat;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSigger();

var app = builder.Build();

app.UseSiggerUi();
app.UseSigger(o => o
    .WithHub<ChatHub>("/hubs/v1/chat")
);


app.Run();