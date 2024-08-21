using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tgs.GameRecord.Handle.Client;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.TPhere.Core.ApplicationServices;

internal class HandleRecordCommandService(
	ILogger<HandleRecordCommandService> logger,
	IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
	IOptions<PolicySettingsOptions> policySettingsOptions,
	IGameRecordHandleClient client) : ICommandService<HandleRecordCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfoOptions = gameRecordInfoOptions.Value;
	private readonly PolicySettingsOptions _policySettingsOptions = policySettingsOptions.Value;

	public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
	{
		var externalCreatedOn = command.GameWager.ExternalCreatedOn.ToDateTimeOffset();
		var betState = Handle.Common.Protos.BetState.Running;
		var freeGameState = Handle.Common.Protos.FreeGameState.None;
		var resettleStates = Handle.Common.Protos.ResettleState.None;
		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var bet = command.GameWager.Bet.ToDecimal().GetValueOrDefault(0);
		var validBet = 0.0m;
		var winLoss = 0.0m;
		var settle = 0.0m;

		if (command.GameWager.IsClosed)
		{
			externalClosedOn = command.GameWager.ExternalClosedOn.ToDateTimeOffset();
			bet = command.GameWager.Bet.ToDecimal().GetValueOrDefault(0);
			validBet = command.GameWager.ValidBet.ToDecimal().GetValueOrDefault(0);
			winLoss = command.GameWager.WinLoss.ToDecimal().GetValueOrDefault(0);
			settle = command.GameWager.Settle.ToDecimal().GetValueOrDefault(0);
			betState = command.GameWager.IsVoid ? Handle.Common.Protos.BetState.Void : (winLoss > 0) ? Handle.Common.Protos.BetState.Win : Handle.Common.Protos.BetState.Loss;
			freeGameState = command.GameWager.IsFreeGame ? Handle.Common.Protos.FreeGameState.FreeGame : Handle.Common.Protos.FreeGameState.None;
			resettleStates = command.GameWager.IsResettle ? Handle.Common.Protos.ResettleState.Resettle : Handle.Common.Protos.ResettleState.None;
		}

		var source = new Handle.Common.ViewModels.GameRecordHandleViewModel
		{
			GameProviderCode = _gameRecordInfoOptions.GameProviderCode,
			PlayerCode = command.GameWager.PlayerCode,
			ExternalRoundId = command.GameWager.ExternalRoundId,
			ExternalBetId = command.GameWager.ExternalWagerId,
			ExternalGameId = command.GameWager.ExternalGameId,
			ExternalCreatedOn = externalCreatedOn,
			ExternalClosedOn = externalClosedOn,
			ExternalPayoffOn = externalClosedOn,
			ExternalResettleOn = DateTimeOffset.UnixEpoch,
			BetState = betState,
			BetType = Handle.Common.Protos.BetType.Normal,
			FreeGameState = freeGameState,
			GambleState = Handle.Common.Protos.GambleState.None,
			CashoutState = Handle.Common.Protos.CashoutState.None,
			ResettleState = resettleStates,
			JackpotState = Handle.Common.Protos.JackpotState.None,
			Bet = bet,
			ValidBet = validBet,
			Settle = settle,
			Commission = 0.0m,
			Turnover = validBet,
			WinLoss = winLoss,
			Jackpot = 0.0M,
			AdditionalData = command.GameWager.AdditionalData
		};

		var retryCount = _policySettingsOptions.RetryCount;
		var retrySeconds = _policySettingsOptions.RetrySeconds;
		var retryPolicy = Policy
			.Handle<HttpRequestException>()
			.WaitAndRetryAsync(
				retryCount: retryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(retrySeconds, retryAttempt)),
				onRetry: (exception, retryTime, retryCount, context) => logger.LogWarning(exception, "{logWarning}",
					new
					{
						LogOn = DateTime.Now.ToString("u"),
						RetryTime = retryTime,
						RetryCount = retryCount,
						Data = new
						{
							source.GameProviderCode,
							source.PlayerCode,
							source.ExternalRoundId,
							source.ExternalBetId,
							source.ExternalGameId,
							ExternalCreatedOn = source.ExternalCreatedOn.ToString("u"),
							ExternalClosedOn = source.ExternalClosedOn.ToString("u"),
							source.BetState
						},
						exception.Message
					}));

		await retryPolicy.ExecuteAsync(() => client.HandleAsync(
			source: source,
			cancellationToken: cancellationToken).AsTask())
			.ConfigureAwait(false);
	}
}