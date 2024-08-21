using System.Text;
using System.Text.Json;
using Tgs.GameRecord.TPhere.Common;
using Tgs.GameRecord.TPhere.Core;
using Tgs.GameRecord.TPhere.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.TPhere.Messages;

public class GameRecordManualByBettimeMessageHandler(
	ILogger<GameRecordManualByBettimeMessageHandler> logger,
	IPullRecordByBettimeDispatcher dispatcher) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<TPhereGameRecordMessage>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					message.ApiUrl,
					message.ApiKey,
					message.ApiToken,
					StartTime = message.StartTime.ToString("u"),
					EndTime = message.EndTime.ToString("u")
				},
				Message = "This receive message handler."
			}));

		return dispatcher.PushCommandAsync(
			command: new PullRecordByBettimeCommand(
				ApiUrl: message.ApiUrl,
				ApiKey: message.ApiKey,
				ApiToken: message.ApiToken,
				StartTime: message.StartTime,
				EndTime: message.EndTime),
			cancellationToken: cancellationToken);
	}
}
