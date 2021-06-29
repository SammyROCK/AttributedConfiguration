using System;

namespace AttributedConfiguration {
	public class ConfigureAttribute : Attribute {
		public ConfigureAttribute(string? section = null, Type? serviceType = null) {
			this.Section = section;
			this.ServiceType = serviceType;
		}

		public string? Section { get; }

		public Type? ServiceType { get; }
	}
}
