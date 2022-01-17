#nullable disable warnings
using AttributedConfiguration;
using Microsoft.Extensions.Configuration;

namespace Sample;

[Configure("[configuration]")]
public class NestedConfiguration {
	public IConfiguration Configuration { get; set; }

	[Optional]
	public bool? Bool { get; }

	[Optional]
	public int? Int { get; }

	[Optional]
	public decimal? Decimal { get; }

	[Optional]
	public char? Char { get; }

	[Optional]
	public string? String { get; }

	[Optional]
	public SampleEnum Enum { get; }

	[Optional]
	public TimeSpan? Time { get; }

	[Optional]
	public NestedConfiguration? Object { get; }
}
