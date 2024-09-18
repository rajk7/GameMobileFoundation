﻿using System;
using System.Collections.Generic;
#if PANCAKE_ALCHEMY
using Alchemy.Inspector;
#endif
using Pancake.Common;
using UnityEngine;

namespace Pancake.Component
{
    public class ScannerSensor2D : Sensor
    {
#if PANCAKE_ALCHEMY
        [Blockquote("How many sensor points should there be along the start and end point\nHigher = less performant but more accurate")]
#endif
        [SerializeField]
        private int sensorNumber = 2;

        [Space(8)] [SerializeField] private RaycastType raycastType;

        [Space(8)] [SerializeField] private bool stopAfterFirstHit;
        [SerializeField] private bool detectOnStart = true;
#if UNITY_EDITOR
        [SerializeField] private bool showGizmos = true;
#endif
#if PANCAKE_ALCHEMY
        [Required]
#endif
        [Space(8), SerializeField]
        private Transform start;

#if PANCAKE_ALCHEMY
        [Required]
#endif
        [SerializeField]
        private Transform end;

#if PANCAKE_ALCHEMY
        [Required]
#endif
        [SerializeField]
        private Transform source;

        [SerializeField] private GameObjectUnityEvent detectedEvent;

        private Vector2[] _sensors;
        private Vector2[] _lastPositions;
        private readonly RaycastHit2D[] _hits = new RaycastHit2D[16];
        private readonly HashSet<Collider2D> _hitObjects = new();

        private int _frames;

        [Flags]
        private enum RaycastType
        {
            Horizontal = 1 << 0,
            Vertical = 1 << 1,
            IntersectionTop = 1 << 2,
            IntersectionBottom = 1 << 3,
        };

        private void Awake()
        {
            Init();
            if (detectOnStart) Pulse();
        }

        private void Init()
        {
            _sensors = new Vector2[sensorNumber];
            _lastPositions = new Vector2[sensorNumber];

            float d = 1f / (sensorNumber - 1);
            var lerpValue = 0f;
            for (var i = 0; i < sensorNumber; i++)
            {
                _sensors[i] = Vector2.Lerp(start.localPosition, end.localPosition, lerpValue);
                lerpValue += d;
            }
        }

        public override void Pulse()
        {
            // Reset _lastPositions
            for (var i = 0; i < _lastPositions.Length; ++i) _lastPositions[i] = source.TransformPoint(_sensors[i]);
            _hitObjects.Clear();
            isPlaying = true;
        }


        protected void FixedUpdate()
        {
            if (!isPlaying) return;

            _frames++;
            if (_frames % raycastRate != 0) return;
            _frames = 0;
            ProcedureTransform();
        }

        private void ProcedureTransform()
        {
            for (int i = 0; i < _sensors.Length; i++)
            {
                Vector2 currentPosition = source.TransformPoint(_sensors[i]);

                if (C.HasFlagUnsafe(raycastType, RaycastType.Horizontal)) Raycast(_lastPositions[i], currentPosition, RaycastType.Horizontal);

                //Raycast in intersection shape ( \ shape ) Top-to-Bottom
                if (C.HasFlagUnsafe(raycastType, RaycastType.IntersectionTop) && i > 0)
                    Raycast(_lastPositions[i], source.TransformPoint(_sensors[i - 1]), RaycastType.IntersectionTop);

                //Raycast in intersection shape ( / shape ) Bottom-to-Top
                if (C.HasFlagUnsafe(raycastType, RaycastType.IntersectionBottom) && i < sensorNumber - 1)
                    Raycast(_lastPositions[i], source.TransformPoint(_sensors[i + 1]), RaycastType.IntersectionBottom);

                _lastPositions[i] = currentPosition;
            }

            if (C.HasFlagUnsafe(raycastType, RaycastType.Vertical)) Raycast(_lastPositions[0], _lastPositions[sensorNumber - 1], RaycastType.Vertical);
        }

        private void Raycast(Vector2 from, Vector2 to, RaycastType type)
        {
#if UNITY_EDITOR
#pragma warning disable 0219
            var hitDetected = false;
#pragma warning restore 0219
#endif
            int count = Physics2D.Linecast(from, to, new ContactFilter2D {layerMask = layer}, _hits);
            if (count <= 0) return;
            for (var i = 0; i < count; i++)
            {
                var hit = _hits[i];
                if (hit.collider != null && hit.collider.transform != source)
                {
#if UNITY_EDITOR
                    hitDetected = true;
#endif
                    HandleHit(hit);
                }
            }

#if UNITY_EDITOR
            if (showGizmos)
            {
                var lineColor = type switch
                {
                    RaycastType.Horizontal => Color.white,
                    RaycastType.Vertical => Color.cyan,
                    RaycastType.IntersectionTop => Color.magenta,
                    RaycastType.IntersectionBottom => Color.yellow,
                    _ => Color.white
                };

                Debug.DrawLine(from, to, hitDetected ? Color.red : lineColor, 0.4f);
            }
#endif
        }

        private void HandleHit(RaycastHit2D hit)
        {
            if (_hitObjects.Contains(hit.collider)) return;

            _hitObjects.Add(hit.collider);
            detectedEvent?.Invoke(hit.collider.gameObject);
            if (stopAfterFirstHit) Stop();

#if UNITY_EDITOR
            if (showGizmos)
            {
                Debug.DrawRay(hit.point + new Vector2(0, 0.2f), Vector2.down * 0.4f, Color.red, 0.6f);
                Debug.DrawRay(hit.point + new Vector2(-0.2f, 0), Vector2.right * 0.4f, Color.red, 0.6f);
            }
#endif
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.green;
                foreach (var s in _sensors) Gizmos.DrawWireSphere(source.TransformPoint(s), 0.075f);
            }

            if (start != null && end != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(start.position, 0.1f);
                Gizmos.DrawWireSphere(end.position, 0.1f);
            }
        }
#endif
    }
}