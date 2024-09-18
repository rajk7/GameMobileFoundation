﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#if UNITY_EDITOR
using Sisus.Init.EditorOnly;
#endif

namespace Sisus.Init.Internal
{
	/// <summary>
	/// Utility methods related to clients that can be initialized with arguments.
	/// </summary>
	public static class InitializableUtility
	{
		#if UNITY_EDITOR
		public static event Action<IInitializableEditorOnly> InstanceLoadedInEditor;
		public static void RegisterLoadedInstanceInEditor(IInitializableEditorOnly instance) => InstanceLoadedInEditor?.Invoke(instance);
		#endif

		internal static readonly Dictionary<Type, int> argumentCountsByIInitializableTypeDefinition = new(12)
		{
			{ typeof(IInitializable<>), 1 },
			{ typeof(IInitializable<,>), 2 },
			{ typeof(IInitializable<,,>), 3 },
			{ typeof(IInitializable<,,,>), 4 },
			{ typeof(IInitializable<,,,,>), 5 },
			{ typeof(IInitializable<,,,,,>), 6 },
			{ typeof(IInitializable<,,,,,,>), 7 },
			{ typeof(IInitializable<,,,,,,,>), 8 },
			{ typeof(IInitializable<,,,,,,,,>), 9 },
			{ typeof(IInitializable<,,,,,,,,,>), 10 },
			{ typeof(IInitializable<,,,,,,,,,,>), 11 },
			{ typeof(IInitializable<,,,,,,,,,,,>), 12 }
		};

		internal static bool TryGetParameterTypes([DisallowNull] Type clientType, [MaybeNullWhen(false), NotNullWhen(true)] out Type[] parameterTypes)
		{
			foreach(var interfaceType in clientType.GetInterfaces())
			{
				if(!interfaceType.IsGenericType)
				{
					continue;
				}

				var genericTypeDefinition = interfaceType.IsGenericTypeDefinition ? interfaceType : interfaceType.GetGenericTypeDefinition();
				if(InitializableUtility.argumentCountsByIArgsTypeDefinition.ContainsKey(genericTypeDefinition))
				{
					parameterTypes = interfaceType.GetGenericArguments();
					return true;
				}
			}

			parameterTypes = null;
			return false;
		}

		internal static readonly Dictionary<Type, int> argumentCountsByIArgsTypeDefinition = new(12)
		{
			{ typeof(IArgs<>), 1 },
			{ typeof(IArgs<,>), 2 },
			{ typeof(IArgs<,,>), 3 },
			{ typeof(IArgs<,,,>), 4 },
			{ typeof(IArgs<,,,,>), 5 },
			{ typeof(IArgs<,,,,,>), 6 },
			{ typeof(IArgs<,,,,,,>), 7 },
			{ typeof(IArgs<,,,,,,,>), 8 },
			{ typeof(IArgs<,,,,,,,,>), 9 },
			{ typeof(IArgs<,,,,,,,,,>), 10 },
			{ typeof(IArgs<,,,,,,,,,,>), 11 },
			{ typeof(IArgs<,,,,,,,,,,,>), 12 }
		};

		public static Type GetIInitializableType(int argumentCount)
		{
			foreach((Type type, int count) in argumentCountsByIInitializableTypeDefinition)
			{
				if(count == argumentCount)
				{
					return type;
				}
			}

			return null;
		}

		public static Type GetIInitializableType(Type[] genericArguments) => GetIInitializableType(genericArguments.Length)?.MakeGenericType(genericArguments);

		/// <summary>
		/// Does the client derive from base class that can handle automatically initializing itself with all services?
		/// </summary>
		public static bool CanSelfInitializeWithoutInitializer([DisallowNull] object client) => client is Component component && CanSelfInitializeWithoutInitializer(component);
		
		/// <summary>
		/// Does the client derive from base class that can handle automatically initializing itself with all services?
		/// </summary>
		public static bool CanSelfInitializeWithoutInitializer([DisallowNull] Component client) => TypeUtility.DerivesFromGenericBaseType(client.GetType());
	}
}