using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.TPhere.Common;

public record TPhereGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	public string ApiKey { get; init; } = default!;
	[Required]
	public string ApiToken { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset StartTime { get; init; }
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset EndTime { get; init; }
}
