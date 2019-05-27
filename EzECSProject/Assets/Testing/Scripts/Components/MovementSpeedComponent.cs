using Ez.Scripts;
using UnityEngine;

namespace Testing.Scripts.Components
{
    public sealed class MovementSpeedComponent : EzComponent
    {
        [Range(0f, 20f)]
        public float MetersPerSecond = 1f;
    }
}