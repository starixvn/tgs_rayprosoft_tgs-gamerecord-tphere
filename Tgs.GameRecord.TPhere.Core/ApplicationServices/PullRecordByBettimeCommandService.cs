using Microsoft.Extensions.Logging;
using Tgs.GameInterface.TPhere.Client;
using Tgs.GameInterface.TPhere.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TPhere.Core.ApplicationServices;

internal class PullRecordByBettimeCommandService(
	ILogger<PullRecordByBettimeCommandService> logger,
	IGameInterfaceTPhereClient client,
	ICommandService<HandleRecordCommand> service) : ICommandService<PullRecordByBettimeCommand>
{
	public async ValueTask ExecuteAsync(PullRecordByBettimeCommand command, CancellationToken cancellationToken = default)
	{
		var page = 1;
		var pageSize = 5000;
		IEnumerable<GameWagerInfo> gameWagers;

		do
		{
			gameWagers = await client.TP2GameWagerBetLogByBettimeAsync(
				apiUrl: command.ApiUrl,
				apiKey: command.ApiKey,
				apiToken: command.ApiToken,
				startTime: command.StartTime,
				endTime: command.EndTime,
				page: page,
				pageSize: pageSize,
				cancellationToken: cancellationToken)
				.ToArrayAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? Enumerable.Empty<GameWagerInfo>();
			foreach (var gameWager in gameWagers)
			{
				logger.LogDebug("LogOn:{logOn} GameWager:{gameWager}", DateTime.Now.ToString("o"), gameWager);

				try
				{
					await service
						.ExecuteAsync(
						command: new(gameWager),
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game wager.({gameWager})", DateTime.Now.ToString("o"), gameWager);
				}
			}

			page++;
		} while (gameWagers.Any());
	}
}