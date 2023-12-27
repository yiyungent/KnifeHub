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


    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Allow all authenticated users to see the Dashboard (potentially dangerous).
        return httpContext.User.Identity?.IsAuthenticated ?? false;
    }
}
