using System;

namespace AttributedConfiguration {
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class OptionalAttribute : Attribute {
	}
}
