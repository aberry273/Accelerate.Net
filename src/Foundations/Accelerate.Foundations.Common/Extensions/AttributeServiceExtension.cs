using Accelerate.Foundations.Common.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Extensions
{
    public static class AttributeServiceExtension
    {
        //https://medium.com/@asad99/simplifying-dynamic-and-consistent-service-registration-in-net-core-fd423c3ca4fe
        public static IServiceCollection RegisterServicesWithAttributes(this IServiceCollection serviceCollection, Assembly assembly)
        {
            var targetServices = assembly.GetTypes()
                 .Where(type =>
                  type.GetCustomAttribute<ScopedServiceAttribute>() != null
                  ||
                  type.GetCustomAttribute<TransientServiceAttribute>() != null
                  ||
                  type.GetCustomAttribute<SingletonServiceAttribute>() != null
                 );

            foreach (var serviceType in targetServices)
            {
                //get all interface that the class implemented
                var implementedInterfaces = serviceType.GetInterfaces();

                //get the lifetime of the class
                ServiceLifetime lifetime = GetLifetimeFromAttribute(serviceType);

                //Class implemented interface
                if (implementedInterfaces != null && implementedInterfaces.Any())
                {
                    foreach (var @interface in implementedInterfaces)
                    {
                        RegisterService(serviceCollection, @interface, serviceType, lifetime);
                    }
                }
                else
                {
                    //Class doesn't implemented interface
                    RegisterService(serviceCollection, null, serviceType, lifetime);
                }
            }
            return serviceCollection;
        }
        private static IServiceCollection RegisterService(IServiceCollection serviceCollection, Type? @interface, Type serviceType, ServiceLifetime lifetime)
        {
            _ = lifetime switch
            {
                ServiceLifetime.Singleton => @interface != null ?
                   serviceCollection.AddSingleton(@interface, serviceType) :
                   serviceCollection.AddSingleton(serviceType),
                ServiceLifetime.Scoped => @interface != null ?
                   serviceCollection.AddScoped(@interface, serviceType) :
                   serviceCollection.AddScoped(serviceType),
                ServiceLifetime.Transient => @interface != null ?
                   serviceCollection.AddTransient(@interface, serviceType) :
                   serviceCollection.AddTransient(serviceType),
                _ => @interface != null ?
                   serviceCollection.AddScoped(@interface, serviceType) :
                   serviceCollection.AddScoped(serviceType),
            };
            return serviceCollection;
        }

        private static ServiceLifetime GetLifetimeFromAttribute(Type serviceType)
        {
            if (serviceType.GetCustomAttribute<ScopedServiceAttribute>() != null)
            {
                return ServiceLifetime.Scoped;
            }
            else if (serviceType.GetCustomAttribute<TransientServiceAttribute>() != null)
            {
                return ServiceLifetime.Transient;
            }
            else if (serviceType.GetCustomAttribute<SingletonServiceAttribute>() != null)
            {
                return ServiceLifetime.Singleton;
            }
            return ServiceLifetime.Scoped;
        }
    }
}
