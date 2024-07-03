using Microsoft.Extensions.Configuration;
using System;

namespace NeoAttributedConfiguration {
	public abstract class ConfigurationException : Exception {
		private readonly Lazy<string> message;

		public ConfigurationException(
			IConfiguration configuration,
			string key,
			Exception? innerException = null
		) : base(string.Empty, innerException) {
			this.Configuration = configuration;
			this.Key = key;

			var configurationPath = configuration is IConfigurationSection configurationSection
				  ? $"{configurationSection.Path}:{key}"
				  : key;
			this.message = new Lazy<string>(() => this.FormatMessage(configurationPath));
		}

		public IConfiguration Configuration { get; }
		public string Key { get; }
		public override string Message => this.message.Value;

		protected abstract string FormatMessage(string configurationPath);
	}
}
