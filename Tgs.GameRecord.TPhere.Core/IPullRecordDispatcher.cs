using Tgs.GameRecord.TPhere.Core.ApplicationServices;

namespace Tgs.GameRecord.TPhere.Core;

public interface IPullRecordDispatcher
{
	ValueTask PushCommandAsync(PullRecordCommand command, CancellationToken cancellationToken = default);
}
