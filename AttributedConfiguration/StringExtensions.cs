using System;

namespace AttributedConfiguration;

public static class StringExtensions {
	public static TimeSource GetTimeSource(this string key) {
		if(key.IsFromTimeSource(TimeSource.InDays)) { return TimeSource.InDays; }
		if(key.IsFromTimeSource(TimeSource.InHours)) { return TimeSource.InHours; }
		if(key.IsFromTimeSource(TimeSource.InMinutes)) { return TimeSource.InMinutes; }
		if(key.IsFromTimeSource(TimeSource.InSeconds)) { return TimeSource.InSeconds; }
		if(key.IsFromTimeSource(TimeSource.InMilliseconds)) { return TimeSource.InMilliseconds; }
		throw new NotImplementedException($"Could not identify {nameof(TimeSource)} for section {key}");
	}

	public static bool IsFromTimeSource(this string key, TimeSource timeSource)
		=> key.EndsWith(timeSource.ToString(), StringComparison.OrdinalIgnoreCase);
}
