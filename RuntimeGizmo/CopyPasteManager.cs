using SRLE.Components;
using SRLE.RuntimeGizmo.Objects.Commands;
using SRLE.RuntimeGizmo.UndoRedo;
using UnityEngine;

namespace SRLE.RuntimeGizmo
{
    public static class CopyPasteManager
    {
        private static GameObject s_CopiedObject;

        public static bool HasCopy => s_CopiedObject != null;

        /// <summary>Stores the currently selected object for a later Paste.</summary>
        public static void Copy()
        {
            var target = SRLECamera.Instance.transformGizmo.mainTargetRoot;
            if (target == null) return;
            s_CopiedObject = target.gameObject;
            EntryPoint.ConsoleInstance.Log($"Copied: {s_CopiedObject.name}");
        }

        /// <summary>Pastes the previously copied object.</summary>
        public static void Paste()
        {
            if (!HasCopy)
            {
                EntryPoint.ConsoleInstance.Log("Nothing to paste.");
                return;
            }
            UndoRedoManager.Execute(new PasteCommand(s_CopiedObject));
        }

        /// <summary>Duplicates the currently selected object in one step.</summary>
        public static void Duplicate()
        {
            var target = SRLECamera.Instance.transformGizmo.mainTargetRoot;
            if (target == null) return;
            UndoRedoManager.Execute(new PasteCommand(target.gameObject));
        }
    }
}
