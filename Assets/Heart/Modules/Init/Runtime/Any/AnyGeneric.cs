﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sisus.Init.Internal;
using Sisus.Init.Serialization;
using Sisus.Init.ValueProviders;
using UnityEngine;
using Component = UnityEngine.Component;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

[assembly: InternalsVisibleTo("InitArgs.uGUI")]

namespace Sisus.Init
{
	/// <summary>
	/// Represents a value of generic type <typeparamref name="T"/> that can be assigned
	/// through Unity's inspector as well as serialized by its serializer.
	/// <para>
	/// <see cref="Any{T}"/> does not support serializing generic types - with the exception of <see cref="List{T}"/>
	/// - so this type should be used for that purpose instead, in custom initializer and such.
	/// </para>
	/// <para>
	/// If <typeparamref name="T"/> is a <see cref="ServiceAttribute">service</see> type
	/// then this object represents the <see cref="Service{T}.Instance">shared instance</see> of that service.
	/// </para>
	/// <para>
	/// If a class derives from <see cref="Object"/> and implements <see cref="IValueProvider{T}"/> then
	/// <see cref="AnyGeneric{T}"/> can wrap an instance of this class and return its <see cref="IValueProvider{T}.Value"/>
	/// when <see cref="Value"/> is called.
	/// </para>
	/// <para>
	/// If a class derives from <see cref="Object"/> and implements <see cref="IValueByTypeProvider"/> then
	/// <see cref="AnyGeneric{T}"/> can wrap an instance of this class and use <see cref="IValueByTypeProvider.TryGetFor"/>
	/// to resolve the value to return when <see cref="Value"/> is called.
	/// </para>
	/// </summary>
	/// <typeparam name="T">
	/// Type of the value that this object wraps.
	/// <para>
	/// Can be an interface type, a class type or a value type.
	/// </para>
	/// </typeparam>
	[Serializable]
	#if ODIN_INSPECTOR
	[Sirenix.OdinInspector.InlineProperty]
	#endif
	public struct AnyGeneric<T> : IAny
		#if DEBUG || INIT_ARGS_SAFE_MODE
		, ISerializationCallbackReceiver
		#endif
		#if UNITY_EDITOR
		, INullGuard
		#endif
	{
		[SerializeField]
		internal Object reference;

		[SerializeField]
		internal T value;

		/// <summary>
		/// Gets a value indicating whether the current <see cref="AnyGeneric{T}"/> object
		/// has a non-null underlying type.
		/// <para>
		/// <see langword="true"/> if the current <see cref="AnyGeneric{T}"/> object has a value;
		/// <see langword="false"/> if the current <see cref="AnyGeneric{T}"/> object has no value.
		/// </para>
		/// </summary>
		public bool HasValue
		{
			get
			{
				#if UNITY_EDITOR
				if(!ServiceUtility.ServicesAreReady && ServiceUtility.IsServiceDefiningType<T>())
				{
					return true;
				}
				#endif

				return TryGetValue(out _);
			}
		}

		/// <summary>
		/// Gets the value of the current <see cref="AnyGeneric{T}"/> object
		/// if it has been assigned a valid underlying value;
		/// otherwise, the default value of <see cref="T"/>.
		/// <para>
		/// If <see cref="T"/> is a <see cref="ServiceAttribute">service</see> type then returns
		/// the <see cref="Service{T}.Instance">shared instance</see> of that service.
		/// </para>
		/// </summary>
		[MaybeNull, Obsolete(nameof(Value) + " should be used instead.", false)]
		public T ValueOrDefault => TryGetValue(out T result) ? result : default;
		
		/// <summary>
		/// Gets the value of the current <see cref="AnyGeneric{T}"/> object
		/// if it has been assigned a valid underlying value;
		/// otherwise, the default value of <see cref="T"/>.
		/// <para>
		/// If <see cref="T"/> is a <see cref="ServiceAttribute">service</see> type then returns
		/// the <see cref="Service{T}.Instance">shared instance</see> of that service.
		/// </para>
		/// </summary>
		public T Value => GetValue(null as Component, Context.MainThread);

		/// <summary>
		/// Initializes a new instance of the <see cref="AnyGeneric"/> struct with the given underlying value.
		/// </summary>
		/// <param name="value"> The underlying value of the <see cref="AnyGeneric{T}"/> object. </param>
		public AnyGeneric(T value)
		{
			if(value is Object unityObject)
			{
				this.value = default;
				this.reference = unityObject;
				return;
			}

			this.value = value;
			this.reference = default;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AnyGeneric"/> struct with the given underlying value.
		/// </summary>
		/// <param name="value">
		/// The underlying value of the <see cref="AnyGeneric{T}"/> object.
		/// </param>
		/// <exception cref="InvalidCastException">
		/// Thrown if <paramref name="value"/> could not be cast to <see cref="T"/> nor
		/// converted to it via an <see cref="IValueByTypeProvider"/> or <see cref="IValueProvider{T}"/> implementation.
		/// </exception>
		public AnyGeneric(Object value)
		{
			AssertConstructorArgumentIsValid(value);

			this.value = default;
			reference = value;
		}

		/// <summary>
		/// Creates a new instance of the <see cref="AnyGeneric"/> struct with the given underlying value.
		/// </summary>
		/// <param name="value"> The underlying value of the <see cref="AnyGeneric{T}"/> object. </param>
		/// <returns> A new instance of the <see cref="AnyGeneric{T}"/> struct. </returns>
		public static AnyGeneric<T> FromValue(T value) => new AnyGeneric<T>(value);

		/// <summary>
		/// Creates a new instance of the <see cref="AnyGeneric"/> struct with the given underlying value.
		/// </summary>
		/// <param name="value">
		/// The underlying value of the <see cref="AnyGeneric{T}"/> object.
		/// </param>
		/// <returns> A new instance of the <see cref="AnyGeneric{T}"/> struct. </returns>
		public static AnyGeneric<T> FromObject([AllowNull] Object value) => new AnyGeneric<T>(value);

		/// <summary>
		/// Returns a value indicating whether or not it is possible to create an instance of the
		/// <see cref="AnyGeneric"/> struct with the underlying value of <paramref name="fromReference"/>.
		/// </summary>
		/// <param name="fromReference"> The underlying value of the <see cref="AnyGeneric{T}"/> object. </param>
		/// <returns> A new instance of the <see cref="AnyGeneric{T}"/> struct. </returns>
		public static bool IsCreatableFrom([AllowNull] Object fromReference, Component forClient)
		{
			if(!fromReference)
			{
				return typeof(T).IsValueType;
			}

			if(fromReference is T or IValueProvider<T> or IValueProviderAsync<T>)
			{
				return true;
			}

			if(fromReference is IValueByTypeProvider valueByTypeProvider && valueByTypeProvider.CanProvideValue<T>(forClient))
			{
				return true;
			}

			if(fromReference is IValueByTypeProviderAsync valueByTypeProviderAsync && valueByTypeProviderAsync.CanProvideValue<T>(forClient))
			{
				return true;
			}

			if(fromReference is IValueProvider)
			{
				if(fromReference is INullGuard nullGuard && nullGuard.EvaluateNullGuard(forClient) == NullGuardResult.Passed)
				{
					return true;
				}

				if(fromReference is INullGuardByType nullGuardByType && nullGuardByType.EvaluateNullGuard<T>(forClient) == NullGuardResult.Passed)
				{
					return true;
				}

				foreach(Type interfaceType in fromReference.GetType().GetInterfaces())
				{
					if(!interfaceType.IsGenericType || interfaceType.GetGenericTypeDefinition() != typeof(IValueProvider<>))
					{
						continue;
					}

					Type providedValueType = interfaceType.GetGenericArguments()[0];
					if(typeof(T).IsAssignableFrom(providedValueType))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Defines an implicit conversion of <see cref="AnyGeneric{T}"/> to the <see cref="T"/> object that it wraps.
		/// </summary>
		/// <param name="instance"> The <see cref="AnyGeneric{T}"/> instance to convert. </param>
		public static implicit operator T(AnyGeneric<T> instance) => instance.Value;

		/// <summary>
		/// Defines an explicit conversion of <see cref="T"/> to an <see cref="AnyGeneric{T}"/> object.
		/// </summary>
		/// <param name="value"> The value to convert. </param>
		public static implicit operator AnyGeneric<T>(T value) => new AnyGeneric<T>(value);
 
		/// <summary>
		/// Gets the current value of this object.
		/// <para>
		/// If the underlying value is <see langword="null"/> and <see cref="T"/> is the defining
		/// type of a service that is accessible to the <paramref name="client"/>,
		/// then returns the service.
		/// </para>
		/// <para>
		/// If the underlying value is a <see cref="IValueProvider{T}"/>, then returns the
		/// value provided by it.
		/// </para>
		/// </summary>
		/// <param name="client">
		/// The scriptable object requesting the value, or <see langword="null"/> if the requester
		/// is unknown or not a component.
		/// </param>
		/// <param name="context"> The context from which the request is being made. </param>
		/// <returns>
		/// The value associated with the current <see cref="AnyGeneric{T}"/> object, if it has a value;
		/// otherwise, the default value of <see cref="T"/>.
		/// </returns>
		public T GetValue([DisallowNull] ScriptableObject client, Context context = Context.MainThread) => GetValue(null as Component, context);

		/// <summary>
		/// Gets the current value of this object.
		/// <para>
		/// If the underlying value is <see langword="null"/> and <see cref="T"/> is the defining
		/// type of a service that is accessible to the <paramref name="client"/>,
		/// then returns the service.
		/// </para>
		/// <para>
		/// If the underlying value is a <see cref="IValueProvider{T}"/>, then returns the
		/// value provided by it.
		/// </para>
		/// </summary>
		/// <param name="client">
		/// The component requesting the value, or <see langword="null"/> if the requester
		/// is unknown or is not a component.
		/// </param>
		/// <param name="context"> The context from which the request is being made. </param>
		/// <returns>
		/// The value associated with the current <see cref="AnyGeneric{T}"/> object, if it has a value;
		/// otherwise, the default value of <see cref="T"/>.
		/// </returns>
		public T GetValue([AllowNull] Component client, Context context = Context.MainThread)
		{
			// Prefer cached value over a value provider - this can help avoid issues
			// such as addressable assets being loaded more than once by the same client
			// and causing the internal counter to get incremented too many times.
			// However, this should only be done with types where Unity's serialization
			// does not automatically give them a non-null value. A string field for
			// example will never have a null value
			if(value is not null)
			{
				// Unity won't automatically assign values to Object type fields.
				if(value is Object cachedObject)
				{
					if(cachedObject.GetHashCode() != 0)
					{
						return value;
					}
				}
				// Unity won't automatically assign values if it's an abstract type field.
				else if(value.GetType().IsAbstract)
				{
					return value;
				}
			}

			if(reference is not null && reference.GetHashCode() != 0)
			{
				switch(reference)
				{
					case T referenceValue:
						return referenceValue;
					// If value is _Null token, don't return Service, even if T is defining type of a service
					case _Null _:
						return value;
					// Prefer sync value providers over async ones when using GetValue.
					case IValueProvider<T> valueProvider:
						return valueProvider.TryGetFor(client, out T result) ? CacheValueProviderResult(result) : default;
					case IValueByTypeProvider valueProvider:
						return valueProvider.TryGetFor(client, out result) ? CacheValueProviderResult(result) : default;
					case IValueProvider valueProvider when valueProvider.Value is object objectValue && Find.In(objectValue, out result):
						return CacheValueProviderResult(result);
					case IValueProviderAsync<T> valueProvider:
						var awaitable = valueProvider.GetForAsync(client);
						#if UNITY_2023_1_OR_NEWER
						return awaitable.GetAwaiter().IsCompleted ? CacheValueProviderResult(awaitable.GetAwaiter().GetResult()) : default;
						#else
						return awaitable.IsCompletedSuccessfully ? CacheValueProviderResult(awaitable.Result) : default;
						#endif
					case IValueByTypeProviderAsync valueProvider:
						awaitable = valueProvider.GetForAsync<T>(client);
						#if UNITY_2023_1_OR_NEWER
						return awaitable.GetAwaiter().IsCompleted ? CacheValueProviderResult(awaitable.GetAwaiter().GetResult()) : default;
						#else
						return awaitable.IsCompletedSuccessfully ? CacheValueProviderResult(awaitable.Result) : default;
						#endif
				}
			}

			return value switch
			{
				null when client && context.IsUnitySafeContext() => Service.TryGetFor(client, out T service) ? service : default,
				null => Service.TryGet(out T service) ? service : default,
				// Needed to support value providers in AnyGeneric.Serialization.cs
				IValueProvider<T> valueProvider => valueProvider.Value,
				_ => value
			};
		}
		
		private T CacheValueProviderResult(T result)
		{
			// Don't cache in edit mode
			#if UNITY_EDITOR
			if(!Application.isPlaying || !string.IsNullOrEmpty(UnityEditor.AssetDatabase.GetAssetPath(reference)))
			{
				return result;
			}
			#endif

			value = result;
			return result;
		}

		/// <summary>
		/// Gets the value of the current <see cref="AnyGeneric{T}"/> object if it has one.
		/// <para>
		/// If the underlying value is <see langword="null"/> and <see cref="T"/> is a <see cref="ServiceAttribute">global service</see>
		/// or a <see cref="Services">scene service</see> which is accessible to the <paramref name="client"/> then gets the service.
		/// </para>
		/// </summary>
		/// <typeparam name="TClient"> Type of the client requesting the value. </typeparam>
		/// <param name="client"> The client requesting the value. </param>
		/// <param name="context"> The context from which the request is being made. </param>
		/// <returns>
		/// The value associated with the current <see cref="AnyGeneric{T}"/> object, if it has a value;
		/// otherwise, the default value of <see cref="T"/>.
		/// </returns>
		[Obsolete(nameof(GetValue) + " should be used instead.", false)]
		public T GetValueOrDefault([DisallowNull] object client, Context context = Context.MainThread) => GetValue(client as Component, context);

		/// <summary>
		/// Gets the value of the current <see cref="AnyGeneric{T}"/> object if it has one.
		/// <para>
		/// If the underlying value is <see langword="null"/> and <see cref="T"/> is a <see cref="ServiceAttribute">global service</see>
		/// or a <see cref="Services">scene service</see> which is accessible to the <paramref name="client"/> then gets the service.
		/// </para>
		/// </summary>
		/// <typeparam name="TClient"> Type of the client requesting the value. </typeparam>
		/// <param name="client"> The client requesting the value. </param>
		/// <param name="context"> The context from which the request is being made. </param>
		/// <returns>
		/// The value associated with the current <see cref="AnyGeneric{T}"/> object, if it has a value;
		/// otherwise, the default value of <see cref="T"/>.
		/// </returns>
		public async
		#if UNITY_2023_1_OR_NEWER
		Awaitable<T>
		#else
		Task<T>
		#endif
		GetValueAsync(Component client = null, Context context = Context.MainThread)
		{
			// Prefer cached value over a value provider - this can help avoid issues
			// such as addressable assets being loaded more than once by the same client
			// and causing the internal counter to get incremented too many times.
			// However, this should only be done with types where Unity's serialization
			// does not automatically give them a non-null value. A string field for
			// example will never have a null value
			if(value is not null)
			{
				// Unity won't automatically assign values to Object type fields.
				if(value is Object cachedObject)
				{
					if(cachedObject.GetHashCode() != 0)
					{
						return value;
					}
				}
				// Unity won't automatically assign values if it's an abstract type field.
				else if(value.GetType().IsAbstract)
				{
					return value;
				}
			}

			return reference switch
			{
				// Handle "fake null" references
				null => value switch
				{
					null when client && context.IsUnitySafeContext() => Service.TryGetFor(client, out T service) ? service : default,
					null => Service.TryGet(out T service) ? service : default,
					// Needed to support value providers in AnyGeneric.Serialization.cs
					IValueProvider<T> valueProvider => valueProvider.Value,
					_ => value
				},
				_ when reference.GetHashCode() == 0 => value switch
				{
					null when client && context.IsUnitySafeContext() => Service.TryGetFor(client, out T service) ? service : default,
					null => Service.TryGet(out T service) ? service : default,
					// Needed to support value providers in AnyGeneric.Serialization.cs
					IValueProvider<T> valueProvider => valueProvider.Value,
					_ => value
				},
				// Support value providers in AnyGeneric.Serialization.cs, like _String
				_ when value is IValueProvider<T> valueProvider => valueProvider.Value,
				T directReference => directReference,
				// If value is _Null token, don't return Service, even if T is defining type of a service
				_Null => value,
				// Prefer async value providers over sync ones since this is an async method
				IValueByTypeProviderAsync valueProvider => CacheValueProviderResult(await valueProvider.GetForAsync<T>(client)),
				IValueProviderAsync<T> valueProvider => CacheValueProviderResult(await valueProvider.GetForAsync(client)),
				IValueProvider<T> valueProvider => valueProvider.TryGetFor(client, out T result) ? CacheValueProviderResult(result) : default,
				IValueByTypeProvider valueProvider => valueProvider.TryGetFor(client, out T result) ? CacheValueProviderResult(result) : default,
				IValueProvider valueProvider when valueProvider.Value is object objectValue && Find.In(objectValue, out T result) => CacheValueProviderResult(result),
				_ when client && context.IsUnitySafeContext() => Service.TryGetFor(client, out T service) ? service : default,
				_ when Service.TryGet(out T service) => service,
				_ => value
			};
		}

		/// <summary>
		/// Gets a value indicating whether the current <see cref="AnyGeneric{T}"/> object has a non-null value
		/// when retrieved by the <paramref name="client"/> in question in the given <paramref name="context"/>.
		/// </summary>
		/// <typeparam name="TClient"> Type of the client checking the value. </typeparam>
		/// <param name="client"> The client checking the value. </param>
		/// <param name="context">
		/// The context from which the checking is being made.
		/// <para>
		/// Some services might not be accessible in a <see cref="Context.Threaded"/> context.
		/// </para>
		/// </param>
		/// <returns>
		/// <see langword="true"/> if the <see cref="AnyGeneric{T}"/> object has a non-null value.
		/// <para>
		/// <see langword="true"/> if <see cref="T"/> is the defining type of a <see cref="ServiceAttribute">global service</see>
		/// or a <see cref="Services">scene service</see> that is accessible to the <paramref name="client"/>
		/// in the current <paramref name="context"/>.
		/// </para>
		/// <para>
		/// Otherwise, <see langword="false"/>.
		/// </para>
		/// </returns>
		public bool GetHasValue([AllowNull] Component client, Context context = Context.MainThread)
		{
			if(reference is INullGuard nullGuard && reference)
			{
				return nullGuard.EvaluateNullGuard(client) == NullGuardResult.Passed;
			}

			if(reference is INullGuardByType nullGuardByType && reference)
			{
				return nullGuardByType.EvaluateNullGuard<T>(client) == NullGuardResult.Passed;
			}

			// Prefer cached value over a value provider - this can help avoid issues
			// such as addressable assets being loaded more than once by the same client
			// and causing the internal counter to get incremented too many times.
			// However, this should only be done with types where Unity's serialization
			// does not automatically give them a non-null value. A string field for
			// example will never have a null value
			if(value is not null)
			{
				// Unity won't automatically assign values to Object type fields.
				if(value is Object cachedObject)
				{
					if(cachedObject.GetHashCode() != 0)
					{
						return true;
					}
				}
				// Unity won't automatically assign values if it's an abstract type field.
				else if(value.GetType().IsAbstract)
				{
					return true;
				}
			}

			if(reference is not null && reference.GetHashCode() != 0)
			{
				switch(reference)
				{
					case T:
						return true;
					// If value is _Null token, don't return Service, even if T is defining type of a service
					case _Null:
						return value is not null;
					// Prefer non-async value provider interfaces over async ones
					case IValueProvider<T> valueProvider:
						return valueProvider.TryGetFor(client, out _);
					case IValueByTypeProvider valueProvider:
						return valueProvider.TryGetFor<T>(client, out _);
					case IValueProvider valueProvider when valueProvider.Value is T:
						return true;
					case IValueProviderAsync<T> valueProvider:
						var awaitable = valueProvider.GetForAsync(client);
						#if UNITY_2023_1_OR_NEWER
						return awaitable.GetAwaiter().IsCompleted && awaitable.GetAwaiter().GetResult() is not null;
						#else
						return !awaitable.IsFaulted;
						#endif
					case IValueByTypeProviderAsync valueProvider:
						awaitable = valueProvider.GetForAsync<T>(client);
						#if UNITY_2023_1_OR_NEWER
						return awaitable.GetAwaiter().IsCompleted && awaitable.GetAwaiter().GetResult() is not null;
						#else
						return !awaitable.IsFaulted;
						#endif
				}
			}

			#if UNITY_EDITOR
			if(context.IsEditMode() && ServiceUtility.IsServiceDefiningType<T>())
			{
				return true;
			}
			#endif

			return value switch
			{
				null when client && context.IsUnitySafeContext() => Service.ExistsFor<T>(client),
				null => Service.Exists<T>(),
				// Support value providers in AnyGeneric.Serialization.cs, like _String.
				IValueProvider<T> valueProvider => valueProvider.Value is not null,
				_ => true,
			};
		}

		/// <summary>
		/// Gets the current value of the <see cref="AnyGeneric{T}"/> object if it has one.
		/// <para>
		/// This method can only be called from the main thread.
		/// </para>
		/// </summary>
		/// <param name="value">
		/// When this method returns, contains the current value of the <see cref="AnyGeneric{T}"/> object, if it has been assigned one.
		/// <para>
		/// If <see cref="T"/> is the defining type of a <see cref="ServiceAttribute">global service</see>
		/// or a <see cref="Services">scene service</see> that is accessible to all clients, then contains
		/// the shared instance of that service.
		/// </para>
		/// Otherwise, contains the default value of <see cref="T"/>.
		/// <para>
		/// This parameter is passed uninitialized.
		/// </para>
		/// </param>
		/// <returns>
		/// <see langword="true"/> if the <see cref="AnyGeneric{T}"/> object has a non-null value.
		/// <para>
		/// <see langword="true"/> if <see cref="T"/> is the defining type of a <see cref="ServiceAttribute">global service</see>
		/// or a <see cref="Services">scene service</see> that is accessible to all clients.
		/// </para>
		/// <para>
		/// Otherwise, <see langword="false"/>.
		/// </para>
		public bool TryGetValue(out T value) => TryGetValue(null, Context.MainThread, out value);

		/// <summary>
		/// Gets the value of the current <see cref="AnyGeneric{T}"/> object if it has one.
		/// <para>
		/// If the underlying value is <see langword="null"/> and <see cref="T"/> is a <see cref="ServiceAttribute">global service</see>
		/// or a <see cref="Services">scene service</see> which is accessible to the <paramref name="client"/> then gets the service.
		/// </para>
		/// </summary>
		/// <param name="value">
		/// When this method returns, contains the value associated with the current <see cref="AnyGeneric{T}"/> object, if it has a value;
		/// otherwise, the default value of <see cref="T"/>. This parameter is passed uninitialized.
		/// </param>
		/// <returns> <see langword="true"/> if a valid underlying value has been assigned; otherwise, <see langword="false"/>. </returns>
		public bool TryGetValue(Component client, Context context, out T result)
		{
			// Prefer cached value over a value provider - this can help avoid issues
			// such as addressable assets being loaded more than once by the same client
			// and causing the internal counter to get incremented too many times.
			// However, this should only be done with types where Unity's serialization
			// does not automatically give them a non-null value. A string field for
			// example will never have a null value
			if(value is not null)
			{
				// Unity won't automatically assign values to Object type fields.
				if(value is Object cachedObject)
				{
					if(cachedObject.GetHashCode() != 0)
					{
						result = value;
						return true;
					}
				}
				// Unity won't automatically assign values if it's an abstract type field.
				else if(value.GetType().IsAbstract)
				{
					result = value;
					return true;
				}
			}

			if(reference is not null && reference.GetHashCode() != 0)
			{
				switch(reference)
				{
					case T referenceValue:
						result = referenceValue;
						return true;
					// If value is _Null token, don't return Service, even if T is defining type of a service
					case _Null _:
						result = value;
						return value is not null;
					// Prefer non-async value provider interfaces over async ones
					case IValueProvider<T> valueProvider:
						if(valueProvider.TryGetFor(client, out result))
						{
							CacheValueProviderResult(result);
							return true;
						}

						return false;
					case IValueByTypeProvider valueByTypeProvider:
						if(valueByTypeProvider.TryGetFor(client, out result))
						{
							CacheValueProviderResult(result);
							return true;
						}

						return false;
					case IValueProvider valueProvider when valueProvider.Value is T valueAsT:
						result = CacheValueProviderResult(valueAsT);
						return true;
					case IValueProviderAsync<T> valueProvider:
						var awaitable = valueProvider.GetForAsync(client);
						#if UNITY_2023_1_OR_NEWER
						var awaiter = awaitable.GetAwaiter();
						if(awaiter.IsCompleted)
						{
							result = CacheValueProviderResult(awaiter.GetResult());
							return result is not null;
						}
						#else
						if(awaitable.IsCompletedSuccessfully)
						{
							result = CacheValueProviderResult(awaitable.Result);
							return result is not null;
						}
						#endif

						result = default;
						return false;
					case IValueByTypeProviderAsync valueProvider:
						awaitable = valueProvider.GetForAsync<T>(client);
						#if UNITY_2023_1_OR_NEWER
						awaiter = awaitable.GetAwaiter();
						if(awaiter.IsCompleted)
						{
							result = CacheValueProviderResult(awaiter.GetResult());
							return result is not null;
						}
						#else
						if(awaitable.IsCompletedSuccessfully)
						{
							result = CacheValueProviderResult(awaitable.Result);
							return result is not null;
						}
						#endif

						result = default;
						return false;
				}
			}

			switch(value)
			{
				case null when client && context.IsUnitySafeContext():
					return Service.TryGetFor(client, out result);
				case null:
					return Service.TryGet(out result);
				// Support value providers in AnyGeneric.Serialization.cs, like _String.
				case IValueProvider<T> valueProvider:
					result = valueProvider.Value;
					return result is not null;
				default:
					result = value;
					return result is not null;
			}
		}

		/// <summary>
		/// Gets the value of the current <see cref="AnyGeneric{T}"/> object if it has one.
		/// <para>
		/// If the underlying value is <see langword="null"/> and <see cref="T"/> is a <see cref="ServiceAttribute">global service</see>
		/// or a <see cref="Services">scene service</see> which is accessible to the <paramref name="client"/> then gets the service.
		/// </para>
		/// <para>
		/// This method can only be called from the main thread.
		/// </para>
		/// </summary>
		/// <param name="value">
		/// When this method returns, contains the value associated with the current <see cref="AnyGeneric{T}"/> object, if it has a value;
		/// otherwise, the default value of <see cref="T"/>. This parameter is passed uninitialized.
		/// </param>
		/// <returns> <see langword="true"/> if a valid underlying value has been assigned; otherwise, <see langword="false"/>. </returns>
		public bool TryGetValue(Component client, out T value) => TryGetValue(client, Context.MainThread, out value);

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj"> The object to compare with the current object. </param>
		/// <returns> <see langword="true"/> if the specified object is equal to the current object; otherwise, <see langword="false"/>. </returns>
		public override bool Equals(object obj)
		{
			if(obj is null)
			{
				return value is null && !reference && !ServiceUtility.IsServiceDefiningType<T>();
			}

			if(obj is AnyGeneric<T> any)
			{
				return Equals(any);
			}

			return obj.Equals(Value);
		}

		/// <summary>
		/// Determines whether the specified object is equal to the value of the current object.
		/// </summary>
		/// <param name="other"> The object to compare with the current object's value. </param>
		/// <returns> <see langword="true"/> if the specified object is equal to the current object's value; otherwise, <see langword="false"/>. </returns>
		public bool Equals(T other)
		{
			if(other is null)
			{
				return value is null && !reference && !ServiceUtility.IsServiceDefiningType<T>();
			}

			return EqualityComparer<T>.Default.Equals(other, Value);
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other"> The object to compare with the current object. </param>
		/// <returns> <see langword="true"/> if the specified object is equal to the current object; otherwise, <see langword="false"/>. </returns>
		public bool Equals(AnyGeneric<T> other) => reference == other.reference && EqualityComparer<T>.Default.Equals(value, other.value);

		/// <summary>
		/// Determines whether the specified object is equal to the value of the current object.
		/// </summary>
		/// <param name="other"> The object to compare with the current object's value. </param>
		/// <returns> <see langword="true"/> if the specified object is equal to the current object's value; otherwise, <see langword="false"/>. </returns>
		public bool Equals(Object other, Context context = Context.MainThread)
		{
			if(!other)
			{
				return value is null && !reference && !ServiceUtility.IsServiceDefiningType<T>();
			}

			if(reference == other)
			{
				return true;
			}

			if(reference)
			{
				return false;
			}

			return Service.TryGetFor(context, out T service) && service as Object == other;
		}

		public
		#if UNITY_2023_1_OR_NEWER
		Awaitable<T>.Awaiter GetAwaiter() => GetValueAsync().GetAwaiter();
		#else
		System.Runtime.CompilerServices.TaskAwaiter<T> GetAwaiter() => GetValueAsync().GetAwaiter();
		#endif

		public async Task<T> AsTask() => await GetValueAsync();
		public async ValueTask<T> AsValueTask() => await GetValueAsync();

		/// <summary>
		/// Returns a string that represents the underlying object.
		/// </summary>
		/// <returns> A string that represents the underlying object, if it has been assigned; otherwise, and empty string. </returns>
		public override string ToString() => TryGetValue(out T value) ? value.ToString() : "";

		/// <summary>
		/// Returns the hash code for this <see cref="AnyGeneric{T}"/> object.
		/// </summary>
		/// <returns> Hash code of the underlying object, if it has been assigned; otherwise, 0. </returns>
		public override int GetHashCode() => TryGetValue(out T value) ? value.GetHashCode() : 0;

		[Conditional("DEBUG"), Conditional("INIT_ARGS_SAFE_MODE")]
		private static void AssertConstructorArgumentIsValid(Object reference)
		{
			#if DEBUG || INIT_ARGS_SAFE_MODE
			if(!reference)
			{
				if(typeof(T).IsValueType)
				{
					throw new InvalidCastException($"AnyGeneric<{typeof(T).Name}> can not have a null value because {typeof(T).Name} is a value type.");
				}

				return;
			}

			if(reference is T or IValueProvider or IValueByTypeProvider or IValueProviderAsync or IValueByTypeProviderAsync)
			{
				return;
			}

			throw new InvalidCastException($"AnyGeneric<{typeof(T).Name}> can not have a value of type {reference.GetType().Name}.");
			#endif
		}

		[Conditional("DEBUG"), Conditional("INIT_ARGS_SAFE_MODE")]
		private static void AssertSerializedReferenceIsValid(Object reference)
		{
			#if DEBUG || INIT_ARGS_SAFE_MODE
			if(!reference || reference is T or IValueProvider or IValueProviderAsync or IValueByTypeProvider or IValueByTypeProviderAsync)
			{
				return;
			}

			throw new InvalidCastException($"AnyGeneric<{typeof(T).Name}> can not have a value of type {reference.GetType().Name}.");
			#endif
		}

		#if DEBUG || INIT_ARGS_SAFE_MODE
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if(typeof(T) == typeof(object) && value is not null)
			{
				if(value is int intValue)
				{
					value = (T)(object)new _Integer() { value = intValue };
				}
				else if(value is bool boolValue)
				{
					value = (T)(object)new _Boolean() { value = boolValue };
				}
				else if(value is float floatValue)
				{
					value = (T)(object)new _Float() { value = floatValue };
				}
				else if(value is string stringValue)
				{
					value = (T)(object)new _String() { value = stringValue };
				}
				else if(value is double doubleValue)
				{
					value = (T)(object)new _Double() { value = doubleValue };
				}
				else if(value is Type typeValue)
				{
					value = (T)(object)new _Type(typeValue, null);
				}
			}

			if(value is Object)
			{
				value = default;
			}

			if(reference is CrossSceneReference crossSceneReference && !crossSceneReference.isCrossScene)
			{
				var crossSceneReferenceValue = crossSceneReference.Value;
				if(crossSceneReferenceValue)
				{
					#if DEV_MODE
					Debug.Log($"CrossSceneReference {crossSceneReferenceValue.name} ({crossSceneReferenceValue.GetType().Name}) isCrossScene was false. Changing into a direct reference.");
					#endif

					if(crossSceneReference.Value is T directReference)
					{
						value = directReference;
					}
					#if DEV_MODE
					else
					{
						Debug.LogWarning($"CrossSceneReference {crossSceneReferenceValue.name} of type {crossSceneReferenceValue.GetType().Name} not convertible to {typeof(T).Name}.");
					}
					#endif
				}
			}

			try
			{
				AssertSerializedReferenceIsValid(reference);
			}
			catch(InvalidCastException e)
			{
				if(reference is GameObject gameObject)
				{
					foreach(var component in gameObject.GetComponentsNonAlloc<Component>())
					{
						if(IsCreatableFrom(component, component))
						{
							reference = component;
							return;
						}
					}
				}

				Debug.LogWarning(e.Message, reference);
				reference = null;
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize() { }
		#endif

		#if UNITY_EDITOR
		internal bool HasSerializedValue() => reference || value is not null;
		#endif

		#if UNITY_EDITOR
		NullGuardResult INullGuard.EvaluateNullGuard(Component client) => EvaluateNullGuard(client, Context.MainThread);
		internal NullGuardResult EvaluateNullGuard(Component client = null, Context context = Context.MainThread)
		{
			if(reference is not null && reference.GetHashCode() != 0)
			{
				switch(reference)
				{
					case T:
						return NullGuardResult.Passed;
					// If value is _Null token, don't return Service, even if T is defining type of a service
					case _Null:
						return value is not null ? NullGuardResult.Passed : NullGuardResult.ValueMissing;
					// Prefer non-async value provider interfaces over async ones
					case INullGuard nullGuard:
						return nullGuard.EvaluateNullGuard(client);
					case INullGuardByType nullGuardByType:
						return nullGuardByType.EvaluateNullGuard<T>(client);
					case IValueProvider<T> valueProvider:
						return valueProvider.TryGetFor(client, out _)
							? NullGuardResult.Passed
							: EditorOnly.ThreadSafe.Application.IsPlaying
							? NullGuardResult.ValueProviderValueMissing
							: NullGuardResult.ValueProviderValueNullInEditMode;
					case IValueByTypeProvider valueProvider:
						return valueProvider.TryGetFor<T>(client, out _)
							? NullGuardResult.Passed
							: EditorOnly.ThreadSafe.Application.IsPlaying
							? NullGuardResult.ValueProviderValueMissing
							: NullGuardResult.ValueProviderValueNullInEditMode;
					case IValueProvider valueProvider:
						return valueProvider.Value is object objectValue && Find.In<T>(objectValue, out _)
							? NullGuardResult.Passed
							: EditorOnly.ThreadSafe.Application.IsPlaying
							? NullGuardResult.ValueProviderValueMissing
							: NullGuardResult.ValueProviderValueNullInEditMode;
					case IValueProviderAsync<T> valueProvider:
						var awaitable = valueProvider.GetForAsync(client);
						#if UNITY_2023_1_OR_NEWER
						var awaiter = awaitable.GetAwaiter();
						return awaiter.IsCompleted && awaitable.GetAwaiter().GetResult() is not null
						? NullGuardResult.Passed
						: EditorOnly.ThreadSafe.Application.IsPlaying
						? NullGuardResult.ValueProviderValueMissing
						: NullGuardResult.ValueProviderValueNullInEditMode;
						#else
						return awaitable.IsFaulted ? NullGuardResult.ValueProviderException : NullGuardResult.Passed;
						#endif

					case IValueByTypeProviderAsync valueProvider:
						awaitable = valueProvider.GetForAsync<T>(client);
						#if UNITY_2023_1_OR_NEWER
						awaiter = awaitable.GetAwaiter();
						return awaiter.IsCompleted && awaitable.GetAwaiter().GetResult() is not null
						? NullGuardResult.Passed
						: EditorOnly.ThreadSafe.Application.IsPlaying
						? NullGuardResult.ValueProviderValueMissing
						: NullGuardResult.ValueProviderValueNullInEditMode;
						#else
						return awaitable.IsFaulted ? NullGuardResult.ValueProviderException : NullGuardResult.Passed;
						#endif
				}
			}

			bool isPlaying = context  == Context.MainThread ? Application.isPlaying : EditorOnly.ThreadSafe.Application.IsPlaying;
			if(!isPlaying && ServiceUtility.IsServiceDefiningType<T>())
			{
				return NullGuardResult.Passed;
			}

			return value switch
			{
				null when client && context.IsUnitySafeContext() => Service.ExistsFor<T>(client) ? NullGuardResult.Passed : NullGuardResult.ValueMissing,
				null => Service.Exists<T>() ? NullGuardResult.Passed : NullGuardResult.ValueMissing,
				// Support value providers in Any.Serialization.cs, like _String.
				IValueProvider<T> valueProvider => valueProvider.Value is not null ? NullGuardResult.Passed : NullGuardResult.InvalidValueProviderState,
				_ => NullGuardResult.Passed,
			};
		}
		#endif
	}
}