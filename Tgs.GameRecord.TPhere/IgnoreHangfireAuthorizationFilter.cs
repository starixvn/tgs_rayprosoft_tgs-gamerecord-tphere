using Hangfire.Dashboard;

namespace Tgs.GameRecord.TPhere;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}