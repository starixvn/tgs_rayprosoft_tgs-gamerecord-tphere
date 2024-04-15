using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.TPhere.Core.ApplicationServices;

internal class HandleRecordCommandService(
	IOptions<GameRecordInfoOptions> options,
	IGameRecordHandleClient client) : ICommandService<HandleRecordCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

	public ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
	{
		var externalCreatedOn = command.GameWager.ExternalCreatedOn.ToDateTimeOffset();
		var wagerState = WagerStates.Running;
		var freeGameState = FreeGameStates.None;
		var resettleStates = ResettleStates.None;
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
			wagerState = command.GameWager.IsVoid ? WagerStates.Void : (winLoss > 0) ? WagerStates.Win : WagerStates.Lose;
			freeGameState = command.GameWager.IsFreeGame ? FreeGameStates.FreeGame : FreeGameStates.None;
			resettleStates = command.GameWager.IsResettle ? ResettleStates.Resettle : ResettleStates.None;
		}

		return client.MessageAsync(
			gameProviderCode: _gameRecordInfo.GameProviderCode,
			playerCode: command.GameWager.PlayerCode,
			wagerState: wagerState,
			wagerType: WagerTypes.Normal,
			freeGameState: freeGameState,
			gambleState: GambleStates.None,
			cashoutState: CashoutStates.None,
			resettleState: resettleStates,
			externalRoundId: command.GameWager.ExternalWagerId,
			externalWagerId: command.GameWager.ExternalWagerId,
			externalGameId: command.GameWager.ExternalGameId,
			externalCreatedOn: externalCreatedOn,
			externalClosedOn: externalClosedOn,
			externalPayoffOn: externalClosedOn,
			externalResettleOn: DateTimeOffset.UnixEpoch,
			bet: bet,
			validBet: validBet,
			settle: settle,
			commission: 0.0m,
			turnover: validBet,
			winLoss: winLoss,
			additionalData: JsonNode.Parse(command.GameWager.AdditionalData)!,
			cancellationToken: cancellationToken);
	}
}