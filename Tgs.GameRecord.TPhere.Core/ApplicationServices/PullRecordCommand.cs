namespace Tgs.GameRecord.TPhere.Core.ApplicationServices;

public record PullRecordCommand(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
