using System;
using UnityEngine;

namespace Testing.Scripts.Views
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class ColliderView : MonoBehaviour
    {
        public delegate void CollidedHandler(ColliderView view, Collider2D other);

        public event CollidedHandler Triggered = delegate { };

        private void OnTriggerEnter2D(Collider2D other)
        {
            Triggered.Invoke(this, other);
        }
    }
}