using System.Collections.Generic;
using Ez.Scripts;
using Ez.Scripts.Util;
using Testing.Scripts.Components;
using UnityEngine;

namespace Testing.Scripts.Systems
{
    public sealed class CollisionHandlingSystem : EzSystem, IUpdatableSystem
    {
        protected override void OnInit(EzEntitySpace entitySpace)
        {
            _health = new EzFamilyCache<HealthFamily>(entitySpace);
            _health.Init();
            _health.NodeAdded += HealthOnNodeAdded;
            _health.NodeRemoved += HealthOnNodeRemoved;
        }

        private void HealthOnNodeAdded(EzFamilyCache<HealthFamily> cache, HealthFamily node)
        {
            node.Collider.Triggered += ColliderOnTriggered;
        }

        private void ColliderOnTriggered(Collider2DComponent component, Collider2D collider)
        {
            if (!enabled) return;
            
            var entity = component.GetComponent<EzEntity>();
            var health = entity.GetEntityComponent<HealthComponent>();

            var relation = collider.gameObject.GetComponent<EzRelationship>();

            if (!relation) return;

            var damage = relation.Entity.GetEntityComponent<DamageComponent>();

            if (!damage) return;

            _damages.Enqueue(new PendingDamage(health, damage.DamageToInflict));
        }
        
        private readonly Queue<PendingDamage> _damages = new Queue<PendingDamage>(32);

        private void HealthOnNodeRemoved(EzFamilyCache<HealthFamily> cache, HealthFamily node)
        {
            node.Collider.Triggered -= ColliderOnTriggered;
        }

        public void OnUpdate(EzEntitySpace space)
        {
            while (_damages.Count > 0)
            {
                var process = _damages.Dequeue();
                process.AffectedHealth.Health -= process.Damage;
            }
        }

        private EzFamilyCache<HealthFamily> _health;

        public struct HealthFamily 
        {
            public readonly Collider2DComponent Collider;
            public readonly HealthComponent Health;
        }

        struct PendingDamage
        {
            public readonly HealthComponent AffectedHealth;
            public readonly int Damage;

            public PendingDamage(HealthComponent affectedHealth, int damage)
            {
                AffectedHealth = affectedHealth;
                Damage = damage;
            }
        }

     
    }
}