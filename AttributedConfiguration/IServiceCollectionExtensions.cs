using AttributeSdk;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AttributedConfiguration {
	public static class IServiceCollectionExtensions {
		public static IServiceCollection AddAttributedConfigurations(this IServiceCollection serviceCollection, IConfiguration configuration) {
			foreach(var (type, configureAttribute) in AppDomain.CurrentDomain.EnumerateTypesWithAttribute<ConfigureAttribute>()) {
				var serviceType = configureAttribute.ServiceType
					?? type.GetInterfaces().FirstOrDefault()
					?? type;

				serviceCollection.AddSingleton(
					serviceType,
					serviceProvider => configuration.Resolve(type, configureAttribute)
				);
			}
			return serviceCollection;
		}
	}
}