using System.Collections.Generic;
using UnityEngine;

namespace SRLE.RuntimeGizmo.Objects
{
	public class AxisVectors
	{
		public readonly List<Vector3> X = new List<Vector3>();
		public readonly List<Vector3> Y = new List<Vector3>();
		public readonly List<Vector3> Z = new List<Vector3>();
		public readonly List<Vector3> All = new List<Vector3>();

		public void Add(AxisVectors axisVectors)
		{
			X.AddRange(axisVectors.X);
			Y.AddRange(axisVectors.Y);
			Z.AddRange(axisVectors.Z);
			All.AddRange(axisVectors.All);
		}

		public void Clear()
		{
			X.Clear();
			Y.Clear();
			Z.Clear();
			All.Clear();
		}
	}
}