using AttributeSdk;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;

namespace NeoAttributedConfiguration {
	public static class IServiceCollectionExtensions {
		public static IServiceCollection AddAttributedConfigurations(this IServiceCollection serviceCollection, IConfiguration configuration) {
			foreach(var (type, configureAttribute) in AppDomain.CurrentDomain.EnumerateTypesWithAttribute<ConfigureAttribute>()) {
				var serviceType = configureAttribute.ServiceType
					?? type.GetInterfaces().FirstOrDefault()
					?? type;

				serviceCollection.TryAdd(new ServiceDescriptor(
					serviceType,
					serviceProvider => configuration.Resolve(type, configureAttribute),
					ServiceLifetime.Singleton
				));
			}
			return serviceCollection;
		}

		public static IServiceCollection AddAttributedConfigurationsFromAssemblyContaining<T>(this IServiceCollection serviceCollection, IConfiguration configuration) {
			foreach(var (type, configureAttribute) in typeof(T).Assembly.EnumerateTypesWithAttribute<ConfigureAttribute>()) {
				var serviceType = configureAttribute.ServiceType
					?? type.GetInterfaces().FirstOrDefault()
					?? type;

				serviceCollection.TryAdd(new ServiceDescriptor(
					serviceType,
					serviceProvider => configuration.Resolve(type, configureAttribute),
					ServiceLifetime.Singleton
				));
			}
			return serviceCollection;
		}

		public static IServiceCollection AddAttributedConfigurationsFromNamespaceOf<T>(this IServiceCollection serviceCollection, IConfiguration configuration) {
			foreach(var (type, configureAttribute) in typeof(T).Assembly.EnumerateTypesWithAttribute<ConfigureAttribute>()) {
				if(type.Namespace != typeof(T).Namespace) { continue; }

				var serviceType = configureAttribute.ServiceType
					?? type.GetInterfaces().FirstOrDefault()
					?? type;

				serviceCollection.TryAdd(new ServiceDescriptor(
					serviceType,
					serviceProvider => configuration.Resolve(type, configureAttribute),
					ServiceLifetime.Singleton
				));
			}
			return serviceCollection;
		}
	}
}
