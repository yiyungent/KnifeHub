using Hangfire.Dashboard;

namespace HangfirePlugin;

/// <summary>
/// https://docs.hangfire.io/en/latest/configuration/using-dashboard.html#configuring-authorization
/// </summary>
public class HangfirePluginAuthorizationFilter : IDashboardAuthorizationFilter
{
    // private readonly AccountManager _accountManager;

    // public HangfirePluginAuthorizationFilter(AccountManager accountManager)
    // {
    //     _accountManager = accountManager;
    // }

    /// <summary>
    /// 注意: 不是每次请求都会 new 构造函数, 而只有一开始会 new
    /// </summary>
    public HangfirePluginAuthorizationFilter()
    {

    }

    /// <summary>
    /// 每次请求 /hangfire 授权页面都会调用此方法
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Allow all authenticated users to see the Dashboard (potentially dangerous).
        // return httpContext.User.Identity?.IsAuthenticated ?? false;

        var authenticationType = httpContext.User.Identity?.AuthenticationType;

        // AspNetCoreAuthenticationClaimType: 旧版: "PluginCore.Token" , 新版: "PluginCore.Admin.Token"
        List<string> allowedClaimTypes = new List<string>
        {
            "PluginCore.Token",
            "PluginCore.Admin.Token"
        };
        bool result = httpContext.User.Claims.Any(x => allowedClaimTypes.Contains(x.Type));
        result = result && (httpContext.User.Identity?.IsAuthenticated ?? false);

        return result;
    }
}
