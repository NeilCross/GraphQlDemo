using System.Linq;
using System.Reflection;
using GraphQlDemo.Middlewares.GraphQlTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQlDemo.Middlewares
{
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class GraphQlMiddlewareExtensions
    {
        public static IApplicationBuilder UseGraphQl(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GraphQlMiddleware>();
        }
        public static void AddGraphQl(this IServiceCollection collection)
        {
            foreach (var type in Assembly.GetEntryAssembly().GetExportedTypes().Where(t => typeof(GraphQlController).IsAssignableFrom(t)))
            {
                collection.Add(new ServiceDescriptor(type,type, ServiceLifetime.Scoped));
            }
        }
    }
}
