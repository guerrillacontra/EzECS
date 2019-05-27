using Ez.Scripts;
using Testing.Scripts.Components;

namespace Testing.Scripts.Systems
{
    public sealed class HealthSystem : EzSystem, IUpdatableSystem
    {
        protected override void OnInit(EzEntitySpace entitySpace)
        {
            _healthFamily = new EzFamilyCache<HealthFamily>(entitySpace);
            _healthFamily.Init();
        }
        public void OnUpdate(EzEntitySpace space)
        {
            for (int i = 0; i < _healthFamily.Nodes.Count; i++)
            {
                var node = _healthFamily.Nodes[i];

                if (node.Health.IsDead)
                {
                    Destroy(node.Entity.gameObject);
                }
            }
        }

        private EzFamilyCache<HealthFamily> _healthFamily;
        
        private struct HealthFamily 
        {
            public readonly EzEntity Entity;
            public readonly HealthComponent Health;
        }
    }
}