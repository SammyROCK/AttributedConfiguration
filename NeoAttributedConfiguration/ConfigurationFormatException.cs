using Microsoft.Extensions.Configuration;
using System;

namespace NeoAttributedConfiguration {
	public class ConfigurationFormatException : ConfigurationException {
		public ConfigurationFormatException(IConfiguration configuration, string key, Type targetType, Exception? innerException = null)
			: base(configuration, key, innerException) {
			this.TargetType = targetType;
		}

		public Type TargetType { get; }

		protected override string FormatMessage(string configurationPath) {
			return $"Could not convert configuration {configurationPath} with value \"{this.Configuration[this.Key]}\" to {this.TargetType}";
		}
	}
}
