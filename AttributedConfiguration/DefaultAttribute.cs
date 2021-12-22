using System;

namespace AttributedConfiguration;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class DefaultAttribute : OptionalAttribute {
	public DefaultAttribute(object? defaultValue = null) {
		this.DefaultValue = defaultValue;
	}

	public DefaultAttribute(double defaultValue, TimeSource timeSource) {
		this.DefaultValue = defaultValue;
		this.TimeSource = timeSource;
	}

	public object? DefaultValue { get; }
	public TimeSource TimeSource { get; }
}
