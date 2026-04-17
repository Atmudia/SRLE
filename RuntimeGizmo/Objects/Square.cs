using UnityEngine;

namespace SRLE.RuntimeGizmo.Objects
{
	public struct Square
	{
		public Vector3 BottomLeft;
		public Vector3 BottomRight;
		public Vector3 TopLeft;
		public Vector3 TopRight;

		public Vector3 this[int index]
		{
			get
			{
				return index switch
				{
					0 => this.BottomLeft,
					1 => this.TopLeft,
					2 => this.TopRight,
					3 => this.BottomRight,
					4 => this.BottomLeft //so we wrap around back to start
					,
					_ => Vector3.zero
				};
			}
		}
	}
}
