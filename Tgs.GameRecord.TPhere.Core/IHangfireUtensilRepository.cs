using Tgs.GameRecord.TPhere.Core.JobApplicationServices;

namespace Tgs.GameRecord.TPhere.Core;

public interface IHangfireUtensilRepository
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
