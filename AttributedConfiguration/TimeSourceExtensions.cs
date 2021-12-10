using System;

namespace AttributedConfiguration {
	public static class TimeSourceExtensions {
		public static TimeSpan Parse(this TimeSource timeSource, string stringValue) {
			var doubleValue = double.Parse(stringValue);
			return timeSource.Parse(doubleValue);
		}

		public static TimeSpan Parse(this TimeSource timeSource, double value)
			=> timeSource switch {
				TimeSource.InMilliseconds => TimeSpan.FromMilliseconds(value),
				TimeSource.InSeconds => TimeSpan.FromSeconds(value),
				TimeSource.InMinutes => TimeSpan.FromMinutes(value),
				TimeSource.InHours => TimeSpan.FromHours(value),
				TimeSource.InDays => TimeSpan.FromDays(value),
				_ => throw new NotImplementedException($"{nameof(TimeSource)} of value {timeSource} has not been implemented"),
			};
	}
}
