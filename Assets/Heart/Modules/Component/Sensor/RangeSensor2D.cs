using System.Collections.Generic;
using Pancake.Common;
#if PANCAKE_ALCHEMY
using Alchemy.Inspector;
#endif
using UnityEngine;

namespace Pancake.Component
{
    public class RangeSensor2D : Sensor
    {
        [Space(8)] [SerializeField] private float radius = 1f;
        [Space(8)] [SerializeField] private bool stopAfterFirstHit;
        [SerializeField] private bool detectOnStart = true;
#if UNITY_EDITOR
        [SerializeField] private bool showGizmos = true;
#endif
#if PANCAKE_ALCHEMY
        [Required]
#endif
        [Space(8), SerializeField]
        private Transform center;
#if PANCAKE_ALCHEMY
        [Required]
#endif
        [SerializeField]
        private Transform source;

        [SerializeField] private GameObjectUnityEvent detectedEvent;

        private readonly Collider2D[] _hits = new Collider2D[16];
        private readonly HashSet<Collider2D> _hitObjects = new();
        private int _frames;

        private void Awake()
        {
            if (detectOnStart) Pulse();
        }

        public override void Pulse()
        {
            _hitObjects.Clear();
            isPlaying = true;
        }

        protected void FixedUpdate()
        {
            if (!isPlaying) return;
            _frames++;
            if (_frames % raycastRate != 0) return;
            _frames = 0;
            Procedure();
        }

        private void Procedure()
        {
            var currentPosition = source.TransformPoint(center.localPosition);
            Raycast(currentPosition);
        }


        private void Raycast(Vector2 center)
        {
#if UNITY_EDITOR
#pragma warning disable 0219
            var hitDetected = false;
#pragma warning restore 0219
#endif
            int count = Physics2D.OverlapCircle(center, radius, new ContactFilter2D {layerMask = layer}, _hits);
            if (count <= 0) return;

            for (var i = 0; i < count; i++)
            {
                var hit = _hits[i];
                if (hit != null && hit.transform != source)
                {
#if UNITY_EDITOR
                    hitDetected = true;
#endif
                    HandleHit(hit);
                }
            }

#if UNITY_EDITOR
            if (showGizmos)
                DebugEditor.DrawCircle(center,
                    radius,
                    32,
                    hitDetected ? Color.red : Color.cyan,
                    0.4f);
#endif
        }


        private void HandleHit(Collider2D hit)
        {
            if (_hitObjects.Contains(hit)) return;
            _hitObjects.Add(hit);
            detectedEvent?.Invoke(hit.gameObject);
            if (stopAfterFirstHit) Stop();

#if UNITY_EDITOR
            if (showGizmos)
            {
                Debug.DrawRay((Vector2) hit.transform.position + new Vector2(0, 0.2f), Vector2.down * 0.4f, Color.red, 0.6f);
                Debug.DrawRay((Vector2) hit.transform.position + new Vector2(-0.2f, 0), Vector2.right * 0.4f, Color.red, 0.6f);
            }
#endif
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (center != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(center.position, 0.1f);

                if (showGizmos)
                {
                    Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
                    Gizmos.DrawWireSphere(center.position, radius);
                }
            }
        }
#endif
    }
}