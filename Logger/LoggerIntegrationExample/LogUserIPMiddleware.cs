using CustomLogger;

namespace LoggerIntegrationExample
{
    public class LogUserIpMiddleware
    {
        readonly RequestDelegate next;
        public LogUserIpMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress;
            var route = context.Request.Path;

            CustomLogger.CustomLogger.GetLogger()
                .LogMessage(this.ToString(),
                    "Запрос с адреса: " + ipAddress + "\n" + 
                    "МаршрутЗапроса: " + route,
                    LogMessageSeverity.Debug);

            await next.Invoke(context);
            return;
        }
    }
}
