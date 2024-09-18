﻿using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Scripting;

namespace Sisus.Init
{
	/// <summary>
	/// Represents an object that can provide a value of a requested type to a <see cref="GameObject"/> client.
	/// <para>
	/// If a class derives from <see cref="Object"/> and implements <see cref="IValueByTypeProviderAsync"/> then
	/// <see cref="Any{T}"/> can wrap an instance of this class and return its value when <see cref="Any{T}.GetValue{TClient}"/> is called.
	/// </para>
	/// </summary>
	/// <seealso cref="IValueProvider{TValue}"/>
	/// <seealso cref="IValueByTypeProvider"/>
	/// <seealso cref="IValueByTypeProviderAsync"/>
	[RequireImplementors]
	public interface IValueByTypeProvider
	{
		/// <summary>
		/// Gets the value of type <typeparamref name="TValue"/> for the <paramref name="client"/>.
		/// </summary>
		/// <typeparam name="TValue"> Type of the requested <paramref name="value"/>. </typeparam>
		/// <param name="client">
		/// The component requesting the value, if request is coming from a component; otherwise, <see langword="null"/>.
		/// </param>
		/// <param name="value">
		/// When this method returns, contains the value of type <typeparamref name="TValue"/>, if available; otherwise, the default value of <typeparamref name="TValue"/>.
		/// This parameter is passed uninitialized.
		/// </param>
		/// <see langword="true"/> if was able to retrieve the value; otherwise, <see langword="false"/>.
		bool TryGetFor<TValue>([AllowNull] Component client, [NotNullWhen(true), MaybeNullWhen(false)] out TValue value);

		/// <summary>
		/// Gets a value indicating whether or not this value provider can potentially provide
		/// a value of the given type to the client at runtime.
		/// <para>
		/// Used by the Inspector to determine if the value provider can be assigned to an Init argument field.
		/// </para>
		/// </summary>
		/// <typeparam name="TValue"> Type of the value that would be provided. </typeparam>
		/// <param name="client"> The client component that would receive the value. </param>
		/// <returns>
		/// <see langword="true"/> if can potentially provide a value of the given type to the client
		/// at runtime; otherwise, <see langword="false"/>.
		/// </returns>
		bool CanProvideValue<TValue>([AllowNull] Component client) => TryGetFor<TValue>(client, out _);
	}
}
