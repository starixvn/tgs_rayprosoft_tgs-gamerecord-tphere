using System.Reflection;
using Tgs.GameCommand.Client;
using Tgs.GameInterface.TPhere.Client;
using Tgs.GameRecord.TPhere;
using Tgs.GameRecord.TPhere.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecord(
	   this IServiceCollection services,
	   Action<RedisOptions, IServiceProvider> redisDBconfigure,
	   Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
	   Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
	   Action<GameInterfaceTPhereClientOptions, IServiceProvider> gameInterfaceTPhereClientOptions)
		=> services
		.AddOptions<RedisOptions>()
		.Configure(redisDBconfigure)
		.Services
		.AddOptions<GameRecordInfoOptions>()
		.Configure(gameRecordInfoOptions)
		.Services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddSingleton<RedisConnectionManager>()
		.AddGameRecordCore(
			gameCommandClientOptions,
			gameInterfaceTPhereClientOptions,
			redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
