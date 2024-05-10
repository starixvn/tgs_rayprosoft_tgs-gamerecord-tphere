using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.TPhere.Core.ApplicationServices;

internal class HandleRecordCommandService(
	ILogger<HandleRecordCommandService> logger,
	IOptions<GameRecordInfoOptions> options,
	IMessageSender messageSender) : ICommandService<HandleRecordCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

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

		var request = new Handle.Common.Protos.GameRecordHandleRequest
		{
			GameProviderCode = _gameRecordInfo.GameProviderCode,
			PlayerCode = command.GameWager.PlayerCode,
			ExternalRoundId = command.GameWager.ExternalRoundId,
			ExternalBetId = command.GameWager.ExternalWagerId,
			ExternalGameId = command.GameWager.ExternalGameId,
			ExternalCreatedOn = externalCreatedOn.ToTimestamp(),
			ExternalClosedOn = externalClosedOn.ToTimestamp(),
			ExternalPayoffOn = externalClosedOn.ToTimestamp(),
			ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
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

		await messageSender.PublishAsync(
			subject: Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			data: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This publication requests information.({request})",
			DateTime.Now.ToString("o"),
			Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			new
			{
				request.GameProviderCode,
				request.PlayerCode,
				request.ExternalRoundId,
				request.ExternalBetId,
				request.ExternalGameId,
				ExternalCreatedOn = request.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = request.ExternalClosedOn.ToDateTimeOffset(),
				request.BetState
			});
	}
}