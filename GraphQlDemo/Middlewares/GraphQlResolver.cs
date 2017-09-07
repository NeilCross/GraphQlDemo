using GraphQL.Types;
using GraphQlDemo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GraphQlDemo.Middlewares.GraphQlTypes
{
    public class GraphQlResolver : ObjectGraphType
    {
        private readonly IServiceProvider serviceProvicer;

        public GraphQlResolver(IServiceProvider serviceProvicer)
        {
            this.serviceProvicer = serviceProvicer;
            // resolve "controllers"
            var controllers = Assembly.GetEntryAssembly().GetExportedTypes().Where(t => typeof(GraphQlController).IsAssignableFrom(t));

            foreach (var controller in controllers)
            { 
                var methods = controller.GetMethods().Where(m => m.DeclaringType == controller);
            
                foreach (var method in methods)
                {
                    CreateField(method);
                }
            }
        }

        private void CreateField(MethodInfo method)
        {
            var returnType = GetGraphType(method.ReturnType);
            if (returnType == null)
            {
                return;
            }

            var parameters = method.GetParameters();

            var arguments = parameters.Select(p =>
                new QueryArgument(GetGraphType(p.ParameterType))
                {
                    Name = p.Name
                });

            Func<ResolveFieldContext<object>, object> invocation =
                (ResolveFieldContext<object> context) =>
                {
                    return InvokeWithContext(context, method, parameters);
                };

            Field(returnType, method.Name, null, new QueryArguments(arguments), invocation, null);
        }

        private object InvokeWithContext(ResolveFieldContext<object> context, MethodInfo method, ParameterInfo[] parameters)
        {
            // get the GetArgument<> method
            var getArgumentMethodUnbound = typeof(ResolveFieldContext<object>).GetMethod("GetArgument");

            // get the bound GetArgument method
            var parameterValues = parameters.Select(p =>
            {
                return getArgumentMethodUnbound
                    .MakeGenericMethod(p.ParameterType)
                    .Invoke(context, new object[] { p.Name, GetDefault(p.ParameterType) });
            });

            var targetController = serviceProvicer.GetService(method.DeclaringType);

            var result = method.Invoke(targetController, parameterValues.ToArray());
            return result;
        }

        public static object GetDefault(Type type)
        {
            if (type.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        private Type GetGraphType(Type t)
        {
            switch (t.Name.ToLower())
            {
                case "string":
                    return typeof(StringGraphType);
                    break;
                case "book":
                    return typeof(BookType);
                    break;
                case "ienumerable`1":
                    var graphType = GetGraphType(t.GenericTypeArguments[0]);
                    if (graphType != null)
                    { 
                        return typeof(ListGraphType<>).MakeGenericType(graphType);
                    }
                    return null;                    
                    break;
                default:
                    return null;
            }
        }
    }
}
