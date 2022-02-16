using Microsoft.Extensions.Configuration;

namespace AttributedConfiguration {
	public class ConfigurationNotFoundException : ConfigurationException {
		public ConfigurationNotFoundException(IConfiguration configuration, string key)
			: base(configuration, key) {
		}

		protected override string FormatMessage(string configurationPath) {
			return $"Configuration {configurationPath} was not found";
		}
	}
}
