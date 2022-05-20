using CustomLogger;
using LoggerIntegrationExample;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var logger = CustomLogger.CustomLogger.GetLogger();

app.UseMiddleware<LogUserIpMiddleware>();
app.UseMiddleware<ExceptionsCatchMiddleware>();

app.Map("/throwexception", (context) =>
{
    throw new Exception("Специально выброшеное исключение");
});

app.Map("/logwarn", async (context) =>
{
    if(context.Request.Query.Count == 0)
    {
        CustomLogger.CustomLogger.GetLogger().LogMessage(
            app.ToString(),
            "0 arguments transfered",
            LogMessageSeverity.Error);
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("0 arguments transfered");
    }
    string message = "";
    foreach (var param in context.Request.Query)
    {
        message += "[ " + param.Key + " : " + param.Value + " ], ";
    }
    message = message.Substring(0, message.Length - 2);
    message += ".";
    CustomLogger.CustomLogger.GetLogger().LogMessage(
        app.ToString(),
        message,
        LogMessageSeverity.Warning
        );
    context.Response.StatusCode = 200;
    await context.Response.WriteAsync(message);
});

app.Map("/", async (context) =>
{
    await context.Response.WriteAsync("a");
});

//context.Request.Query

app.Run();
