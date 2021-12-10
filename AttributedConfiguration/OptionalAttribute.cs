using System;

namespace AttributedConfiguration {
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class OptionalAttribute : Attribute {
		public OptionalAttribute() {
		}
	}
}
