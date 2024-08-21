using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.TPhere.Common;

public record TPhereGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	[DefaultValue(typeof(Uri), "https://apidemo.tphere.com/")]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "6309572cf90658e17847a7b021723106")]
	public string ApiKey { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "29dad28e8ab9c9855e6aa4b9a9e4a869325931d4976dfcf53266ccbe0c5f3387ea48258c002e3543edbb35f63caa5cacc65ac5c664ccfe0619e1d63018a87dbb")]
	public string ApiToken { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(string), "2024-04-15T14:25:00+08:00")]
	public DateTimeOffset StartTime { get; init; }
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(string), "2024-04-15T14:30:00+08:00")]
	public DateTimeOffset EndTime { get; init; }
}
