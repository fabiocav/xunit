﻿using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using Mono.Cecil;

namespace Xunit
{
	/// <summary>
	/// This class provides utility functions related to assemblies.
	/// </summary>
	public static class AssemblyUtility
	{
		/// <summary>
		/// Gets the value for an unknown target framework.
		/// </summary>
		// Note: This value matches AssemblyUtility.UnknownTargetFramework from xunit.v3.core
		public const string UnknownTargetFramework = "UnknownTargetFramework";

		/// <summary>
		/// Gets the target framework name for the given assembly (on disk).
		/// </summary>
		/// <param name="assemblyFileName">The assembly filename.</param>
		/// <returns>The target framework (typically in a format like ".NETFramework,Version=v4.7.2"
		/// or ".NETCoreApp,Version=v2.1"). If the target framework type is unknown (missing file,
		/// missing attribute, etc.) then returns "UnknownTargetFramework".</returns>
		public static string GetTargetFramework(string assemblyFileName)
		{
			// TODO: Should we use File.Exists?
			if (!string.IsNullOrWhiteSpace(assemblyFileName))
			{
				try
				{
					var moduleDefinition = ModuleDefinition.ReadModule(assemblyFileName);
					var targetFrameworkAttribute =
						moduleDefinition
							.GetCustomAttributes()
							.FirstOrDefault(ca => ca.AttributeType.FullName == typeof(TargetFrameworkAttribute).FullName);

					if (targetFrameworkAttribute != null)
					{
						var ctorArg = targetFrameworkAttribute.ConstructorArguments[0];
						if (ctorArg.Value is string targetFramework)
							return targetFramework;
					}
				}
				catch { }  // Eat exceptions so we just return our unknown value
			}

			return UnknownTargetFramework;
		}

		/// <summary>
		/// Gets the target framework name for the given assembly (in memory).
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <returns>The target framework (typically in a format like ".NETFramework,Version=v4.7.2"
		/// or ".NETCoreApp,Version=v2.1"). If the target framework type is unknown (missing file,
		/// missing attribute, etc.) then returns "UnknownTargetFramework".</returns>
		public static string GetTargetFramework(Assembly assembly)
		{
			var targetFrameworkAttribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();
			if (targetFrameworkAttribute != null)
				return targetFrameworkAttribute.FrameworkName;

			return UnknownTargetFramework;
		}
	}
}
