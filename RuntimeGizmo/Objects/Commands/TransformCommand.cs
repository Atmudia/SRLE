using SRLE.RuntimeGizmo.UndoRedo;
using UnityEngine;

namespace SRLE.RuntimeGizmo.Objects.Commands
{
	public class TransformCommand : ICommand
	{
		TransformValues newValues;
		TransformValues oldValues;

		Transform transform;
		TransformGizmo transformGizmo;

		public TransformCommand(TransformGizmo transformGizmo, Transform transform)
		{
			this.transformGizmo = transformGizmo;
			this.transform = transform;

			oldValues = new TransformValues() {position=transform.position, rotation=transform.rotation, scale=transform.localScale};
		}

		public void StoreNewTransformValues()
		{
			newValues = new TransformValues() {position=transform.position, rotation=transform.rotation, scale=transform.localScale};
		}
		
		public void Execute()
		{
			var prevPosition = transform.position;
			transform.position = newValues.position;
			transform.rotation = newValues.rotation;
			transform.localScale = newValues.scale;
			ChunkManager.Reregister(transform, prevPosition);

			transformGizmo.SetPivotPoint();
		}

		public void UnExecute()
		{
			var prevPosition = transform.position;
			transform.position = oldValues.position;
			transform.rotation = oldValues.rotation;
			transform.localScale = oldValues.scale;
			ChunkManager.Reregister(transform, prevPosition);

			transformGizmo.SetPivotPoint();
		}

		struct TransformValues
		{
			public Vector3 position;
			public Quaternion rotation;
			public Vector3 scale;
		}
	}
}
