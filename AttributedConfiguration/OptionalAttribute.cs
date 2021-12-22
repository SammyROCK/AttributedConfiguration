using System;

namespace AttributedConfiguration;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class OptionalAttribute : Attribute {
	public OptionalAttribute(object? defaultValue = null) {
		this.DefaultValue = defaultValue;
	}

	public OptionalAttribute(double defaultValue, TimeSource timeSource) {
		this.DefaultValue = defaultValue;
		this.TimeSource = timeSource;
	}

	public object? DefaultValue { get; }
	public TimeSource TimeSource { get; }
}
