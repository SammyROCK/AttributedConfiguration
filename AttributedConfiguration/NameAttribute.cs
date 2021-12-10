using System;

namespace AttributedConfiguration;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class NameAttribute : Attribute {
	public NameAttribute(string name) {
		this.Name = name;
	}

	public string Name { get; }
}
