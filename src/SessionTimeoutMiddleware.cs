namespace Manage_CLB_HTSV
{
    public class SessionTimeoutMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionTimeoutMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Session.SetString("SessionExpired", "Phiên làm việc của bạn đã hết hạn.");
            }

            await _next(context);
        }
    }

}
