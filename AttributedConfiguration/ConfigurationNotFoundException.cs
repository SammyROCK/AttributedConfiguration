using Microsoft.Extensions.Configuration;
using System;

namespace AttributedConfiguration {
	public class ConfigurationNotFoundException : Exception {
		public ConfigurationNotFoundException(IConfiguration configuration, string key)
			: base(FormatMessage(configuration,key)) {
			this.Configuration = configuration;
			this.Key = key;
		}

		public IConfiguration Configuration { get; }
		public string Key { get; }

		private static string FormatMessage(IConfiguration configuration, string key) {
			var configurationPath = configuration is IConfigurationSection configurationSection
				  ? $"{configurationSection.Path}:{key}"
				  : key;

			return $"Configuration {configurationPath} was not found";
		}
	}
}
