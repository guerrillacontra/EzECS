using UnityEngine;
using UnityEngine.Assertions;

namespace Ez.Scripts
{
    /// <summary>
    /// A system that can process data from one or more "Entity Families".
    /// Each system has a priority which can be used to order when systems
    /// execute their update functions.
    /// </summary>
    [RequireComponent(typeof(EzEntitySpace))]
    public abstract class EzSystem : MonoBehaviour
    {
        public delegate void PriorityChangedHandler(EzSystem system);

        public event PriorityChangedHandler PriorityChanged = delegate { };

        public int Priority
        {
            get { return _priority; }
            set
            {
                if (value == _priority) return;
                _priority = value;
                PriorityChanged.Invoke(this);
            }
        }

        [SerializeField] [Range(0, 100)] private int _priority = 0;

        

        private void Start()
        {
            var space = GetComponent<EzEntitySpace>();
            Assert.IsNotNull(space);

            space.RegisterSystem(this);
        }

        private void OnDestroy()
        {
            var space = GetComponent<EzEntitySpace>();
            Assert.IsNotNull(space);

            space.UnregisterSystem(this);
        }

        public void Init(EzEntitySpace entitySpace)
        {
            _space = entitySpace;
            OnInit(entitySpace);
        }

        private EzEntitySpace _space;

     
        protected abstract void OnInit(EzEntitySpace space);


        public void Dispose(EzEntitySpace space)
        {
            OnDispose(space);
        }

        protected virtual void OnDispose(EzEntitySpace entitySpace)
        {
        }
    }
}