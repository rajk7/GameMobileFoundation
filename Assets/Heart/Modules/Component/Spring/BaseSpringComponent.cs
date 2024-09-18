﻿using UnityEngine;

namespace Pancake.Spring
{
    [EditorIcon("icon_default")]
    public abstract class BaseSpringComponent : GameComponent
    {
        [SerializeField, Range(0f, 100f)] protected float damping = 26f;
        [SerializeField, Range(0f, 500f)] protected float stiffness = 169f;
    }
}