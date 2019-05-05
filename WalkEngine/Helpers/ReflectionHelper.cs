using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WalkEngine
{
    public static class ReflectionHelper
    {
        public static IEnumerable<T> GetImplementations<T>() =>
            GetImplementationTypes(typeof(T))
                .Select(Activator.CreateInstance)
                .Cast<T>();
        
        public static IEnumerable<Type> GetImplementationTypes(Type baseType)
        {
            var targetAssembly = baseType.Assembly;
            var targetAssemblyName = targetAssembly.GetName().Name;

            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly == targetAssembly || IsReferencing(assembly, targetAssemblyName))
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsGenericTypeDefinition &&
                               !type.IsAbstract &&
                               baseType.IsAssignableFrom(type) &&
                               type.GetConstructor(Array.Empty<Type>()) != null);
        }

        private static bool IsReferencing(Assembly assembly, string targetAssemblyName) =>
            assembly.GetReferencedAssemblies().Any(assemblyName => assemblyName.Name == targetAssemblyName);
    }
}