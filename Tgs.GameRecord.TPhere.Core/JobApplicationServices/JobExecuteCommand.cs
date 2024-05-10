namespace Tgs.GameRecord.TPhere.Core.JobApplicationServices;

public record JobExecuteCommand(
	Uri ApiUrl,
	string ApiKey,
	string ApiToken);
