﻿using System;
using UnityEditor;

namespace Sisus.Init.EditorOnly
{
	[CustomEditor(typeof(StateMachineBehaviour<,>), true, isFallback = true), CanEditMultipleObjects]
	public sealed class StateMachineBehaviourT2Editor : InitializableEditor
	{
		protected override Type BaseTypeDefinition => typeof(StateMachineBehaviour<,>);
	}
}