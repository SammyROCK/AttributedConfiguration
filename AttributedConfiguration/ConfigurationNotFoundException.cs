using System;

namespace AttributedConfiguration {
	public class ConfigurationNotFoundException : Exception {
		public ConfigurationNotFoundException(string configuration)
			: base($"Configuration {configuration} was not found")
			=> this.Configuration = configuration;

		public string Configuration { get; }
	}
}
