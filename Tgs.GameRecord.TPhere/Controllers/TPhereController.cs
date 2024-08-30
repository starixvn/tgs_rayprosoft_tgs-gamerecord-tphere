using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.TPhere.Common;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.TPhere.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TPhereController : ControllerBase
{
	/// <summary>
	/// Games the record manual asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("manual")]
	public ValueTask GameRecordManualAsync(
		[FromServices] IMessageSender messageSender,
		TPhereGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: TPhereNatsConst.ManualPublish,
			data: JsonSerializer.SerializeToUtf8Bytes(source),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record manual bettime asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("manual/bettime")]
	public ValueTask GameRecordManualBettimeAsync(
		[FromServices] IMessageSender messageSender,
		TPhereGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: TPhereNatsConst.ManualBettimePublish,
			data: JsonSerializer.SerializeToUtf8Bytes(source),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record schedule asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("schedule")]
	public ValueTask GameRecordScheduleAsync(
		[FromServices] IMessageSender messageSender,
		TPhereGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: TPhereNatsConst.SchedulePublish,
			data: JsonSerializer.SerializeToUtf8Bytes(source),
			cancellationToken: HttpContext.RequestAborted);
}
