using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AttributedConfiguration {
	public abstract class BaseConfiguration {
		private readonly IConfiguration configuration;
		private readonly string? path;

		public BaseConfiguration(IConfiguration configuration) {
			this.configuration = configuration;
			this.path = configuration is IConfigurationSection configurationSection
				? configurationSection.Path
				: null;
		}

		protected TEnum GetEnum<TEnum>(string key) where TEnum : struct
			=> Enum.Parse<TEnum>(this.GetString(key), true);

		protected TimeSpan GetTimespan(string key_prefix, TimeSource timeSource) {
			var key = $"{key_prefix}{timeSource}";
			var value = this.GetDouble(key);
			return timeSource switch {
				TimeSource.InMilliseconds => TimeSpan.FromMilliseconds(value),
				TimeSource.InSeconds => TimeSpan.FromSeconds(value),
				TimeSource.InMinutes => TimeSpan.FromMinutes(value),
				TimeSource.InHours => TimeSpan.FromHours(value),
				TimeSource.InDays => TimeSpan.FromDays(value),
				_ => throw new NotImplementedException($"{nameof(TimeSource)} of value {timeSource} has not been implemented"),
			};
		}

		protected double GetDouble(string key)
			=> double.Parse(this.GetString(key), CultureInfo.InvariantCulture);

		protected int GetInt(string key)
			=> int.Parse(this.GetString(key));

		protected char GetChar(string key)
			=> this.GetString(key).Single();

		protected bool? TryGetBool(string key)
			=> bool.TryParse(this.TryGetString(key), out var value) ? value : default(bool?);

		protected string GetString(string key)
			=> this.TryGetString(key)
				?? throw new ConfigurationNotFoundException(this.BuildConfigurationPath(key));

		protected string? TryGetString(string key)
			=> this.configuration[key];

		protected string[] GetStrings(string key)
			=> this.GetSections(key)
				.Select(section => section.Value)
				.ToArray();

		protected T? TryGet<T>(string key) where T : BaseConfiguration {
			var section = this.configuration.GetSection(key);
			if(section.Exists() is false) { return null; }

			return section.Resolve<T>();
		}

		protected T Get<T>(string key) where T : BaseConfiguration
			=> this.GetSection(key).Resolve<T>();

		protected T[] GetMany<T>(string key) where T : BaseConfiguration
			=> this.GetSections(key)
				.Select(section => section.Resolve<T>())
				.ToArray();

		protected IDictionary<string, string> GetDict(string key)
			=> this.GetSections(key)
				.ToDictionary(s => s.Key, s => s.Value);

		protected IDictionary<int, int> GetIntIntDict(string key)
			=> this.GetSections(key)
				.ToDictionary(
					s => int.Parse(s.Key),
					s => int.Parse(s.Value));

		protected IEnumerable<IConfigurationSection> GetSections(string key)
			=> this.GetSection(key)
				.GetChildren();

		protected IConfigurationSection GetSection(string key)
			=> this.configuration.GetSection(key)
				?? throw new ConfigurationNotFoundException(this.BuildConfigurationPath(key));

		private string BuildConfigurationPath(string key)
			=> this.path is null
				? key
				: $"{this.path}:{key}";
	}
}