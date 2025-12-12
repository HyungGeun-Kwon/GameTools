using GameTools.Server.Application.Abstractions.Users;

namespace GameTools.Server.Api.Middlewares
{
    public sealed class CurrentUserMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext ctx, ICurrentUser currentUser)
        {
            // 인증 사용 시
            var actor = ctx.User?.Identity?.Name;

            // 무인증/개발 환경시
            actor ??= ctx.Request.Headers["X-Actor"].Count > 0
                ? ctx.Request.Headers["X-Actor"].ToString()
                : null;

            currentUser.Set(actor ?? "api_unknown");
            await next(ctx);
        }
    }
}
