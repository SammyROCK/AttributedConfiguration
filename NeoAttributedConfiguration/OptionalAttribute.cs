using System;

namespace NeoAttributedConfiguration {
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class OptionalAttribute : Attribute {
	}
}
