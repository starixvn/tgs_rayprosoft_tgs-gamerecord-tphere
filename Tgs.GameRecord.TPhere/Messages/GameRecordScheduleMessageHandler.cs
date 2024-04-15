using System.Text;
using System.Text.Json;
using Tgs.GameRecord.TPhere.Common;
using Tgs.GameRecord.TPhere.Core;
using Tgs.GameRecord.TPhere.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.TPhere.Messages;

public class GameRecordScheduleMessageHandler(
	ILogger<GameRecordScheduleMessageHandler> logger,
	IPullRecordDispatcher dispatcher) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<TPhereGameRecordMessage>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Information Request:{message}",
			DateTime.Now.ToString("o"),
			subject,
			message);

		var command = new PullRecordCommand(
			ApiUrl: message.ApiUrl,
			ApiKey: message.ApiKey,
			ApiToken: message.ApiToken,
			StartTime: message.StartTime,
			EndTime: message.EndTime);

		return dispatcher.PushCommandAsync(
			command: command,
			cancellationToken: cancellationToken);
	}
}
