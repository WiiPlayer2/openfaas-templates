using DarkLink.OpenFaaS;

var builder = WebApplication.CreateBuilder(args);
builder.AddOpenFaaS();

var app = builder.Build();
app.MapFunction((ctx, arg) => $"Hello {arg} using {ctx.Request.Headers.UserAgent}!");

app.Run();
