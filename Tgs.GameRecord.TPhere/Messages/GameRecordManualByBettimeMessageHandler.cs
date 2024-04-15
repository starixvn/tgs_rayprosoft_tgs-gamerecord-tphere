using System.Text;
using System.Text.Json;
using Tgs.GameRecord.TPhere.Common;
using Tgs.GameRecord.TPhere.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.TPhere.Messages;

public class GameRecordManualByBettimeMessageHandler(
	ILogger<GameRecordManualByBettimeMessageHandler> logger,
	ICommandService<PullRecordByBettimeCommand> service) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This message was received.",
			DateTime.Now.ToString("o"),
			subject);

		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<TPhereGameRecordMessage>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Information Request:{message}",
			DateTime.Now.ToString("o"),
			subject,
			message);

		var command = new PullRecordByBettimeCommand(
			ApiUrl: message.ApiUrl,
			ApiKey: message.ApiKey,
			ApiToken: message.ApiToken,
			StartTime: message.StartTime,
			EndTime: message.EndTime);

		return service.ExecuteAsync(
			command: command,
			cancellationToken: cancellationToken);
	}
}
