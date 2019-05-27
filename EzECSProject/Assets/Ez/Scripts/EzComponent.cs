using UnityEngine;

namespace Ez.Scripts
{
    
    /// <summary>
    /// A component that represents a unit of data represented inside an Entity.
    /// The data inside each component can be processed by an EntityComponentSystem.
    /// </summary>
    [RequireComponent(typeof(EzEntity))]
    public abstract class EzComponent : MonoBehaviour
    {
        public delegate void DestroyedHandler(EzComponent component);

        public event DestroyedHandler Destroyed = delegate { };

        private void Start()
        {
            var entity = GetComponent<EzEntity>();
            
            if(entity && !entity.HasEntityComponent(GetType()))
                entity.RegisterComponent(this);
            
            OnEntityComponentStarted();
        }

        private void OnEnable()
        {
            var entity = GetComponent<EzEntity>();
            
            if(entity && !entity.HasEntityComponent(GetType()))
                entity.RegisterComponent(this);
        }

        private void OnDisable()
        {
            var entity = GetComponent<EzEntity>();
            
            if(entity && entity.HasEntityComponent(GetType()))
                entity.UnregisterComponent(this);
        }

     

        protected virtual void OnEntityComponentStarted()
        {
        }

        private void OnDestroy()
        {
            OnEntityComponentDestroyed();
            Destroyed.Invoke(this);
        }

        protected virtual void OnEntityComponentDestroyed()
        {
        }
    }
}