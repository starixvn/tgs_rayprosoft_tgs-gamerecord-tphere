namespace Tgs.GameRecord.TPhere.Core.ApplicationServices;

public record PullRecordByBettimeCommand(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
