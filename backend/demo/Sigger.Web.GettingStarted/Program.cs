using Sigger.Generator;
using Sigger.UI;
using Sigger.Web.GettingStarted.Hubs.Chat;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSigger(o => o
    .WithHub<ChatHub>("/hubs/v1/chat")
);

var app = builder.Build();

app.UseSiggerUi(o =>
    o.WithIgnoreCaching());
app.UseSigger();


app.Run();