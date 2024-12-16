using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

public class SessionTimeoutMiddleware
{
    private readonly RequestDelegate _next;

    public SessionTimeoutMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Chỉ kiểm tra session timeout khi user đã đăng nhập
        if (context.User.Identity.IsAuthenticated)
        {
            if (!context.Session.TryGetValue("LastActivity", out _))
            {
                context.Session.Set("LastActivity", BitConverter.GetBytes(DateTime.UtcNow.Ticks));
            }
            else
            {
                var lastActivity = new DateTime(BitConverter.ToInt64(context.Session.Get("LastActivity"), 0));
                if (DateTime.UtcNow - lastActivity > TimeSpan.FromMinutes(60))
                {
                    await context.SignOutAsync(IdentityConstants.ApplicationScheme);
                    context.Session.Clear();
                    context.Response.Redirect("/Identity/Account/Login");
                    return;
                }
                context.Session.Set("LastActivity", BitConverter.GetBytes(DateTime.UtcNow.Ticks));
            }
        }

        await _next(context);
    }
} 