using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ez.Scripts
{
    /// <summary>
    /// A container into which components can be created/destroyed.
    /// 
    /// Each component represents "state/date" inside the entity, which EntityComponentSystem's can use
    /// to process.
    /// 
    /// The "state" of an Entity, is based on what Types of components are stored inside the entity (think Set Theory)
    /// instead of what variables within are.
    ///
    /// An entity will auto register it's self to an EntitySpace if the space is the parent of the transform.
    /// This allows entities to be moved between spaces at run time!
    /// </summary>
    public sealed class EzEntity : MonoBehaviour, IEnumerable<EzComponent>
    {
        public delegate void EntityHandler(EzEntity entity);

        /// <summary>
        /// Tiggered when this entity component is destroyed by Unity.
        /// </summary>
        public event EntityHandler Destroyed = delegate { };

        public delegate void EntityComponentHandler(EzEntity entity, EzComponent component);

        /// <summary>
        /// Triggered when a component has been registered.
        /// </summary>
        public event EntityComponentHandler ComponentRegistered = delegate { };

        /// <summary>
        /// Triggered when a component has been unregistered.
        /// </summary>
        public event EntityComponentHandler ComponentUnregistered = delegate { };

        /// <summary>
        /// Enumerate over all components registered to this entity.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<EzComponent> GetEnumerator()
        {
            foreach (var com in _componentsList)
            {
                yield return com;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Try to get a registered entity component.
        /// </summary>
        /// <typeparam name="T">A type of EntityComponent</typeparam>
        /// <returns>The component or null if not found.</returns>
        public T GetEntityComponent<T>() where T : EzComponent
        {
            if (!HasEntityComponent<T>()) return null;

            return (T) _registeredComponents[typeof(T)];
        }

        /// <summary>
        /// Create a new EntityComponent and register it to this entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CreateEntityComponent<T>() where T : EzComponent
        {
            var componentInstance = gameObject.AddComponent<T>();
            RegisterComponent(componentInstance);
        }

        public void RegisterComponent(EzComponent componentInstance)
        {
            var type = componentInstance.GetType();

            if (HasEntityComponent(type))
            {
                Debug.LogWarningFormat("Entity '{0}' all ready contains a component of type '{1}', skipping", name,
                    type);
                return;
            }

            componentInstance.Destroyed += OnRegisteredEntityComponentDestroyed;

            _registeredComponents.Add(type, componentInstance);
            _componentsList.Add(componentInstance);

            ComponentRegistered.Invoke(this, componentInstance);
        }

        public void UnregisterComponent(EzComponent entityComponent)
        {
            var type = entityComponent.GetType();

            if (!HasEntityComponent(type)) return;

            var componentInstance = _registeredComponents[type];

            componentInstance.Destroyed -= OnRegisteredEntityComponentDestroyed;
            _registeredComponents.Remove(type);
            _componentsList.Remove(componentInstance);

            ComponentUnregistered.Invoke(this, componentInstance);
        }

        public void DestroyEntityComponent(Type type)
        {
            if (!HasEntityComponent(type)) return;

            var componentInstance = _registeredComponents[type];

            UnregisterComponent(componentInstance);

            Destroy(componentInstance);
        }


        public void DestroyEntityComponent(EzComponent component)
        {
            var type = component.GetType();

            DestroyEntityComponent(type);
        }

        public void DestroyEntityComponent<T>() where T : EzComponent
        {
            DestroyEntityComponent(typeof(T));
        }


        public void DestroyAllEntityComponents()
        {
            var coms = _componentsList.ToArray();

            for (int i = coms.Length - 1; i >= 0; i--)
            {
                DestroyEntityComponent(coms[i]);
            }
        }

        public bool HasEntityComponent(Type type)
        {
            return _registeredComponents.ContainsKey(type);
        }

        public bool HasEntityComponent<T>() where T : EzComponent
        {
            var type = typeof(T);
            return HasEntityComponent(type);
        }


        private void OnRegisteredEntityComponentDestroyed(EzComponent component)
        {
            DestroyEntityComponent(component);
        }

        #region Unity Messages

        private void OnDestroy()
        {
            DestroyAllEntityComponents();

            Destroyed.Invoke(this);
        }

        private void OnEnable()
        {
            InvalidateSpace();
        }

        private void OnDisable()
        {
            if (!_cachedParent) return;

            var oldSpace = _cachedParent ? _cachedParent.GetComponent<EzEntitySpace>() : null;

            if (!oldSpace) return;

            oldSpace.UnregisterEntityDeferred(this);

            _cachedParent = null;
        }

#if UNITY_EDITOR


        private void Update()
        {
            InvalidateSpace();
        }

        /// <summary>
        /// Check if the entity has changed space and if so, migrate to the new space.
        /// </summary>
        private void InvalidateSpace()
        {
            if (_cachedParent != transform.parent)
            {
                var oldSpace = _cachedParent ? _cachedParent.GetComponent<EzEntitySpace>() : null;
                var newSpace = transform.parent ? transform.parent.GetComponent<EzEntitySpace>() : null;

                _cachedParent = transform.parent;

                if (oldSpace != null)
                {
                    oldSpace.UnregisterEntityDeferred(this);
                }

                if (newSpace != null)
                {
                    newSpace.RegisterEntityDeferred(this);
                }
            }
        }

        private Transform _cachedParent;

#endif

        #endregion

        private readonly Dictionary<Type, EzComponent> _registeredComponents =
            new Dictionary<Type, EzComponent>(16);

        private readonly List<EzComponent> _componentsList = new List<EzComponent>(16);
    }
}