using Microsoft.Extensions.Configuration;
using System;

namespace NeoAttributedConfiguration {
	public static class IConfigurationSectionExtensions {
		public static TimeSpan ResolveTimeSpan(this IConfigurationSection section) {
			var timeSource = section.Key.GetTimeSource();
			return timeSource.Parse(section.Value!);
		}
	}
}
