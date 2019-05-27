using Ez.Scripts;
using Testing.Scripts.Views;
using UnityEngine;

namespace Testing.Scripts.Components
{
    public sealed class Collider2DComponent : EzComponent
    {
        public delegate void TriggeredHandler(Collider2DComponent component, Collider2D collider);

        public event TriggeredHandler Triggered = delegate { };

        [SerializeField] private ColliderView _collider;

        private void OnEnable()
        {
            _collider.Triggered += ColliderOnTriggered;
        }

        private void OnDisable()
        {
            _collider.Triggered -= ColliderOnTriggered;
        }

        private void ColliderOnTriggered(ColliderView view, Collider2D other)
        {
            Triggered.Invoke(this, other);
        }
    }
}