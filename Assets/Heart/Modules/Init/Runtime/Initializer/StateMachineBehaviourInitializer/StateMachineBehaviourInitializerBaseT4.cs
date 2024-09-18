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
	/// A base class for an animator component that can be used to specify the four arguments used to
	/// initialize a state machine behaviour of type <typeparamref name="TStateMachineBehaviour"/>
	/// that implements <see cref="IInitializable{TFirstArgument, TSecondArgument, TThirdArgument, TFourthArgument}"/>.
	/// <para>
	/// The arguments are injected to the <typeparamref name="TStateMachineBehaviour">client</typeparamref> during the Awake event.
	/// </para>
	/// <para>
	/// The client receives the argument via the <see cref="IInitializable{TFirstArgument, TSecondArgument, TThirdArgument, TFourthArgument}.Init">Init</see>
	/// method where it can assign them to a member field or property.
	/// </para>
	/// <para>
	/// After the argument has been injected the initializer is removed from the <see cref="GameObject"/> that holds it.
	/// </para>
	/// <para>
	/// When you derive your Initializer class from <see cref="StateMachineBehaviourInitializerBase{,,,,}"/>
	/// you are responsible for implementing the argument properties and serializing their value.
	/// This means you will need to write a little bit more code, but it also grants you more options
	/// in how to handle the serialization, making it possible to support types that Unity can't serialize
	/// automatically. If you derive from <see cref="StateMachineBehaviourInitializer{,,,,}"/> instead,
	/// then these things will be handled for you.
	/// </para>
	/// </summary>
	/// <typeparam name="TStateMachineBehaviour"> Type of the initialized state machine behaviour client. </typeparam>
	/// <typeparam name="TFirstArgument"> Type of the first argument to pass to the client's Init function. </typeparam>
	/// <typeparam name="TSecondArgument"> Type of the second argument to pass to the client's Init function. </typeparam>
	/// <typeparam name="TThirdArgument"> Type of the third argument to pass to the client's Init function. </typeparam>
	/// <typeparam name="TFourthArgument"> Type of the fourth argument to pass to the client's Init function. </typeparam>
	public abstract class StateMachineBehaviourInitializerBase<TStateMachineBehaviour, TFirstArgument, TSecondArgument, TThirdArgument, TFourthArgument> : StateMachineBehaviourInitializerBaseInternal<TStateMachineBehaviour>, IInitializer<TStateMachineBehaviour, TFirstArgument, TSecondArgument, TThirdArgument, TFourthArgument>, IInitializable
		#if UNITY_EDITOR
		, IInitializerEditorOnly<TStateMachineBehaviour, TFirstArgument, TSecondArgument, TThirdArgument, TFourthArgument>
		#endif
		where TStateMachineBehaviour : StateMachineBehaviour, IInitializable<TFirstArgument, TSecondArgument, TThirdArgument, TFourthArgument>
	{
		/// <summary>
		/// The first argument passed to the <typeparamref name="TStateMachineBehaviour">client</typeparamref>'s Init function.
		/// </summary>
		protected abstract TFirstArgument FirstArgument { get; set; }

		/// <summary>
		/// The second argument passed to the <typeparamref name="TStateMachineBehaviour">client</typeparamref>'s Init function.
		/// </summary>
		protected abstract TSecondArgument SecondArgument { get; set; }

		/// <summary>
		/// The third argument passed to the <typeparamref name="TStateMachineBehaviour">client</typeparamref>'s Init function.
		/// </summary>
		protected abstract TThirdArgument ThirdArgument { get; set; }

		/// <summary>
		/// The fourth argument passed to the <typeparamref name="TStateMachineBehaviour">client</typeparamref>'s Init function.
		/// </summary>
		protected abstract TFourthArgument FourthArgument { get; set; }

		/// <inheritdoc/>
		[return: NotNull]
		private protected override TStateMachineBehaviour InitTarget([DisallowNull] Animator target)
		{
			var firstArgument = FirstArgument;
			var secondArgument = SecondArgument;
			var thirdArgument = ThirdArgument;
			var fourthArgument = FourthArgument;

			#if DEBUG || INIT_ARGS_SAFE_MODE
			if(IsRuntimeNullGuardActive) ValidateArgumentsAtRuntime(firstArgument, secondArgument, thirdArgument, fourthArgument);
			#endif

			var behaviours = target.GetBehaviours<TStateMachineBehaviour>();
			int count = behaviours.Length;
			
			#if DEBUG || INIT_ARGS_SAFE_MODE
			if(count == 0) throw new MissingComponentException($"No {typeof(TStateMachineBehaviour).Name} was found in the Animator on '{name}'.", null);
			#endif

			for(int i = count - 1; i >= 0; i--)
			{
				behaviours[i].Init(firstArgument, secondArgument, thirdArgument, fourthArgument);
			}

			return behaviours[0];
		}

		bool IInitializable.HasInitializer => false;
		
		bool IInitializable.Init(Context context)
		{
			#if UNITY_EDITOR
			if(context.IsEditMode())
			{
				AutoInitInEditMode<StateMachineBehaviourInitializerBase<TStateMachineBehaviour, TFirstArgument, TSecondArgument, TThirdArgument, TFourthArgument>, TStateMachineBehaviour, TFirstArgument, TSecondArgument, TThirdArgument, TFourthArgument>(this);
			}
			#endif

			InitTarget();
			return true;
		}

		/// <summary>
		/// Resets the Init arguments to their default values.
		/// <para>
		/// <see cref="OnReset"/> is called when the user hits the Reset button in the Inspector's
		/// context menu or when adding the initializer to an Animator the first time.
		/// <para>
		/// This function is only called in the editor in edit mode.
		/// </summary>
		/// <param name="firstArgument"> The first argument to reset. </param>
		/// <param name="secondArgument"> The second argument to reset. </param>
		/// <param name="thirdArgument"> The third argument to reset. </param>
		/// <param name="fourthArgument"> The fourth argument to reset. </param>
		protected virtual void OnReset(ref TFirstArgument firstArgument, ref TSecondArgument secondArgument, ref TThirdArgument thirdArgument, ref TFourthArgument fourthArgument) { }

		#if DEBUG || INIT_ARGS_SAFE_MODE
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private protected void ValidateArgumentsAtRuntime(TFirstArgument firstArgument, TSecondArgument secondArgument, TThirdArgument thirdArgument, TFourthArgument fourthArgument)
		{
			ThrowIfMissing(firstArgument); ThrowIfMissing(secondArgument); ThrowIfMissing(thirdArgument); ThrowIfMissing(fourthArgument);
		}
		#endif

		#if UNITY_EDITOR
		private protected override NullGuardResult EvaluateNullGuard() => IsNull(FirstArgument) || IsNull(SecondArgument) || IsNull(ThirdArgument) || IsNull(FourthArgument) ? NullGuardResult.ValueMissing : NullGuardResult.Passed;
		TFirstArgument IInitializerEditorOnly<TStateMachineBehaviour, TFirstArgument, TSecondArgument, TThirdArgument, TFourthArgument>.FirstArgument { get => FirstArgument; set => FirstArgument = value; }
		TSecondArgument IInitializerEditorOnly<TStateMachineBehaviour, TFirstArgument, TSecondArgument, TThirdArgument, TFourthArgument>.SecondArgument { get => SecondArgument; set => SecondArgument = value; }
		TThirdArgument IInitializerEditorOnly<TStateMachineBehaviour, TFirstArgument, TSecondArgument, TThirdArgument, TFourthArgument>.ThirdArgument { get => ThirdArgument; set => ThirdArgument = value; }
		TFourthArgument IInitializerEditorOnly<TStateMachineBehaviour, TFirstArgument, TSecondArgument, TThirdArgument, TFourthArgument>.FourthArgument { get => FourthArgument; set => FourthArgument = value; }
		void IInitializerEditorOnly<TStateMachineBehaviour, TFirstArgument, TSecondArgument, TThirdArgument, TFourthArgument>.OnReset(ref TFirstArgument firstArgument, ref TSecondArgument secondArgument, ref TThirdArgument thirdArgument, ref TFourthArgument fourthArgument) => OnReset(ref firstArgument, ref secondArgument, ref thirdArgument, ref fourthArgument);
		private protected sealed override void Reset() => Reset<StateMachineBehaviourInitializerBase<TStateMachineBehaviour, TFirstArgument, TSecondArgument, TThirdArgument, TFourthArgument>, TStateMachineBehaviour, TFirstArgument, TSecondArgument, TThirdArgument, TFourthArgument>(this, gameObject);
		private protected override void OnValidate() => Validate(this, gameObject, FirstArgument, SecondArgument, ThirdArgument, FourthArgument);
		#endif
	}
}