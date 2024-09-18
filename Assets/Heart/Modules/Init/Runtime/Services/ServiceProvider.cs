﻿using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using static Sisus.Init.ValueProviders.ValueProviderUtility;

namespace Sisus.Init
{
	/// <summary>
	/// Class that can provide an instance of any class that has the <see cref="ServiceAttribute"/> on demand.
	/// <para>
	/// This is a simple proxy for the static <see cref="Service{TDefiningClassOrInterface}"/> class;
	/// Calling the <see cref="Get"/> method on any instance of this class will return the shared
	/// service instance stored in <see cref="Service{TDefiningClassOrInterface}.Instance"/>.
	/// </para>
	/// <para>
	/// A benefit of using <see cref="ServiceProvider"/> instead of <see cref="Service{}"/> directly,
	/// is the ability to call <see cref="Get"/> through the <see cref="IServiceProvider"/> interface.
	/// This makes it possible to create mock implementations of the interface for unit tests.
	/// </para>
	/// <para>
	/// Additionally, it makes it easier to swap your service provider with another implementation at a later time.
	/// </para>
	/// A third benefit is that it makes your code less coupled with other classes, making it much easier to
	/// port the code over to another project for example.
	/// </para>
	/// <para>
	/// The <see cref="ServiceProvider"/> class is a <see cref="ServiceAttribute">service</see> itself.
	/// This means that an instance of the class can be automatically received by any classes that derive from
	/// <see cref="MonoBehaviour{IServiceProvider}"/> or <see cref="ScriptableObject{IServiceProvider}"/>.
	/// </para>
	/// </summary>
	[Service(typeof(IServiceProvider)), Service(typeof(System.IServiceProvider))]
	#if !INIT_ARGS_DISABLE_VALUE_PROVIDER_MENU_ITEMS
	[ValueProviderMenu(MENU_NAME, WhereAny = Is.Class | Is.Interface, WhereNone = Is.BuiltIn | Is.Service, Order = 100f, Tooltip = "An instance of this dynamic service is expected to become available for the client at runtime.\n\nService can be a component that has the Service Tag, or an Object registered as a service in a Services component, that is located in another scene or prefab.\n\nThe service could also be manually registered in code using " + nameof(Service) + "." + nameof(Service.SetInstance) + ".")]
	#endif
	#if DEV_MODE
	[CreateAssetMenu(fileName = MENU_NAME, menuName = CREATE_ASSET_MENU_GROUP + MENU_NAME)]
	#endif
	public sealed class ServiceProvider : ScriptableObject, IServiceProvider, System.IServiceProvider, IValueByTypeProvider, INullGuardByType
	{
		private const string MENU_NAME = "Service (Local)";

		/// <inheritdoc/>
		public bool TryGet<TService>(out TService service) => Service.TryGet(out service);

		/// <inheritdoc/>
		public bool TryGetFor<TService>(Component client, out TService service) => Service.TryGetFor(client, out service);

		/// <inheritdoc/>
		public bool CanProvideValue<TService>([AllowNull] Component client) => !typeof(TService).IsValueType && typeof(TService) != typeof(string) && typeof(TService) != typeof(object);

		/// <summary>
		/// Returns shared instance of <typeparamref name="TService"/> service.
		/// </summary>
		/// <typeparam name="TService">
		/// Interface or class type that defines the service.
		/// <para>
		/// This should be an interface that the service implements, a base type that the service derives from, or the exact type of the service.
		/// </para>
		/// </typeparam>
		/// <returns>
		/// An instance of a class that derives from <typeparamref name="TService"/>
		/// or is <typeparamref name="TService"/> and has the <see cref="ServiceAttribute"/>,
		/// if one is found in the project; otherwise, <see langword="null"/>.
		/// </returns>
		bool IValueByTypeProvider.TryGetFor<TService>([AllowNull] Component client, [MaybeNullWhen(false), NotNullWhen(true)] out TService service)
			=> client != null ? Service.TryGet(out service) : Service.TryGetFor(client, out service);

		/// <returns>
		/// Returns always <see langword="true"/> as long as <typeparamref name="TService"/> is not a value type.
		/// <para>
		/// We will always assume that the service will be available at runtime to avoid warnings being shown
		/// to the user about missing arguments.
		/// </para>
		/// </returns>
		NullGuardResult INullGuardByType.EvaluateNullGuard<TService>(Component client)
		{
			if(client ? Service.ExistsFor<TService>(client) : Service.Exists<TService>())
			{
				return NullGuardResult.Passed;
			}

			if(ServiceUtility.IsServiceDefiningType<TService>())
			{
				return NullGuardResult.Passed;
			}

			return NullGuardResult.ValueProviderValueNullInEditMode;
		}

		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		/// <param name="serviceType"> The type of service object to get. </param>
		/// <returns>
		/// A service object of type <paramref name="serviceType"/> or <see langword="null"/>
		/// if there is no service object of type <paramref name="serviceType"/>.
		/// </returns>
		object System.IServiceProvider.GetService(Type serviceType) => ServiceUtility.Get(serviceType);
	}
}