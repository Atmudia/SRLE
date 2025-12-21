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
        pastedObj = Object.Instantiate(copyObj, ObjectManager.World.transform);
        var buildObject = copyObj.GetComponent<BuildObject>();
        pastedObj.GetComponent<BuildObject>().ID = buildObject.ID;
        transformGizmo.ClearAndAddTarget(pastedObj.transform);
        ObjectManager.AddObject(buildObject.ID.Id, pastedObj);

    }

    public void UnExecute()
    {
        ObjectManager.RemoveObject(pastedObj);
        Object.Destroy(pastedObj);

    }
}