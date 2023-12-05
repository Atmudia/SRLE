using MelonLoader;
using SRLE.Components;
using SRLE.RuntimeGizmo.Objects.Commands;
using SRLE.RuntimeGizmo.UndoRedo;
using UnityEngine;

namespace SRLE.RuntimeGizmo;

public static class CopyPasteManager
{
    public static GameObject copiedObject;
    public static void Copy()
    {
        copiedObject = SRLECamera.Instance.transformGizmo.mainTargetRoot.gameObject;
        MelonLogger.Msg($"Copied object with: {copiedObject.ToString()}");
    }
    public static void Paste()
    {
        var pasteCommand = new PasteCommand();
        pasteCommand.Execute();
        UndoRedoManager.Insert(pasteCommand);
        MelonLogger.Msg($"Pasted object with: {copiedObject.ToString()}");

    }
}