using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ez.Scripts
{
    /// <summary>
    /// Stores all entities and related systems in isolation.
    ///
    /// You could have multiple spaces in your hierarchy at once with entities
    /// running in isolation from each other.
    /// </summary>
    public sealed class EzEntitySpace : MonoBehaviour
    {
        public delegate void EntitySpaceEntityHandler(EzEntitySpace space, EzEntity entity);

        public event EntitySpaceEntityHandler EntityRegistered = delegate { };
        public event EntitySpaceEntityHandler EntityUnregistered = delegate { };

        public delegate void EntitySpaceEntityComponentsChangedHandler(EzEntitySpace space, EzEntity entity,
            Type componentType, object component);

        public event EntitySpaceEntityComponentsChangedHandler EntityComponentStateChanged = delegate { };

        public void RegisterSystem(EzSystem system)
        {
            _systems.Add(system);

            system.PriorityChanged += SystemOnPriorityChanged;
            system.Init(this);

            foreach (var entity in _entities.ToArray())
            {
                foreach (var component in entity)
                {
                    EntityOnComponentRegistered(entity, component);
                }
            }
        }

        private void SystemOnPriorityChanged(EzSystem system)
        {
            _shouldSystemsBeSorted = true;
        }

        private bool _shouldSystemsBeSorted = true;

        public void UnregisterSystem(EzSystem entityComponentSystem)
        {
            entityComponentSystem.PriorityChanged -= SystemOnPriorityChanged;
            _systems.Remove(entityComponentSystem);
            entityComponentSystem.Dispose(this);
        }
        
        public void PrioritiseSystems()
        {
            _systems.Clear();

            var array = GetComponents(typeof(EzSystem)).OfType<EzSystem>().ToArray();
            
            for (var i = 0; i < array.Length; i++)
            {
                var system = array[i];
                system.Priority = i;
                _systems.Add(system);
            }

            _shouldSystemsBeSorted = true;
        }

        public void RegisterEntityDeferred(EzEntity entity)
        {
            _processingQueue.Enqueue(new EntityProcessingItem(entity, EntityProcessingType.Register));
        }

        public void UnregisterEntityDeferred(EzEntity entity)
        {
            _processingQueue.Enqueue(new EntityProcessingItem(entity, EntityProcessingType.Unregister));
        }

     
        void Update()
        {
            while (_processingQueue.Count > 0)
            {
                var process = _processingQueue.Dequeue();

                var entity = process.Entity;

                if (process.Type == EntityProcessingType.Register)
                {
                    RegisterEntity(entity);
                }
                else if (process.Type == EntityProcessingType.Unregister)
                {
                    UnregisterEntity(entity);
                }
            }

            if (_shouldSystemsBeSorted)
            {
                _systems.Sort((x, y) => x.Priority - y.Priority);
            }

            for (int i = 0; i < _systems.Count; i++)
            {
                var system = _systems[i];

                var asUpdatable = system as IUpdatableSystem;

                if (asUpdatable != null && system.enabled)
                {
                    asUpdatable.OnUpdate(this);
                }
            }
        }


        private void FixedUpdate()
        {
            for (int i = 0; i < _systems.Count; i++)
            {
                var system = _systems[i];

                var asUpdatable = system as IFixedUpdatableSystem;

                if (asUpdatable != null && system.enabled)
                {
                    asUpdatable.OnFixedUpdate(this);
                }
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _systems.Count; i++)
            {
                var system = _systems[i];

                var asUpdatable = system as ILateUpdatableSystem;

                if (asUpdatable != null && system.enabled)
                {
                    asUpdatable.OnLateUpdate(this);
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var e in _entities.ToArray())
            {
                UnregisterEntity(e);
            }

            _entities.Clear();

            _processingQueue.Clear();

            foreach (var system in _systems)
            {
                system.PriorityChanged -= SystemOnPriorityChanged;
                system.Dispose(this);
            }
        }
        
        private void RegisterEntity(EzEntity entity)
        {
            entity.ComponentRegistered += EntityOnComponentRegistered;
            entity.ComponentUnregistered += EntityOnComponentUnregistered;
            entity.Destroyed += EntityDestroyed;

            _entities.Add(entity);

            EntityRegistered.Invoke(this, entity);
        }

        private void UnregisterEntity(EzEntity entity)
        {
            entity.ComponentRegistered -= EntityOnComponentRegistered;
            entity.ComponentUnregistered -= EntityOnComponentUnregistered;
            entity.Destroyed -= EntityDestroyed;

            _entities.Remove(entity);

            EntityUnregistered.Invoke(this, entity);
        }

        private void EntityDestroyed(EzEntity entity)
        {
            UnregisterEntity(entity);
        }

        private void EntityOnComponentUnregistered(EzEntity entity, EzComponent component)
        {
            EntityComponentStateChanged.Invoke(this, entity, component.GetType(), component);
        }

        private void EntityOnComponentRegistered(EzEntity entity, EzComponent component)
        {
            EntityComponentStateChanged.Invoke(this, entity, component.GetType(), component);
        }

        private readonly List<EzSystem> _systems = new List<EzSystem>();
        private readonly Queue<EntityProcessingItem> _processingQueue = new Queue<EntityProcessingItem>();
        private readonly List<EzEntity> _entities = new List<EzEntity>();


        enum EntityProcessingType
        {
            Register,
            Unregister
        }

        struct EntityProcessingItem
        {
            public readonly EzEntity Entity;
            public readonly EntityProcessingType Type;

            public EntityProcessingItem(EzEntity entity, EntityProcessingType type) : this()
            {
                Entity = entity;
                Type = type;
            }
        }

 
    }
}