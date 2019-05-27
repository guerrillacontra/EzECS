using System;
using System.Collections.Generic;
using System.Linq;
using Ez.Scripts;
using UnityEngine;

namespace Testing.Scripts.Components
{
    public sealed class HealthComponent : EzComponent
    {
        [Range(0,9999)]
        public int Health = 100;

        public bool IsDead
        {
            get { return Health <= 0; }
        }

      

    }
}