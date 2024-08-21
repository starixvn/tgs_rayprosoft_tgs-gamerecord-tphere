using Tgs.GameRecord.TPhere.Core.ApplicationServices;

namespace Tgs.GameRecord.TPhere.Core;

public interface IPullRecordByBettimeDispatcher
{
	ValueTask PushCommandAsync(PullRecordByBettimeCommand command, CancellationToken cancellationToken = default);
}
