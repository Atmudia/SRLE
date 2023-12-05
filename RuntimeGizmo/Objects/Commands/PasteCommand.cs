using SRLE.Components;
using SRLE.RuntimeGizmo.UndoRedo;
using UnityEngine;

namespace SRLE.RuntimeGizmo.Objects.Commands;

public class PasteCommand : ICommand
{
    public GameObject pastedObj;
    public GameObject copyObj;

    public void Execute()
    {
        
        var transformGizmo = SRLECamera.Instance.transformGizmo;
        copyObj = CopyPasteManager.copiedObject;
        pastedObj = Object.Instantiate(copyObj, SRLEManager.World.transform, true);
        pastedObj.GetComponent<BuildObjectId>().IdClass = copyObj.GetComponent<BuildObjectId>().IdClass;
        transformGizmo.ClearAndAddTarget(pastedObj.transform);

    }

    public void UnExecute()
    {
        Object.Destroy(pastedObj);
    }
}