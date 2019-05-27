using Ez.Scripts;
using UnityEngine;

namespace Testing.Scripts.Components
{
	public sealed class PlayerInputComponent : EzComponent
	{
		public KeyCode Left = KeyCode.A;
		public KeyCode Right = KeyCode.D;
		public KeyCode Up = KeyCode.W;
		public KeyCode Down = KeyCode.S;

		public Vector2 CurrentInput;
	}
}
