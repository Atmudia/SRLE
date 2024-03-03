using UnityEngine;

namespace SRLE.RuntimeGizmo.Objects
{
	public struct IntersectPoints(Vector3 first, Vector3 second)
	{
		public Vector3 first = first;
		public Vector3 second = second;
	}
}