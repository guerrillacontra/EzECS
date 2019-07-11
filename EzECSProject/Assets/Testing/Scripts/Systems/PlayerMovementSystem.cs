using System.Collections.ObjectModel;
using Ez.Scripts;
using Testing.Scripts.Components;
using UnityEngine;

namespace Testing.Scripts.Systems
{
    public class PlayerMovementSystem : EzSystem, IFixedUpdatableSystem
    {
        protected override void OnInit(EzEntitySpace entitySpace)
        {
            var fam = CreateFamilyCache<PlayerFamily>();
            _players = fam.Nodes;
        }

        private static readonly Vector2[] Dirs = {Vector2.left, Vector2.right, Vector2.up, Vector2.down};
        private static readonly KeyCode[] Inputs = new KeyCode[4];

        public void OnFixedUpdate(EzEntitySpace space)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                var node = _players[i];

                var dir = Vector2.zero;

                Inputs[0] = node.Input.Left;
                Inputs[1] = node.Input.Right;
                Inputs[2] = node.Input.Up;
                Inputs[3] = node.Input.Down;

                for (int k = 0; k < Inputs.Length; k++)
                {
                    if (Input.GetKey(Inputs[k]))
                    {
                        dir += Dirs[k];
                    }
                }

                dir.Normalize();

                node.Input.CurrentInput = dir;

                Vector2 currentPos = node.Entity.transform.position;
                Vector2 nextPos = currentPos + (node.Speed.MetersPerSecond * Time.deltaTime * dir);
                node.Entity.transform.position = nextPos;
            }
        }

        private ReadOnlyCollection<PlayerFamily> _players;

        public struct PlayerFamily
        {
            public readonly EzEntity Entity;
            public readonly PlayerInputComponent Input;
            public readonly MovementSpeedComponent Speed;
        }
    }
}