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
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///				"apiUrl": "https://apidemo.tphere.com/",
	///				"apiKey": "6309572cf90658e17847a7b021723106",
	///				"apiToken": "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
	///				"startTime": "2024-04-10T15:30:00+08:00",
	///				"endTime": "2024-04-10T16:00:00+08:00"
	///         }
	/// </remarks>
	[HttpPost("manual")]
	public ValueTask GameRecordManualAsync(
		[FromServices] IMessageSender messageSender,
		TPhereGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: TPhereNatsConst.ManualPublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record manual bettime asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///				"apiUrl": "https://apidemo.tphere.com/",
	///				"apiKey": "6309572cf90658e17847a7b021723106",
	///				"apiToken": "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
	///				"startTime": "2024-03-19T03:00:00+00:00",
	///				"endTime": "2024-03-19T04:00:00+00:00"
	///         }
	/// </remarks>
	[HttpPost("manual/bettime")]
	public ValueTask GameRecordManualBettimeAsync(
		[FromServices] IMessageSender messageSender,
		TPhereGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: TPhereNatsConst.ManualBettimePublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record schedule asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///				"apiUrl": "https://apidemo.tphere.com/",
	///				"apiKey": "6309572cf90658e17847a7b021723106",
	///				"apiToken": "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb",
	///				"startTime": "2024-04-10T15:30:00+08:00",
	///				"endTime": "2024-04-10T16:00:00+08:00"
	///         }
	/// </remarks>
	[HttpPost("schedule")]
	public ValueTask GameRecordScheduleAsync(
		[FromServices] IMessageSender messageSender,
		TPhereGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: TPhereNatsConst.SchedulePublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);
}
