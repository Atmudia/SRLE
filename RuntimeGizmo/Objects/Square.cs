using UnityEngine;

namespace SRLE.RuntimeGizmo.Objects
{
	public struct Square
	{
		public Vector3 bottomLeft;
		public Vector3 bottomRight;
		public Vector3 topLeft;
		public Vector3 topRight;

		public Vector3 this[int index]
		{
			get
			{
				return index switch
				{
					0 => this.bottomLeft,
					1 => this.topLeft,
					2 => this.topRight,
					3 => this.bottomRight,
					4 => this.bottomLeft //so we wrap around back to start
					,
					_ => Vector3.zero
				};
			}
		}
	}
}
