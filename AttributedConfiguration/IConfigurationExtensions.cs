using AttributeSdk;
using Microsoft.Extensions.Configuration;
using System;

namespace AttributedConfiguration {
	public static class IConfigurationExtensions {
		public static T Resolve<T>(this IConfiguration configuration)
			where T : BaseConfiguration {
			var type = typeof(T);

			var configureAttribute = type.GetAttribute<ConfigureAttribute>();

			return (T)configuration.Resolve(type, configureAttribute);
		}

		public static object Resolve(this IConfiguration configuration, Type type, ConfigureAttribute? configureAttribute) {
			var attibuteSection = configureAttribute?.Section;
			if(attibuteSection?.Equals("[Configuration]", StringComparison.OrdinalIgnoreCase) == true) {
				attibuteSection = type.Name.Replace("Configuration", string.Empty);
			}

			var configurationSection = attibuteSection is null ?
				configuration :
				configuration.GetSection(attibuteSection);

			return Activator.CreateInstance(type, configurationSection)
				?? throw new InvalidOperationException($"Could not resolve {type.Name}");
		}
	}
}
