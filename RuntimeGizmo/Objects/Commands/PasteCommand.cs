using SRLE.Components;
using SRLE.RuntimeGizmo.UndoRedo;
using UnityEngine;

namespace SRLE.RuntimeGizmo.Objects.Commands
{
    public class PasteCommand : ICommand
    {
        private readonly GameObject m_Source;
        private GameObject m_Pasted;

        public PasteCommand(GameObject source)
        {
            m_Source = source;
        }

        public void Execute()
        {
            if (m_Source == null)
            {
                EntryPoint.ConsoleInstance.Log("[SRLE] PasteCommand: source object no longer exists.");
                return;
            }

            // Resolve BuildObject from source — it may be on a parent or child, not the root
            if (!ObjectManager.GetBuildObject(m_Source, out var sourceBuildObject) || sourceBuildObject.ID == null)
            {
                EntryPoint.ConsoleInstance.Log($"[SRLE] PasteCommand: could not find BuildObject or ID on '{m_Source.name}'.");
                return;
            }

            // Offset slightly so the duplicate isn't hidden under the original
            var offset = new Vector3(1f, 0f, 1f);
            int oldHandler = sourceBuildObject.IdHandler;
            sourceBuildObject.IdHandler = 0;
            m_Pasted = Object.Instantiate(
                m_Source,
                m_Source.transform.position + offset,
                m_Source.transform.rotation,
                ObjectManager.World.transform);
            m_Pasted.SetActive(true);

            // IdClass uses auto-properties which Unity's serializer can't round-trip through
            // Instantiate, so the ID reference must be restored explicitly.
            if (ObjectManager.GetBuildObject(m_Pasted, out var pastedBuildObject))
                pastedBuildObject.ID = sourceBuildObject.ID;
            sourceBuildObject.IdHandler = oldHandler;
            ObjectManager.AddObject(sourceBuildObject.ID.Id, m_Pasted);
            SRLECamera.Instance.transformGizmo.ClearAndAddTarget(m_Pasted.transform);
        }

        public void UnExecute()
        {
            // ObjectManager.RemoveObject handles DestroyImmediate internally — don't destroy again
            ObjectManager.RemoveObject(m_Pasted);
        }
    }
}
