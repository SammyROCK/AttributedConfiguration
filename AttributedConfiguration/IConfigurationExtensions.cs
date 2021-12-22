using AttributeSdk;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace AttributedConfiguration;

public static class IConfigurationExtensions {
	public static T Resolve<T>(this IConfiguration configuration) {
		var type = typeof(T);

		var configureAttribute = type.GetAttribute<ConfigureAttribute>();

		return (T)configuration.Resolve(type, configureAttribute);
	}

	public static object Resolve(this IConfiguration configuration, Type type, ConfigureAttribute? configureAttribute) {
		var attributeSection = configureAttribute?.Section;
		if(attributeSection?.Equals("[Configuration]", StringComparison.OrdinalIgnoreCase) == true) {
			attributeSection = type.Name.Replace("Configuration", string.Empty);
		}

		var configurationSection = attributeSection is null ?
			configuration :
			configuration.GetSection(attributeSection);

		return configurationSection.Resolve(type);
	}

	public static object Resolve(this IConfiguration configuration, Type type) {
		var serviceProvider = new ServiceCollection()
			.AddSingleton(configuration)
			.BuildServiceProvider();

		var instance = ActivatorUtilities.CreateInstance(serviceProvider, type);

		foreach(var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)) {
			var value = configuration.GetValue(prop);

			if(prop.SetMethod is not null) {
				prop.SetValue(instance, value);
				continue;
			}

			var field = type.GetField($"<{prop.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
			field?.SetValue(instance, value);
		}

		return instance;
	}

	public static object? GetValue(this IConfiguration configuration, PropertyInfo propertyInfo) {
		var optionalAttribute = propertyInfo.GetCustomAttribute<OptionalAttribute>();
		var key = propertyInfo.GetCustomAttribute<NameAttribute>()?.Name ?? propertyInfo.Name;

		var propertyType = propertyInfo.PropertyType;
		if(propertyType.IsGenericType) {
			var genericType = propertyType.GetGenericTypeDefinition();

			if(genericType == typeof(Nullable<>)) {
				propertyType = propertyType.GetGenericArguments().Single();
			}
		}

		if(optionalAttribute is not null) {
			var value = configuration.TryGet(key, propertyType);

			if(optionalAttribute is DefaultAttribute defaultAttribute) {
				value ??= defaultAttribute.TimeSource is not TimeSource.Undefined
					? defaultAttribute.TimeSource.Parse((double)defaultAttribute.DefaultValue!)
					: defaultAttribute.DefaultValue;
			}

			return value;
		}

		return configuration.Get(key, propertyType);
	}

	public static object Get(this IConfiguration configuration, string key, Type type) {
		if(type.IsEnum) { return configuration.GetEnum(key, type); }
		if(type == typeof(string)) { return configuration.GetString(key); }
		if(type.IsPrimitive || type == typeof(decimal)) {
			return Convert.ChangeType(configuration.GetString(key), type, CultureInfo.InvariantCulture);
		}
		if(type == typeof(TimeSpan)) { return configuration.GetTimespan(key); }

		if(type.IsArray) {
			var array = configuration.GetMany(key, type.GetElementType()!);
			if(array.Length is 0) { throw new ConfigurationNotFoundException(configuration, key); }
			return array;
		}

		if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
			var genericArguments = type.GetGenericArguments();
			var dict = configuration.GetDict(key, genericArguments[0], genericArguments[1]);
			if(dict.Count is 0) { throw new ConfigurationNotFoundException(configuration, key); }
			return dict;
		}

		return configuration.GetSection(key).Resolve(type);
	}

	public static object? TryGet(this IConfiguration configuration, string key, Type type) {
		if(type.IsEnum) { return configuration.TryGetEnum(key, type); }
		if(type == typeof(string)) { return configuration.TryGetString(key); }
		if(type.IsPrimitive || type == typeof(decimal)) {
			var stringValue = configuration.TryGetString(key);
			if(stringValue is null) { return null; }
			return Convert.ChangeType(stringValue, type, CultureInfo.InvariantCulture);
		}
		if(type == typeof(TimeSpan)) { return configuration.TryGetTimespan(key); }

		if(type.IsArray) {
			return configuration.GetMany(key, type.GetElementType()!);
		}

		if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
			var genericArguments = type.GetGenericArguments();
			return configuration.GetDict(key, genericArguments[0], genericArguments[1]);
		}

		var configurationSection = configuration.GetSection(key);
		if(configurationSection.Exists() is false) { return null; }
		return configurationSection.Resolve(type);
	}

	public static TimeSpan GetTimespan(this IConfiguration configuration, string key)
		=> configuration.TryGetTimespan(key, TimeSource.InMilliseconds)
		?? configuration.TryGetTimespan(key, TimeSource.InSeconds)
		?? configuration.TryGetTimespan(key, TimeSource.InMinutes)
		?? configuration.TryGetTimespan(key, TimeSource.InHours)
		?? configuration.TryGetTimespan(key, TimeSource.InDays)
		?? throw new ConfigurationNotFoundException(configuration, key);

	public static TimeSpan? TryGetTimespan(this IConfiguration configuration, string key)
		=> configuration.TryGetTimespan(key, TimeSource.InMilliseconds)
		?? configuration.TryGetTimespan(key, TimeSource.InSeconds)
		?? configuration.TryGetTimespan(key, TimeSource.InMinutes)
		?? configuration.TryGetTimespan(key, TimeSource.InHours)
		?? configuration.TryGetTimespan(key, TimeSource.InDays);

	public static TimeSpan? TryGetTimespan(this IConfiguration configuration, string key, TimeSource timeSource) {
		var nullableValue = configuration.TryGetDouble($"{key}{timeSource}");
		if(nullableValue is null) { return null; }
		return timeSource.Parse(nullableValue.Value);
	}

	public static object GetEnum(this IConfiguration configuration, string key, Type enumType)
		=> Enum.Parse(enumType, configuration.GetString(key), true);

	public static object? TryGetEnum(this IConfiguration configuration, string key, Type enumType)
		=> Enum.TryParse(enumType, configuration.TryGetString(key), out var value) ? value : null;

	public static double? TryGetDouble(this IConfiguration configuration, string key)
		=> double.TryParse(configuration.TryGetString(key), NumberStyles.Any, CultureInfo.InvariantCulture, out var value) ? value : null;

	public static string GetString(this IConfiguration configuration, string key)
		=> configuration.TryGetString(key)
			?? throw new ConfigurationNotFoundException(configuration, key);

	public static string? TryGetString(this IConfiguration configuration, string key)
		=> configuration[key];

	public static Array GetMany(this IConfiguration configuration, string key, Type type) {
		if(type.IsEnum) {
			return configuration.GetStrings(key)
				.Select(stringValue => Enum.Parse(type, stringValue, true))
				.ToTypeArray(type);
		}
		if(type == typeof(string)) { return configuration.GetStrings(key); }
		if(type.IsPrimitive || type == typeof(decimal)) {
			return configuration.GetStrings(key)
				.Select(stringValue => Convert.ChangeType(stringValue, type, CultureInfo.InvariantCulture))
				.ToTypeArray(type);
		}
		if(type == typeof(TimeSpan)) { return configuration.GetManyTimeSpan(key); }

		return configuration.GetSections(key)
			.Select(section => section.Resolve(type))
			.ToTypeArray(type);
	}

	public static string[] GetStrings(this IConfiguration configuration, string key)
		=> configuration.GetSections(key)
			.Select(section => section.Value)
			.ToArray();

	public static TimeSpan[] GetManyTimeSpan(this IConfiguration configuration, string key)
		=> Enum.GetValues<TimeSource>()
			.SelectMany(
				timeSource => configuration.GetSections($"{key}{timeSource}")
					.Select(
						section => timeSource.Parse(section.Value)
					)
			).ToArray();

	public static IDictionary GetDict(this IConfiguration configuration, string key, Type keyType, Type valueType) {
		var dict = configuration.GetSections(key)
			.ToDictionary(
				section => {
					if(keyType.IsEnum) {
						return Enum.Parse(keyType, section.Key, true);
					}
					if(keyType == typeof(string)) { return section.Key; }
					if(keyType.IsPrimitive || keyType == typeof(decimal)) {
						return Convert.ChangeType(section.Key, keyType, CultureInfo.InvariantCulture);
					}
					if(keyType == typeof(TimeSpan)) {
						var timeSource = key.GetTimeSource();
						return timeSource.Parse(section.Key);
					}

					throw new NotImplementedException($"Type {keyType} could not be resolved as a Dictionary Key");
				},
				section => {
					if(valueType.IsEnum) {
						return Enum.Parse(valueType, section.Value, true);
					}
					if(valueType == typeof(string)) { return section.Value; }
					if(valueType.IsPrimitive || valueType == typeof(decimal)) {
						return Convert.ChangeType(section.Value, valueType, CultureInfo.InvariantCulture);
					}
					if(valueType == typeof(TimeSpan)) { return section.ResolveTimeSpan(); }

					return section.Resolve(valueType);
				}
			);

		var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
		var typedDict = (IDictionary)Activator.CreateInstance(dictType)!;
		foreach(var kvp in dict) {
			typedDict.Add(kvp.Key, kvp.Value);
		}

		return typedDict;
	}

	public static IEnumerable<IConfigurationSection> GetSections(this IConfiguration configuration, string key)
		=> configuration.GetSection(key)
			.GetChildren();
}
