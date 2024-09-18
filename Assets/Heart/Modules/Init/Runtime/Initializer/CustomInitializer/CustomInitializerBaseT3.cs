﻿#pragma warning disable CS0414

using System.Diagnostics.CodeAnalysis;
using Sisus.Init.Internal;
using UnityEngine;
using static Sisus.Init.Internal.InitializerUtility;
#if UNITY_EDITOR
using Sisus.Init.EditorOnly;
#endif

namespace Sisus.Init
{
	/// <summary>
	/// A base class for a component that can specify the three arguments used to initialize an object of type <typeparamref name="TClient"/>.
	/// <para>
	/// The arguments can be assigned using the inspector and are serialized as part of the client's scene or prefab asset.
	/// </para>
	/// <para>
	/// The <typeparamref name="TClient">client</typeparamref> does not need to implement the
	/// <see cref="IInitializable{TFirstArgument, TSecondArgument, TThirdArgument}"/> interface.
	/// The initialization arguments can instead be injected, for example, directly into properties with public setters.
	/// </para>
	/// <para>
	/// After the arguments have been injected the initializer is removed from the <see cref="GameObject"/> that holds it.
	/// </para>
	/// <para>
	/// When you derive your initializer class from <see cref="CustomInitializerBase{,,,}"/>
	/// you are responsible for implementing the argument properties and serializing their values.
	/// This means you will need to write a little bit more code, but it also grants you more options
	/// in how to handle the serialization, making it possible to support types that Unity can't serialize
	/// automatically. If you derive from <see cref="CustomInitializer{,,,}"/> instead, then these things will be handled for you.
	/// </para>
	/// </summary>
	/// <typeparam name="TClient"> Type of the initialized client component. </typeparam>
	/// <typeparam name="TFirstArgument"> Type of the first argument to inject to the client. </typeparam>
	/// <typeparam name="TSecondArgument"> Type of the second argument to inject to the client. </typeparam>
	/// <typeparam name="TThirdArgument"> Type of the third argument to inject to the client. </typeparam>
	public abstract class CustomInitializerBase<TClient, TFirstArgument, TSecondArgument, TThirdArgument>
		: InitializerBaseInternal<TClient>, IInitializer<TClient, TFirstArgument, TSecondArgument, TThirdArgument>, IInitializable
		#if UNITY_EDITOR
		, IInitializerEditorOnly<TClient, TFirstArgument, TSecondArgument, TThirdArgument>
		#endif
		where TClient : Component
	{
		/// <summary>
		/// The first argument injected to the <typeparamref name="TClient">client</typeparamref>.
		/// </summary>
		protected abstract TFirstArgument FirstArgument { get; set; }

		/// <summary>
		/// The second argument injected to the <typeparamref name="TClient">client</typeparamref>.
		/// </summary>
		protected abstract TSecondArgument SecondArgument { get; set; }

		/// <summary>
		/// The third argument injected to the <typeparamref name="TClient">client</typeparamref>.
		/// </summary>
		protected abstract TThirdArgument ThirdArgument { get; set; }

		[return: NotNull]
		private protected sealed override TClient InitTarget([AllowNull] TClient target)
		{
			var firstArgument = FirstArgument;
			var secondArgument = SecondArgument;
			var thirdArgument = ThirdArgument;

			#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				InitTarget(target, firstArgument, secondArgument, thirdArgument);
				return target;
			}
			#endif

			#if DEBUG || INIT_ARGS_SAFE_MODE
			if(IsRuntimeNullGuardActive) ValidateArgumentsAtRuntime(firstArgument, secondArgument, thirdArgument);
			#endif

			#if UNITY_EDITOR
			if(target == null)
			#else
			if(target is null)
			#endif
			{
				target = gameObject.AddComponent<TClient>();
			}
			else if(target.gameObject != gameObject)
			{
				target = Instantiate(target);
			}

			InitTarget(target, firstArgument, secondArgument, thirdArgument);
			return target;
		}

		/// <summary>
		/// Initializes the <paramref name="target"/> of type <see cref="TClient"/> using the provided arguments.
		/// </summary>
		/// <param name="target"> The target to initialize. </param>
		/// <param name="firstArgument"> The first argument used to initialize the target. </param>
		/// <param name="secondArgument"> The second argument used to initialize the target. </param>
		/// <param name="thirdArgument"> The third argument used to initialize the target. </param>
		protected abstract void InitTarget(TClient target, TFirstArgument firstArgument, TSecondArgument secondArgument, TThirdArgument thirdArgument);

		bool IInitializable.HasInitializer => false;

		bool IInitializable.Init(Context context)
		{
			#if UNITY_EDITOR
			if(context.IsEditMode())
			{
				AutoInitInEditMode<CustomInitializerBase<TClient, TFirstArgument, TSecondArgument, TThirdArgument>, TClient, TFirstArgument, TSecondArgument, TThirdArgument>(this);
			}
			#endif

			InitTarget();
			return true;
		}

		/// <summary>
		/// Resets the Init arguments to their default values.
		/// <para>
		/// <see cref="OnReset"/> is called when the user hits the Reset button in the Inspector's
		/// context menu or when adding the component to a GameObject the first time.
		/// <para>
		/// This function is only called in the editor in edit mode.
		/// </summary>
		/// <param name="firstArgument"> The first argument to reset. </param>
		/// <param name="secondArgument"> The second argument to reset. </param>
		/// <param name="thirdArgument"> The third argument to reset. </param>
		protected virtual void OnReset(ref TFirstArgument firstArgument, ref TSecondArgument secondArgument, ref TThirdArgument thirdArgument) { }

		#if DEBUG || INIT_ARGS_SAFE_MODE
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private protected void ValidateArgumentsAtRuntime(TFirstArgument firstArgument, TSecondArgument secondArgument, TThirdArgument thirdArgument)
		{
			ThrowIfMissing(firstArgument); ThrowIfMissing(secondArgument); ThrowIfMissing(thirdArgument);
		}
		#endif

		#if UNITY_EDITOR
		private protected override NullGuardResult EvaluateNullGuard() => IsNull(FirstArgument) || IsNull(SecondArgument) || IsNull(ThirdArgument) ? NullGuardResult.ValueMissing : NullGuardResult.Passed;
		TFirstArgument IInitializerEditorOnly<TClient, TFirstArgument, TSecondArgument, TThirdArgument>.FirstArgument { get => FirstArgument; set => FirstArgument = value; }
		TSecondArgument IInitializerEditorOnly<TClient, TFirstArgument, TSecondArgument, TThirdArgument>.SecondArgument { get => SecondArgument; set => SecondArgument = value; }
		TThirdArgument IInitializerEditorOnly<TClient, TFirstArgument, TSecondArgument, TThirdArgument>.ThirdArgument { get => ThirdArgument; set => ThirdArgument = value; }
		void IInitializerEditorOnly<TClient, TFirstArgument, TSecondArgument, TThirdArgument>.OnReset(ref TFirstArgument firstArgument, ref TSecondArgument secondArgument, ref TThirdArgument thirdArgument) => OnReset(ref firstArgument, ref secondArgument, ref thirdArgument);
		private protected sealed override void Reset() => Reset<CustomInitializerBase<TClient, TFirstArgument, TSecondArgument, TThirdArgument>, TClient, TFirstArgument, TSecondArgument, TThirdArgument>(this, gameObject);
		private protected override void OnValidate() => Validate(this, gameObject, FirstArgument, SecondArgument, ThirdArgument);
		#endif
	}
}