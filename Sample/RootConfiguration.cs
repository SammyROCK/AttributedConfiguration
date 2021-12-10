#pragma warning disable CS8618
using AttributedConfiguration;

namespace Sample {
	[Configure]
	public class RootConfiguration {
		public bool Bool { get; }

		public int Int { get; }

		public double Double { get; }

		public decimal Decimal { get; }

		public char Char { get; }

		public string String { get; }

		public SampleEnum Enum { get; }

		public TimeSpan Time { get; }

		[Name("Nested")]
		public NestedConfiguration Object { get; }

		public bool[] BoolArray { get; }

		public int[] IntArray { get; }

		public decimal[] DecimalArray { get; }

		public char[] CharArray { get; }

		public string[] StringArray { get; }

		public SampleEnum[] EnumArray { get; }

		public TimeSpan[] TimeArray { get; }

		[Name("NestedArray")]
		public NestedConfiguration[] ObjectArray { get; }

		[Optional]
		public string[] MissingStringArray { get; }

		[Optional]
		public SampleEnum[] MissingEnumArray { get; }

		[Optional]
		public TimeSpan[] MissingTimeArray { get; }

		[Optional]
		public Dictionary<int, int> IntIntDict { get; }

		[Optional]
		public Dictionary<string, string> StringStringDict { get; }

		[Optional]
		public Dictionary<string, NestedConfiguration> StringObjectDict { get; }
	}
}
