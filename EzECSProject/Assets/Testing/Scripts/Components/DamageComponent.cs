using Ez.Scripts;
using UnityEngine;

namespace Testing.Scripts.Components
{
    public sealed class DamageComponent : EzComponent
    {
        [Range(0,999999)]
        public int DamageToInflict = 1;
    }
}