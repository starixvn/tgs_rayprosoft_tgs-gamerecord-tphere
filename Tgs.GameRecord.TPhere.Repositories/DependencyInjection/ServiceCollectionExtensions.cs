using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Tgs.GameRecord.TPhere.Core;
using Tgs.GameRecord.TPhere.Repositories;

namespace Microsoft.Extensions.DependencyInjection;
public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecordRepository(
		this IServiceCollection services,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver)
		=> services
		.AddTransient<IHangfireUtensilRepository, HangfireUtensilRepository>()
		.AddTransient<IHangfireLockRepository>(sp => ActivatorUtilities.CreateInstance<HangfireLockRepository>(sp, redisDatabaseResolver(sp)));
}