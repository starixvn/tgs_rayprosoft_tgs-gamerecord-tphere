using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.TPhere.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.TPhere.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.TPhere.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
	[Fact]
	public async Task Loss()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "TP",
			GameProviderConfiguration = "TPConfiguration",
			HangfirePrefix = "gamerecord-tp-hangfire:",
			HangfireLocking = "gamerecord-tp-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 15,
			IntervalMinutesMinimum = 1
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(
			new GameWagerInfo
			{
				ExternalRoundId = "570321",
				ExternalWagerId = "663339213a699d70ef2b8dc2",
				ExternalGameId = "ds_1008",
				PlayerCode = "4ump0o770f9h",
				ExternalCreatedOn = DateTimeOffset.Parse("2024-05-02 14:56:42+08:00").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2024-05-02 14:56:42+08:00").ToTimestamp(),
				IsClosed = true,
				IsFreeGame = false,
				IsResettle = false,
				IsVoid = false,
				Bet = 5.0m,
				ValidBet = 5.0m,
				Settle = 0,
				WinLoss = -5.0m,
				AdditionalData = "{\"hall\":\"gpk2\",\"round\":\"570321\",\"rowid\":\"663339213a699d70ef2b8dc2\",\"trace\":false,\"gameid\":\"ds_1008\",\"status\":\"1\",\"betinfo\":null,\"bettime\":\"2024-05-02 02:56:33\",\"bettype\":null,\"pca_win\":\"0\",\"revenue\":\"0\",\"betvalid\":\"5\",\"category\":\"fishing\",\"freegame\":null,\"betamount\":\"5\",\"betresult\":\"-5\",\"game_name\":\"Demon Conquered\",\"gameresult\":null,\"reporttime\":\"2024-05-02 03:02:51\",\"payout_time\":\"2024-05-02 02:56:33\",\"casino_account\":\"4ump0o770f9h\",\"pca_contribute\":\"0\"}"
			});

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.GameWager.PlayerCode,
				ExternalRoundId = command.GameWager.ExternalRoundId,
				ExternalBetId = command.GameWager.ExternalWagerId,
				ExternalGameId = command.GameWager.ExternalGameId,
				ExternalCreatedOn = command.GameWager.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = command.GameWager.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn = command.GameWager.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Loss,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 5.0m,
				ValidBet = 5.0m,
				Settle = 0.0M,
				Commission = 0.0M,
				Turnover = 5.0m,
				WinLoss = -5.0m,
				Jackpot = 0.0M,
				AdditionalData = command.GameWager.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Win()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "TP",
			GameProviderConfiguration = "TPConfiguration",
			HangfirePrefix = "gamerecord-tp-hangfire:",
			HangfireLocking = "gamerecord-tp-locking:",
			IntervalMinutes = 3,
			IntervalMinutesMax = 15,
			IntervalMinutesMinimum = 1
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(
			new GameWagerInfo
			{
				ExternalRoundId = "570321",
				ExternalWagerId = "6633393c3a699d70ef2b8dda",
				ExternalGameId = "ds_1008",
				PlayerCode = "4ump0o770f9h",
				ExternalCreatedOn = DateTimeOffset.Parse("2024-05-02 14:57:00+08:00").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2024-05-02 14:57:00+08:00").ToTimestamp(),
				IsClosed = true,
				IsFreeGame = false,
				IsResettle = false,
				IsVoid = false,
				Bet = 13.0m,
				ValidBet = 13.0m,
				Settle = 20.0m,
				WinLoss = 7.00m,
				AdditionalData = "{\"hall\":\"gpk2\",\"round\":\"570321\",\"rowid\":\"6633393c3a699d70ef2b8dda\",\"trace\":false,\"gameid\":\"ds_1008\",\"status\":\"1\",\"betinfo\":null,\"bettime\":\"2024-05-02 02:57:00\",\"bettype\":null,\"pca_win\":\"0\",\"revenue\":\"0\",\"betvalid\":\"13\",\"category\":\"fishing\",\"freegame\":null,\"betamount\":\"13\",\"betresult\":\"7\",\"game_name\":\"Demon Conquered\",\"gameresult\":null,\"reporttime\":\"2024-05-02 03:02:51\",\"payout_time\":\"2024-05-02 02:57:00\",\"casino_account\":\"4ump0o770f9h\",\"pca_contribute\":\"0\"}"
			});

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.GameWager.PlayerCode,
				ExternalRoundId = command.GameWager.ExternalRoundId,
				ExternalBetId = command.GameWager.ExternalWagerId,
				ExternalGameId = command.GameWager.ExternalGameId,
				ExternalCreatedOn = command.GameWager.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = command.GameWager.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn = command.GameWager.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Win,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 13.0m,
				ValidBet = 13.0m,
				Settle = 20.0m,
				Commission = 0.0M,
				Turnover = 13.0m,
				WinLoss = 7.00m,
				Jackpot = 0.0M,
				AdditionalData = command.GameWager.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
