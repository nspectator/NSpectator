using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NSpectator.Domain.Extensions
{
    public static class DomainExtensions
    {
        public static T Instance<T>(this Type type) where T : class
        {
            return type.GetConstructors()[0].Invoke(new object[0]) as T;
        }

        /// <summary>
        /// Get all non-async methods of the type with specific build-in filter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> AllMethods(this Type type)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;

            var exclusions = typeof(Spec).GetMethods(flags).Select(m => m.Name);

            var methodInfos = type.GetAbstractBaseClassChainWithClass().SelectMany(t => t.GetMethods(flags));

            return methodInfos
                .Where(m => !exclusions.Contains(m.Name) && !(m.Name.Contains("<") || m.Name.Contains("$")) && m.Name.Contains("_"))
                .Where(m => m.GetParameters().Length == 0)
                .Where(m => !m.IsAsync())
                .Where(m => m.ReturnType == typeof(void)).ToList();
        }

        /// <summary>
        /// Get all async methods of the type with specific build-in filter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> AsyncMethods(this Type type)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;

            var exclusions = typeof(Spec).GetMethods(flags).Select(m => m.Name);

            var methodInfos = type.GetAbstractBaseClassChainWithClass().SelectMany(t => t.GetMethods(flags));

            return methodInfos
                .Where(m => !exclusions.Contains(m.Name) && !(m.Name.Contains("<") || m.Name.Contains("$")) && m.Name.Contains("_"))
                .Where(m => m.GetParameters().Length == 0)
                .Where(m => m.IsAsync()).ToList();
        }

        public static IEnumerable<Type> GetAbstractBaseClassChainWithClass(this Type type)
        {
            var baseClasses = new Stack<Type>();

            for (Type baseClass = type.BaseType;
                baseClass != null && baseClass.IsAbstract;
                baseClass = baseClass.BaseType)
            {
                baseClasses.Push(baseClass);
            }

            while (baseClasses.Count > 0) yield return baseClasses.Pop();

            yield return type;
        }

        public static string CleanName(this Type type)
        {
            if (!type.IsGenericType) return type.Name;

            return $"{type.Name.Remove(type.Name.IndexOf('`'))}<{string.Join(", ", type.GetGenericArguments().Select(CleanName).ToArray())}>";
        }

        public static string CleanMessage(this Exception exception)
        {
            var exc = exception.Message.Trim().Replace("\r\n", ", ").Replace("\n", ", ").Trim();

            while (exc.Contains("  ")) exc = exc.Replace("  ", " ");

            return exc;
        }

        public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var t in enumerable)
            {
                action(t);
            }
        }

        /// <summary>
        /// Check if method of the type is async
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsAsync(this MethodInfo method)
        {
            // Taken from: https://github.com/nunit/nunit/blob/master/src/NUnitFramework/framework/Internal/AsyncInvocationRegion.cs

            return method.ReturnType.FullName.StartsWith(TaskTypeName) ||
                   method.GetCustomAttributes(false).Any(attr => AsyncAttributeTypeName == attr.GetType().FullName);
        }

        /// <summary>
        /// Check if action is async
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool IsAsync(this Action action)
        {
            return IsAsync(action.Method);
        }
        
        const string TaskTypeName = "System.Threading.Tasks.Task";
        const string AsyncAttributeTypeName = "System.Runtime.CompilerServices.AsyncStateMachineAttribute";
    }
}