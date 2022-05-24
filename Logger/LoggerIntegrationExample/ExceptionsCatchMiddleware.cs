using Customlogging;

namespace LoggerIntegrationExample
{
    public class ExceptionsCatchMiddleware
    {
        readonly RequestDelegate next;
        public ExceptionsCatchMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch(Exception ex)
            {
                CustomLogger.GetLogger()
                    .LogException(this.ToString() ?? "ExceptionsCatchMiddleware", ex, false);
            }
            return;
        }
    }
}
