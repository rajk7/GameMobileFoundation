﻿using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Scripting;

namespace Sisus.Init
{
	/// <summary>
	/// Represents an object that can asynchronously retrieve a value of type <typeparamref name="TValue"/> for a client <see cref="Component"/>.
	/// <para>
	/// If a class derives from <see cref="Object"/> and implements <see cref="IValueProviderAsync{T}"/> then
	/// <see cref="Any{T}"/> can wrap an instance of this class and return its value when <see cref="Any{T}.GetValueAsync"/> is called.
	/// </para>
	/// </summary>
	/// <typeparam name="TValue"> Type of the provided value. </typeparam>
	/// <seealso cref="IValueProvider{TValue}"/>
	/// <seealso cref="IValueByTypeProvider"/>
	/// <seealso cref="IValueByTypeProviderAsync"/>
	[RequireImplementors]
	public interface IValueProviderAsync<TValue> : IValueProviderAsync
	{
		/// <summary>
		/// Gets the value of type <typeparamref name="TValue"/> for the <paramref name="client"/>.
		/// </summary>
		/// <param name="client">
		/// The component requesting the value, if request is coming from a component; otherwise, <see langword="null"/>.
		/// </param>
		/// <returns>
		/// <see cref="Awaitable{TValue}"/> that can be <see langword="await">awaited</see> to get the value of type <see cref="TValue"/>, if available;
		/// otherwise, a completed task with the result of <see langword="null"/>.
		/// </returns>
		#if UNITY_2023_1_OR_NEWER
		new Awaitable<TValue>
		#else
		new System.Threading.Tasks.Task<TValue>
		#endif
		GetForAsync([AllowNull] Component client);

		/// <summary>
		/// Gets the value of type <typeparamref name="TValue"/> for the <paramref name="client"/>.
		/// </summary>
		/// <param name="client">
		/// The component requesting the value, if request is coming from a component; otherwise, <see langword="null"/>.
		/// </param>
		/// <returns>
		/// An awaitable task for retriving the value for the <paramref name="client"/>, if available;
		/// otherwise, a completed task with the result of <see langword="null"/>.
		/// </returns>
		#if UNITY_2023_1_OR_NEWER
		async Awaitable<object>
		#else
		async System.Threading.Tasks.Task<object>
		#endif
		IValueProviderAsync.GetForAsync([AllowNull] Component client) => await GetForAsync(client);
	}

	/// <summary>
	/// Represents an object that can asynchronously retrieve a value for a client <see cref="Component"/>.
	/// <para>
	/// <para>
	/// Base interface of <see cref="IValueProvider{TValue}"/>.
	/// </para>
	/// </summary>
	[RequireImplementors]
	public interface IValueProviderAsync
	{
		/// <summary>
		/// Gets the value of type <typeparamref name="TValue"/> for the <paramref name="client"/>.
		/// </summary>
		/// <param name="client">
		/// The component requesting the value, if request is coming from a component; otherwise, <see langword="null"/>.
		/// </param>
		/// <returns>
		/// An awaitable task for retriving the value for the <paramref name="client"/>, if available;
		/// otherwise, a completed task with the result of <see langword="null"/>.
		/// </returns>
		#if UNITY_2023_1_OR_NEWER
		Awaitable<object>
		#else
		System.Threading.Tasks.Task<object>
		#endif
		GetForAsync([AllowNull] Component client);
	}
}
